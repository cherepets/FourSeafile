using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace FourSeafile.ViewModel
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public event EventHandler<Exception> PropertyLoadFailed;
        public event PropertyChangedEventHandler PropertyChanged;

        public LoadingState State
        {
            get
            {
                return _state;
            }
            set
            {
                _state = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsLoaded));
                OnPropertyChanged(nameof(IsLoading));
                OnPropertyChanged(nameof(IsNotLoaded));
            }
        }
        private LoadingState _state;

        public bool IsLoaded => State == LoadingState.Loaded;
        public bool IsLoading => State == LoadingState.Loading;
        public bool IsNotLoaded => State == LoadingState.NotLoaded;

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
                        State = LoadingState.Loading;
                        break;
                }
            }
            try
            {
                await LoadAsync();
                State = LoadingState.Loaded;
            }
            catch (Exception ex)
            {
                State = LoadingState.NotLoaded;
                PropertyLoadFailed?.Invoke(this, ex);
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        protected async virtual Task LoadAsync()
        {
            await Task.Yield();
            return;
        }
    }

    public enum LoadingState
    {
        NotLoaded,
        Loading,
        Loaded
    }
}
