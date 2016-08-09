using FourSeafile.UserControls;
using SeafClient.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourSeafile.ViewModel.Mock
{
    public class LibraryViewModelMock : FileViewModelBase
    {
        public override bool IsFolder => true;
        public override string Name => _lib.Name;
        public override string LibId => _lib.Id;
        public override FileViewModelBase Parent => FileRootViewModel.Current;
        public override string Info => _lib.Timestamp.ToString();
        public override IconViewModel Icon => IconViewModel.LibraryIcon;
        public override bool CanUpload => false;

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

        private SeafLibrary _lib;

        public LibraryViewModelMock(SeafLibrary lib)
        {
            _lib = lib;
        }

        protected override async Task LoadAsync()
        {
            var allowed = true;
            if (_lib.Encrypted)
            {
                var dialog = new PasswordInputDialog();
                allowed = false;
                await dialog.ShowAsync();
                if (dialog.Result)
                {
                    var password = dialog.Password;
                    if (!string.IsNullOrEmpty(password))
                        allowed = password == "123";
                }
                if (!allowed)
                    throw new UnauthorizedAccessException(Localization.CantDecrypt);
            }
            if (allowed)
            {
                var dirs = new List<SeafDirEntry>
                {
                    new SeafDirEntry { Id = "0", LibraryId = _lib.Id, Name = "Text.txt", Size = 1024 },
                    new SeafDirEntry { Id = "1", LibraryId = _lib.Id, Name = "Image1.png", Size = 4096 },
                    new SeafDirEntry { Id = "2", LibraryId = _lib.Id, Name = "Image2.png", Size = 4096 },
                    new SeafDirEntry { Id = "3", LibraryId = _lib.Id, Name = "Image3.png", Size = 4096 },
                };
                Files = dirs.Select(f => (FileViewModelBase)new FileViewModelMock(this, f)).ToList();
            }
            else
            {
                Files = new List<FileViewModelBase>();
            }
        }
    }
}
