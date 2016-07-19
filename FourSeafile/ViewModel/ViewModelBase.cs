using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace FourSeafile.ViewModel
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        private LoadingState _state;
        private object _lock = new object();

        protected async void OnPropertyGet()
        {
            lock (_lock)
            {
                switch (_state)
                {
                    case LoadingState.Loaded:
                    case LoadingState.Loading:
                        return;
                    case LoadingState.NotLoaded:
                        _state = LoadingState.Loading;
                        break;
                }
            }
            try
            {
                IsNotLoaded = false;
                IsLoading = true;
                await LoadAsync();
            }
            finally
            {
                IsLoading = false;
                IsLoaded = true;
            }
        }

        public bool IsLoading
        {
            get
            {
                OnPropertyGet();
                return _isLoading;
            }
            private set
            {
                _isLoading = value;
                OnPropertyChanged();
            }
        }
        private bool _isLoading;

        public bool IsLoaded
        {
            get
            {
                OnPropertyGet();
                return _isLoaded;
            }
            private set
            {
                _isLoaded = value;
                OnPropertyChanged();
            }
        }
        private bool _isLoaded;

        public bool IsNotLoaded
        {
            get
            {
                OnPropertyGet();
                return _isNotLoaded;
            }
            private set
            {
                _isNotLoaded = value;
                OnPropertyChanged();
            }
        }
        private bool _isNotLoaded = true;

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        protected async virtual Task LoadAsync() { return; }
    }

    public enum LoadingState
    {
        NotLoaded,
        Loading,
        Loaded
    }
}
