using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace TreeViewEx.Controls
{
    public class OverflowableStackPanel : StackPanel
    {
        public static DependencyProperty OverflowItemCountProperty = DependencyProperty.Register("OverflowItemCount",
            typeof (int),
            typeof (OverflowableStackPanel), new PropertyMetadata(0));

        public static DependencyProperty CanOverflowProperty = DependencyProperty.RegisterAttached("CanOverflow",
            typeof (bool),
            typeof (OverflowableStackPanel), new UIPropertyMetadata(false));

        public static DependencyProperty IsOverflowProperty = DependencyProperty.RegisterAttached("IsOverflow",
            typeof (bool),
            typeof (OverflowableStackPanel), new UIPropertyMetadata(false));

        double _noOverflowableWh;

        double _overflowableWh;

        public int OverflowItemCount
        {
            get => (int) GetValue(OverflowItemCountProperty);
            set => SetValue(OverflowItemCountProperty, value);
        }

        private double GetWh(Size size, Orientation orientation)
        {
            return orientation == Orientation.Horizontal ? size.Width : size.Height;
        }

        private double GetHw(Size size, Orientation orientation)
        {
            return orientation == Orientation.Vertical ? size.Width : size.Height;
        }


        protected override Size MeasureOverride(Size constraint)
        {
            //if (double.IsPositiveInfinity(constraint.Width) || double.IsPositiveInfinity(constraint.Height))
            //    return base.MeasureOverride(constraint);

            var items = InternalChildren.Cast<UIElement>();

            _overflowableWh = 0;
            _noOverflowableWh = 0;
            int overflowCount = 0;
            double maxHw = 0;

            foreach (var item in items)
            {
                item.Measure(constraint);
                maxHw = Math.Max(GetHw(item.DesiredSize, Orientation), maxHw);
                if (GetCanOverflow(item))
                    _overflowableWh += GetWh(item.DesiredSize, Orientation);
                else _noOverflowableWh += GetWh(item.DesiredSize, Orientation);
            }

            foreach (var ele in items.Reverse())
            {
                if (GetCanOverflow(ele))
                    if (_overflowableWh + _noOverflowableWh > GetWh(constraint, Orientation))
                    {
                        overflowCount += 1;
                        SetIsOverflow(ele, true);
                        _overflowableWh -= GetWh(ele.DesiredSize, Orientation);
                    }
                    else SetIsOverflow(ele, false);
            }

            SetValue(OverflowItemCountProperty, overflowCount);

            return Orientation == Orientation.Horizontal
                ? new Size(_overflowableWh + _noOverflowableWh, maxHw)
                : new Size(maxHw, _overflowableWh + _noOverflowableWh);
        }

        protected override Size ArrangeOverride(Size arrangeSize)
        {
            var items = InternalChildren.Cast<UIElement>();
            if (Orientation == Orientation.Horizontal)
            {
                double curX = 0;
                foreach (var item in items)
                {
                    if (!GetCanOverflow(item) || !GetIsOverflow(item)) //Not overflowable or not set overflow
                    {
                        item.Arrange(new Rect(curX, 0, item.DesiredSize.Width, arrangeSize.Height));
                        curX += item.DesiredSize.Width;
                    }
                    else item.Arrange(new Rect(0, 0, 0, 0));
                }
                return arrangeSize;
            }
            return base.ArrangeOverride(arrangeSize);
        }

        public static bool GetCanOverflow(DependencyObject obj)
        {
            return (bool) obj.GetValue(CanOverflowProperty);
        }

        public static void SetCanOverflow(DependencyObject obj, bool value)
        {
            obj.SetValue(CanOverflowProperty, value);
        }

        public static bool GetIsOverflow(DependencyObject obj)
        {
            return (bool) obj.GetValue(IsOverflowProperty);
        }

        public static void SetIsOverflow(DependencyObject obj, bool value)
        {
            obj.SetValue(IsOverflowProperty, value);
        }
    }
}