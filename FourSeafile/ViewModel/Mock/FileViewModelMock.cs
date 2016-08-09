using System.Collections.Generic;
using SeafClient.Types;
using FourSeafile.Extensions;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.FileProperties;
using System;
using Windows.ApplicationModel;
using FourSeafile.Component;

namespace FourSeafile.ViewModel.Mock
{
    public partial class FileViewModelMock : FileViewModelBase, IFileViewModel, IFileComponent
    {
        public override bool IsFolder => false;
        public override string Name => _entry.Name;
        public override string LibId => _entry.LibraryId;
        public string Path => _entry.Path;
        public override bool CanUpload => IsFolder;
        public override FileViewModelBase Parent { get; }
        public override string Info => _entry.Size.ToSizeString();
        public override IconViewModel Icon
        {
            get
            {
                if (IsFolder) return IconViewModel.FolderIcon;
                if (Name.HasExt(new[] { "bmp", "png", "gif", "tiff", "jpg", "jpeg", "ico", "svg" })) return IconViewModel.PictureIcon;
                if (Name.HasExt(new[] { "mp3", "wav", "flac", "amr", "ogg" })) return IconViewModel.MusicIcon;
                if (Name.HasExt(new[] { "avi", "mov", "wmv", "flv", "mp4", "mpeg", "mpg", "mkv" })) return IconViewModel.VideoIcon;
                if (Name.HasExt(new[] { "doc", "docx", "rtf", "pdf", "txt" })) return IconViewModel.DocumentIcon;
                if (Name.HasExt(new[] { "xls", "xlsx" })) return IconViewModel.TableIcon;
                if (Name.HasExt(new[] { "exe", "appx", "apk", "ipa", "swf", "xap" })) return IconViewModel.ExecIcon;
                if (Name.HasExt(new[] { "html", "htm" })) return IconViewModel.WebIcon;
                return IconViewModel.DefaultFileIcon;
            }
        }

        public override List<FileViewModelBase> Files { get; protected set; }

        public FileType Type => FileType.Unknown;

        private SeafDirEntry _entry;

        public FileViewModelMock(FileViewModelBase parent, SeafDirEntry entry)
        {
            Parent = parent;
            _entry = entry;
        }

        public async Task<StorageFile> DownloadAsync(StorageFolder folder)
        {
            StorageFile file = null;
            file = await folder.TryGetItemAsync(Name) as StorageFile;
            if (file != null) return file;
            var installLocaltion = await Package.Current.InstalledLocation.GetFolderAsync(@"ViewModel\Mock\Files");
            var newFile = await installLocaltion.GetFileAsync(Name);
            file = await newFile.CopyAsync(folder, Name, NameCollisionOption.ReplaceExisting);
            return file;
        }

        public async Task<string> GetDownloadLinkAsync()
        {
            var file = await DownloadAsync(ApplicationData.Current.TemporaryFolder);
            return file.Path;
        }

        public IAsyncOperation<string> GetDownloadLinkAsyncOperation()
            => GetDownloadLinkAsync().AsAsyncOperation();

        public async Task<bool> UploadContentAsync(byte[] content)
        {
            MockHelper.Throw();
            return false;
        }

        public async Task DeleteAsync()
            => MockHelper.Throw();

        public IAsyncAction DeleteAsyncAction()
            => DeleteAsync().AsAsyncAction();

        public IStorageFile AsStorageFile(BasicProperties bp = null)
            => new SeaStorageFile(this, bp);
    }
}