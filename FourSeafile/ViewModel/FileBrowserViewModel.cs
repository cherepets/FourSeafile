using FourSeafile.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace FourSeafile.ViewModel
{
    public partial class FileBrowserViewModel : ViewModelBase
    {
        public List<LibraryViewModel> Libraries
        {
            get
            {
                OnPropertyGet();
                return _libs;
            }
            set
            {
                _libs = value;
                OnPropertyChanged();
            }
        }
        private List<LibraryViewModel> _libs;

        public FileViewModelBase SelectedFolder
        {
            get
            {
                OnPropertyGet();
                return _selectedFolder;
            }
            set
            {
                _selectedFolder = value;
                if (!History.Any() || History.Peek() != value) History.Push(value);
                OnPropertyChanged();
                OnPropertyChanged(nameof(Address));
                UpdateBackHandler();
            }
        }

        private FileViewModelBase _selectedFolder;

        public string Address => SelectedFolder?.LibId != null && App.LibCache != null
            ? $"{App.LibCache[SelectedFolder.LibId].Name}{LocalAddress}"
            : string.Empty;

        private string LocalAddress
        {
            get
            {
                var folder = SelectedFolder as FileViewModel;
                return folder?.Path ?? string.Empty;
            }
        }

        private void UpdateBackHandler()
        {
            if (CanGoBack) App.CurrentPage.AttachBackHandler(GoBack);
            else App.CurrentPage.DetachBackHandler();
        }
    }
}
