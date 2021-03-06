﻿using FourSeafile.Extensions;
using FourSeafile.ViewModel;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

namespace FourSeafile.Pages
{
    public sealed partial class MainPage
    {
        public static event EventHandler<NavigationEventArgs> NavigatedTo;
        public static bool ForceReload;

        public const int ViewTrigger = 720;

        public MainPage()
        {
            InitializeComponent();
        }

        private bool _isCompact = true;
        private bool _isOpened = true;
        private string _state;

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            NavigatedTo?.Invoke(this, e);
            if (DataContext == null || ForceReload)
                DataContext = ViewModelFactory.CreateRoot();
        }

        private async void AddressBar_UserInput(object sender, string e)
        {
            var vm = (IRootViewModel)DataContext;
            await vm.FileBrowser.NavigateToAddressAsync(e);
        }

        private void AddressBar_RootRequested(object sender, EventArgs e)
        {
            var vm = (IRootViewModel)DataContext;
            vm.FileBrowser.NavigateToRoot();
        }

        private void Burger_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (_isOpened)
                _isCompact = !_isCompact;
            else
                _isOpened = !_isOpened;
            UpdateVisualState();
        }

        private void Logout_Tapped(object sender, TappedRoutedEventArgs e)
            => App.Logout();

        private void CompactBurger_Click(object sender, RoutedEventArgs e)
        {
            _isOpened = !_isOpened;
            UpdateVisualState();
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e) => UpdateVisualState();

        private void UpdateVisualState()
        {
            var newState = ActualWidth > ViewTrigger
                ? _isCompact
                    ? "TwoPanesCompact"
                    : "TwoPanes"
                : _isOpened
                    ? "RightPane"
                    : "LeftPane";
            // "Mobile" -> "Full"
            if (newState == "TwoPanes" && (_state == "LeftPane" || _state == "RightPane"))
            {
                _isCompact = true;
                newState = "TwoPanesCompact";
            }
            // Back handler
            if (Platform.IsMobile)
            {
                if (newState == "LeftPane" && _state != "LeftPane")
                {
                    this.StashHandler();
                    this.AttachBackHandler(() => CompactBurger_Click(null, null));
                }
                if (newState != "LeftPane" && _state == "LeftPane")
                {
                    this.DetachBackHandler();
                    this.UnstashHandler();
                }
            }
            _state = newState;
            VisualStateManager.GoToState(this, _state, false);
        }
    }
}
