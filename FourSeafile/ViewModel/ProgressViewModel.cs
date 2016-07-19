using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Windows.Storage;

namespace FourSeafile.ViewModel
{
    public class ProgressViewModel : ViewModelBase
    {
        public ObservableCollection<NetworkOperationBase> NetworkOperations { get; } = new ObservableCollection<NetworkOperationBase>();
        public string Stats
        {
            get
            {
                OnPropertyGet();
                return _stats;
            }
            private set
            {
                _stats = value;
                OnPropertyChanged();
            }
        }

        public List<DownloadViewModel> Downloads => NetworkOperations.Select(n => n as DownloadViewModel).Where(n => n != null).ToList();
        public List<NetworkOperationBase> Uploads => NetworkOperations.Select(n => n as NetworkOperationBase).Where(n => n != null).ToList();

        private string _stats;

        public ProgressViewModel()
        {
            NetworkOperations.CollectionChanged += UpdateStats;
            FileViewModel.DownloadStarted += (s, e) => NetworkOperations.Add(new DownloadViewModel(this, e));
            FileBrowserViewModel.UploadStarted += (s, e) => NetworkOperations.Add(new UploadViewModel(this, (IStorageFile)s, e));
            UpdateStats(this, null);
        }

        private void UpdateStats(object sender, NotifyCollectionChangedEventArgs e)
            => Stats = NetworkOperations.Any()
            ? $"{Downloads.Count} / {Uploads.Count}"
            : string.Empty;
    }
}
