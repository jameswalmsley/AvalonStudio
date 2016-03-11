using System;
using System.Collections.Generic;
using System.Linq;
using Perspex;
using Perspex.Controls;
using Perspex.Markup.Xaml.Templates;
using Perspex.Styling;

namespace AvalonStudio.Controls.Dock
{
    public class AvalonViewControl : TabControl
    {
        private static readonly HashSet<AvalonViewControl> LoadedInstances = new HashSet<AvalonViewControl>();

        public AvalonViewControl()
        {

        }

        private void AvalonViewControlPropertyChanged(object sender, PerspexPropertyChangedEventArgs e)
        {

        }

        public static IEnumerable<AvalonViewControl> GetLoadedInstances()
        {
            return LoadedInstances.ToList();
        } 

        public static readonly PerspexProperty<Style> CustomHeaderStyleProperty =
            PerspexProperty.Register<AvalonViewControl, Style>("CustomHeaderStyle");

        public Style CustomHeaderStyle
        {
            get { return GetValue(CustomHeaderStyleProperty); }
            set { SetValue(CustomHeaderStyleProperty, value); }
        }

        public static readonly PerspexProperty<DataTemplate> CustomHeaderItemTemplateProperty =
            PerspexProperty.Register<AvalonViewControl, DataTemplate>("CustomHeaderItemTemplate");

        public DataTemplate CustomHeaderItemTemplate
        {
            get { return GetValue(CustomHeaderItemTemplateProperty); }
            set { SetValue(CustomHeaderItemTemplateProperty, value); }
        }

        public static readonly PerspexProperty<double> AdjacentHeaderItemOffsetProperty =
            PerspexProperty.Register<AvalonViewControl, double>("AdjacentHeaderItemOffset");

        //private static void AdjacentHeaderItemOffsetPropertyChangedCallback(PerspexObject perspexObject, PerspexPropertyChangedEventArgs perspexPropertyChangedEventArgs)
        //{
        //    perspexObject.SetValue(HeaderItemsOrganiserProperty, new HorizontalOrganiser((double)perspexPropertyChangedEventArgs.NewValue));
        //}

        public double AdjacentHeaderItemOffset
        {
            get { return GetValue(AdjacentHeaderItemOffsetProperty); }
            set { SetValue(AdjacentHeaderItemOffsetProperty, value); }
        }

        public static readonly PerspexProperty<IItemsOrganizer> HeaderItemsOrganizerProperty =
            PerspexProperty.Register<AvalonViewControl, IItemsOrganizer>("HeaderItemsOrganizer");

        public IItemsOrganizer HeaderItemsOrganizer
        {
            get { return GetValue(HeaderItemsOrganizerProperty); }
            set { SetValue(HeaderItemsOrganizerProperty, value); }
        }

        public static readonly PerspexProperty<string> HeaderMemberPathProperty =
            PerspexProperty.Register<AvalonViewControl, string>("HeaderMemberPath");

        public string HeaderMemberPath
        {
            get { return GetValue(HeaderMemberPathProperty); }
            set { SetValue(HeaderMemberPathProperty, value); }
        }

        public static readonly PerspexProperty<DataTemplate> HeaderItemTemplateProperty =
            PerspexProperty.Register<AvalonViewControl, DataTemplate>("HeaderItemTemplate");

        public DataTemplate HeaderItemTemplate
        {
            get { return GetValue(HeaderItemTemplateProperty); }
            set { SetValue(HeaderItemTemplateProperty, value); }
        }

        public static readonly PerspexProperty<object> HeaderPrefixContentProperty =
            PerspexProperty.Register<AvalonViewControl, object>("HeaderPrefixContent");

        public object HeaderPrefixContent
        {
            get { return GetValue(HeaderPrefixContentProperty); }
            set { SetValue(HeaderPrefixContentProperty, value); }
        }

        public static readonly PerspexProperty<string> HeaderPrefixContentStringFormatProperty =
            PerspexProperty.Register<AvalonViewControl, string>("HeaderPrefixContentStringFormat");

        public string HeaderPrefixContentStringFormat
        {
            get { return GetValue(HeaderPrefixContentStringFormatProperty); }
            set { SetValue(HeaderPrefixContentStringFormatProperty, value); }
        }

        public static readonly PerspexProperty<DataTemplate> HeaderPrefixContentTemplateProperty =
            PerspexProperty.Register<AvalonViewControl, DataTemplate>("HeaderPrefixContentTemplate");

        public DataTemplate HeaderPrefixContentTemplate
        {
            get { return GetValue(HeaderPrefixContentTemplateProperty); }
            set { SetValue(HeaderPrefixContentTemplateProperty, value); }
        }

        //public static readonly PerspexProperty<DataTemplateSelector> HeaderPrefixContentTemplateSelectorProperty =
        //    PerspexProperty.Register<AvalonViewControl, DataTemplateSelector>("HeaderPrefixContentTemplateSelector");

        //public DataTemplateSelector HeaderPrefixContentTemplateSelector
        //{
        //    get { return GetValue(HeaderPrefixContentTemplateSelectorProperty); }
        //    set { SetValue(HeaderPrefixContentTemplateSelectorProperty, value); }
        //}

        public static readonly PerspexProperty<object> HeaderSuffixContentProperty =
            PerspexProperty.Register<AvalonViewControl, object>("HeaderSuffixContent");

        public object HeaderSuffixContent
        {
            get { return GetValue(HeaderSuffixContentProperty); }
            set { SetValue(HeaderSuffixContentProperty, value); }
        }

        public static readonly PerspexProperty<string> HeaderSuffixContentStringFormatProperty =
            PerspexProperty.Register<AvalonViewControl, string>("HeaderSuffixContentStringFormat");

        public string HeaderSuffixContentStringFormat
        {
            get { return GetValue(HeaderSuffixContentStringFormatProperty); }
            set { SetValue(HeaderSuffixContentStringFormatProperty, value); }
        }

        public static readonly PerspexProperty<DataTemplate> HeaderSuffixContentTemplateProperty =
            PerspexProperty.Register<AvalonViewControl, DataTemplate>("HeaderSuffixContentTemplate");

        public DataTemplate HeaderSuffixContentTemplate
        {
            get { return GetValue(HeaderSuffixContentTemplateProperty); }
            set { SetValue(HeaderSuffixContentTemplateProperty, value); }
        }

        //public static readonly PerspexProperty<DataTemplateSelector> HeaderSuffixContentTemplateSelectorProperty =
        //    PerspexProperty.Register<AvalonViewControl, DataTemplateSelector>("HeaderSuffixContentTemplateSelector");

        //public DataTemplateSelector HeaderSuffixContentTemplateSelector
        //{
        //    get { return GetValue(HeaderSuffixContentTemplateSelectorProperty); }
        //    set { SetValue(HeaderSuffixContentTemplateSelectorProperty, value); }
        //}

        public static readonly PerspexProperty<bool> ShowDefaultCloseButtonProperty =
            PerspexProperty.Register<AvalonViewControl, bool>("ShowDefaultCloseButton");

        public bool ShowDefaultCloseButton
        {
            get { return GetValue(ShowDefaultCloseButtonProperty); }
            set { SetValue(ShowDefaultCloseButtonProperty, value); }
        }

        public static readonly PerspexProperty<bool> ShowDefaultAddButtonProperty =
            PerspexProperty.Register<AvalonViewControl, bool>("ShowDefaultAddButton");

        public bool ShowDefaultAddButton
        {
            get { return GetValue(ShowDefaultAddButtonProperty); }
            set { SetValue(ShowDefaultAddButtonProperty, value); }
        }

        public static readonly PerspexProperty<AddLocationHint> AddLocationHintProperty =
            PerspexProperty.Register<AvalonViewControl, AddLocationHint>("AddLocationHint");

        public AddLocationHint AddLocationHint
        {
            get { return GetValue(AddLocationHintProperty); }
            set { SetValue(AddLocationHintProperty, value); }
        }

        public static readonly PerspexProperty<int> FixedHeaderCoundProperty =
            PerspexProperty.Register<AvalonViewControl, int>("FixedHeaderCound");

        public int FixedHeaderCound
        {
            get { return GetValue(FixedHeaderCoundProperty); }
            set { SetValue(FixedHeaderCoundProperty, value); }
        }

        public static readonly PerspexProperty<InterTabController> InterTabControllerProperty =
            PerspexProperty.Register<AvalonViewControl, InterTabController>("InterTabController");

        //private static void InterTabControllerPropertyChangedCallback(PerspexObject perspexObject, PerspexPropertyChangedEventArgs perspexPropertyChangedEventArgs)
        //{
        //    var instance = (AvalonViewControl)perspexObject;
        //    if (perspexPropertyChangedEventArgs.OldValue != null)
        //        instance.RemoveLogicalChild(perspexPropertyChangedEventArgs.OldValue);
        //    if (perspexPropertyChangedEventArgs.NewValue != null)
        //        instance.AddLogicalChild(perspexPropertyChangedEventArgs.NewValue);
        //}


        public InterTabController InterTabController
        {
            get { return GetValue(InterTabControllerProperty); }
            set { SetValue(InterTabControllerProperty, value); }
        }

        public static readonly PerspexProperty<Func<object>> NewItemFactoryProperty =
            PerspexProperty.Register<AvalonViewControl, Func<object>>("NewItemFactory");

        public Func<object> NewItemFactory
        {
            get { return GetValue(NewItemFactoryProperty); }
            set { SetValue(NewItemFactoryProperty, value); }
        }

        // TODO: IsEmpty Property

        // TODO: IsEmptyChanged RoutedEvent

        public static readonly PerspexProperty<ItemActionCallback> ClosingItemCallbackProperty =
            PerspexProperty.Register<AvalonViewControl, ItemActionCallback>("ClosingItemCallback");

        public ItemActionCallback ClosingItemCallback
        {
            get { return GetValue(ClosingItemCallbackProperty); }
            set { SetValue(ClosingItemCallbackProperty, value); }
        }

        public static readonly PerspexProperty<bool> ConsolidateOrphanedItemsProperty =
            PerspexProperty.Register<AvalonViewControl, bool>("ConsolidateOrphanedItems");

        public bool ConsolidateOrphanedItems
        {
            get { return GetValue(ConsolidateOrphanedItemsProperty); }
            set { SetValue(ConsolidateOrphanedItemsProperty, value); }
        }

        public static readonly PerspexProperty<ItemActionCallback> ConsolidatingOrphanedItemCallbackProperty =
            PerspexProperty.Register<AvalonViewControl, ItemActionCallback>("ConsolidatingOrphanedItemCallback");

        public ItemActionCallback ConsolidateOrphanedItemCallback
        {
            get { return GetValue(ConsolidatingOrphanedItemCallbackProperty); }
            set { SetValue(ConsolidatingOrphanedItemCallbackProperty, value); }
        }


        public static AvalonViewControl GetOwnerOfHeaderItems(AvalonViewItemsControl sourceOfAvalonViewItemsControl)
        {
            throw new NotImplementedException();
        }
    }
}
