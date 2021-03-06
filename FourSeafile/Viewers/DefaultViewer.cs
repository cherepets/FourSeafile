﻿using FourSeafile.Extensions;
using FourSeafile.ViewModel;
using System.Threading.Tasks;
using Windows.Storage;

namespace FourSeafile.Viewers
{
    public class DefaultViewer : IViewer
    {
        public Task<bool> CanClose() => Task.FromResult(true);

        public async void Open(IFileViewModel fileVM)
        {
            var file = await fileVM.DownloadAsync(ApplicationData.Current.TemporaryFolder);
            file?.LaunchAsync();
        }
    }
}
