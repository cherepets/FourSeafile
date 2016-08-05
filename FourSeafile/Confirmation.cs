using FourSeafile.Extensions;
using System;
using System.Threading.Tasks;
using Windows.UI.Popups;

namespace FourSeafile
{
    public static class Confirmation
    {
        public static async Task<bool> ShowAsync(string s = null)
        {
            if (s == null) s = Localization.AreYouSure;
            var res = false;
            await new MessageDialog(s, Localization.Confirmation)
                .WithCommand(Localization.Yes, () => res = true)
                .WithCommand(Localization.No, () => res = false)
                .SetDefaultCommandIndex(0)
                .SetCancelCommandIndex(1)
                .ShowAsync();
            return res;
        }
    }
}
