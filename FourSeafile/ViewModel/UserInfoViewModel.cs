using FourSeafile.Extensions;
using FourToolkit.Charts.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace FourSeafile.ViewModel
{
    public interface IUserInfoViewModel
    {
        string Nickname { get; }
        string Avatar { get; }
        ChartData UsageData { get; }
        string UsedSpace { get; }
        string TotalSpace { get; }
    }

    public class UserInfoViewModel : ViewModelBase, IUserInfoViewModel
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

        public ChartData UsageData
        {
            get
            {
                OnPropertyGet();
                return _usageData;
            }
            private set
            {
                _usageData = value;
                OnPropertyChanged();
            }
        }
        private ChartData _usageData;

        public string UsedSpace
        {
            get
            {
                OnPropertyGet();
                return _usedSpace;
            }
            private set
            {
                _usedSpace = value;
                OnPropertyChanged();
            }
        }
        private string _usedSpace;

        public string TotalSpace
        {
            get
            {
                OnPropertyGet();
                return _totalSpace;
            }
            private set
            {
                _totalSpace = value;
                OnPropertyChanged();
            }
        }
        private string _totalSpace;

        protected override async Task LoadAsync()
        {
            var info = await App.Seafile.CheckAccountInfo();
            var avatar = await App.Seafile.GetUserAvatar(128);
            Nickname = info.Nickname ?? info.Email;
            Avatar = avatar.Url;
            if (!info.HasUnlimitedSpace)
            {
                UsedSpace = info.Usage.ToSizeString();
                TotalSpace = info.Quota.ToSizeString();
                var accentBrush = App.Current.Resources["SystemControlBackgroundAccentBrush"] as SolidColorBrush;
                var i = 0;
                UsageData = ChartData.PrepareChartData(new List<long>
                {
                    info.Usage,
                    info.Quota - info.Usage
                }, q =>
                {
                    i++;
                    return
                        i == 1
                        ? accentBrush.Color
                        : Colors.Silver;
                });
            }
        }
    }
}
