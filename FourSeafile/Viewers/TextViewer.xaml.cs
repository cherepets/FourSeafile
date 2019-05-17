using FourSeafile.ViewModel;
using System;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;

namespace FourSeafile.Viewers
{
    public sealed partial class TextViewer : IViewer
    {
        private IFileViewModel _fileVM;
        private IStorageFile _file;
        private bool _modified;
        private bool _ctrl;

        public TextViewer()
        {
            InitializeComponent();
        }

        public async void Open(IFileViewModel fileVM)
        {
            _fileVM = fileVM;
            Header.Text = _fileVM.ToString();
            _file = await _fileVM.DownloadAsync(ApplicationData.Current.TemporaryFolder);
            var text = await FileIO.ReadTextAsync(_file);
            Editor.Document.SetText(TextSetOptions.None, text);
            IsEnabled = true;
            Editor.Focus(FocusState.Programmatic);
        }

        public async Task<bool> CanClose()
        {
            if (!_modified) return true;
            return await Confirmation.ShowAsync();
        }

        private async Task SaveAsync()
        {
            IsEnabled = false;
            try
            {
                Editor.Document.GetText(TextGetOptions.AdjustCrlf, out string text);
                var task = FileIO.WriteTextAsync(_file, text, Windows.Storage.Streams.UnicodeEncoding.Utf8);
                var content = Encoding.UTF8.GetBytes(text);
                var result = await _fileVM.UploadContentAsync(content);
                if (result)
                {
                    _modified = false;
                }
                else
                {
                    await Warning.ShowAsync($"{Localization.CantUpload}: {_fileVM.Name}");
                }
                await task;
            }
            finally
            {
                IsEnabled = true;
                Editor.Focus(FocusState.Programmatic);
            }
        }

        private async void Save_Click(object sender, RoutedEventArgs e) => await SaveAsync();

        private async void SaveLocal_Click(object sender, RoutedEventArgs e)
        {
            IsEnabled = false;
            try
            {
                var folder = await PickFolderAsync(_file.FileType);
                if (folder == null) return;
                Editor.Document.GetText(TextGetOptions.AdjustCrlf, out string text);
                var file = await folder.CreateFileAsync(_fileVM.Name, CreationCollisionOption.ReplaceExisting);
                await FileIO.WriteTextAsync(file, text, Windows.Storage.Streams.UnicodeEncoding.Utf8);
            }
            catch (Exception ex)
            {
                App.HandleException(ex);
            }
            finally
            {
                IsEnabled = true;
                Editor.Focus(FocusState.Programmatic);
            }
        }

        private void Editor_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Control)
                _ctrl = true;
        }

        private async void Editor_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Control)
            {
                _ctrl = false;
                e.Handled = true;
                return;
            }
            if (e.Key == Windows.System.VirtualKey.S && _ctrl)
            {
                _ctrl = false;
                await SaveAsync();
                e.Handled = true;
                return;
            }
            _modified = true;
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
    }
}
