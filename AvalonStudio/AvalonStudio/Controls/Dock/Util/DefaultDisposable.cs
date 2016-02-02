using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvalonStudio.Controls.Dock.Util
{
    internal sealed class DefaultDisposable : IDisposable
    {
        public static readonly DefaultDisposable Instance = new DefaultDisposable();

        static DefaultDisposable()
        {
        }

        private DefaultDisposable()
        {
        }

        public void Dispose()
        {
        }
    }
}
