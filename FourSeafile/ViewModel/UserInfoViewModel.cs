using System.Threading.Tasks;

namespace FourSeafile.ViewModel
{
    public class UserInfoViewModel : ViewModelBase
    {
        public string Nickname
        {
            get
            {
                OnPropertyGet();
                return _nickname;
            }
            private set
            {
                _nickname = value;
                OnPropertyChanged();
            }
        }
        private string _nickname;

        public string Avatar
        {
            get
            {
                OnPropertyGet();
                return _avatar;
            }
            private set
            {
                _avatar = value;
                OnPropertyChanged();
            }
        }
        private string _avatar;

        protected override async Task LoadAsync()
        {
            var info = await App.Seafile.CheckAccountInfo();
            var avatar = await App.Seafile.GetUserAvatar(128);
            Nickname = info.Nickname ?? info.Email;
            Avatar = avatar.Url;
        }
    }
}
