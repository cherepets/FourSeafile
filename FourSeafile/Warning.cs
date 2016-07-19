using System;
using System.Threading.Tasks;
using Windows.UI.Popups;

namespace FourSeafile
{
    public static class Warning
    {
        public static async Task ShowAsync(string s)
            => await new MessageDialog(s, nameof(Warning)).ShowAsync();
    }
}
