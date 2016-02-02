using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Perspex;
using Perspex.Controls;
using Perspex.Controls.Generators;

namespace AvalonStudio.Controls.Dock
{
    public class AvalonViewItemsControl : ItemsControl
    {
        private object[] _previousSortQueryResult;

        static AvalonViewItemsControl()
        {

        }

        public AvalonViewItemsControl()
        {

        }

        public static readonly PerspexProperty<int> FixedItemCountProperty =
            PerspexProperty.Register<AvalonViewItemsControl, int>("FixedItemCount");

        public int FixedItemCount
        {
            get { return GetValue(FixedItemCountProperty); }
            set { SetValue(FixedItemCountProperty, value); }
        }

        public static readonly PerspexProperty<IItemsOrganizer> ItemsOrganiserProperty =
            PerspexProperty.Register<AvalonViewItemsControl, IItemsOrganizer>("ItemsOrganizer");

        public IItemsOrganizer ItemsOrganizer
        {
            get { return GetValue(ItemsOrganiserProperty); }
            set { SetValue(ItemsOrganiserProperty, value); }
        }

        public static readonly PerspexProperty<PositionMonitor> PositionMonitorProperty =
            PerspexProperty.Register<AvalonViewItemsControl, PositionMonitor>("PositionMonitor");

        public PositionMonitor PositionMonitor
        {
            get { return GetValue(PositionMonitorProperty); }
            set { SetValue(PositionMonitorProperty, value); }
        }

        public static readonly PerspexProperty<double> ItemsPresenterWidthProperty =
            PerspexProperty.RegisterDirect<AvalonViewItemsControl, double>("ItemsPresenterWidth",
                (control => control.ItemsPresenterWidth));

        private double _itemsPresenterWidth;

        public double ItemsPresenterWidth
        {
            get { return _itemsPresenterWidth; }
            private set { SetAndRaise(ItemsPresenterWidthProperty, ref _itemsPresenterWidth, value); }
        }

        // New Perspex Direct Property with Getter only (Readonly Property)
        public static readonly PerspexProperty<double> ItemsPresenterHeightProperty =
            PerspexProperty.RegisterDirect<AvalonViewItemsControl, double>("ItemsPresenterHeight",
                (o => o.ItemsPresenterHeight));

        private double _itemsPresenterHeight;

        public double ItemsPresenterHeight
        {
            get { return _itemsPresenterHeight; }
            private set { SetAndRaise(ItemsPresenterHeightProperty, ref _itemsPresenterHeight, value); }
        }

        internal Size? LockedMeasure { get; set; }

        internal IEnumerable<AvalonViewItem> AvalonViewItems()
        {
            return this.ItemContainerGenerator.Containers.OfType<AvalonViewItem>().ToList();
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            if (ItemsOrganizer == null) return base.MeasureOverride(availableSize);

            if (LockedMeasure.HasValue)
            {
                ItemsPresenterWidth = LockedMeasure.Value.Width;
                ItemsPresenterHeight = LockedMeasure.Value.Height;
                return LockedMeasure.Value;
            }

            var avalonViewItems = AvalonViewItems().ToList();
            var maxConstraint = new Size(double.PositiveInfinity, double.PositiveInfinity);

            ItemsOrganizer.Organise(this, maxConstraint, avalonViewItems);
            var measure = ItemsOrganizer.Measure(this, new Size(Width, Height), avalonViewItems);

            ItemsPresenterWidth = measure.Width;
            ItemsPresenterHeight = measure.Height;

            var width = double.IsInfinity(availableSize.Width) ? measure.Width : availableSize.Width;
            var height = double.IsInfinity(availableSize.Height) ? measure.Height : availableSize.Height;

            return new Size(width, height);
        }



        protected override IItemContainerGenerator CreateItemContainerGenerator()
        {
            return new ItemContainerGenerator<AvalonViewItem>(this);
        }

        //TODO Many of the drag and move events need to be added
    }
}
