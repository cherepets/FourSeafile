using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.System;

namespace FourSeafile.Component.Extensions
{
    internal static class StorageFileExt
    {
        private const int BufferSize = 1024;

        public static async Task<bool> LaunchAsync(this IStorageFile file)
            => await Launcher.LaunchFileAsync(file);

        public static async Task WriteFromStreamAsync(this IStorageFile file, Stream stream)
        {
            using (var fileStram = await file.OpenStreamForWriteAsync())
            {
                var buffer = new byte[BufferSize];
                var read = 0;
                while ((read = await stream.ReadAsync(buffer, 0, BufferSize)) > 0)
                    await fileStram.WriteAsync(buffer, 0, read);
            }
        }
    }
}
