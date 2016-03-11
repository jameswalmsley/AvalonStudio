using System;
using Perspex.Controls;
using Perspex.Layout;

namespace AvalonStudio.Controls.Dock.Docking
{

    /// <summary>
    /// Initially needed to restore MDI avalonview items styles after a max then restore,
    /// as the trigger which binds the item width to the canvas width sets the  Width back to the default
    /// (e.g double.NaN) when the trigger is unset.  so we need to re-apply sizes manually
    /// </summary>
    internal class LocationSnapShot
    {
        private readonly double _width;
        private readonly double _height;

        public static LocationSnapShot Take(Control control)
        {
            if (control == null) throw new ArgumentNullException("control");

            return new LocationSnapShot(control.Width, control.Height);
        }

        public LocationSnapShot(double width, double height)
        {
            _width = width;
            _height = height;
        }

        public void Apply(Control control)
        {
            if (control == null) throw new ArgumentNullException("control");

            control.SetValue(Layoutable.WidthProperty, _width);
            control.SetValue(Layoutable.HeightProperty, _height);
        }
    }
}
