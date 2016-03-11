namespace AvalonStudio.Controls.Dock
{
    public interface IManualInterTabClient : IInterTabClient
    {
        void Add(object item);
        void Remove(object item);
    }
}
