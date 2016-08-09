using FourSeafile.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;

namespace FourSeafile.ViewModel.Mock
{
    public partial class FileBrowserViewModelMock : ViewModelBase, IFileBrowserViewModel
    {
        private Stack<FileViewModelBase> History { get; } = new Stack<FileViewModelBase>();

        public FileViewModelBase SelectedFolder
        {
            get { return _selectedFolder; }
            set
            {
                if (SelectedFolder != null)
                    SelectedFolder.PropertyLoadFailed -= SelectedFolder_PropertyLoadFailed;
                _selectedFolder = value;
                SelectedFolder.PropertyLoadFailed += SelectedFolder_PropertyLoadFailed;
                if (!History.Any() || History.Peek() != value) History.Push(value);
                OnPropertyChanged();
                OnPropertyChanged(nameof(Address));
                UpdateBackHandler();
            }
        }
        private FileViewModelBase _selectedFolder;

        public string Address => SelectedFolder?.LibId != null && App.LibCache != null
            ? $"/{App.LibCache[SelectedFolder.LibId].Name}{LocalAddress}"
            : string.Empty;

        private void SelectedFolder_PropertyLoadFailed(object sender, Exception e)
        {
            App.HandleException(e);
            GoBack();
        }

        private string LocalAddress
            => (SelectedFolder as IFileViewModel)?.Path ?? string.Empty;

        private bool CanGoBack => History.Count > 1;

        public void GoUp()
            => SelectedFolder = FileRootViewModelMock.Current;

        public void GoBack()
        {
            History.Pop();
            var prev = History.Pop();
            SelectedFolder = prev;
        }

        public void NavigateToRoot()
            => SelectedFolder = FileRootViewModelMock.Current;

        public async Task NavigateToAddressAsync(string e) => MockHelper.Throw();

        public void Upload(IStorageFile file) => MockHelper.Throw();

        private void UpdateBackHandler()
        {
            if (CanGoBack) App.CurrentPage.AttachBackHandler(GoBack);
            else App.CurrentPage.DetachBackHandler();
        }
    }
}
