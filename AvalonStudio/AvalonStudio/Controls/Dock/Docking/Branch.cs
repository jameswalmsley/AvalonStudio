using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Perspex;
using Perspex.Controls;
using Perspex.Controls.Presenters;
using Perspex.Controls.Primitives;

namespace AvalonStudio.Controls.Dock.Docking
{
    
    // TODO: Styling stuff
    /// <summary>
    /// This control is used to determine where a control is in the docking tree.
    /// <remarks>
    /// Each ContentPresenter in this control can either be another Branch or a "Leaf"
    /// </remarks>
    /// </summary>
    public class Branch : TemplatedControl
    {
        public static readonly PerspexProperty<Orientation> OrientationProperty =
            PerspexProperty.RegisterDirect<Branch, Orientation>("Orientation", o => o.Orientation,
                (o, v) => o.Orientation = v);

        private Orientation _orientation;

        public Orientation Orientation
        {
            get { return _orientation; }
            set { SetAndRaise(OrientationProperty, ref _orientation, value); }
        }

        public static readonly PerspexProperty<object> FirstItemProperty =
            PerspexProperty.RegisterDirect<Branch, object>("FirstItem", o => o.FirstItem,
                (o, v) => o.FirstItem = v);

        private object _firstItem;

        public object FirstItem
        {
            get { return _firstItem; }
            set { SetAndRaise(FirstItemProperty, ref _firstItem, value); }
        }


        public static readonly PerspexProperty<GridLength> FirstItemLengthProperty =
            PerspexProperty.RegisterDirect<Branch, GridLength>("FirstItemLength", o => o.FirstItemLength,
                (o, v) => o.FirstItemLength = v);

        private GridLength _firstItemLength;

        public GridLength FirstItemLength
        {
            get { return _firstItemLength; }
            set { SetAndRaise(FirstItemLengthProperty, ref _firstItemLength, value); }
        }

        public static readonly PerspexProperty<object> SecondItemProperty =
            PerspexProperty.RegisterDirect<Branch, object>("SecondItem", o => o.SecondItem,
                (o, v) => o.SecondItem = v);

        private object _secondItem;

        public object SecondItem
        {
            get { return _secondItem; }
            set { SetAndRaise(SecondItemProperty, ref _secondItem, value); }
        }

        public static readonly PerspexProperty<GridLength> SecondItemLengthProperty =
            PerspexProperty.RegisterDirect<Branch, GridLength>("SecondItemLength", o => o.SecondItemLength,
                (o, v) => o.SecondItemLength = v);

        private GridLength _secondItemLength;

        public GridLength SecondItemLength
        {
            get { return _secondItemLength; }
            set { SetAndRaise(SecondItemLengthProperty, ref _secondItemLength, value); }
        }

        /// <summary>
        /// Gets the proportional size of the first item, between 0 and 1, where 1 would represent the entire size of the branch.
        /// </summary>
        /// <returns></returns>
        public double GetFirstProportion()
        {
            return (1 / (FirstItemLength.Value + SecondItemLength.Value)) * FirstItemLength.Value;
        }

        protected override void OnTemplateApplied(TemplateAppliedEventArgs e)
        {
            base.OnTemplateApplied(e);

            FirstContentPresenter = e.NameScope.Find<ContentPresenter>("PART_FirstContentPresenter");
            SecondContentPresenter = e.NameScope.Find<ContentPresenter>("PART_SecondContentPresenter");

        }

        internal ContentPresenter FirstContentPresenter { get; private set; }
        internal ContentPresenter SecondContentPresenter { get; private set; }

    }
}
