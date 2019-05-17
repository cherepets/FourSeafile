namespace FourSeafile
{
    public class Settings
    {
        public class EncryptedSettings : SettingsBase
        {
            public string Password
            {
                get { return GetProperty<string>(); }
                set { SetProperty(value); }
            }

            public EncryptedSettings()
            {
                SettingsProvider = new EncryptedSettingsProvider();
            }
        }

        public class LocalSettings : SettingsBase
        {
            public string Host
            {
                get { return GetProperty<string>(); }
                set { SetProperty(value); }
            }

            public string Login
            {
                get { return GetProperty<string>(); }
                set { SetProperty(value); }
            }

            public bool UseWindowsHello
            {
                get { return GetProperty<bool>(); }
                set { SetProperty(value); }
            }

            public LocalSettings()
            {
                SettingsProvider = new ApplicationDataSettingsProvider(ApplicationDataContainerType.Local);
            }
        }

        public static EncryptedSettings Encrypted { get; } = new EncryptedSettings();
        public static LocalSettings Local { get; } = new LocalSettings();

        private Settings() { }
    }
}
