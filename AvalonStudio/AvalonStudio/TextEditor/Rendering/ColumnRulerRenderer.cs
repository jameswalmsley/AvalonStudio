using AvalonStudio.TextEditor.Utils;
using Perspex;
using Perspex.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvalonStudio.TextEditor.Rendering
{
    /// <summary>
    /// Renders a ruler at a certain column.
    /// </summary>
    sealed class ColumnRulerRenderer : IBackgroundRenderer
    {
        Pen pen;
        int column;
        TextView textView;

        public static readonly Color DefaultForeground = Colors.LightGray;

        public ColumnRulerRenderer(TextView textView)
        {
            if (textView == null)
                throw new ArgumentNullException("textView");

            this.pen = new Pen(new SolidColorBrush(DefaultForeground), 1);
            //this.pen.Freeze();
            this.textView = textView;
            this.textView.BackgroundRenderers.Add(this);
        }

        public KnownLayer Layer
        {
            get { return KnownLayer.Background; }
        }

        public void SetRuler(int column, Pen pen)
        {
            if (this.column != column)
            {
                this.column = column;
                textView.InvalidateLayer(this.Layer);
            }
            if (this.pen != pen)
            {
                this.pen = pen;
                textView.InvalidateLayer(this.Layer);
            }
        }

        public void Draw(TextView textView, DrawingContext drawingContext)
        {
            if (column < 1) return;
            double offset = textView.WideSpaceWidth * column;
            Size pixelSize = PixelSnapHelpers.GetPixelSize(textView);
            double markerXPos = PixelSnapHelpers.PixelAlign(offset, pixelSize.Width);
            markerXPos -= textView.ScrollOffset.X;
            Point start = new Point(markerXPos, 0);

            throw new Exception("Port to perspex. DocumentHeight is ActualHeight in WPF.");
            Point end = new Point(markerXPos, Math.Max(textView.Bounds.Height, textView.DocumentHeight));

            drawingContext.DrawLine(pen, start, end);
        }
    }
}
