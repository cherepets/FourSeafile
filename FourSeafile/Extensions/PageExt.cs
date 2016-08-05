using System;
using Windows.Phone.UI.Input;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;

namespace FourSeafile.Extensions
{
    public static class PageExt
    {
        private static EventHandler<BackRequestedEventArgs> _desktopHandler;
        private static EventHandler<BackPressedEventArgs> _mobileHandler;
        private static Action _action;
        private static Action _stash;
        private static readonly object Lock = new object();


        public static bool BackHandlerAttached => _desktopHandler != null || _mobileHandler != null;

        public static void AttachBackHandler(this Page page, Action action)
        {
            lock (Lock)
            {
                _action = action;
                if (BackHandlerAttached) page.DetachBackHandler();
                switch (Platform.Current)
                {
                    case Platform.Desktop:
                        var navigationManager = SystemNavigationManager.GetForCurrentView();
                        navigationManager.AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
                        _desktopHandler = (s, e) =>
                        {
                            _action.Invoke();
                            e.Handled = true;
                        };
                        navigationManager.BackRequested += _desktopHandler;
                        return;
                    case Platform.Mobile:
                        _mobileHandler = (s, e) =>
                        {
                            _action.Invoke();
                            e.Handled = true;
                        };
                        HardwareButtons.BackPressed += _mobileHandler;
                        return;
                }
            }
        }

        public static void DetachBackHandler(this Page page)
        {
            if (!BackHandlerAttached) return;
            switch (Platform.Current)
            {
                case Platform.Desktop:
                    var navigationManager = SystemNavigationManager.GetForCurrentView();
                    navigationManager.AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
                    navigationManager.BackRequested -= _desktopHandler;
                    _desktopHandler = null;
                    return;
                case Platform.Mobile:
                    HardwareButtons.BackPressed -= _mobileHandler;
                    _mobileHandler = null;
                    return;
            }
        }

        public static void StashHandler(this Page page)
            => _stash = BackHandlerAttached
            ? _action
            : null;

        public static void UnstashHandler(this Page page)
        {
            if (_stash == null) return;
            page.AttachBackHandler(_stash);
        }
    }
}
