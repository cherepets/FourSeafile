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
            // Empty file required to obtain BasicProperties
            // There's no other way because the class is sealed and has no public constructors
            var empty = await Package.Current.InstalledLocation.GetFileAsync(".empty");
            return await empty.GetBasicPropertiesAsync();
        }
    }
}
