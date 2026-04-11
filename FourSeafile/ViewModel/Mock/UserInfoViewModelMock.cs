using FourSeafile.Extensions;
using FourToolkit.Charts.Data;
using System.Collections.Generic;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace FourSeafile.ViewModel
{
    public class UserInfoViewModelMock : ViewModelBase, IUserInfoViewModel
    {
        private const long Used = 1048576;
        private const long Total = 3145728;

        public string Nickname => "Galactic Cat";
        public string Avatar => "ms-appx:///Assets/demo.jpg";
        public string UsedSpace => Used.ToSizeString();
        public string TotalSpace => Total.ToSizeString();

        public ChartData UsageData
        {
            get
            {
                var accentBrush = App.Current.Resources["SystemControlBackgroundAccentBrush"] as SolidColorBrush;
                var i = 0;
                return ChartData.PrepareChartData(new List<long>
                {
                    Used,
                    Total - Used
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
