using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Perspex.Controls;

namespace AvalonStudio.Controls.Dock
{
    public delegate void ItemActionCallback(ItemActionCallbackArgs<AvalonViewControl> args);

    public class ItemActionCallbackArgs<TOwner> where TOwner : Control
    {
        private readonly Window _window;
        private readonly TOwner _owner;
        private readonly AvalonViewItem _avalonViewItem;

        public ItemActionCallbackArgs(Window window, TOwner owner, AvalonViewItem avalonViewItem)
        {
            if (window == null) throw new ArgumentNullException("window");
            if (owner == null) throw new ArgumentNullException("owner");
            if (avalonViewItem == null) throw new ArgumentNullException("avalonViewItem");

            _window = window;
            _owner = owner;
            _avalonViewItem = avalonViewItem;
        }

        public Window Window1
        {
            get { return _window; }
        }

        public TOwner Owner
        {
            get { return _owner; }
        }

        public AvalonViewItem ViewItem
        {
            get { return _avalonViewItem; }
        }

        public bool IsCancelled { get; private set; }

        public void Cancel()
        {
            IsCancelled = true;
        }
    }
}
