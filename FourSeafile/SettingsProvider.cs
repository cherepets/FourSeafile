using System.Collections.Generic;

namespace FourSeafile
{
    public abstract class SettingsProvider
    {
        public abstract IDictionary<string, object> GetSettings();
        public abstract void SetSetting(string key, object value);
        public abstract bool Readable { get; }
        public abstract bool Writeable { get; }
    }
}
