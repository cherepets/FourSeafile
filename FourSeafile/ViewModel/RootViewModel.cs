namespace FourSeafile.ViewModel
{
    public class RootViewModel : ViewModelBase
    {
        public UserInfoViewModel UserInfo
        {
            get
            {
                OnPropertyGet();
                return _userInfo ?? (_userInfo = new UserInfoViewModel());
            }
            set
            {
                _userInfo = value;
                OnPropertyChanged();
            }
        }
        private UserInfoViewModel _userInfo;

        public FileBrowserViewModel FileBrowser
        {
            get
            {
                OnPropertyGet();
                return _fileBrowser ?? (_fileBrowser = new FileBrowserViewModel { SelectedFolder = FileRootViewModel.Current });
            }
            set
            {
                _fileBrowser = value;
                OnPropertyChanged();
            }
        }
        private FileBrowserViewModel _fileBrowser;

        public ProgressViewModel Progress
        {
            get
            {
                OnPropertyGet();
                return _progress ?? (_progress = new ProgressViewModel());
            }
            set
            {
                _progress = value;
                OnPropertyChanged();
            }
        }
        private ProgressViewModel _progress;
    }
}