using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace FourSeafile.UserControls
{
    public enum StatusBarMode
    {
        ContentUseVisible,
        ContentUseCoreWindow
    }

    internal static class StatusBarModeExt
    {
        internal static ApplicationViewBoundsMode ToApplicationViewBoundsMode(this StatusBarMode mode)
        {
            switch (mode)
            {
                case StatusBarMode.ContentUseCoreWindow:
                    return ApplicationViewBoundsMode.UseCoreWindow;
                case StatusBarMode.ContentUseVisible:
                    return ApplicationViewBoundsMode.UseVisible;
            }
            return default(ApplicationViewBoundsMode);
        }
    }

    public class StatusBarController : Control
    {
        private static StatusBar StatusBar { get; }
        private static ApplicationView ApplicationView { get; }

        public static new readonly DependencyProperty VisibilityProperty = DependencyProperty.Register("Visibility", typeof(Visibility), typeof(StatusBarController), null);
        public new Visibility Visibility
        {
            get
            {
                return (Visibility)GetValue(VisibilityProperty);
            }
            set
            {
                SetValue(VisibilityProperty, value);
                UpdateStatusBarVisibility(value);
            }
        }

        public static readonly DependencyProperty HiddenProperty = DependencyProperty.Register("Hidden", typeof(bool), typeof(StatusBarController), 
            new PropertyMetadata(false, (d, e) => (d as StatusBarController).Visibility = (bool)e.NewValue ? Visibility.Collapsed : Visibility.Visible));
        public bool Hidden
        {
            get
            {
                return Visibility == Visibility.Collapsed;
            }
            set
            {
                Visibility = value ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(StatusBarController), null);
        public string Text
        {
            get
            {
                return (string)GetValue(TextProperty);
            }
            set
            {
                SetValue(TextProperty, value);
                if (StatusBar != null && value != null)
                    StatusBar.ProgressIndicator.Text = value;
                UpdateProgressIndicatorVisibility();
            }
        }

        public static readonly DependencyProperty ProgressValueProperty = DependencyProperty.Register("ProgressValue", typeof(double?), typeof(StatusBarController), null);
        public double? ProgressValue
        {
            get
            {
                return (double?)GetValue(ProgressValueProperty);
            }
            set
            {
                SetValue(ProgressValueProperty, value);
                if (StatusBar != null)
                    StatusBar.ProgressIndicator.ProgressValue =
                        // -1 is indeterminare
                        value == -1
                        ? null
                        // null is off
                        : value == null
                        ? 0
                        : value;
                UpdateProgressIndicatorVisibility();
            }
        }
        
        public static readonly DependencyProperty BackgroundOpacityProperty = DependencyProperty.Register("BackgroundOpacity", typeof(double), typeof(StatusBarController), null);
        public double BackgroundOpacity
        {
            get
            {
                return (double)GetValue(BackgroundOpacityProperty);
            }
            set
            {
                SetValue(BackgroundOpacityProperty, value);
                if (StatusBar != null)
                    StatusBar.BackgroundOpacity = value;
            }
        }

        public static readonly DependencyProperty ModeProperty = DependencyProperty.Register("Mode", typeof(StatusBarMode), typeof(StatusBarController), null);
        public StatusBarMode Mode
        {
            get
            {
                return (StatusBarMode)GetValue(ModeProperty);
            }
            set
            {
                SetValue(ModeProperty, value);
                ApplicationView?.SetDesiredBoundsMode(value.ToApplicationViewBoundsMode());
            }
        }

        public static readonly DependencyProperty ForegroundBrushProperty = DependencyProperty.Register("ForegroundBrush", typeof(Brush), typeof(StatusBarController), null);
        public new Brush Foreground
        {
            get
            {
                return (Brush)GetValue(ForegroundBrushProperty);
            }
            set
            {
                SetValue(ForegroundBrushProperty, value);
                var brush = value as SolidColorBrush;
                if (StatusBar != null && brush != null)
                    StatusBar.ForegroundColor = brush.Color;
            }
        }

        public static readonly DependencyProperty BackgroundBrushProperty = DependencyProperty.Register("BackgroundBrush", typeof(Brush), typeof(StatusBarController), null);
        public new Brush Background
        {
            get
            {
                return (Brush)GetValue(BackgroundBrushProperty);
            }
            set
            {
                SetValue(BackgroundBrushProperty, value);
                var brush = value as SolidColorBrush;
                if (StatusBar != null && brush != null)
                    StatusBar.BackgroundColor = brush.Color;
            }
        }

        private static void UpdateStatusBarVisibility(Visibility value)
        {
            if (value == Visibility.Visible)
                StatusBar?.ShowAsync();
            else
                StatusBar?.HideAsync();
        }

        private void UpdateProgressIndicatorVisibility()
        {
            if (Text == null && ProgressValue == null)
                StatusBar?.ProgressIndicator?.HideAsync();
            else
                StatusBar?.ProgressIndicator?.ShowAsync();
        }

        static StatusBarController()
        {
            StatusBar = Platform.IsMobile ? StatusBar.GetForCurrentView() : null;
            ApplicationView = Platform.IsMobile ? ApplicationView.GetForCurrentView() : null;
        }
    }
}
