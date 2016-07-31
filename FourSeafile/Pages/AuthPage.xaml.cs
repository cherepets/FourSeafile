using SeafClient;
using System;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

namespace FourSeafile.Pages
{
    public sealed partial class AuthPage
    {
        public static event EventHandler<NavigationEventArgs> NavigatedTo;
        public static event EventHandler<SeafSession> TokenReceived;

        private const string RegisterLink = "accounts/register/";

        public AuthPage()
        {
            InitializeComponent();
            var host = Settings.Encrypted.Host;
            var login = Settings.Encrypted.Login;
            if (!string.IsNullOrWhiteSpace(host))
                HostBox.Text = host;
            if (!string.IsNullOrWhiteSpace(login))
            {
                LoginBox.Text = login;
                PasswordBox.Password = string.Empty;
                PasswordBox.Focus(FocusState.Programmatic);
            }
            else
                LoginButton.Focus(FocusState.Programmatic);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
            => NavigatedTo?.Invoke(this, e);

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                IsEnabled = false;
                var host = GetHost();
                var login = LoginBox.Text;
                var password = PasswordBox.Password;
                if (string.IsNullOrWhiteSpace(host) || string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
                    return;
                try
                {
                    var session = await SeafSession.Establish(new Uri(host, UriKind.Absolute), login, password.ToCharArray());
                    if (session != null)
                    {
                        await EnableWindowsHello();
                        TokenReceived?.Invoke(this, session);
                        Credentials.Store(host, login, password);
                    }
                }
                catch (Exception exception)
                {
                    App.HandleException(exception);
                }
            }
            finally
            {
                IsEnabled = true;
            }
        }

        private async System.Threading.Tasks.Task EnableWindowsHello()
        {
            if (WindowsHelloBox.IsChecked ?? false)
            {
                var res = await WindowsHello.VerifyAsync();
                Settings.Local.UseWindowsHello = res;
            }
        }

        private async void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            var host = GetHost();
            if (string.IsNullOrWhiteSpace(host)) return;
            if (!host.EndsWith(RegisterLink))
            {
                if (!host.EndsWith("/")) host += "/";
                host += RegisterLink;
            }
            await Launcher.LaunchUriAsync(new Uri(host, UriKind.Absolute));
        }

        private string GetHost()
        {
            var host = HostBox.Text;
            if (string.IsNullOrWhiteSpace(host)) return null;
            if (!host.StartsWith("http")) host = "https://" + host;
            return host;
        }

        private void Page_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter) LoginButton_Click(null, null);
        }
    }
}
