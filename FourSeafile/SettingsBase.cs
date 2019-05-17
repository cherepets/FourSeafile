using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace FourSeafile
{
    public abstract class SettingsBase
    {
        protected SettingsProvider SettingsProvider
        {
            get { return _provider; }
            set
            {
                _provider = value;
                _dictionary = null;
            }
        }

        private IDictionary<string, object> Dictionary
        {
            get
            {
                if (_dictionary == null) RefreshDictionary();
                return _dictionary;
            }
        }
        private IDictionary<string, object> _dictionary;

        private SettingsProvider _provider;

        public bool Exists(string key) => _provider.Exists(key);

        protected T GetProperty<T>([CallerMemberName] string property = null)
        {
            if (!Dictionary.ContainsKey(property))
                Dictionary.Add(property, default(T));
            if (!(Dictionary[property] is T)
                && (Dictionary[property] != null || typeof(T).GetTypeInfo().IsValueType))
            {
                Debug.WriteLine(property + " is not " + typeof(T).Name);
                return default(T);
            }
            return (T)Dictionary[property];
        }

        protected void SetProperty<T>(T value, [CallerMemberName] string property = null)
        {
            if (_provider != null && _provider.Writeable)
                _provider.SetSetting(property, value);
            Dictionary[property] = value;
        }

        private void RefreshDictionary()
        {
            _dictionary = new Dictionary<string, object>();
            if (_provider != null && _provider.Readable)
                foreach (var pair in _provider.GetSettings())
                    _dictionary[pair.Key] = pair.Value;
        }
    }
}