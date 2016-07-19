namespace FourSeafile.ViewModel
{
    public class IconViewModel : ViewModelBase
    {
        public string Glyph
        {
            get
            {
                OnPropertyGet();
                return _glyph;
            }
            set
            {
                _glyph = value;
                OnPropertyChanged();
            }
        }
        private string _glyph;

        public string Font
        {
            get
            {
                OnPropertyGet();
                return _font;
            }
            set
            {
                _font = value;
                OnPropertyChanged();
            }
        }
        private string _font;

        public static readonly IconViewModel LibraryIcon = new IconViewModel
        {
            Glyph = "",
            Font = SegoeMDL2
        };

        public static readonly IconViewModel FolderIcon = new IconViewModel
        {
            Glyph = "",
            Font = SegoeMDL2
        };

        public static readonly IconViewModel DefaultFileIcon = new IconViewModel
        {
            Glyph = "",
            Font = SegoeMDL2
        };

        #region FileTypes

        public static readonly IconViewModel PictureIcon = new IconViewModel
        {
            Glyph = "",
            Font = SegoeMDL2
        };

        public static readonly IconViewModel MusicIcon = new IconViewModel
        {
            Glyph = "",
            Font = SegoeMDL2
        };        

        public static readonly IconViewModel VideoIcon = new IconViewModel
        {
            Glyph = "",
            Font = SegoeMDL2
        };

        public static readonly IconViewModel DocumentIcon = new IconViewModel
        {
            Glyph = "",
            Font = SegoeMDL2
        };

        public static readonly IconViewModel TableIcon = new IconViewModel
        {
            Glyph = "",
            Font = SegoeMDL2
        };

        public static readonly IconViewModel ExecIcon = new IconViewModel
        {
            Glyph = "",
            Font = SegoeMDL2
        };

        public static readonly IconViewModel WebIcon = new IconViewModel
        {
            Glyph = "",
            Font = SegoeMDL2
        };
        #endregion

        private const string SegoeMDL2 = "Segoe MDL2 Assets";
    }
}
