using FourSeafile.Extensions;
using SeafClient;
using SeafClient.Types;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using System;
using Windows.UI.Core;
using Windows.UI.Popups;
using System.Net.Http;
using System.Runtime.InteropServices;

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
                Frame.Navigate(typeof(MainPage), null);
            };
            Current.UnhandledException += Current_UnhandledException;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;

        }

        public static new App Current => Application.Current as App;

        public static void HandleException(Exception e) => ShowExceptionMessage(e);

        private static void Current_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            if (e.Message == "Unspecified error\r\n" || e.Exception?.Message == null) return;
            HandleException(e.Exception);
        }

        private static void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            e.SetObserved();
            if (e.Exception?.Message == null) return;
            HandleException(e.Exception);
        }

        private static async void ShowExceptionMessage(Exception e)
        {
            var dispatcher = Window.Current?.CoreWindow?.Dispatcher;
            if (dispatcher == null) return;
            await dispatcher.RunAsync(CoreDispatcherPriority.Low, async () =>
            {
                var isNetworkRestriction = e is HttpRequestException && e.InnerException is COMException;
                var dialog = new MessageDialog(
                    isNetworkRestriction ? "Connection cannot be established due network restriction" : e.Message,
                    isNetworkRestriction ? "Error" : e.GetType().Name)
                    .SetDefaultCommandIndex(0);
                if (Platform.IsDesktop)
                    dialog.WithCommand("Close");
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
                dialog.WithCommand("Close");
            if (e.Message != null)
                dialog.WithCommand("Message", () => ShowExceptionMessage(e));
            if (e.InnerException != null)
                dialog.WithCommand("InnerException", () => ShowExceptionMessage(e.InnerException));
            await dialog.ShowAsync();
        }

        public static void Logout()
        {
            Seafile = null;
            Credentials.Clear();
            Frame.Navigate(typeof(AuthPage), null);
        }
        
        protected override async void OnLaunched(LaunchActivatedEventArgs e)
        {
            ChangeNameBarColor();
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
                if (Credentials.Exists())
                {
                    Seafile = await Credentials.AuthenticateAsync();
                    if (Seafile != null) Frame.Navigate(typeof(MainPage), e.Arguments);
                    else Frame.Navigate(typeof(AuthPage), e.Arguments);
                }
                else
                    Frame.Navigate(typeof(AuthPage), e.Arguments);
                Window.Current.Activate();
            }
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
