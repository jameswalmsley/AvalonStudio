using System;
using Perspex.Interactivity;

namespace AvalonStudio.Controls.Dock
{
    public class AvalonViewDragDeltaEventArgs : RoutedEvent
    {
        public AvalonViewDragDeltaEventArgs(string name, RoutingStrategies routingStrategies, Type eventArgsType, Type ownerType) : base(name, routingStrategies, eventArgsType, ownerType)
        {
        }
    }
}
