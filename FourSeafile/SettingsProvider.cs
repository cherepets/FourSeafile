using System.Collections.Generic;
using Windows.Storage;

namespace FourSeafile
{
    public abstract class SettingsProvider
    {
        protected ApplicationDataContainer Container { get; set; }
        public abstract IDictionary<string, object> GetSettings();
        public abstract void SetSetting(string key, object value);
        public abstract bool Readable { get; }
        public abstract bool Writeable { get; }

        public virtual bool Exists(string key) => Container.Values.ContainsKey(key) && Container.Values[key] != null;
    }
}
