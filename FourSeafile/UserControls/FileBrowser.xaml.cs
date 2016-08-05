using FourSeafile.Extensions;
using FourSeafile.Viewers;
using FourSeafile.ViewModel;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;

namespace FourSeafile.UserControls
{
    public sealed partial class FileBrowser
    {
        private const int MinItemWidth = 280;

        private FileBrowserViewModel BrowserVM => DataContext as FileBrowserViewModel;

        public FileBrowser()
        {
            InitializeComponent();
        }
        
        private void FileIconView_Click(object sender, EventArgs e)
        {
            var vm = (sender as FrameworkElement)?.DataContext as FileViewModelBase;
            if (vm == null) return;
            if (vm.IsFolder)
                BrowserVM.SelectedFolder = vm;
            else
            {
                var fvm = vm as FileViewModel;
                if (fvm == null) return;
                var viewer = ViewerFactory.Get(fvm);
                viewer.Open(fvm);
                viewer.NavigateTo();
            }
        }

        private async void Refresh_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var task = ApplicationData.Current.TemporaryFolder.ClearAsync();
                await BrowserVM.SelectedFolder.RefreshContentAsync();
                await task;
            }
            catch (Exception ex)
            {
                App.HandleException(ex);
            }
        }

        private void Up_Click(object sender, RoutedEventArgs e)
            => BrowserVM.GoUp();

        private async void Upload_Click(object sender, RoutedEventArgs e)
        {
            var picker = new FileOpenPicker();
            picker.FileTypeFilter.Add("*");
            var file = await picker.PickSingleFileAsync();
            if (file == null) return;
            StatusBar.Text = $"{file.Name} {Localization.IsUploading}...";
            BrowserVM.Upload(file);
            await Task.Delay(1000);
            StatusBar.Text = null;
        }

        private async void GridView_Drop(object sender, DragEventArgs e)
        {
            var files = await e.DataView.GetStorageItemsAsync();
            foreach (IStorageFile file in files)
                BrowserVM.Upload(file);
        }

        private void FileIconView_Drop(object sender, DragEventArgs e)
        {
            var vm = (sender as FrameworkElement)?.DataContext as FileViewModelBase;
            if (vm == null) return;
            BrowserVM.SelectedFolder = vm;
            GridView_Drop(sender, e);
        }

        private void GridView_DragOver(object sender, DragEventArgs e)
        {
            if (BrowserVM.SelectedFolder is FileViewModel && BrowserVM.SelectedFolder.IsFolder)
                e.AcceptedOperation = DataPackageOperation.Copy | DataPackageOperation.Move;
        }

        private void FileIconView_DragOver(object sender, DragEventArgs e)
        {
            var vm = (sender as FrameworkElement)?.DataContext as FileViewModelBase;
            if (vm == null) return;
            if (vm is FileViewModel && vm.IsFolder)
                e.AcceptedOperation = DataPackageOperation.Copy | DataPackageOperation.Move;
        }

        private async void FileIconView_DragStarting(UIElement sender, DragStartingEventArgs e)
        {
            var deferral = e.GetDeferral();
            var vm = (sender as FrameworkElement)?.DataContext as FileViewModel;
            if (vm == null) return;
            var bp = await BasicPropertiesFactory.GetAsync();
            e.Data.SetStorageItems(new[] { vm.AsStorageFile(bp) }, false);
            deferral.Complete();
        }
    }
}
