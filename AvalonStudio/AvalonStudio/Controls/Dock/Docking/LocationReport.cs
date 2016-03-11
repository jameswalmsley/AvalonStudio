using System;

namespace AvalonStudio.Controls.Dock.Docking
{
    public class LocationReport
    {
        private readonly AvalonViewControl _avalonViewControl;
        private readonly Layout _rootLayout;
        private readonly Branch _parentBranch;
        private readonly bool _isLeaf;
        private readonly bool _isSecondLeaf;

        internal LocationReport(AvalonViewControl avalonViewControl, Layout rootLayout)
            : this(avalonViewControl, rootLayout, null, false)
        { }

        internal LocationReport(AvalonViewControl avalonViewControl, Layout rootLayout, Branch parentBranch, bool isSecondLeaf)
        {
            if (avalonViewControl == null) throw new ArgumentNullException("avalonViewControl");
            if (rootLayout == null) throw new ArgumentNullException("rootLayout");

            _avalonViewControl = avalonViewControl;
            _rootLayout = rootLayout;
            _parentBranch = parentBranch;
            _isLeaf = _parentBranch != null;
            _isSecondLeaf = isSecondLeaf;
        }


        public AvalonViewControl AvalonViewControl
        {
            get { return _avalonViewControl; }
        }

        public Layout RootLayout
        {
            get { return _rootLayout; }
        }

        /// <summary>
        /// Gets the parent branch if this is a leaf. If the <see cref="AvalonViewControl"/> is directly under the <see cref="RootLayout"/> will be <c>null</c>.
        /// </summary>
        public Branch ParentBranch
        {
            get { return _parentBranch; }
        }

        /// <summary>
        /// Idicates if this is a leaf in a branch. <c>True</c> if <see cref="ParentBranch"/> is not null.
        /// </summary>
        public bool IsLeaf
        {
            get { return _isLeaf; }
        }

        public bool IsSecondLeaf
        {
            get { return _isSecondLeaf; }
        }
    }
}
