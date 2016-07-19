using FourSeafile.Extensions;
using System;
using System.Threading.Tasks;
using Windows.UI.Popups;

namespace FourSeafile
{
    public static class Confirmation
    {
        public static async Task<bool> ShowAsync(string s)
        {
            var res = false;
            await new MessageDialog(s, "Confirmation")
                .WithCommand("Yes", () => res = true)
                .WithCommand("No", () => res = false)
                .SetDefaultCommandIndex(0)
                .SetCancelCommandIndex(1)
                .ShowAsync();
            return res;
        }
    }
}
