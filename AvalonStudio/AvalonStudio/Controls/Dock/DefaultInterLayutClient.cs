using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Perspex;
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
