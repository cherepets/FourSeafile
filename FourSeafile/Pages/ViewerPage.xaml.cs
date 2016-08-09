using FourSeafile.Extensions;
using FourSeafile.Viewers;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace FourSeafile.Pages
{
    public sealed partial class ViewerPage
    {
        public ViewerPage()
        {
            InitializeComponent();
            this.StashHandler();
            this.AttachBackHandler(async () =>
            {
                var viewer = Content as IViewer;
                if (viewer == null || await viewer.CanClose())
                {
                    MainPage.ForceReload = false;
                    App.Frame.GoBack();
                    this.DetachBackHandler();
                    this.UnstashHandler();
                }
            });
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is UserControl)
                SetViewer((UserControl)e.Parameter);
            base.OnNavigatedTo(e);
        }

        public void SetViewer(UserControl uc)
        {
            if (uc is IViewer)
                Content = uc;
        }
    }
}
