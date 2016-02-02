using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Perspex;

namespace AvalonStudio.Controls.Dock
{
    internal class ContainerCustomizations
    {
        private readonly Func<AvalonViewItem> _getContainerForItemOverride;
        private readonly Action<PerspexObject, object> _prepareContainerForItemOverride;
        private readonly Action<PerspexObject, object> _clearingContainerForItemOverride;

        public ContainerCustomizations(Func<AvalonViewItem> getContainerForItemOverride = null, Action<PerspexObject, object> prepareContainerForItemOverride = null, Action<PerspexObject, object> clearingContainerForItemOverride = null)
        {
            _getContainerForItemOverride = getContainerForItemOverride;
            _prepareContainerForItemOverride = prepareContainerForItemOverride;
            _clearingContainerForItemOverride = clearingContainerForItemOverride;
        }

        public Func<AvalonViewItem> GetContainerForItemOverride
        {
            get { return _getContainerForItemOverride; }
        }

        public Action<PerspexObject, object> PrepareContainerForItemOverride
        {
            get { return _prepareContainerForItemOverride; }
        }

        public Action<PerspexObject, object> ClearingContainerForItemOverride
        {
            get { return _clearingContainerForItemOverride; }
        }
    }
}
