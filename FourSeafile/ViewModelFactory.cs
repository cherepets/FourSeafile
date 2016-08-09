using FourSeafile.ViewModel;
using FourSeafile.ViewModel.Mock;

namespace FourSeafile
{
    public static class ViewModelFactory
    {
        public static IRootViewModel CreateRoot()
            => App.Demo
            ? new RootViewModelMock()
            : (IRootViewModel)new RootViewModel();
    }
}
