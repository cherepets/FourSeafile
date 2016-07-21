using Windows.System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace FourSeafile.UserControls
{
    public sealed partial class PasswordInputDialog
    {
        public string Password => PasswordBox.Password;

        public PasswordInputDialog()
        {
            InitializeComponent();
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            PasswordBox.Password = string.Empty;
        }

        private void Page_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter) Hide();
        }
    }
}
