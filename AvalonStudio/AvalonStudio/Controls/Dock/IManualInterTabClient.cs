using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvalonStudio.Controls.Dock
{
    public interface IManualInterTabClient : IInterTabClient
    {
        void Add(object item);
        void Remove(object item);
    }
}
