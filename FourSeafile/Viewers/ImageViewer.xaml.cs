using System;
using System.Threading.Tasks;
using FourSeafile.ViewModel;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml;
using System.Linq;
using FourSeafile.Extensions;
using Windows.Storage.Pickers;
using Windows.Storage;
using System.Collections.Generic;

namespace FourSeafile.Viewers
{
    public sealed partial class ImageViewer : IViewer
    {
        private IFileViewModel _fileVM;
        private List<FileViewModelBase> _files;
        private readonly string[] _exts = new[]
        {
            "bmp", "png", "gif", "jpg", "jpeg"
        };

        public ImageViewer()
        {
            InitializeComponent();
        }

        public Task<bool> CanClose() => Task.FromResult(true);

        public async void Open(IFileViewModel fileVM)
        {
            _fileVM = fileVM;
            _files = _fileVM.Parent.Files.Where(f => f.Name.HasExt(_exts)).ToList();
            foreach (var file in _files)
            {
                FlipView.Items.Add(await CreateContentAsync((IFileViewModel)file));
            }
            FlipView.SelectedIndex = _files.IndexOf((FileViewModelBase)fileVM);
            FlipView.Focus(FocusState.Programmatic);
        }

        private async Task<Image> CreateContentAsync(IFileViewModel file)
        {
            return new Image
            {
                Source = new BitmapImage(new Uri(await file.GetDownloadLinkAsync()))
            };
        }

        private async void SaveLocal_Click(object sender, RoutedEventArgs e)
        {
            IsEnabled = false;
            try
            {
                var file = (IFileViewModel)_files[FlipView.SelectedIndex];
                var folder = await PickFolderAsync(file.AsStorageFile().FileType);
                if (folder == null) return;
                await file.DownloadAsync(folder);
            }
            catch (Exception ex)
            {
                App.HandleException(ex);
            }
            finally
            {
                IsEnabled = true;
                FlipView.Focus(FocusState.Programmatic);
            }
        }

        private static async Task<StorageFolder> PickFolderAsync(string ext)
        {
            var picker = new FolderPicker
            {
                SuggestedStartLocation = PickerLocationId.Downloads
            };
            picker.FileTypeFilter.Add(ext);
            return await picker.PickSingleFolderAsync();
        }

        private void FlipView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var file = _files[FlipView.SelectedIndex];
            Header.Text = file.ToString();
        }
    }
}
