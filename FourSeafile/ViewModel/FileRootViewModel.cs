using FourSeafile.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace FourSeafile.ViewModel
{
    public class FileRootViewModel : FileViewModelBase
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
            set
            {
                _files = value;
                OnPropertyChanged();
            }
        }
        private List<FileViewModelBase> _files;
        
        private FileRootViewModel() { }

        public static FileRootViewModel Current => _current ?? (_current = new FileRootViewModel());
        private static FileRootViewModel _current;

        public static void Reset()
            => _current = null;

        protected override async Task LoadAsync()
        {
            var libs = await App.Seafile.ListLibraries();
            App.LibCache = libs.Distinct(l => l.Id).ToDictionary(l => l.Id, l => l);
            Files = libs.Select(l => (FileViewModelBase)new LibraryViewModel(l)).ToList();
        }
    }
}