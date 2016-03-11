using Perspex.Controls;

namespace AvalonStudio.Controls.Dock
{
    /// <summary>
    /// Implementors should provide a mechanism to provide a new host control which contains a new <see cref="AvalonViewControl"/>.
    /// </summary>
    public interface IInterLayoutClient
    {
        /// <summary>
        /// Provide a new host control and tab into which will be placed into a newly created layout branch.
        /// </summary>
        /// <param name="partition">Provides the partition where the drag operation was initiated.</param>
        /// <param name="source">The source control where a dragging operation was initiated.</param>
        /// <returns></returns>
        INewTabHost<Control> GetNewHost(object partition, AvalonViewControl source);
    }
}
