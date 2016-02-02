using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvalonStudio.Controls.Dock.Docking
{
    public class BranchResult
    {
        private readonly Branch _branch;
        private readonly AvalonViewControl _avalonViewControl;

        public BranchResult(Branch branch, AvalonViewControl avalonViewControl)
        {
            if (branch == null) throw new ArgumentNullException("branch");
            if (avalonViewControl == null) throw new ArgumentNullException("avalonViewControl");

            _branch = branch;
            _avalonViewControl = avalonViewControl;
        }

        public Branch Branch
        {
            get { return _branch; }
        }

        public AvalonViewControl ViewControl
        {
            get { return _avalonViewControl; }
        }
    }
}
