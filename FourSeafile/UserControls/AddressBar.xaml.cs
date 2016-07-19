using System;
using System.Collections.Generic;
using System.Linq;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace FourSeafile.UserControls
{
    public sealed partial class AddressBar
    {
        public event EventHandler<string> UserInput;
        public event EventHandler RootRequested;

        public static DependencyProperty AddressProperty = DependencyProperty.Register(nameof(Address), typeof(string), typeof(AddressBar), new PropertyMetadata(string.Empty,
            (s, e) => (s as AddressBar).Redraw()));

        public string Address
        {
            get { return (string)GetValue(AddressProperty); }
            set { SetValue(AddressProperty, value); }
        }

        private IList<string> AddressParts
        {
            get
            {
                _separator = default(char);
                if (Address.Contains('/')) _separator = '/';
                if (Address.Contains('\\')) _separator = '\\';
                return Address?.Split(new [] { _separator }, StringSplitOptions.RemoveEmptyEntries);
            }
        }

        private char _separator;

        public AddressBar()
        {
            InitializeComponent();
            Root.DataContext = this;
            Redraw();
        }

        private void TextBox_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter || e.Key == VirtualKey.Accept || e.Key == VirtualKey.NavigationAccept)
                OnInput(TextBox.Text);
        }

        private void Grid_Tapped(object sender, TappedRoutedEventArgs e)
            => ToTextState();

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
            => ToButtonsState();

        private void ToTextState()
        {
            TextBox.Visibility = Visibility.Visible;
            ButtonsViewer.Visibility = Visibility.Collapsed;
            TextBox.Focus(FocusState.Programmatic);
            TextBox.SelectAll();
        }

        private void ToButtonsState()
        {
            TextBox.Visibility = Visibility.Collapsed;
            ButtonsViewer.Visibility = Visibility.Visible;
        }

        private void Redraw()
        {
            ButtonsPanel.Children.Clear();
            AddButton("/", (s, e) => OnRootRequested());
            var parts = AddressParts;
            if (parts == null || !parts.Any()) return;
            var input = string.Empty;
            for (var i = 0; i < parts.Count - 1; i++)
            {
                input += parts[i] + _separator;
                var icopy = input;
                AddButton(parts[i], (s, e) => OnInput(icopy));
            }
            AddButton(parts[parts.Count - 1], null, false);
            ToButtonsState();
        }

        private void AddButton(string text, RoutedEventHandler handler, bool withArrow = true)
        {
            var button = new Button { Content = text + " »" };
            if (handler != null) button.Click += handler;
            button.Tapped += (s, e) => e.Handled = true;
            ButtonsPanel.Children.Add(button);
        }

        private void OnInput(string s) => UserInput?.Invoke(this, s);
        private void OnRootRequested() => RootRequested?.Invoke(this, null);
    }
}
