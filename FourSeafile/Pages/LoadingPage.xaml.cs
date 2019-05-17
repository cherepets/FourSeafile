using Windows.UI.Xaml;

namespace FourSeafile.Pages
{
    public sealed partial class LoadingPage
    {
        public LoadingPage()
        {
            InitializeComponent();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (Credentials.Exists())
            {
                if (Settings.Local.UseWindowsHello && !await WindowsHello.VerifyAsync())
                {
                    Frame.Navigate(typeof(AuthPage));
                    return;
                }
                var session = await Credentials.AuthenticateAsync();
                if (session != null)
                {
                    App.OnTokenReceived(session);
                }
                else Frame.Navigate(typeof(AuthPage));
            }
            else
                Frame.Navigate(typeof(AuthPage));
        }
    }
}
