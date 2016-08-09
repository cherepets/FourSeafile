namespace FourSeafile.ViewModel.Mock
{
    public class RootViewModelMock : IRootViewModel
    {
        public IUserInfoViewModel UserInfo
            => _userInfo ?? (_userInfo = new UserInfoViewModelMock());
        private IUserInfoViewModel _userInfo;

        public IFileBrowserViewModel FileBrowser
            => _fileBrowser ?? (_fileBrowser = new FileBrowserViewModelMock { SelectedFolder = FileRootViewModelMock.Current });
        private IFileBrowserViewModel _fileBrowser;

        public ProgressViewModel Progress
            => _progress ?? (_progress = new ProgressViewModel());
        private ProgressViewModel _progress;
    }
}