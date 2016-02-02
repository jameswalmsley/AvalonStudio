using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Perspex;

namespace AvalonStudio.Controls.Dock
{
    public interface IItemsOrganizer
    {
        void Organise(AvalonViewItemsControl requestor, Size measureBounds, IEnumerable<AvalonViewItem> items);
        void Organise(AvalonViewItemsControl requestor, Size measureBounds, IOrderedEnumerable<AvalonViewItem> items);
        void OrganiseOnMouseDownWithin(AvalonViewItemsControl requestor, Size measureBounds, List<AvalonViewItem> siblingItems, AvalonViewItem viewItem);
        void OrganiseOnDragStarted(AvalonViewItemsControl requestor, Size measureBounds, IEnumerable<AvalonViewItem> siblingItems, AvalonViewItem dragItem);
        void OrganiseOnDrag(AvalonViewItemsControl requestor, Size measureBounds, IEnumerable<AvalonViewItem> siblingItems, AvalonViewItem dragItem);
        void OrganiseOnDragCompleted(AvalonViewItemsControl requestor, Size measureBounds, IEnumerable<AvalonViewItem> siblingItems, AvalonViewItem dragItem);
        Point ConstrainLocation(AvalonViewItemsControl requestor, Size measureBounds, Point itemCurrentLocation, Size itemCurrentSize, Point itemDesiredLocation, Size itemDesiredSize);
        Size Measure(AvalonViewItemsControl requestor, Size availableSize, IEnumerable<AvalonViewItem> items);
        IEnumerable<AvalonViewItem> Sort(IEnumerable<AvalonViewItem> items);
    }
}
