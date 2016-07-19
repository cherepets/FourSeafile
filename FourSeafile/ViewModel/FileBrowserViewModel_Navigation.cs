using FourSeafile.Component;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;

namespace FourSeafile.ViewModel
{
    public partial class FileBrowserViewModel : ViewModelBase
    {
        private Stack<FileViewModelBase> History { get; } = new Stack<FileViewModelBase>();
        public static event EventHandler<StructContainer<double>> UploadStarted;

        private bool CanGoBack => History.Count > 1;

        public void GoUp()
        {
            var parent = SelectedFolder.Parent;
            if (parent != null) SelectedFolder = parent;
        }

        public void GoBack()
        {
            History.Pop();
            var prev = History.Pop();
            SelectedFolder = prev;
        }

        public void NavigateToRoot()
            => SelectedFolder = FileRootViewModel.Current;

        public async Task NavigateToAddressAsync(string e)
        {
            if (string.IsNullOrWhiteSpace(e)) return;
            e = e.Replace("\\", "/");
            var path = e.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            if (!path.Any())
            {
                NavigateToRoot();
                return;
            }
            var lib = App.LibCache.Values.FirstOrDefault(l => l.Name == path[0]);
            if (lib == null) return;
            path.RemoveAt(0);
            var vm = (FileViewModelBase)new LibraryViewModel(lib);
            while (path.Any() && vm.IsFolder)
            {
                await vm.RefreshContentAsync();
                var file = vm.Files.FirstOrDefault(f => f.Name == path[0]);
                if (file != null) vm = file;
                else break;
                path.RemoveAt(0);
            }
            if (vm.IsFolder) SelectedFolder = vm;
        }

        public async void Upload(IStorageFile file)
        {
            var sf = file as SeaStorageFile;
            if (sf != null) await UploadSeaStorageFileAsync(sf);
            else await UploadStorageFileAsync(file);
            SelectedFolder.RefreshContent();
        }

        private async Task UploadStorageFileAsync(IStorageFile file)
        {
            if (string.IsNullOrEmpty(SelectedFolder.LibId)) return;
            var lib = App.LibCache[SelectedFolder.LibId];
            Stream stream = null;
            try
            {
                stream = await file.OpenStreamForReadAsync();
            }
            catch (UnauthorizedAccessException)
            {
                await Warning.ShowAsync("Cannot access file: " + file.Name);
                return;
            }
            try
            {
                var progress = new StructContainer<double>();
                UploadStarted?.Invoke(file, progress);
                var result = await App.Seafile.UploadSingle(lib, LocalAddress, file.Name, stream, f => progress.Value = f);
            }
            catch
            {
                await Warning.ShowAsync("Cannot upload file: " + file.Name);
            }
        }

        private async Task UploadSeaStorageFileAsync(SeaStorageFile file)
        {
            if (string.IsNullOrEmpty(SelectedFolder.LibId)) return;
            var lib = App.LibCache[SelectedFolder.LibId];
            var vm = file.Implementation as FileViewModel;
            if (vm == null) return;
            await App.Seafile.CopyFile(App.LibCache[vm.LibId], vm.Path, lib, LocalAddress);
        }
    }
}
