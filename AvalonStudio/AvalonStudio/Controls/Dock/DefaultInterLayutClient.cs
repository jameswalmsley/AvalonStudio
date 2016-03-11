using Perspex.Controls;

namespace AvalonStudio.Controls.Dock
{
    public  class DefaultInterLayutClient : IInterLayoutClient
    {
        public INewTabHost<Control> GetNewHost(object partition, AvalonViewControl source)
        {
             return new NewTabHost<Control>(source, source);
        }
    }
}
