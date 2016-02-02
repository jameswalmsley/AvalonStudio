using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvalonStudio.Controls.Dock.Docking
{
    internal class FloatTransfer
    {
        private readonly object _content;

        public FloatTransfer(object content)
        {
            if (content == null) throw new ArgumentNullException("content");

            _content = content;
        }

        public static FloatTransfer TakeSnapshot(AvalonViewItem avalonViewItem, AvalonViewControl sourceTabControl)
        {
            if (avalonViewItem == null) throw new ArgumentNullException("avalonViewItem");

            return new FloatTransfer(sourceTabControl.ActualWidth, sourceTabControl.ActualHeight, avalonViewItem.UnderlyingContent ?? avalonViewItem.Content ?? avalonViewItem);
        }

        public object Content
        {
            get { return _content; }
        }
    }
}
