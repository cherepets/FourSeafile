using System;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace FourSeafile.UserControls
{
    public class WrapPanel : Panel
    {
        public static readonly DependencyProperty ItemHeightProperty =
            DependencyProperty.Register("ItemHeight", typeof(double), typeof(WrapPanel), new PropertyMetadata(double.NaN, OnItemSizeChanged));

        public double ItemHeight
        {
            get { return (double)GetValue(ItemHeightProperty); }
            set { SetValue(ItemHeightProperty, value); }
        }

        public static readonly DependencyProperty ItemWidthProperty =
            DependencyProperty.Register("ItemWidth", typeof(double), typeof(WrapPanel), new PropertyMetadata(double.NaN, OnItemSizeChanged));

        public double ItemWidth
        {
            get { return (double)GetValue(ItemWidthProperty); }
            set { SetValue(ItemWidthProperty, value); }
        }

        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation", typeof(Orientation), typeof(WrapPanel), new PropertyMetadata(Orientation.Horizontal, OnItemSizeChanged));

        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        private static void OnItemSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
            => (d as WrapPanel)?.InvalidateMeasure();

        protected override Size MeasureOverride(Size constraint)
        {
            var itemWidth = ItemWidth;
            var itemHeight = ItemHeight;
            var orientation = Orientation;
            var lineDirectSize = 0d;
            var lineIndirectSize = 0d;
            var totalDirectSize = 0d;
            var totalIndirectSize = 0d;
            var maximumSize = GetDirectSize(orientation, constraint.Width, constraint.Height);
            var hasFixedWidth = !double.IsNaN(itemWidth);
            var hasFixedHeight = !double.IsNaN(itemHeight);
            var itemSize = new Size(
                hasFixedWidth ? itemWidth : constraint.Width,
                hasFixedHeight ? itemHeight : constraint.Height);
            foreach (var element in Children)
            {
                element.Measure(itemSize);
                var elementWidth = hasFixedWidth ? itemWidth : Math.Floor(element.DesiredSize.Width);
                var elementHeight = hasFixedHeight ? itemHeight : Math.Floor(element.DesiredSize.Height);
                var elementDirectSize = GetDirectSize(orientation, elementWidth, elementHeight);
                var elementIndirectSize = GetIndirectSize(orientation, elementWidth, elementHeight);
                if (lineDirectSize + elementDirectSize > maximumSize)
                {
                    totalDirectSize = lineDirectSize > totalDirectSize
                        ? lineDirectSize
                        : totalDirectSize;
                    totalIndirectSize += lineIndirectSize;
                    lineDirectSize = elementDirectSize;
                    lineIndirectSize = elementIndirectSize;
                    if (elementDirectSize > maximumSize)
                    {
                        totalDirectSize = elementDirectSize > totalDirectSize
                            ? elementDirectSize
                            : totalDirectSize;
                        totalIndirectSize += elementIndirectSize;
                        lineDirectSize = 0d;
                        lineIndirectSize = 0d;
                    }
                }
                else
                {
                    lineDirectSize += elementDirectSize;
                    lineIndirectSize =
                    totalDirectSize = lineIndirectSize > elementIndirectSize
                        ? lineIndirectSize
                        : elementIndirectSize;
                }
            }
            totalDirectSize = lineDirectSize > totalDirectSize
                ? lineDirectSize
                : totalDirectSize;
            totalIndirectSize += lineIndirectSize;
            return new Size(
                GetWidth(orientation, totalDirectSize, totalIndirectSize),
                GetHeight(orientation, totalDirectSize, totalIndirectSize));
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            var itemWidth = ItemWidth;
            var itemHeight = ItemHeight;
            var orientation = Orientation;
            var lineDirectSize = 0d;
            var lineIndirectSize = 0d;
            var maximumSize = GetDirectSize(orientation, finalSize.Width, finalSize.Height);
            var hasFixedWidth = !double.IsNaN(itemWidth);
            var hasFixedHeight = !double.IsNaN(itemHeight);
            var indirectOffset = 0d;
            var directDelta = (orientation == Orientation.Horizontal) ?
                (hasFixedWidth ? (double?)itemWidth : null) :
                (hasFixedHeight ? (double?)itemHeight : null);
            var children = Children;
            var lineStart = 0;
            for (var i = 0; i < children.Count; i++)
            {
                var element = children[i];
                var elementWidth = hasFixedWidth ? itemWidth : Math.Floor(element.DesiredSize.Width);
                var elementHeight = hasFixedHeight ? itemHeight : Math.Floor(element.DesiredSize.Height);
                var elementDirectSize = GetDirectSize(orientation, elementWidth, elementHeight);
                var elementIndirectSize = GetIndirectSize(orientation, elementWidth, elementHeight);
                if (lineDirectSize + elementDirectSize > maximumSize)
                {
                    ArrangeLine(lineStart, i, directDelta, indirectOffset, lineIndirectSize);
                    indirectOffset += lineIndirectSize;
                    lineDirectSize = elementDirectSize;
                    lineIndirectSize = elementIndirectSize;
                    if (elementDirectSize > maximumSize)
                    {
                        ArrangeLine(i, i++, directDelta, indirectOffset, elementIndirectSize);
                        indirectOffset += lineIndirectSize;
                        lineDirectSize = 0d;
                        lineIndirectSize = 0d;
                    }
                    lineStart = i;
                }
                else
                {
                    lineDirectSize += elementDirectSize;
                    lineIndirectSize = Math.Max(lineIndirectSize, elementIndirectSize);
                }
            }
            if (lineStart < children.Count)
                ArrangeLine(lineStart, children.Count, directDelta, indirectOffset, lineIndirectSize);
            return finalSize;
        }

        private void ArrangeLine(int lineStart, int lineEnd, double? directDelta, double indirectOffset, double indirectGrowth)
        {
            var directOffset = 0d;
            var orientation = Orientation;
            var children = Children;
            for (var i = lineStart; i < lineEnd; i++)
            {
                var element = children[i];
                var elementWidth = Math.Floor(element.DesiredSize.Width);
                var elementHeight =Math.Floor(element.DesiredSize.Height);
                var directGrowth = directDelta != null
                    ? directDelta.Value
                    : GetDirectSize(orientation, elementWidth, elementHeight);
                var bounds = orientation == Orientation.Horizontal
                    ? new Rect(directOffset, indirectOffset, directGrowth, indirectGrowth)
                    : new Rect(indirectOffset, directOffset, indirectGrowth, directGrowth);
                element.Arrange(bounds);
                directOffset += directGrowth;
            }
        }

        private double GetWidth(Orientation orientation, double direct, double indirect)
            => Orientation == Orientation.Horizontal ? direct : indirect;

        private double GetHeight(Orientation orientation, double direct, double indirect)
            => Orientation == Orientation.Horizontal ? indirect : direct;

        private double GetDirectSize(Orientation orientation, double width, double height)
            => Orientation == Orientation.Horizontal ? width : height;

        private double GetIndirectSize(Orientation orientation, double width, double height)
            => Orientation == Orientation.Horizontal ? height : width;
    }
}
