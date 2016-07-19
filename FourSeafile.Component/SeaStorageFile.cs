using FourSeafile.Component.Extensions;
using System;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Streams;

namespace FourSeafile.Component
{
    public sealed class SeaStorageFile : IStorageFile
    {
        public IFileComponent Implementation { get; }
        private BasicProperties _bp;

        public SeaStorageFile(IFileComponent ifk, BasicProperties bp)
        {
            Implementation = ifk;
            _bp = bp;
        }

        public FileAttributes Attributes => FileAttributes.ReadOnly;

        public string ContentType => "application/octet-stream";

        public DateTimeOffset DateCreated => DateTime.UtcNow;

        public string FileType => Name.Split('.').LastOrDefault();

        public string Name => Implementation.Name;

        public string Path => Implementation.Name;

        public IAsyncAction CopyAndReplaceAsync(IStorageFile fileToReplace)
        {
            throw new NotImplementedException();
        }

        public IAsyncOperation<StorageFile> CopyAsync(IStorageFolder destinationFolder)
            => CopyAsync(destinationFolder, Implementation.Name, default(NameCollisionOption));

        public IAsyncOperation<StorageFile> CopyAsync(IStorageFolder destinationFolder, string desiredNewName)
            => CopyAsync(destinationFolder, desiredNewName, default(NameCollisionOption));

        public IAsyncOperation<StorageFile> CopyAsync(IStorageFolder destinationFolder, string desiredNewName, NameCollisionOption option)
            => CopyAsTask(destinationFolder, desiredNewName, option).AsAsyncOperation();

        private async Task<StorageFile> CopyAsTask(IStorageFolder destinationFolder, string desiredNewName, NameCollisionOption option)
        {
            var link = await Implementation.GetDownloadLinkAsyncOperation();
            var client = new HttpClient();
            var stream = await client.GetStreamAsync(link);
            var file = await destinationFolder.CreateFileAsync(desiredNewName, option.ToCreationOption());
            await file.WriteFromStreamAsync(stream);
            return file;
        }

        public IAsyncAction DeleteAsync()
            => DeleteAsTask().AsAsyncAction();

        public IAsyncAction DeleteAsync(StorageDeleteOption option)
            => DeleteAsTask().AsAsyncAction();

        private async Task DeleteAsTask()
            => await Implementation.DeleteAsyncAction();

        public IAsyncOperation<BasicProperties> GetBasicPropertiesAsync()
            => GetBasicPropertiesAsTask().AsAsyncOperation();

        private async Task<BasicProperties> GetBasicPropertiesAsTask()
        {
            var b = typeof(BasicProperties).GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance);
            return _bp;
        }

        public bool IsOfType(StorageItemTypes type)
        {
            throw new NotImplementedException();
        }

        public IAsyncAction MoveAndReplaceAsync(IStorageFile fileToReplace)
        {
            throw new NotImplementedException();
        }

        public IAsyncAction MoveAsync(IStorageFolder destinationFolder)
        {
            throw new NotImplementedException();
        }

        public IAsyncAction MoveAsync(IStorageFolder destinationFolder, string desiredNewName)
        {
            throw new NotImplementedException();
        }

        public IAsyncAction MoveAsync(IStorageFolder destinationFolder, string desiredNewName, NameCollisionOption option)
        {
            throw new NotImplementedException();
        }

        public IAsyncOperation<IRandomAccessStream> OpenAsync(FileAccessMode accessMode)
        {
            throw new NotImplementedException();
        }

        public IAsyncOperation<IRandomAccessStreamWithContentType> OpenReadAsync()
        {
            throw new NotImplementedException();
        }

        public IAsyncOperation<IInputStream> OpenSequentialReadAsync()
        {
            throw new NotImplementedException();
        }

        public IAsyncOperation<StorageStreamTransaction> OpenTransactedWriteAsync()
        {
            throw new NotImplementedException();
        }

        public IAsyncAction RenameAsync(string desiredName)
        {
            throw new NotImplementedException();
        }

        public IAsyncAction RenameAsync(string desiredName, NameCollisionOption option)
        {
            throw new NotImplementedException();
        }
    }
}
