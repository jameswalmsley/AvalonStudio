using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvalonStudio.Controls.Dock.Util
{
    public interface ICancelable : IDisposable
    {
        bool IsDisposed { get; }
    }
}
