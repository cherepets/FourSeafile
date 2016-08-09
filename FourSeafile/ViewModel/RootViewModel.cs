namespace FourSeafile.ViewModel
{
    public interface IRootViewModel
    {
        IUserInfoViewModel UserInfo { get; }
        IFileBrowserViewModel FileBrowser { get; }
        ProgressViewModel Progress { get; }
    }

    public class RootViewModel : IRootViewModel
    {
        public IUserInfoViewModel UserInfo
            => _userInfo ?? (_userInfo = new UserInfoViewModel());
        private IUserInfoViewModel _userInfo;

        public IFileBrowserViewModel FileBrowser
            => _fileBrowser ?? (_fileBrowser = new FileBrowserViewModel { SelectedFolder = FileRootViewModel.Current });
        private IFileBrowserViewModel _fileBrowser;

        public ProgressViewModel Progress
            => _progress ?? (_progress = new ProgressViewModel());
        private ProgressViewModel _progress;
    }
}