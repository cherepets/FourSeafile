using FourSeafile.UserControls;
using SeafClient;
using SeafClient.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace FourSeafile.ViewModel
{
    public class LibraryViewModel : FileViewModelBase
    {
        public override bool IsFolder => true;
        public override string Name => _lib.Name;
        public override string LibId => _lib.Id;
        public override FileViewModelBase Parent => FileRootViewModel.Current;
        public override string Info => _lib.Timestamp.ToString();
        public override IconViewModel Icon => IconViewModel.LibraryIcon;
        public override bool CanUpload => true;

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

        private SeafLibrary _lib;

        public LibraryViewModel(SeafLibrary lib)
        {
            _lib = lib;
        }

        protected override async Task LoadAsync()
        {
            if (_lib.Encrypted)
            {
                var dialog = new PasswordInputDialog();
                var result = await dialog.ShowAsync();
                if (result == ContentDialogResult.Primary)
                {
                    var password = dialog.Password;
                    if (!string.IsNullOrEmpty(password))
                    {
                        try
                        {
                            await App.Seafile.DecryptLibrary(_lib, password.ToCharArray());
                        }
                        catch (SeafException exception)
                        {
                            App.HandleException(exception);
                        }
                    }
                }
            }
            var dirs = await App.Seafile.ListDirectory(_lib);
            Files = dirs.Select(f => (FileViewModelBase)new FileViewModel(this, f)).ToList();
        }
    }
}
