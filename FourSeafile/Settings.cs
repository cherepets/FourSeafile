namespace FourSeafile
{
    public class Settings : SettingsBase
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

        public string Password
        {
            get { return GetProperty<string>(); }
            set { SetProperty(value); }
        }

        public static Settings Current { get; } = new Settings();

        public Settings()
        {
            SettingsProvider = new EncryptedSettingsProvider(); 
        }
    }
}
