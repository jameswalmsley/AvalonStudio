using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Perspex;
using Perspex.Controls;
using Perspex.Controls.Presenters;
using Perspex.Controls.Primitives;
using Perspex.LogicalTree;
using Perspex.Markup.Xaml.Templates;
using Perspex.Styling;
using Perspex.VisualTree;

namespace AvalonStudio.Controls.Dock.Docking
{
	public delegate void ClosingFloatingItemCallback(ItemActionCallbackArgs<Layout> args);

	public class Layout : ContentControl
	{
		private static readonly HashSet<Layout> LoadedLayouts = new HashSet<Layout>();
		private const string TopDropZonePartName = "PART_TopDropZone";
		private const string RightDropZonePartName = "PART_RightDropZone";
		private const string BottomDropZonePartName = "PART_BottomDropZone";
		private const string LeftDropZonePartName = "PART_LeftDropZone";
		private const string FloatingDropZonePartName = "PART_FloatDropZone";
		private const string FloatingContentPresenterPartName = "PART_FloatContentPresenter";

		//public static RoutedCommand UnfloatItemCommand = new RoutedCommand();
		//public static RoutedCommand MaximiseFloatingItem = new RoutedCommand();
		//public static RoutedCommand RestoreFloatingItem = new RoutedCommand();
		//public static RoutedCommand CloseFloatingItem = new RoutedCommand();
		//public static RoutedCommand TileFloatingItemsCommand = new RoutedCommand();
		//public static RoutedCommand TileFloatingItemsVerticallyCommand = new RoutedCommand();
		//public static RoutedCommand TileFloatingItemsHorizontallyCommand = new RoutedCommand();

		private readonly IDictionary<DropZoneLocation, DropZone> _dropZones = new Dictionary<DropZoneLocation, DropZone>();
		private static Tuple<Layout, DropZone> _currentlyOfferedDropZone;
		private readonly AvalonViewItemsControl _floatingItems;

		static Layout()
		{
			//DefaultStyleKeyProperty.OverrideMetadata(typeof(Layout), new FrameworkPropertyMetadata(typeof(Layout)));

			//EventManager.RegisterClassHandler(typeof(DragablzItem), DragablzItem.DragStarted, new DragablzDragStartedEventHandler(ItemDragStarted));
			//EventManager.RegisterClassHandler(typeof(DragablzItem), DragablzItem.PreviewDragDelta, new DragablzDragDeltaEventHandler(PreviewItemDragDelta), true);
			//EventManager.RegisterClassHandler(typeof(DragablzItem), DragablzItem.DragCompleted, new DragablzDragCompletedEventHandler(ItemDragCompleted));
		}

		public Layout()
		{
			//Loaded += (sender, args) => LoadedLayouts.Add(this);
			//Unloaded += (sender, args) => LoadedLayouts.Remove(this);

			//CommandBindings.Add(new CommandBinding(UnfloatItemCommand, UnfloatExecuted, CanExecuteUnfloat));
			//CommandBindings.Add(new CommandBinding(MaximiseFloatingItem, MaximiseFloatingItemExecuted, CanExecuteMaximiseFloatingItem));
			//CommandBindings.Add(new CommandBinding(CloseFloatingItem, CloseFloatingItemExecuted, CanExecuteCloseFloatingItem));
			//CommandBindings.Add(new CommandBinding(RestoreFloatingItem, RestoreFloatingItemExecuted, CanExecuteRestoreFloatingItem));
			//CommandBindings.Add(new CommandBinding(TileFloatingItemsCommand, TileFloatingItemsExecuted));
			//CommandBindings.Add(new CommandBinding(TileFloatingItemsCommand, TileFloatingItemsExecuted));
			//CommandBindings.Add(new CommandBinding(TileFloatingItemsVerticallyCommand, TileFloatingItemsVerticallyExecuted));
			//CommandBindings.Add(new CommandBinding(TileFloatingItemsHorizontallyCommand, TileFloatingItemsHorizontallyExecuted));

			////TODO bad bad behaviour.  Pick up this from the template.
			//_floatingItems = new DragablzItemsControl
			//{
			//    ContainerCustomisations = new ContainerCustomisations(
			//        GetFloatingContainerForItemOverride,
			//        PrepareFloatingContainerForItemOverride,
			//        ClearingFloatingContainerForItemOverride)
			//};

			//var floatingItemsSourceBinding = new Binding("FloatingItemsSource") { Source = this };
			//_floatingItems.SetBinding(ItemsControl.ItemsSourceProperty, floatingItemsSourceBinding);
			//var floatingItemsControlStyleBinding = new Binding("FloatingItemsControlStyle") { Source = this };
			//_floatingItems.SetBinding(StyleProperty, floatingItemsControlStyleBinding);
			//var floatingItemTemplateBinding = new Binding("FloatingItemTemplate") { Source = this };
			//_floatingItems.SetBinding(ItemsControl.ItemTemplateProperty, floatingItemTemplateBinding);
			//var floatingItemTemplateSelectorBinding = new Binding("FloatingItemTemplateSelector") { Source = this };
			//_floatingItems.SetBinding(ItemsControl.ItemTemplateSelectorProperty, floatingItemTemplateSelectorBinding);
			//var floatingItemContainerStyeBinding = new Binding("FloatingItemContainerStyle") { Source = this };
			//_floatingItems.SetBinding(ItemsControl.ItemContainerStyleProperty, floatingItemContainerStyeBinding);
			//var floatingItemContainerStyleSelectorBinding = new Binding("FloatingItemContainerStyleSelector") { Source = this };
			//_floatingItems.SetBinding(ItemsControl.ItemContainerStyleSelectorProperty, floatingItemContainerStyleSelectorBinding);
		}

		/// <summary>
		/// Helper method to get all the currently loaded layouts.
		/// </summary>
		/// <returns></returns>
		public static IEnumerable<Layout> GetLoadedInstances()
		{
			return LoadedLayouts.ToList();
		}

		/// <summary>
		/// Finds the location of a tab control withing a layout.
		/// </summary>
		/// <param name="avalonViewControl"></param>
		/// <returns></returns>
		public static LocationReport Find(AvalonViewControl avalonViewControl)
		{
			if (avalonViewControl == null) throw new ArgumentNullException("avalonViewControl");

			return Finder.Find(avalonViewControl);
		}

		/// <summary>
		/// Creates a split in a layout, at the location of a specified <see cref="avalonViewControl"/>.
		/// </summary>
		/// <para></para>
		/// <param name="avalonViewControl">Tab control to be split.</param>
		/// <param name="orientation">Direction of split.</param>
		/// <param name="makeSecond">Set to <c>true</c> to make the current tab control push into the right hand or bottom of the split.</param>
		/// <remarks>The tab control to be split must be hosted in a layout control.</remarks>
		public static BranchResult Branch(AvalonViewControl avalonViewControl, Orientation orientation, bool makeSecond)
		{
			return Branch(avalonViewControl, orientation, makeSecond, .5);
		}

		/// <summary>
		/// Creates a split in a layout, at the location of a specified <see cref="avalonViewControl"/>.
		/// </summary>
		/// <para></para>
		/// <param name="avalonViewControl">Tab control to be split.</param>
		/// <param name="orientation">Direction of split.</param>
		/// <param name="makeSecond">Set to <c>true</c> to make the current tab control push into the right hand or bottom of the split.</param>
		/// <param name="firstItemProportion">Sets the proportion of the first tab control, with 0.5 being 50% of available space.</param>
		/// <remarks>The tab control to be split must be hosted in a layout control.</remarks>
		public static BranchResult Branch(AvalonViewControl avalonViewControl, Orientation orientation, bool makeSecond, double firstItemProportion)
		{
			if (firstItemProportion < 0.0 || firstItemProportion > 1.0) throw new ArgumentOutOfRangeException("firstItemProportion", "Must be >= 0.0 and <= 1.0");

			var locationReport = Find(avalonViewControl);

			Action<Branch> applier;
			object existingContent;
			if (!locationReport.IsLeaf)
			{
				existingContent = locationReport.RootLayout.Content;
				applier = branch => locationReport.RootLayout.Content = branch;
			}
			else if (!locationReport.IsSecondLeaf)
			{
				existingContent = locationReport.ParentBranch.FirstItem;
				applier = branch => locationReport.ParentBranch.FirstItem = branch;
			}
			else
			{
				existingContent = locationReport.ParentBranch.SecondItem;
				applier = branch => locationReport.ParentBranch.SecondItem = branch;
			}

			var selectedItem = avalonViewControl.SelectedItem;
			var branchResult = Branch(orientation, firstItemProportion, makeSecond, locationReport.RootLayout.BranchTemplate, existingContent, applier);
			avalonViewControl.SelectedItem = selectedItem;

			return branchResult;
		}

		private static BranchResult Branch(Orientation orientation, double proportion, bool makeSecond, DataTemplate branchTemplate, object existingContent, Action<Branch> applier)
		{
			var branchItem = new Branch
			{
				Orientation = orientation
			};

			var newContent = new ContentControl
			{
				Content = new object(),
			};

			newContent.DataTemplates.Add(branchTemplate);

			if (!makeSecond)
			{
				branchItem.FirstItem = existingContent;
				branchItem.SecondItem = newContent;
			}
			else
			{
				branchItem.FirstItem = newContent;
				branchItem.SecondItem = existingContent;
			}

			branchItem.SetValue(Docking.Branch.FirstItemLengthProperty, new GridLength(proportion, GridUnitType.Star));
			branchItem.SetValue(Docking.Branch.SecondItemLengthProperty, new GridLength(1 - proportion, GridUnitType.Star));

			applier(branchItem);

			var newAvalonViewControl = newContent.GetVisualChildren().OfType<AvalonViewControl>().FirstOrDefault();

			if (newAvalonViewControl == null)
				throw new ApplicationException("New AvalonViewControl was not generated inside branch.");

			return new BranchResult(branchItem, newAvalonViewControl);
		}


		public string Partition { get; set; }

		public static readonly PerspexProperty<IInterLayoutClient> InterLayoutClientProperty =
			PerspexProperty.Register<Layout, IInterLayoutClient>("InterLayoutClient");

		public IInterLayoutClient InterLayoutClient
		{
			get { return GetValue(InterLayoutClientProperty); }
			set { SetValue(InterLayoutClientProperty, value); }
		}

		internal static bool IsContainedWithinBranch(PerspexObject perspexObject)
		{
			do
			{
				perspexObject = ((IVisual)perspexObject).GetVisualParent() as PerspexObject;
				if (perspexObject is Branch)
					return true;
			} while (perspexObject != null);
			return false;
		}


		public static readonly PerspexProperty<bool> IsParticipatingInDragProperty =
			PerspexProperty.RegisterDirect<Layout, bool>("IsParticipatingInDrag", o => o.IsParticipatingInDrag,
				(o, v) => o.IsParticipatingInDrag = v);

		private bool _isParticipatingInDrag;

		public bool IsParticipatingInDrag
		{
			get { return _isParticipatingInDrag; }
			private set { SetAndRaise(IsParticipatingInDragProperty, ref _isParticipatingInDrag, value); }
		}

		public static readonly PerspexProperty<DataTemplate> BranchItemTemplateProperty =
			PerspexProperty.Register<Layout, DataTemplate>("BranchItemTemplate");

		public DataTemplate BranchItemTemplate
		{
			get { return GetValue(BranchItemTemplateProperty); }
			set { SetValue(BranchItemTemplateProperty, value); }
		}

		public static readonly PerspexProperty<bool> IsFloatingDropZoneEnabledProperty =
			PerspexProperty.Register<Layout, bool>("IsFloatingDropZoneEnabled");

		public bool IsFloatingDropZoneEnabled
		{
			get { return GetValue(IsFloatingDropZoneEnabledProperty); }
			set { SetValue(IsFloatingDropZoneEnabledProperty, value); }
		}

		public static readonly PerspexProperty<Thickness> FloatingItemsContainerMarginProperty =
			PerspexProperty.Register<Layout, Thickness>("FloatingItemsContainerMargin");

		public Thickness FloatingItemsContainerMargin
		{
			get { return GetValue(FloatingItemsContainerMarginProperty); }
			set { SetValue(FloatingItemsContainerMarginProperty, value); }
		}

		public IEnumerable FloatingItems
		{
			get { return _floatingItems.Items; }
		}



		public static readonly PerspexProperty<Style> FloatingItemsControlStyleProperty =
			PerspexProperty.Register<Layout, Style>("FloatingItemsControlStyle");

		public Style FloatingItemsControlStyle
		{
			get { return GetValue(FloatingItemsControlStyleProperty); }
			set { SetValue(FloatingItemsControlStyleProperty, value); }
		}

		public static readonly PerspexProperty<Style> FloatingItemsContainerStyleProperty =
			PerspexProperty.Register<Layout, Style>("FloatingItemsContainerStyle");

		public Style FloatingItemsContainerStyle
		{
			get { return GetValue(FloatingItemsContainerStyleProperty); }
			set { SetValue(FloatingItemsContainerStyleProperty, value); }
		}

		public static readonly PerspexProperty<DataTemplate> FloatingItemTemplateProperty =
			PerspexProperty.Register<Layout, DataTemplate>("FloatingItemTemplate");

		public DataTemplate FloatingItemTemplate
		{
			get { return GetValue(FloatingItemTemplateProperty); }
			set { SetValue(FloatingItemTemplateProperty, value); }
		}

		public static readonly PerspexProperty<string> FloatingItemHeaderMemberPathProperty =
			PerspexProperty.Register<Layout, string>("FloatingItemHeaderMemberPath");

		public string FloatingItemHeaderMemberPath
		{
			get { return GetValue(FloatingItemHeaderMemberPathProperty); }
			set { SetValue(FloatingItemHeaderMemberPathProperty, value); }
		}

		public static readonly PerspexProperty<string> FloatingItemDisplayMemberPathProperty =
			PerspexProperty.Register<Layout, string>("FloatingItemDisplayMemberPath");

		public string FloatingItemDisplayMemberPath
		{
			get { return GetValue(FloatingItemDisplayMemberPathProperty); }
			set { SetValue(FloatingItemDisplayMemberPathProperty, value); }
		}

		public static readonly PerspexProperty<ClosingFloatingItemCallback> ClosingFloatingItemCallbackProperty =
			PerspexProperty.Register<Layout, ClosingFloatingItemCallback>("ClosingFloatingItemCallback");

		public ClosingFloatingItemCallback ClosingFloatingItemCallback
		{
			get { return GetValue(ClosingFloatingItemCallbackProperty); }
			set { SetValue(ClosingFloatingItemCallbackProperty, value); }
		}

		public static readonly AttachedProperty<bool> IsFloatingInLayoutProperty =
			PerspexProperty.RegisterAttached<Layout, Control, bool>("IsFloatingInLayout");

		public static void SetIsFloatingInLayout(Control element, bool value)
		{
			element.SetValue(IsFloatingInLayoutProperty, value);
		}

		public static bool GetIsFloatingInLayout(Control element)
		{
			return element.GetValue(IsFloatingInLayoutProperty);
		}

		protected override void OnTemplateApplied(TemplateAppliedEventArgs e)
		{
			var floatingItemsContentPresenter = e.NameScope.Find<ContentPresenter>(FloatingContentPresenterPartName);
			if (floatingItemsContentPresenter != null)
				floatingItemsContentPresenter.Content = _floatingItems;

			_dropZones[DropZoneLocation.Top] = e.NameScope.Find<DropZone>(TopDropZonePartName);
			_dropZones[DropZoneLocation.Right] = e.NameScope.Find<DropZone>(RightDropZonePartName);
			_dropZones[DropZoneLocation.Bottom] = e.NameScope.Find<DropZone>(BottomDropZonePartName);
			_dropZones[DropZoneLocation.Left] = e.NameScope.Find<DropZone>(LeftDropZonePartName);
			_dropZones[DropZoneLocation.Floating] = e.NameScope.Find<DropZone>(FloatingDropZonePartName);

			base.OnTemplateApplied(e);
		}

		internal IEnumerable<AvalonViewItem> FloatingAvalonViewItems()
		{
			return _floatingItems.AvalonViewItems();
		}

		internal static void RestoreFloatingItemSnapShots(Control ancestor, IEnumerable<FloatingItemSnapShot> floatingItemSnapShots)
		{
			var layouts = ancestor.GetSelfAndVisualDescendents().OfType<Layout>().ToList();
			foreach (var floatingDragablzItem in layouts.SelectMany(l => l.FloatingAvalonViewItems()))
			{
				var itemSnapShots = floatingItemSnapShots as FloatingItemSnapShot[] ?? floatingItemSnapShots.ToArray();
				var floatingItemSnapShot = itemSnapShots.FirstOrDefault(
					ss => ss.Content == floatingDragablzItem.Content);
				if (floatingItemSnapShot != null)
					floatingItemSnapShot.Apply(floatingDragablzItem);
			}
		}

		// TODO DragStarted

		private static void SetupParticipatingLayouts(AvalonViewItem avalonViewItem)
		{
			var sourceOfAvalonViewItemsControl = avalonViewItem.GetSelfAndLogicalAncestors().OfType<ItemsControl>().FirstOrDefault() as AvalonViewItemsControl;
			if (sourceOfAvalonViewItemsControl == null || (sourceOfAvalonViewItemsControl.Items as ICollection).Count != 1) return;

			var draggingWindow = avalonViewItem.GetSelfAndVisualAncestors().OfType<Window>().First();
			if (draggingWindow == null) return;

			foreach (var loadedLayout in LoadedLayouts.Where(l =>
				l.Partition == avalonViewItem.PartitionAtDragStart &&
				!Equals(l.GetSelfAndVisualAncestors().OfType<Window>().FirstOrDefault(), draggingWindow)))

			{
				loadedLayout.IsParticipatingInDrag = true;
			}
		}

		private void MonitorDropZones(Point cursorPos)
		{
			//var myWindow = this.GetSelfAndVisualAncestors().OfType<Window>().First();
			//if (myWindow == null) return;

			//foreach (var dropZone in _dropZones.Values.Where(dz => dz != null))
			//{
			//    TODO pointFromScreen not Implemented
			//    var pointFromScreen = myWindow.PointFromScreen(cursorPos);

			//    var pointRelativeToDropZone = myWindow.TranslatePoint(pointFromScreen, dropZone);
			//    var inputHitTest = dropZone.InputHitTest(pointRelativeToDropZone);
			//    TODO better halding when windows are layered over each other
			//    if (inputHitTest != null)
			//    {
			//        if (_currentlyOfferedDropZone != null)
			//            _currentlyOfferedDropZone.Item2.IsOffered = false;
			//        dropZone.IsOffered = true;
			//        _currentlyOfferedDropZone = new Tuple<Layout, DropZone>(this, dropZone);
			//    }
			//    else
			//    {
			//        dropZone.IsOffered = false;
			//        if (_currentlyOfferedDropZone != null && _currentlyOfferedDropZone.Item2 == dropZone)
			//            _currentlyOfferedDropZone = null;
			//    }
			//}

			throw new NotImplementedException();
		}

		private static bool TryGetSourceTabControl(AvalonViewItem avalonViewItem, out AvalonViewControl avalonViewControl)
		{
			var sourceOfAvalonViewItemsControl = avalonViewItem.GetSelfAndLogicalAncestors().OfType<ItemsControl>().FirstOrDefault() as AvalonViewItemsControl;
			if (sourceOfAvalonViewItemsControl == null) throw new ApplicationException("Unable to determine source items control.");

			avalonViewControl = AvalonViewControl.GetOwnerOfHeaderItems(sourceOfAvalonViewItemsControl);

			return avalonViewControl != null;
		}

		private void Branch(DropZoneLocation location, AvalonViewItem sourceAvalonViewItem)
		{
			//if (InterLayoutClient == null)
			//    throw new InvalidOperationException("InterLayoutClient is not set.");

			//var sourceOfAvalonViewItemsControl = sourceAvalonViewItem.GetSelfAndLogicalAncestors().OfType<ItemsControl>().FirstOrDefault() as AvalonViewItemsControl;
			//if (sourceOfAvalonViewItemsControl == null) throw new ApplicationException("Unable to determin source items control.");

			//var sourceTabControl = AvalonViewControl.GetOwnerOfHeaderItems(sourceOfAvalonViewItemsControl);
			//if (sourceTabControl == null) throw new ApplicationException("Unable to determin source tab control.");

			//var floatingItemSnapShots = sourceTabControl.GetSelfAndVisualDescendents()
			//    .OfType<Layout>()
			//    .SelectMany(l => l.FloatingAvalonViewItems().Select(FloatingItemSnapShot.Take))
			//    .ToList();

			//var sourceItemIndex = sourceOfAvalonViewItemsControl.ItemContainerGenerator.IndexFromContainer(sourceAvalonViewItem);
			//var sourceItem = sourceOfAvalonViewItemsControl.Items[sourceItemIndex];
			//sourceTabControl.RemoveItem(sourceAvalonViewItem);

			//var branchItem = new Branch
			//{
			//    Orientation = (location == DropZoneLocation.Right || location == DropZoneLocation.Left) ? Orientation.Horizontal : Orientation.Vertical
			//};

			//object newContent;
			//if (BranchTemplate == null)
			//{
			//    var newTabHost = InterLayoutClient.GetNewHost(Partition, sourceTabControl);
			//    if (newTabHost == null)
			//        throw new ApplicationException("InterLayoutClient did not provide a new tab host.");
			//    newTabHost.AvalonViewControl.AddToSource(sourceItem);
			//    newTabHost.AvalonViewControl.SelectedItem = sourceItem;
			//    newContent = newTabHost.Container;

			//    Dispatcher.UIThread.InvokeAsync(() => RestoreFloatingItemSnapShots(newTabHost.AvalonViewControl, floatingItemSnapShots), DispatcherPriority.Loaded);
			//}
			//else
			//{
			//    newContent = new ContentControl
			//    {
			//        Content = new object(),
			//        ContentTemplate = BranchTemplate,
			//    };
			//    Dispatcher.UIThread.InvokeAsync(() =>
			//    {
			//        //TODO might need to improve this a bit, make it a bit more declarative for complex trees
			//        var newTabControl =
			//            ((ContentControl) newContent).GetSelfAndLogicalAncestors()
			//                .OfType<AvalonViewControl>()
			//                .FirstOrDefault();
			//        if (newTabControl == null) return;

			//        newTabControl.DataContext = sourceTabControl.DataContext;
			//        newTabControl.AddToSource(sourceItem);
			//        newTabControl.SelectedItem = sourceItem;
			//        Dispatcher.UIThread.InvokeAsync(() => RestoreFloatingItemSnapShots(newTabControl, floatingItemSnapShots), DispatcherPriority.Loaded);
			//    }, DispatcherPriority.Loaded);
			//}

			//if (location == DropZoneLocation.Right || location == DropZoneLocation.Bottom)
			//{
			//    branchItem.FirstItem = Content;
			//    branchItem.SecondItem = newContent;
			//}
			//else
			//{
			//    branchItem.FirstItem = newContent;
			//    branchItem.SecondItem = Content;
			//}

			//Content = branchItem;

			throw new NotImplementedException();
		}

		internal IEnumerable<AvalonViewItem> AvalonViewItems()
		{
			throw new NotImplementedException();
		}

		public static readonly PerspexProperty<DataTemplate> BranchTemplateProperty =
			PerspexProperty.Register<Layout, DataTemplate>("BranchTemplate");

		public DataTemplate BranchTemplate
		{
			get { return GetValue(BranchTemplateProperty); }
			set { SetValue(BranchTemplateProperty, value); }
		}

		public static readonly PerspexProperty<WindowState> FloatingItemStateProperty =
			PerspexProperty.RegisterDirect<Layout, WindowState>("FloatingItemsState", o => o.FloatingItemState,
				(o, v) => o.FloatingItemState = v);

		private WindowState _floatingItemsState;

		public WindowState FloatingItemState
		{
			get { return _floatingItemsState; }
			set { SetAndRaise(FloatingItemStateProperty, ref _floatingItemsState, value); }
		}

		public static void SetFloatingItemState(Control control, WindowState state)
		{
			control.SetValue(FloatingItemStateProperty, state);
		}

		public static WindowState GetFloatingItemState(Control control)
		{
			return control.GetValue(FloatingItemStateProperty);
		}

		// TODO: Location Snapshot as Attached PerspexProperty

		// TODO: More of the functions and events

		private bool IsHostingTab()
		{
			return this.GetVisualChildren().OfType<AvalonViewControl>()
				.FirstOrDefault(t => t.InterTabController != null && t.InterTabController.Partition == Partition)
				!= null;
		}

	}
}
