using System.Runtime.CompilerServices;
using Windows.ApplicationModel.Resources;

namespace FourSeafile
{
    public static class Localization
    {
        public static string Cancel => Get();
        public static string CantAccess => Get();
        public static string CantDecrypt => Get();
        public static string CantUpload => Get();
        public static string Close => Get();
        public static string Confirmation => Get();
        public static string ConnectionError => Get();
        public static string Delete => Get();
        public static string Error => Get();
        public static string HelloAuth => Get();
        public static string HelloError => Get();
        public static string HelloRetry => Get();
        public static string HelloUnavialable => Get();
        public static string IsDownloading => Get();
        public static string IsUploading => Get();
        public static string No => Get();
        public static string Password => Get();
        public static string Ok => Get();
        public static string Warning => Get();
        public static string Yes => Get();

        private static ResourceLoader _loader = new ResourceLoader();
        private static string Get([CallerMemberName] string property = null)
            => _loader.GetString(property);
    }
}
