using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Perspex.Controls;
using Perspex.LogicalTree;
using Perspex.Threading;
using Perspex.VisualTree;

namespace AvalonStudio.Controls.Dock
{
    public class DefaultInterTabClient : IInterTabClient
    {
        public virtual INewTabHost<Window> GetNewHost(IInterTabClient interTabClient, object partition, AvalonViewControl source)
        {
            if (source == null) throw new ArgumentNullException("source");
            var sourceWindow = source.GetSelfAndVisualAncestors().OfType<Window>().First();
            if (sourceWindow == null) throw new ApplicationException("Unable to ascrtain source window.");
            var newWindow = (Window)Activator.CreateInstance(sourceWindow.GetType());

            Dispatcher.UIThread.InvokeAsync(new Action(() => { }), DispatcherPriority.DataBind);

            var newAvalonViewControl = newWindow.GetSelfAndLogicalAncestors().OfType<AvalonViewControl>().FirstOrDefault();
            if (newAvalonViewControl == null) throw new ApplicationException("Unable to ascrtain tab control.");

            return new NewTabHost<Window>(newWindow, newAvalonViewControl);

        }

        public virtual TabEmptiedResponse TabEmptiedHandler(AvalonViewControl tabControl, Window window)
        {
            return TabEmptiedResponse.CloseWindowOrLayoutBranch;
        }
    }
}
