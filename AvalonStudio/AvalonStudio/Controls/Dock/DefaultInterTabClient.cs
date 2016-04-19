using System;
using System.Linq;
using Perspex;
using Perspex.Controls;
using Perspex.Controls.Primitives;
using Perspex.LogicalTree;
using Perspex.Threading;
using Perspex.VisualTree;

namespace AvalonStudio.Controls.Dock
{
	public class DefaultInterTabClient : IInterTabClient
	{
		public virtual INewTabHost<TopLevel> GetNewHost(IInterTabClient interTabClient, object partition, AvalonViewControl source)
		{
			if (source == null) throw new ArgumentNullException("source");
			var sourceWindow = source.GetSelfAndVisualAncestors().OfType<TopLevel>().First();
			if (sourceWindow == null) throw new ApplicationException("Unable to ascrtain source window.");
		
			var newHost = (TopLevel)Activator.CreateInstance(sourceWindow.GetType());

			Dispatcher.UIThread.InvokeAsync(new Action(() => { }), DispatcherPriority.DataBind);

			var newAvalonViewControl = newHost.GetSelfAndLogicalAncestors().OfType<AvalonViewControl>().FirstOrDefault();
			if (newAvalonViewControl == null) throw new ApplicationException("Unable to ascrtain tab control.");

			return new NewTabHost<TopLevel>(newHost, newAvalonViewControl);

		}

		public virtual TabEmptiedResponse TabEmptiedHandler(AvalonViewControl tabControl, TopLevel window)
		{
			return TabEmptiedResponse.CloseWindowOrLayoutBranch;
		}
	}
}
