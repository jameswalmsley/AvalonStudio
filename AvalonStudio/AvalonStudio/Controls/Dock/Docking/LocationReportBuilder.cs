using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvalonStudio.Controls.Dock.Docking
{
    public class LocationReportBuilder
    {
        private readonly AvalonViewControl _targetAvalonViewControl;
        private Branch _branch;
        private bool _isSecondLeaf;
        private Layout _layout;

        public LocationReportBuilder(AvalonViewControl targetAvalonViewControl)
        {
            _targetAvalonViewControl = targetAvalonViewControl;
        }

        public AvalonViewControl TargetAvalonViewControl
        {
            get { return _targetAvalonViewControl; }
        }

        public bool IsFound { get; private set; }

        public void MarkFound()
        {
            if (IsFound)
                throw new InvalidOperationException("Already found.");

            IsFound = true;

            _layout = CurrentLayout;
        }

        public void MarkFound(Branch branch, bool isSecondLeaf)
        {
            if (branch == null) throw new ArgumentNullException("branch");
            if (IsFound)
                throw new InvalidOperationException("Already found.");

            IsFound = true;

            _layout = CurrentLayout;
            _branch = branch;
            _isSecondLeaf = isSecondLeaf;
        }

        public Layout CurrentLayout { get; set; }

        public LocationReport ToLocationReport()
        {
            return new LocationReport(_targetAvalonViewControl, _layout, _branch, _isSecondLeaf);
        }
    }
}
