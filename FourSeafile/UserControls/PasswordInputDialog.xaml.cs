using Windows.UI.Xaml.Controls;

namespace FourSeafile.UserControls
{
    public sealed partial class PasswordInputDialog
    {
        public string Password => PasswordBox.Password;

        public PasswordInputDialog()
        {
            InitializeComponent();
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            PasswordBox.Password = string.Empty;
        }
    }
}
