using Perspex.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Perspex.Media;

namespace AvalonStudio.TextEditor.Rendering
{
    /// <summary>
    /// Base class for known layers.
    /// </summary>
    class Layer : Control
    {
        protected readonly TextView textView;
        protected readonly KnownLayer knownLayer;

        public Layer(TextView textView, KnownLayer knownLayer)
        {
            Debug.Assert(textView != null);
            this.textView = textView;
            this.knownLayer = knownLayer;
            this.Focusable = false;
        }

        //protected override GeometryHitTestResult HitTestCore(GeometryHitTestParameters hitTestParameters)
        //{
        //    return null;
        //}

        //protected override HitTestResult HitTestCore(PointHitTestParameters hitTestParameters)
        //{
        //    return null;
        //}

        public override void Render(DrawingContext context)
        {
            base.Render(context);

            textView.RenderBackground(context, knownLayer);
        }
    }
}
