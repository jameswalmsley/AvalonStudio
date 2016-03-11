using System;

namespace AvalonStudio.Controls.Dock.Util
{
    public interface ICancelable : IDisposable
    {
        bool IsDisposed { get; }
    }
}
