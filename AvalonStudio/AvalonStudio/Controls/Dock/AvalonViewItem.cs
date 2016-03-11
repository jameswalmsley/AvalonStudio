using Perspex;
using Perspex.Controls;

namespace AvalonStudio.Controls.Dock
{
    public enum SizeGrip
    {
        NotApplicable,
        Left,
        TopLeft,
        Top,
        TopRight,
        Right,
        BottomRight,
        Bottom,
        BottomLeft
    }

    public class AvalonViewItem : ContentControl
    {
        public static readonly PerspexProperty<double> XProperty =
            PerspexProperty.RegisterDirect<AvalonViewItem, double>("X", o => o.X,
                (o, v) => o.X = v);

        private double _x;

        public double X
        {
            get { return _x; }
            set { SetAndRaise(XProperty, ref _x, value); }
        }

        public static readonly PerspexProperty<double> YProperty =
            PerspexProperty.RegisterDirect<AvalonViewItem, double>("Y", o => o.Y,
                (o, v) => o.Y = v);

        private double _y;

        public double Y
        {
            get { return _y; }
            set { SetAndRaise(YProperty, ref _y, value); }
        }

        internal string PartitionAtDragStart { get; set; }
    }
}
