using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace FourSeafile.UserControls
{
    public class GridView : UserControl, INotifyPropertyChanged
    {
        public event SelectionChangedEventHandler SelectionChanged;
        public event PropertyChangedEventHandler PropertyChanged;
        
        public static DependencyProperty ItemsSourceProperty = DependencyProperty.Register(nameof(ItemsSource), typeof(IList), typeof(GridView), new PropertyMetadata(string.Empty, OnItemsSourceChanged));
        
        public DataTemplate ItemTemplate { get; set; }

        public DataTemplate ItemsPanel
        {
            get { return _itemsPanel; }
            set
            {
                var panel = value.LoadContent() as Panel;
                if (panel == null) throw new ArgumentException("GridView.ItemsPanel.DataTemplate is not a panel!");
                _panel = panel;
                _scroll.Content = _panel;
                _itemsPanel = value;
            }
        }
        private DataTemplate _itemsPanel;

        public IList ItemsSource
        {
            get { return GetValue(ItemsSourceProperty) as IList; }
            set { SetValue(ItemsSourceProperty, value); }
        }
        private IList _itemsSource;

        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var view = d as GridView;
            var value = e.NewValue as IList;
            view._itemsSource = value;
            var observable = view._itemsSource as INotifyCollectionChanged;
            if (observable != null)
                observable.CollectionChanged += (s, a) => view.Redraw();
            view.SelectedIndex = 0;
        }

        public int SelectedIndex
        {
            get
            {
                return _selectedIndex;
            }
            set
            {
                _selectedIndex = value;
                Redraw();
 // TODO:               ScrollInto();
                OnSelectionChanged();
                OnPropertyChanged();
                OnPropertyChanged(nameof(SelectedItem));
            }
        }
        private int _selectedIndex;

        public double DesiredItemSize
        {
            get { return _desiredItemSize; }
            set
            {
                _desiredItemSize = value;
                OnPropertyChanged();
                Redraw();
            }
        }
        private double _desiredItemSize;

        private Panel _panel;
        private ScrollViewer _scroll;

        public GridView()
        {
            _panel = new WrapPanel();
            _scroll = new ScrollViewer
            {
                Content = _panel,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto
            };
            Content = _scroll;
            SizeChanged += (s, e) => ResizeAll(GetColumnSize());
        }

        private void Redraw()
        {
            if (ItemTemplate == null) return;
            if (ItemsSource == null) return;
            _panel.Children.Clear();
            var size = GetColumnSize();
            for (var i = 0; i < ItemsSource.Count; i++)
            {
                var c = i;
                var obj = new Grid { Background = new SolidColorBrush(Colors.Transparent) };
                Resize(obj, size);
                obj.Children.Add(ItemTemplate.LoadContent() as FrameworkElement);
                obj.DataContext = ItemsSource[c];
                _panel.Children.Add(obj);
                obj.Tapped += (s, e) => SelectedIndex = c;
            }
        }

        private void ResizeAll(double? size)
        {
            foreach (FrameworkElement element in _panel.Children)
                Resize(element, size);
        }

        private void Resize(FrameworkElement element, double? size)
        {
            if (size.HasValue) element.Width = size.Value;
        }

        private double? GetColumnSize()
        {
            if (double.IsNaN(DesiredItemSize) || DesiredItemSize == 0) return null;
            var width = ActualWidth;
            var columns = (int)(width / DesiredItemSize);
            if (columns <= 0) columns = 1;
            var extraSize = width - columns * DesiredItemSize;
            var size = (int)(DesiredItemSize + extraSize / columns);
            return size;
        }

        public object SelectedItem => ItemsSource?[_selectedIndex];
        protected virtual void OnSelectionChanged()
            => SelectionChanged?.Invoke(this, new SelectionChangedEventArgs(new List<object>(), new List<object> { SelectedItem }));

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
