using FourSeafile.Extensions;
using FourSeafile.Pages;
using FourSeafile.ViewModel;
using SeafClient;
using SeafClient.Types;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace FourSeafile
{
    sealed partial class App
    {
        public static SeafSession Seafile { get; private set; }
        public static Page CurrentPage { get; private set; }
        public static Frame Frame { get; private set; }

        public static Dictionary<string, SeafLibrary> LibCache { get; set; }

        public App()
        {
            InitializeComponent();
            AuthPage.NavigatedTo += (s, e) => CurrentPage = (Page)s;
            MainPage.NavigatedTo += (s, e) => CurrentPage = (Page)s;
            AuthPage.TokenReceived += (s, e) =>
            {
                Seafile = e;
                Frame.Navigate(typeof(MainPage), true);
            };
            Current.UnhandledException += Current_UnhandledException;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
        }

        public static new App Current => Application.Current as App;

        public static void HandleException(Exception e) => ShowExceptionMessage(e);

        private static void Current_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            HandleException(e.Exception);
        }

        private static void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            e.SetObserved();
            HandleException(e.Exception);
        }

        private static async void ShowExceptionMessage(Exception e)
        {
            if (e.Message.StartsWith("UnspecifiedError") || e?.Message == null) return;
            var dispatcher = Window.Current?.CoreWindow?.Dispatcher;
            if (dispatcher == null) return;
            await dispatcher.RunAsync(CoreDispatcherPriority.Low, async () =>
            {
                var isNetworkRestriction = e is HttpRequestException && e.InnerException is COMException;
                var dialog = new MessageDialog(
                    isNetworkRestriction ? Localization.ConnectionError : e.Message,
                    isNetworkRestriction ? Localization.Error : e.GetType().Name)
                    .SetDefaultCommandIndex(0);
                if (Platform.IsDesktop)
                    dialog.WithCommand(Localization.Close);
                if (e.StackTrace != null)
                    dialog.WithCommand("StackTrace", () => ShowStackTrace(e));
                if (e.InnerException != null)
                    dialog.WithCommand("InnerException", () => ShowExceptionMessage(e.InnerException));
                await dialog.ShowAsync();
            });
        }

        private static async void ShowStackTrace(Exception e)
        {
            var dialog = new MessageDialog(e.StackTrace, e.GetType().Name)
                .SetDefaultCommandIndex(0);
            if (Platform.IsDesktop)
                dialog.WithCommand(Localization.Close);
            if (e.Message != null)
                dialog.WithCommand("Message", () => ShowExceptionMessage(e));
            if (e.InnerException != null)
                dialog.WithCommand("InnerException", () => ShowExceptionMessage(e.InnerException));
            await dialog.ShowAsync();
        }

        public static void Logout()
        {
            Seafile = null;
            LibCache = null;
            Credentials.Clear();
            FileRootViewModel.Reset();
            Frame.Navigate(typeof(AuthPage), null);
        }
        
        protected override async void OnLaunched(LaunchActivatedEventArgs e)
        {
            Localization.Apply();
            ChangeNameBarColor();
            var clearTask = ApplicationData.Current.TemporaryFolder.ClearAsync();
            Frame = Window.Current.Content as Frame;
            if (Frame == null)
            {
                Frame = new Frame();
                Window.Current.Content = Frame;
            }
            if (e.PrelaunchActivated == false)
            {
                if (Frame.Content == null)
                    Frame.Navigate(typeof(LoadingPage), e.Arguments);
                if (Credentials.Exists() && (!Settings.Local.UseWindowsHello || await WindowsHello.VerifyAsync()))
                {
                    Seafile = await Credentials.AuthenticateAsync();
                    if (Seafile != null) Frame.Navigate(typeof(MainPage), e.Arguments);
                    else Frame.Navigate(typeof(AuthPage), e.Arguments);
                }
                else
                    Frame.Navigate(typeof(AuthPage), e.Arguments);
                Window.Current.Activate();
            }
            await clearTask;
        }
        private void ChangeNameBarColor()
        {
            if (!Platform.IsDesktop) return;
            var titleBar = ApplicationView.GetForCurrentView().TitleBar;
            var accentBrush = Resources["SystemControlBackgroundAccentBrush"] as SolidColorBrush;
            if (accentBrush != null)
            {
                var accent = accentBrush.Color;
                var light = accent.Lighten();
                var dark = accent.Darken();
                titleBar.BackgroundColor = accent;
                titleBar.ForegroundColor = Colors.Black;
                titleBar.ButtonBackgroundColor = accent;
                titleBar.ButtonForegroundColor = Colors.Black;
                titleBar.ButtonHoverBackgroundColor = dark;
                titleBar.ButtonHoverForegroundColor = Colors.Black;
                titleBar.ButtonPressedBackgroundColor = light;
                titleBar.ButtonPressedForegroundColor = Colors.White;
                titleBar.ButtonInactiveBackgroundColor = accent;
                titleBar.ButtonInactiveForegroundColor = Colors.Black;
                titleBar.InactiveBackgroundColor = dark;
            }
            titleBar.InactiveForegroundColor = Colors.Black;
        }
    }
}
