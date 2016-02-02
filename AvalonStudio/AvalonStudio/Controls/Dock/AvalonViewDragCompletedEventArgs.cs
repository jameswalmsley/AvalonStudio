using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Perspex.Interactivity;
using Perspex.Controls.Primitives;
using Perspex.Input;

namespace AvalonStudio.Controls.Dock
{
    public delegate void AvalonViewDragCompletedEventHandler(object sender, AvalonViewDragCompletedEventArgs e);

    public class AvalonViewDragCompletedEventArgs : RoutedEventArgs
    {
        private readonly AvalonViewItem _avalonViewItem;
        private readonly bool _isDropTargetFound;
        private readonly VectorEventArgs _drageVectorEventArgs;

        public AvalonViewDragCompletedEventArgs(AvalonViewItem avalonViewItem, VectorEventArgs dragVectorEventArgs)
        {
            if (avalonViewItem == null) throw new ArgumentNullException("avalonViewItem");
            if (dragVectorEventArgs == null) throw new ArgumentNullException("dragVectorEventArgs");

            _avalonViewItem = avalonViewItem;
            _drageVectorEventArgs = dragVectorEventArgs;
        }

        public AvalonViewDragCompletedEventArgs(RoutedEvent routedEvent, AvalonViewItem avalonViewItem, VectorEventArgs dragVectorEventArgs)
            : base(routedEvent)
        {
            if (avalonViewItem == null) throw new ArgumentNullException("avalonViewItem");
            if (dragVectorEventArgs == null) throw new ArgumentNullException("dragVectorEventArgs");

            _avalonViewItem = avalonViewItem;
            _drageVectorEventArgs = dragVectorEventArgs;
        }

        // it appears that Perspex routed events doesnt have a contructor that takes object
        //public AvalonViewDragCompletedEventArgs(RoutedEvent routedEvent, object source, AvalonViewItem avalonViewItem, VectorEventArgs dragVectorEventArgs)
        //    : base(routedEvent, source)
        //{
            
        //}
        public AvalonViewItem ViewItem
        {
            get { return _avalonViewItem; }
        }

        public VectorEventArgs DrageVectorEventArgs
        {
            get { return _drageVectorEventArgs; }
        }
    }
}
