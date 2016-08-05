using FourSeafile.Pages;
using FourSeafile.ViewModel;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace FourSeafile.Viewers
{
    public interface IViewer
    {
        void Open(FileViewModel fileVM);
        Task<bool> CanClose();
    }

    public static class IViewerExt
    {
        public static void NavigateTo(this IViewer viewer)
        {
            if (viewer is UserControl)
                App.Frame.Navigate(typeof(ViewerPage), (UserControl)viewer);
        }
    }
}
