using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Perspex;
using Perspex.VisualTree;

namespace AvalonStudio.Controls.Dock.Docking
{
    public class BranchAccessor
    {
        private readonly Branch _branch;
        private readonly BranchAccessor _firstBranchAccessor;
        private readonly BranchAccessor _secondBranchAccessor;
        private readonly AvalonViewControl _firstAvalonViewControl;
        private readonly AvalonViewControl _secondAvalonViewControl;

        public BranchAccessor(Branch branch)
        {
            if (branch == null) throw new ArgumentNullException("branch");

            _branch = branch;

            var firstChildBranch = branch.FirstItem as Branch;
            if (firstChildBranch != null)
                _firstBranchAccessor = new BranchAccessor(firstChildBranch);
            else
                _firstAvalonViewControl = FindAvalonViewControl(branch.FirstItem, branch.FirstContentPresenter);

            var secondChildBranch = branch.SecondItem as Branch;
            if (secondChildBranch != null)
                _secondBranchAccessor = new BranchAccessor(secondChildBranch);
            else
                _secondAvalonViewControl = FindAvalonViewControl(branch.SecondItem, branch.SecondContentPresenter);

        }

        public Branch Branch
        {
            get { return _branch; }
        }

        public BranchAccessor FirstBranchAccessor
        {
            get { return _firstBranchAccessor; }
        }

        public BranchAccessor SecondBranchAccessor
        {
            get { return _secondBranchAccessor; }
        }

        public AvalonViewControl FirstAvalonViewControl
        {
            get { return _firstAvalonViewControl; }
        }

        public AvalonViewControl SecondAvalonViewControl
        {
            get { return _secondAvalonViewControl; }
        }

        private static AvalonViewControl FindAvalonViewControl(object item, PerspexObject contentPresenter)
        {
            var result = item as AvalonViewControl;
            return result ??
                   ((IVisual) contentPresenter).GetSelfAndVisualDescendents()
                       .OfType<AvalonViewControl>()
                       .FirstOrDefault();
        }

        public BranchAccessor Visit(BranchItem childItem, Action<BranchAccessor> childBranchVisitor = null,
            Action<AvalonViewControl> childAvalonViewControlVisitor = null, Action<object> childContentVisitor = null)
        {
            Func<BranchAccessor> branchGetter;
            Func<AvalonViewControl> tabGetter;
            Func<object> contentGetter;

            switch (childItem)
            {
                case BranchItem.First:
                    branchGetter = () => _firstBranchAccessor;
                    tabGetter = () => _firstAvalonViewControl;
                    contentGetter = () => _branch.FirstItem;
                    break;
                case BranchItem.Second:
                    branchGetter = () => _secondBranchAccessor;
                    tabGetter = () => _secondAvalonViewControl;
                    contentGetter = () => _branch.SecondItem;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("childItem");
            }

            var branchDescription = branchGetter();
            if (branchDescription != null)
            {
                if (childBranchVisitor != null)
                    childBranchVisitor(branchDescription);
                return this;
            }

            var avalonViewControl = tabGetter();
            if (avalonViewControl != null)
            {
                if (childAvalonViewControlVisitor != null)
                    childAvalonViewControlVisitor(avalonViewControl);
                return this;
            }

            if (childContentVisitor == null) return this;

            var content = contentGetter();
            if (content != null)
                childContentVisitor(content);

            return this;
        }
    }
}
