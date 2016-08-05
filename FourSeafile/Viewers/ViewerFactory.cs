using FourSeafile.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FourSeafile.Viewers
{
    public static class ViewerFactory
    {
        private static readonly Dictionary<string, Type> _registredTypes = new Dictionary<string, Type>
        {
            { "txt", typeof(TextViewer) },
            { "js", typeof(TextViewer) },
            { "json", typeof(TextViewer) },
            { "cs", typeof(TextViewer) },
            { "html", typeof(TextViewer) },
            { "xml", typeof(TextViewer) },
            { "xaml", typeof(TextViewer) },
            { "php", typeof(TextViewer) },
            { "log", typeof(TextViewer) },
            { "md", typeof(TextViewer) },
        };

        private static readonly Type _default = typeof(DefaultViewer);

        public static IViewer Get(FileViewModel fileVM)
        {
            var type = _default;
            var ext = fileVM.Name.Split('.').Last().ToLowerInvariant();
            if (_registredTypes.ContainsKey(ext))
                type = _registredTypes[ext];
            return (IViewer)Activator.CreateInstance(type);
        }
    }
}
