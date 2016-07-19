using FourSeafile.Component;
using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.Networking.BackgroundTransfer;
using Windows.Storage;
using Windows.Foundation;
using Windows.Storage.FileProperties;

namespace FourSeafile.ViewModel
{
    public partial class FileViewModel : IFileComponent
    {
        public static event EventHandler<DownloadOperation> DownloadStarted;

        public async Task<StorageFile> DownloadAsync(StorageFolder folder)
            => await DownloadAsync(folder, await GetDownloadLinkAsync());

        public async Task<string> GetDownloadLinkAsync()
            => await App.Seafile.GetFileDownloadLink(App.LibCache[LibId], Path);

        public IAsyncOperation<string> GetDownloadLinkAsyncOperation()
            => GetDownloadLinkAsync().AsAsyncOperation();

        public async Task DeleteAsync()
        {
            if (IsFile)
                await App.Seafile.DeleteFile(App.LibCache[LibId], Path);
            else
                await App.Seafile.DeleteDirectory(App.LibCache[LibId], Path);
        }

        public IAsyncAction DeleteAsyncAction()
            => DeleteAsync().AsAsyncAction();

        public IStorageFile AsStorageFile(BasicProperties bp = null)
            => new SeaStorageFile(this, bp);

        private static async Task<StorageFile> DownloadAsync(StorageFolder folder, string query)
        {
            try
            {
                if (string.IsNullOrEmpty(query)) return null;
                var name = Uri.UnescapeDataString(query.Split('/').Last());
                StorageFile file = null;
                file = await folder.TryGetItemAsync(name) as StorageFile;
                if (file != null) return file;
                file = await folder.CreateFileAsync(name, CreationCollisionOption.ReplaceExisting);
                var downloader = new BackgroundDownloader();
                var download = downloader.CreateDownload(new Uri(query), file);
                var process = download.StartAsync();
                DownloadStarted?.Invoke(null, download);
                await process;
                return download.Progress.Status == BackgroundTransferStatus.Completed
                    ? file
                    : null;
            }
            catch
            {
                return null;
            }
        }
    }
}