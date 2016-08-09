using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace FourSeafile.ViewModel.Mock
{
    public class FileRootViewModelMock : FileViewModelBase
    {
        public override bool IsFolder => true;
        public override string Name => "";
        public override string LibId => null;
        public string Path => null;
        public override bool CanUpload => false;

        public override FileViewModelBase Parent => null;
        public override IconViewModel Icon => IconViewModel.LibraryIcon;

        public override List<FileViewModelBase> Files
        {
            get
            {
                OnPropertyGet();
                return _files;
            }
            protected set
            {
                _files = value;
                OnPropertyChanged();
            }
        }
        private List<FileViewModelBase> _files;
        
        private FileRootViewModelMock() { }

        public static FileRootViewModelMock Current => _current ?? (_current = new FileRootViewModelMock());
        private static FileRootViewModelMock _current;

        public static void Reset()
            => _current = null;

        protected override async Task LoadAsync()
        {
            App.LibCache = new Dictionary<string, SeafClient.Types.SeafLibrary>
            {
                { "1", new SeafClient.Types.SeafLibrary { Id = "1", Name = "Test", Timestamp = DateTime.Now } },
                { "2", new SeafClient.Types.SeafLibrary { Id = "2", Name = "Encrypted", Timestamp = DateTime.Now, Encrypted = true } }
            };
            Files = App.LibCache.Values.Select(l => (FileViewModelBase)new LibraryViewModelMock(l)).ToList();
        }
    }
}