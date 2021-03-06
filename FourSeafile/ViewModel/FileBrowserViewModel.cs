﻿using FourSeafile.Extensions;
using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;

namespace FourSeafile.ViewModel
{
    public interface IFileBrowserViewModel
    {
        FileViewModelBase SelectedFolder { get; set; }
        string Address { get; }
        void GoUp();
        void GoBack();
        void NavigateToRoot();
        Task NavigateToAddressAsync(string e);
        void Upload(IStorageFile file);
    }

    public partial class FileBrowserViewModel : ViewModelBase, IFileBrowserViewModel
    {
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

        private void UpdateBackHandler()
        {
            if (CanGoBack) App.CurrentPage.AttachBackHandler(GoBack);
            else App.CurrentPage.DetachBackHandler();
        }
    }
}
