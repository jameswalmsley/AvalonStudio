using System;
using Perspex.Controls;

namespace AvalonStudio.Controls.Dock
{
    public class NewTabHost<TElement> : INewTabHost<TElement> where TElement : Control
    {
        private readonly TElement _container;
        private readonly AvalonViewControl _avalonViewControl;

        public NewTabHost(TElement container, AvalonViewControl avalonViewControl)
        {
            if (container == null) throw new ArgumentNullException("container");
            if (avalonViewControl == null) throw new ArgumentNullException("avalonViewControl");

            _container = container;
            _avalonViewControl = avalonViewControl;
        }

        public TElement Container
        {
            get { return _container; }
        }

        public AvalonViewControl AvalonViewControl
        {
            get { return _avalonViewControl; }
        }
    }
}
