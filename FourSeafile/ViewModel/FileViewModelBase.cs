using System.Collections.Generic;
using System.Threading.Tasks;

namespace FourSeafile.ViewModel
{
    public abstract class FileViewModelBase : ViewModelBase
    {
        public abstract List<FileViewModelBase> Files { get; set; }
        public abstract FileViewModelBase Parent { get; }
        public abstract string Name { get; }
        public abstract string LibId { get; }
        public abstract bool IsFolder { get; }
        public abstract bool CanUpload { get; }

        public virtual string Info => string.Empty;
        public virtual IconViewModel Icon => null;

        public bool IsFile => !IsFolder;
        public bool CanGoUp => Parent != null;
        public async Task RefreshContentAsync() => await LoadAsync();
        public async void RefreshContent() => await RefreshContentAsync();

        public override string ToString() => $"{App.LibCache[LibId]}/../{Name}";
    }
}
