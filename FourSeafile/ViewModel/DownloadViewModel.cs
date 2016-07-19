using System;
using System.Threading.Tasks;
using Windows.Networking.BackgroundTransfer;

namespace FourSeafile.ViewModel
{
    public class DownloadViewModel : NetworkOperationBase
    {
        public override double Value
        {
            get
            {
                OnPropertyGet();
                return _value;
            }
            protected set
            {
                _value = value;
                OnPropertyChanged();
            }
        }
        private double _value;

        public override double Total
        {
            get
            {
                OnPropertyGet();
                return _total;
            }
            protected set
            {
                _total = value;
                OnPropertyChanged();
            }
        }
        private double _total;

        public override string Text { get; }
        
        private DownloadOperation _oper;

        public DownloadViewModel(ProgressViewModel parent, DownloadOperation oper)
        {
            Parent = parent;
            _oper = oper;
            StatusUpdater();
            Text = $"{_oper.ResultFile.Name} is downloading";
            Total = 1;
        }

        private async void StatusUpdater()
        {
            while (true)
            {
                Value = _oper.Progress.BytesReceived;
                Total = _oper.Progress.TotalBytesToReceive;
                if (_oper.Progress.Status == BackgroundTransferStatus.Canceled
                    || _oper.Progress.Status == BackgroundTransferStatus.Completed
                    || _oper.Progress.Status == BackgroundTransferStatus.Error)
                {
                    Parent.NetworkOperations.Remove(this);
                    return;
                }
                await Task.Delay(TimeSpan.FromMilliseconds(1000));
            }
        }
    }
}
