using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;

namespace FourSeafile.Extensions
{
    public static class StorageFolderExt
    {
        public static async Task ClearAsync(this IStorageFolder folder)
        {
            var items = await folder.GetItemsAsync();
            var tasks = new List<IAsyncAction>();
            foreach (var item in items)
                tasks.Add(item.DeleteAsync(StorageDeleteOption.PermanentDelete));
            foreach (var task in tasks)
                await task;
        }
    }
}
