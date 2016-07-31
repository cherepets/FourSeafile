using System.Collections.Generic;
using Windows.Storage;

namespace FourSeafile
{
    public class ApplicationDataSettingsProvider : SettingsProvider
    {
        private readonly ApplicationDataContainer _container;

        public ApplicationDataSettingsProvider(ApplicationDataContainerType type)
        {
            switch (type)
            {
                case ApplicationDataContainerType.Local:
                    _container = ApplicationData.Current.LocalSettings;
                    break;
                case ApplicationDataContainerType.Roaming:
                    _container = ApplicationData.Current.RoamingSettings;
                    break;
            }
        }

        public override IDictionary<string, object> GetSettings()
        {
            return _container.Values;
        }

        public override void SetSetting(string key, object value)
        {
            _container.Values[key] = value;
        }

        public override bool Readable => true;

        public override bool Writeable => true;
    }

    public enum ApplicationDataContainerType
    {
        Local, Roaming
    }
}
