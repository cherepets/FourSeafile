using SeafClient;
using System;
using System.Threading.Tasks;

namespace FourSeafile
{
    public static class Credentials
    {
        public static bool Exists()
            => Settings.Encrypted.Exists(nameof(Settings.Encrypted.Password));

        public static async Task<SeafSession> AuthenticateAsync()
        {
            try
            {
                return await SeafSession.Establish(new Uri(Settings.Local.Host, UriKind.Absolute), Settings.Local.Login, Settings.Encrypted.Password.ToCharArray());
            }
            catch (Exception exception)
            {
                App.HandleException(exception);
            }
            return null;
        }

        public static void Store(string host, string login, string password)
        {
            Settings.Local.Host = host;
            Settings.Local.Login = login;

            if (WindowsHello.IsVerified)
            {
                Settings.Encrypted.Password = password;
            }
        }

        public static void Clear()
            => Settings.Encrypted.Password = null;
    }
}
