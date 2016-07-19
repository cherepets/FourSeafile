using Windows.System.Profile;

namespace FourSeafile
{
    public static class Platform
    {
        public const byte Desktop = 0;
        public const byte Mobile = 1;
        public const byte Xbox = 2;
        public const byte IoT = 3;

        private const string WindowsMobile = "Windows.Mobile";

        public static byte Current
        {
            get
            {
                if (_current == null) _current = Detect();
                return _current.Value;
            }
        }
        private static byte? _current;

        public static bool IsDesktop => Current == Desktop;
        public static bool IsMobile => Current == Mobile;
        public static bool IsXbox => Current == Xbox;
        public static bool IsIoT => Current == IoT;

        private static byte Detect()
        {
            switch (AnalyticsInfo.VersionInfo.DeviceFamily)
            {
                case WindowsMobile: return Mobile;
                default: return Desktop;
            }
        }
    }
}
