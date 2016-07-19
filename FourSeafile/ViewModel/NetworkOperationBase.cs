namespace FourSeafile.ViewModel
{
    public abstract class NetworkOperationBase : ViewModelBase
    {
        public abstract double Value { get; protected set; }
        public abstract double Total { get; protected set; }
        public abstract string Text { get; }
        public ProgressViewModel Parent { get; protected set; }
    }
}
