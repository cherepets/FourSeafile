using System.Collections.Generic;
using Windows.Storage;

namespace FourSeafile
{
    public class ApplicationDataSettingsProvider : SettingsProvider
    {
        public ApplicationDataSettingsProvider(ApplicationDataContainerType type)
        {
            switch (type)
            {
                case ApplicationDataContainerType.Local:
                    Container = ApplicationData.Current.LocalSettings;
                    break;
                case ApplicationDataContainerType.Roaming:
                    Container = ApplicationData.Current.RoamingSettings;
                    break;
            }
        }

        public override IDictionary<string, object> GetSettings()
        {
            return Container.Values;
        }

        public override void SetSetting(string key, object value)
        {
            Container.Values[key] = value;
        }

        public override bool Readable => true;

        public override bool Writeable => true;
    }

    public enum ApplicationDataContainerType
    {
        Local, Roaming
    }
}
