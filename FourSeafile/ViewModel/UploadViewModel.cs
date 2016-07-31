using System;
using System.Threading.Tasks;
using Windows.Storage;

namespace FourSeafile.ViewModel
{
    public class UploadViewModel : NetworkOperationBase
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

        private StructContainer<double> _oper;

        public UploadViewModel(ProgressViewModel parent, IStorageFile file, StructContainer<double> oper)
        {
            Parent = parent;
            _oper = oper;
            StatusUpdater();
            Text = $"{file.Name} {Localization.IsUploading}";
            Total = 100;
        }

        private async void StatusUpdater()
        {
            while (true)
            {
                Value = _oper.Value;
                if (Value == Total)
                {
                    Parent.NetworkOperations.Remove(this);
                    return;
                }
                await Task.Delay(TimeSpan.FromMilliseconds(1000));
            }
        }
    }
}
