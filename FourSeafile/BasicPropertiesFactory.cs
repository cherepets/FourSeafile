using System;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Storage.FileProperties;

namespace FourSeafile
{
    public static class BasicPropertiesFactory
    {
        public static async Task<BasicProperties> GetAsync()
        {
            var empty = await Package.Current.InstalledLocation.GetFileAsync(".empty");
            return await empty.GetBasicPropertiesAsync();
        }
    }
}
