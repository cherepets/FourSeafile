using Windows.Foundation;

namespace FourSeafile.Component
{
    public interface IFileComponent
    {
        IAsyncOperation<string> GetDownloadLinkAsyncOperation();
        IAsyncAction DeleteAsyncAction();
        string Name { get; }
    }
}
