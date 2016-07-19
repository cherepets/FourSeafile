using SeafClient;
using System;
using System.Threading.Tasks;

namespace FourSeafile
{
    public static class Credentials
    {
        public static bool Exists()
            => Settings.Current.Host != null 
            && Settings.Current.Login != null 
            && Settings.Current.Password != null;

        public static async Task<SeafSession> AuthenticateAsync()
        {
            try
            {
                return await SeafSession.Establish(new Uri(Settings.Current.Host, UriKind.Absolute), Settings.Current.Login, Settings.Current.Password.ToCharArray());
            }
            catch (Exception exception)
            {
                App.HandleException(exception);
            }
            return null;
        }

        public static void Store(string host, string login, string password)
        {
            Settings.Current.Host = host;
            Settings.Current.Login = login;
            Settings.Current.Password = password;
        }

        public static void Clear()
            => Settings.Current.Password = null;
    }
}
