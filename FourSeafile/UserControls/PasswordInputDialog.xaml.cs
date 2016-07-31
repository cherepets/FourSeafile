using Windows.System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace FourSeafile.UserControls
{
    public sealed partial class PasswordInputDialog
    {
        public string Password => PasswordBox.Password;

        public bool Result { get; private set; }

        public PasswordInputDialog()
        {
            InitializeComponent();
            PrimaryButtonText = Localization.Ok;
            SecondaryButtonText = Localization.Cancel;
            Title = Localization.Password;
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
            => Result = true;

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
            => PasswordBox.Password = string.Empty;

        private void Page_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                Result = true;
                Hide();
                e.Handled = true;
            }
        }
    }
}
