using FourSeafile.ViewModel;
using System;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;

namespace FourSeafile.UserControls
{
    public sealed partial class FileIconView
    {
        public event EventHandler Click;

        private FileViewModelBase FileVM => DataContext as FileViewModelBase;

        public FileIconView()
        {
            InitializeComponent();
            IsTabStop = true;
            KeyUp += (s, e) =>
            {
                if (CheckKey(e)) OnClick();
            };
            GotFocus += (s, e) =>
            {
                Ants.Visibility = Visibility.Visible;
                Overlay.Visibility = Visibility.Visible;
            };
            LostFocus += (s, e) =>
            {
                Ants.Visibility = Visibility.Collapsed;
                Overlay.Visibility = Visibility.Collapsed;
            };
            PointerEntered += (s, e) => Focus(FocusState.Pointer);
            Tapped += (s, e) => OnClick();
        }

        #region Conext Menu
        private void Grid_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            var element = sender as FrameworkElement;
            var flyout = FlyoutBase.GetAttachedFlyout(element);
            flyout.ShowAt(element);
        }

        private void OpenButton_Click(object sender, RoutedEventArgs e)
            => OnClick();

        private async void DownloadButton_Click(object sender, RoutedEventArgs e)
        {
            var fvm = FileVM as FileViewModel;
            var file = fvm.AsStorageFile();
            var folder = await PickFolderAsync(file.FileType);
            if (folder != null)
                await file.CopyAsync(folder, file.Name, NameCollisionOption.ReplaceExisting);
        }

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var fvm = FileVM as FileViewModel;
            var file = fvm.AsStorageFile();
            if (await Confirmation.ShowAsync($"{Localization.Delete} {file.Name}?"))
            {
                await file.DeleteAsync();
                await fvm.Parent.RefreshContentAsync();
            }
        }
        #endregion

        private static bool CheckKey(KeyRoutedEventArgs e)
            => e.Key == VirtualKey.Enter || e.Key == VirtualKey.Space;

        private async void OnClick()
        {
            StatusBar.Text = $"{FileVM.Name} {Localization.IsDownloading}...";
            Click?.Invoke(this, null);
            await Task.Delay(1000);
            StatusBar.Text = null;
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
