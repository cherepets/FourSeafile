using SeafClient;
using System;
using System.Threading.Tasks;

namespace FourSeafile
{
    public static class Credentials
    {
        public static bool Exists()
            => Settings.Encrypted.Host != null 
            && Settings.Encrypted.Login != null 
            && Settings.Encrypted.Password != null;

        public static async Task<SeafSession> AuthenticateAsync()
        {
            try
            {
                return await SeafSession.Establish(new Uri(Settings.Encrypted.Host, UriKind.Absolute), Settings.Encrypted.Login, Settings.Encrypted.Password.ToCharArray());
            }
            catch (Exception exception)
            {
                App.HandleException(exception);
            }
            return null;
        }

        public static void Store(string host, string login, string password)
        {
            Settings.Encrypted.Host = host;
            Settings.Encrypted.Login = login;
            Settings.Encrypted.Password = password;
        }

        public static void Clear()
            => Settings.Encrypted.Password = null;
    }
}
