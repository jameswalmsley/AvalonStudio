using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Perspex;
using Perspex.Controls;

namespace AvalonStudio.Controls.Dock
{
    public class InterTabController : Control
    {
        public InterTabController()
        {
            
        }

        public static readonly PerspexProperty<double> HorizontalPopoutGraceProperty =
            PerspexProperty.Register<InterTabController, double>("HorizontalPopoutGrace", 8.0);

        public double HorizontalPopoutGrace
        {
            get { return GetValue(HorizontalPopoutGraceProperty); }
            set { SetValue(HorizontalPopoutGraceProperty, value); }
        }

        public static readonly PerspexProperty<double> VerticalPopoutGraceProperty =
            PerspexProperty.Register<InterTabController, double>("VerticalPopoutGrace", 8.0);

        public double VerticalPopoutGrace
        {
            get { return GetValue(VerticalPopoutGraceProperty); }
            set { SetValue(VerticalPopoutGraceProperty, value); }
        }

        public static readonly PerspexProperty<bool> MoveWindowWithSolitaryTabsProperty =
            PerspexProperty.Register<InterTabController, bool>("MoveWindowWithSolitaryTabs", true);

        public bool MoveWindowWithSolitaryTabs
        {
            get { return GetValue(MoveWindowWithSolitaryTabsProperty); }
            set { SetValue(MoveWindowWithSolitaryTabsProperty, value); }
        }

        public static readonly PerspexProperty<IInterTabClient> InterTabClientProperty =
            PerspexProperty.Register<InterTabController, IInterTabClient>("InterTabClient", new DefaultInterTabClient());

        public IInterTabClient InterTabClient
        {
            get { return GetValue(InterTabClientProperty); }
            set { SetValue(InterTabClientProperty, value); }
        }
    }
}
