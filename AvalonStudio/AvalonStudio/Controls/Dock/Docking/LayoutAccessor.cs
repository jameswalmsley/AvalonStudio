using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvalonStudio.Controls.Dock.Docking
{
    /// <summary>
    /// Provides information about the <see cref="Layout"/> instance.
    /// </summary>
    public class LayoutAccessor
    {
        private readonly Layout _layout;
        private readonly BranchAccessor _branchAccessor;
        private readonly AvalonViewControl _avalonViewControl;

        public LayoutAccessor(Layout layout)
        {
            if (layout == null) throw new ArgumentNullException("layout");

            _layout = layout;

            var branch = Layout.Content as Branch;
            if (branch != null)
                _branchAccessor = new BranchAccessor(branch);
            else
                _avalonViewControl = Layout.Content as AvalonViewControl;
        }

        public Layout Layout
        {
            get { return _layout; }
        }

        public IEnumerable<AvalonViewItem> FloatingItems
        {
            get { return _layout.FloatingAvalonViewItems(); }
        }

        /// <summary>
        /// <see cref="BranchAccessor"/> and <see cref="AvalonViewControl"/> are mutually exclusive, according to whether the layout has been split, or just contains a tab control.
        /// </summary>
        public BranchAccessor BranchAccessor
        {
            get { return _branchAccessor; }
        }

        /// <summary>
        /// <see cref="BranchAccessor"/> and <see cref="AvalonViewControl"/> are mutually exclusive, according to whether the layout has been split, or just contains a tab control.
        /// </summary>
        public AvalonViewControl AvalonViewControl
        {
            get { return _avalonViewControl; }
        }

        /// <summary>
        /// Visits the content of the layout, according to its content type.  No more than one of the provided <see cref="Action"/>
        /// callbacks will be called.  
        /// </summary>        
        public LayoutAccessor Visit(
            Action<BranchAccessor> branchVisitor = null,
            Action<AvalonViewControl> avalonViewControlVisitor = null,
            Action<object> contentVisitor = null)
        {
            if (_branchAccessor != null)
            {
                if (branchVisitor != null)
                    branchVisitor(_branchAccessor);

                return this;
            }

            if (_avalonViewControl != null)
            {
                if (avalonViewControlVisitor != null)
                    avalonViewControlVisitor(_avalonViewControl);

                return this;
            }

            if (_layout.Content != null && contentVisitor != null)
                contentVisitor(_layout.Content);

            return this;
        }
    }
}
