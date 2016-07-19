using System.Collections.Generic;
using SeafClient.Types;
using System.Threading.Tasks;
using System.Linq;
using FourSeafile.Extensions;
using System;

namespace FourSeafile.ViewModel
{
    public partial class FileViewModel : FileViewModelBase
    {
        public override bool IsFolder => Type == FileType.Folder;
        public override string Name => _entry.Name;
        public override string LibId => _entry.LibraryId;
        public string Path => _entry.Path;
        public override bool CanUpload => IsFolder;
        public override FileViewModelBase Parent { get; }
        public override string Info => _entry.Timestamp.ToString();
        public override IconViewModel Icon
        {
            get
            {
                if (IsFolder) return IconViewModel.FolderIcon;
                if (Name.HasExt(new[] { "bmp", "png", "gif", "tiff", "jpg", "jpeg", "ico", "swg" })) return IconViewModel.PictureIcon;
                if (Name.HasExt(new[] { "mp3", "wav", "flac", "amr", "ogg" })) return IconViewModel.MusicIcon;
                if (Name.HasExt(new[] { "avi", "mov", "wmv", "flv", "mp4", "mpeg", "mpg", "mkv" })) return IconViewModel.VideoIcon;
                if (Name.HasExt(new[] { "doc", "docx", "rtf", "pdf" , "txt" })) return IconViewModel.DocumentIcon;
                if (Name.HasExt(new[] { "xls", "xlsx" })) return IconViewModel.TableIcon;
                if (Name.HasExt(new[] { "exe", "appx", "apk", "ipa", "swf", "xap" })) return IconViewModel.ExecIcon;
                if (Name.HasExt(new[] { "html", "htm" })) return IconViewModel.WebIcon;
                return IconViewModel.DefaultFileIcon;
            }
        }

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

        public FileType Type
        {
            get
            {
                OnPropertyGet();
                return _type;
            }
            set
            {
                _type = value;
                OnPropertyChanged();
            }
        }
        private FileType _type;

        private SeafDirEntry _entry;

        public FileViewModel(FileViewModelBase parent, SeafDirEntry entry)
        {
            Parent = parent;
            _entry = entry;
            Type = entry.Type == DirEntryType.File
                ? FileType.Unknown
                : FileType.Folder;
        }

        protected override async Task LoadAsync()
        {
            if (Type != FileType.Folder) return;
            var lib = App.LibCache[_entry.LibraryId];
            var dirs = await App.Seafile.ListDirectory(lib, _entry.Path);
            Files = dirs.Select(f => (FileViewModelBase)new FileViewModel(this, f)).ToList();
        }
    }

    public enum FileType
    {
        Folder, Unknown
    }
}