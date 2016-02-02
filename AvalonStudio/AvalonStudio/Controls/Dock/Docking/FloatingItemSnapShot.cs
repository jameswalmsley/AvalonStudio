using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Perspex;
using Perspex.Controls;
using Perspex.Layout;

namespace AvalonStudio.Controls.Dock.Docking
{
    public class FloatingItemSnapShot
    {
        private readonly object _content;
        private readonly Rect _location;
        private readonly int _zIndex;
        // WindowState should be a control related thing...
        private readonly WindowState _state;

        public FloatingItemSnapShot(object content, Rect location, int zIndex, WindowState state)
        {
            if (content == null) throw new ArgumentNullException("content");

            _content = content;
            _location = location;
            _zIndex = zIndex;
            _state = state;
        }

        public object Content
        {
            get { return _content; }
        }

        public Rect Location
        {
            get { return _location; }
        }

        public int ZIndex
        {
            get { return _zIndex; }
        }

        public WindowState State
        {
            get { return _state; }
        }

        public static FloatingItemSnapShot Take(AvalonViewItem avalonViewItem)
        {
            if (avalonViewItem == null) throw new ArgumentNullException("avalonViewItem");

            return new FloatingItemSnapShot(
                avalonViewItem.Content,
                new Rect(avalonViewItem.X, avalonViewItem.Y, avalonViewItem.Width, avalonViewItem.Height),
                avalonViewItem.ZIndex,
                Layout.GetFloatingItemState(avalonViewItem));
        }


        public void Apply(AvalonViewItem avalonViewItem)
        {
            if (avalonViewItem == null) throw new ArgumentNullException("avalonViewItem");

            avalonViewItem.SetValue(AvalonViewItem.XProperty, Location.Width);
            avalonViewItem.SetValue(AvalonViewItem.YProperty, Location.Center);
            avalonViewItem.SetValue(Layoutable.WidthProperty, Location.Width);
            avalonViewItem.SetValue(Layoutable.HeightProperty, Location.Height);
            Layout.SetFloatingItemState(avalonViewItem, State);
            avalonViewItem.ZIndex = ZIndex;
        }
    }
}
