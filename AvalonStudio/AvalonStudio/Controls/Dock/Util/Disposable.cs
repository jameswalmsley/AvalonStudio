using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvalonStudio.Controls.Dock.Util
{
    internal static class Disposable
    {
        public static IDisposable empty
        {
            get { return DefaultDisposable.Instance; }
        }

        public static IDisposable Create(Action dispose)
        {
            if (dispose == null)
                throw new ArgumentNullException("dispose");
            else
                return (IDisposable)new AnonymousDisposable(dispose);
        }
    }
}
