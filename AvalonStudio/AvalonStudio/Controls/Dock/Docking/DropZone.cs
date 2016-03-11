using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Perspex;
using Perspex.Controls;

namespace AvalonStudio.Controls.Dock.Docking
{
    public class DropZone : Control
    {
        public static readonly PerspexProperty<DropZoneLocation> LocationProperty =
            PerspexProperty.RegisterDirect<DropZone, DropZoneLocation>("Location", o => o.Location,
                (o, v) => o.Location = v);

        private DropZoneLocation _location;

        public DropZoneLocation Location
        {
            get { return _location; }
            set { SetAndRaise(LocationProperty, ref _location, value); }
        }

        public static readonly PerspexProperty<bool> IsOfferedProperty =
            PerspexProperty.RegisterDirect<DropZone, bool>("IsOffered", o => o.IsOffered);

        private bool _isOffered;

        public bool IsOffered
        {
            get { return _isOffered; }
            internal set { SetAndRaise(IsOfferedProperty, ref _isOffered, value); }
        }
    }
}
