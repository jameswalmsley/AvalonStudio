using Perspex.Controls;

namespace AvalonStudio.Controls.Dock
{
    public interface INewTabHost<out TControl> where TControl : Control
    {
        TControl Container { get; }

        AvalonViewControl AvalonViewControl { get; }
    }
}
