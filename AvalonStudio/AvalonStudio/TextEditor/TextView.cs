namespace AvalonStudio.TextEditor
{
    using Rendering;
    using Document;
    using Perspex;
    using Perspex.Controls;
    using Perspex.Media;
    using Perspex.Threading;
    using Perspex.VisualTree;
    using System;
    using System.Collections.ObjectModel;
    using System.Reactive.Linq;
    using Perspex.Controls.Primitives;
    using System.Collections.Generic;
    using Utils;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Globalization;
    using Perspex.Media.TextFormatting;

    public class TextView : TemplatedControl, IScrollInfo
    {

        public const int WheelScrollLines = 3;
        public static readonly PerspexProperty<int> CaretIndexProperty =
            TextBox.CaretIndexProperty.AddOwner<TextView>();

        public static readonly PerspexProperty<int> SelectionStartProperty =
            TextBox.SelectionStartProperty.AddOwner<TextView>();

        public static readonly PerspexProperty<int> SelectionEndProperty =
            TextBox.SelectionEndProperty.AddOwner<TextView>();

        public static readonly PerspexProperty<TextDocument> DocumentProperty =
            PerspexProperty.Register<TextView, TextDocument>("Document");

        public TextDocument Document
        {
            get { return GetValue(DocumentProperty); }
            set { SetValue(DocumentProperty, value); }
        }

        /// <summary>
		/// Occurs when the document property has changed.
		/// </summary>
		public event EventHandler DocumentChanged;

        void OnDocumentChanged(TextDocument oldValue, TextDocument newValue)
        {
            if (oldValue != null)
            {
                heightTree.Dispose();
                heightTree = null;
                //formatter.Dispose();
                //formatter = null;
                //cachedElements.Dispose();
                //cachedElements = null;
                //TextDocumentWeakEventManager.Changing.RemoveListener(oldValue, this);
            }
            this.document = newValue;
            ClearScrollData();
            ClearVisualLines();
            if (newValue != null)
            {
                //TextDocumentWeakEventManager.Changing.AddListener(newValue, this);
                //formatter = TextFormatterFactory.Create(this);
                InvalidateDefaultTextMetrics(); // measuring DefaultLineHeight depends on formatter
                heightTree = new HeightTree(newValue, DefaultLineHeight);
                //cachedElements = new TextViewCachedElements();
            }
            InvalidateMeasure();

            if (DocumentChanged != null)
                DocumentChanged(this, EventArgs.Empty);
        }

        void InvalidateDefaultTextMetrics()
        {
            defaultTextMetricsValid = false;
            if (heightTree != null)
            {
                // calculate immediately so that height tree gets updated
                CalculateDefaultTextMetrics();
            }
        }

        public static readonly PerspexProperty<TextEditorOptions> OptionsProperty =
            PerspexProperty.Register<TextView, TextEditorOptions>("Options");

        public TextEditorOptions Options
        {
            get { return GetValue(OptionsProperty); }
            set { SetValue(OptionsProperty, value); }
        }

        TextDocument document;
        HeightTree heightTree;

        List<VisualLine> allVisualLines = new List<VisualLine>();
        List<VisualLine> newVisualLines;
        double clippedPixelsOnTop;
        ReadOnlyCollection<VisualLine> visibleVisualLines;

        /// <summary>
        /// Occurs when the TextView was measured and changed its visual lines.
        /// </summary>
        public event EventHandler VisualLinesChanged;

        public ReadOnlyCollection<VisualLine> VisualLines
        {
            get
            {
                if (visibleVisualLines == null)
                    throw new Exception("Visual Lines invalid");
                return visibleVisualLines;
            }
        }

        #region Render
        readonly ObserveAddRemoveCollection<IBackgroundRenderer> backgroundRenderers;

        /// <summary>
        /// Gets the list of background renderers.
        /// </summary>
        public IList<IBackgroundRenderer> BackgroundRenderers
        {
            get { return backgroundRenderers; }
        }
        #endregion

        private readonly DispatcherTimer _caretTimer;

        private bool _caretBlink;

        private IObservable<bool> _canScrollHorizontally;

        private TextEditor editor;

        static TextView()
        {
            DocumentProperty.Changed.AddClassHandler<TextView>((s, v) =>
            {
                (s as TextView).OnDocumentChanged((TextDocument)v.OldValue, (TextDocument)v.NewValue);
            });
        }

        public TextView(TextEditor editor)
        {
            this.editor = editor;

            textLayer = new TextLayer(this);
            
            this.Options = new TextEditorOptions();

            elementGenerators = new ObserveAddRemoveCollection<VisualLineElementGenerator>((v)=> { }, (v)=> { });

            layers = new LayerCollection(this);
            InsertLayer(textLayer, KnownLayer.Text, LayerInsertionPosition.Replace);
            

            _caretTimer = new DispatcherTimer();
            _caretTimer.Interval = TimeSpan.FromMilliseconds(500);
            _caretTimer.Tick += CaretTimerTick;

            //_canScrollHorizontally = GetObservable(TextWrappingProperty)
            //    .Select(x => x == TextWrapping.NoWrap);

            //Observable.Merge(
            //    GetObservable(SelectionStartProperty),
            //    GetObservable(SelectionEndProperty))
            //    .Subscribe(_ => InvalidateFormattedText());

            GetObservable(CaretIndexProperty)
                .Subscribe(CaretIndexChanged);
        }

        /// <summary>
        /// Occurs when a text editor option has changed.
        /// </summary>
        public event PropertyChangedEventHandler OptionChanged;

        /// <summary>
        /// Raises the <see cref="OptionChanged"/> event.
        /// </summary>
        protected virtual void OnOptionChanged(PropertyChangedEventArgs e)
        {
            if (OptionChanged != null)
            {
                OptionChanged(this, e);
            }

            //if (Options.ShowColumnRuler)
            //    columnRulerRenderer.SetRuler(Options.ColumnRulerPosition, ColumnRulerPen);
            //else
            //    columnRulerRenderer.SetRuler(-1, ColumnRulerPen);

            UpdateBuiltinElementGeneratorsFromOptions();
            Redraw();
        }

        //SingleCharacterElementGenerator singleCharacterElementGenerator;
        //LinkElementGenerator linkElementGenerator;
        //MailLinkElementGenerator mailLinkElementGenerator;

        void UpdateBuiltinElementGeneratorsFromOptions()
        {
            TextEditorOptions options = this.Options;

            //			AddRemoveDefaultElementGeneratorOnDemand(ref newLineElementGenerator, options.ShowEndOfLine);
            //AddRemoveDefaultElementGeneratorOnDemand(ref singleCharacterElementGenerator, options.ShowBoxForControlCharacters || options.ShowSpaces || options.ShowTabs);
            //AddRemoveDefaultElementGeneratorOnDemand(ref linkElementGenerator, options.EnableHyperlinks);
            //AddRemoveDefaultElementGeneratorOnDemand(ref mailLinkElementGenerator, options.EnableEmailHyperlinks);
        }

        void AddRemoveDefaultElementGeneratorOnDemand<T>(ref T generator, bool demand)
            where T : VisualLineElementGenerator, IBuiltinElementGenerator, new()
        {
            bool hasGenerator = generator != null;
            if (hasGenerator != demand)
            {
                if (demand)
                {
                    generator = new T();
                    this.ElementGenerators.Add(generator);
                }
                else
                {
                    this.ElementGenerators.Remove(generator);
                    generator = null;
                }
            }
            if (generator != null)
                generator.FetchOptions(this.Options);
        }

        readonly ObserveAddRemoveCollection<VisualLineElementGenerator> elementGenerators;

        /// <summary>
        /// Gets a collection where element generators can be registered.
        /// </summary>
        public IList<VisualLineElementGenerator> ElementGenerators
        {
            get { return elementGenerators; }
        }

        #region Get(OrConstruct)VisualLine
        /// <summary>
        /// Gets the visual line that contains the document line with the specified number.
        /// Returns null if the document line is outside the visible range.
        /// </summary>
        public VisualLine GetVisualLine(int documentLineNumber)
        {
            // TODO: EnsureVisualLines() ?
            foreach (VisualLine visualLine in allVisualLines)
            {
                Debug.Assert(visualLine.IsDisposed == false);
                int start = visualLine.FirstDocumentLine.LineNumber;
                int end = visualLine.LastDocumentLine.LineNumber;
                if (documentLineNumber >= start && documentLineNumber <= end)
                    return visualLine;
            }
            return null;
        }

        static int GetIndentationVisualColumn(VisualLine visualLine)
        {
            if (visualLine.Elements.Count == 0)
                return 0;
            int column = 0;
            int elementIndex = 0;
            VisualLineElement element = visualLine.Elements[elementIndex];
            while (element.IsWhitespace(column))
            {
                column++;
                if (column == element.VisualColumn + element.VisualLength)
                {
                    elementIndex++;
                    if (elementIndex == visualLine.Elements.Count)
                        break;
                    element = visualLine.Elements[elementIndex];
                }
            }
            return column;
        }

        VisualLine BuildVisualLine(DocumentLine documentLine,
                                   TextRunProperties globalTextRunProperties,
                                   VisualLineTextParagraphProperties paragraphProperties,
                                   VisualLineElementGenerator[] elementGeneratorsArray,
                                   IVisualLineTransformer[] lineTransformersArray,
                                   Size availableSize)
        {
            //throw new Exception("port to perspex text rendering.");
            if (heightTree.GetIsCollapsed(documentLine.LineNumber))
                throw new InvalidOperationException("Trying to build visual line from collapsed line");

            //Debug.WriteLine("Building line " + documentLine.LineNumber);

            VisualLine visualLine = new VisualLine(this, documentLine);
            VisualLineTextSource textSource = new VisualLineTextSource(visualLine)
            {
                Document = document,
                //GlobalTextRunProperties = globalTextRunProperties,
                TextView = this
            };

            visualLine.ConstructVisualElements(textSource, elementGeneratorsArray);

            if (visualLine.FirstDocumentLine != visualLine.LastDocumentLine)
            {
                // Check whether the lines are collapsed correctly:
                double firstLinePos = heightTree.GetVisualPosition(visualLine.FirstDocumentLine.NextLine);
                double lastLinePos = heightTree.GetVisualPosition(visualLine.LastDocumentLine.NextLine ?? visualLine.LastDocumentLine);
                if (!firstLinePos.IsClose(lastLinePos))
                {
                    for (int i = visualLine.FirstDocumentLine.LineNumber + 1; i <= visualLine.LastDocumentLine.LineNumber; i++)
                    {
                        if (!heightTree.GetIsCollapsed(i))
                            throw new InvalidOperationException("Line " + i + " was skipped by a VisualLineElementGenerator, but it is not collapsed.");
                    }
                    throw new InvalidOperationException("All lines collapsed but visual pos different - height tree inconsistency?");
                }
            }

            visualLine.RunTransformers(textSource, lineTransformersArray);

            // now construct textLines:
            int textOffset = 0;
            //TextLineBreak lastLineBreak = null;
            //var textLines = new List<TextLine>();
            //paragraphProperties.indent = 0;
            //paragraphProperties.firstLineInParagraph = true;
            while (textOffset <= visualLine.VisualLengthWithEndOfLineMarker)
            {
                //TextLine textLine = formatter.FormatLine(
                //    textSource,
                //    textOffset,
                //    availableSize.Width,
                //    null,
                //    lastLineBreak
                //);
                //textLines.Add(textLine);
                //textOffset += textLine.Length;

                // exit loop so that we don't do the indentation calculation if there's only a single line
                if (textOffset >= visualLine.VisualLengthWithEndOfLineMarker)
                    break;

                //if (paragraphProperties.firstLineInParagraph)
                //{
                //    paragraphProperties.firstLineInParagraph = false;

                //    TextEditorOptions options = this.Options;
                //    double indentation = 0;
                //    if (options.InheritWordWrapIndentation)
                //    {
                //        // determine indentation for next line:
                //        int indentVisualColumn = GetIndentationVisualColumn(visualLine);
                //        if (indentVisualColumn > 0 && indentVisualColumn < textOffset)
                //        {
                //            //indentation = textLine.GetDistanceFromCharacterHit(new CharacterHit(indentVisualColumn, 0));
                //        }
                //    }
                //    indentation += options.WordWrapIndentation;
                //    // apply the calculated indentation unless it's more than half of the text editor size:
                //    if (indentation > 0 && indentation * 2 < availableSize.Width)
                //        paragraphProperties.indent = indentation;
                //}
                //lastLineBreak = textLine.GetTextLineBreak();
            }

            //visualLine.SetTextLines(textLines);
            heightTree.SetHeight(visualLine.FirstDocumentLine, visualLine.Height);
            return visualLine;
        }

        /// <summary>
        /// Gets the visual line that contains the document line with the specified number.
        /// If that line is outside the visible range, a new VisualLine for that document line is constructed.
        /// </summary>
        public VisualLine GetOrConstructVisualLine(DocumentLine documentLine)
        {
            if (documentLine == null)
                throw new ArgumentNullException("documentLine");
            if (!this.Document.Lines.Contains(documentLine))
                throw new InvalidOperationException("Line belongs to wrong document");
            VerifyAccess();

            VisualLine l = GetVisualLine(documentLine.LineNumber);
            if (l == null)
            {
                throw new Exception("Port to perspex.");
                //TextRunProperties globalTextRunProperties = CreateGlobalTextRunProperties();
                //VisualLineTextParagraphProperties paragraphProperties = CreateParagraphProperties(globalTextRunProperties);

                //while (heightTree.GetIsCollapsed(documentLine.LineNumber))
                //{
                //    documentLine = documentLine.PreviousLine;
                //}

                //l = BuildVisualLine(documentLine,
                //                    globalTextRunProperties, paragraphProperties,
                //                    elementGenerators.ToArray(), lineTransformers.ToArray(),
                //                    lastAvailableSize);
                //allVisualLines.Add(l);
                //// update all visual top values (building the line might have changed visual top of other lines due to word wrapping)
                //foreach (var line in allVisualLines)
                //{
                //    line.VisualTop = heightTree.GetVisualPosition(line.FirstDocumentLine);
                //}
            }
            return l;
        }
        #endregion


        /// <summary>
        /// Causes the text editor to regenerate all visual lines.
        /// </summary>
        public void Redraw()
        {
            Redraw(DispatcherPriority.Normal);
        }

        /// <summary>
        /// Causes the text editor to regenerate all visual lines.
        /// </summary>
        public void Redraw(DispatcherPriority redrawPriority)
        {
            VerifyAccess();
            ClearVisualLines();
            InvalidateMeasure();
        }

        /// <summary>
        /// Causes the text editor to regenerate the specified visual line.
        /// </summary>
        public void Redraw(VisualLine visualLine, DispatcherPriority redrawPriority = DispatcherPriority.Normal)
        {
            VerifyAccess();
            if (allVisualLines.Remove(visualLine))
            {
                DisposeVisualLine(visualLine);
                InvalidateMeasure();
            }
        }

        /// <summary>
        /// Causes the text editor to redraw all lines overlapping with the specified segment.
        /// </summary>
        public void Redraw(int offset, int length, DispatcherPriority redrawPriority = DispatcherPriority.Normal)
        {
            VerifyAccess();
            bool changedSomethingBeforeOrInLine = false;
            for (int i = 0; i < allVisualLines.Count; i++)
            {
                VisualLine visualLine = allVisualLines[i];
                int lineStart = visualLine.FirstDocumentLine.Offset;
                int lineEnd = visualLine.LastDocumentLine.Offset + visualLine.LastDocumentLine.TotalLength;
                if (offset <= lineEnd)
                {
                    changedSomethingBeforeOrInLine = true;
                    if (offset + length >= lineStart)
                    {
                        allVisualLines.RemoveAt(i--);
                        DisposeVisualLine(visualLine);
                    }
                }
            }
            if (changedSomethingBeforeOrInLine)
            {
                // Repaint not only when something in visible area was changed, but also when anything in front of it
                // was changed. We might have to redraw the line number margin. Or the highlighting changed.
                // However, we'll try to reuse the existing VisualLines.
                InvalidateMeasure();
            }
        }

        /// <summary>
        /// Causes a known layer to redraw.
        /// This method does not invalidate visual lines;
        /// use the <see cref="Redraw()"/> method to do that.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "knownLayer",
                                                         Justification = "This method is meant to invalidate only a specific layer - I just haven't figured out how to do that, yet.")]
        public void InvalidateLayer(KnownLayer knownLayer)
        {
            InvalidateMeasure();
        }

        /// <summary>
        /// Causes a known layer to redraw.
        /// This method does not invalidate visual lines;
        /// use the <see cref="Redraw()"/> method to do that.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "knownLayer",
                                                         Justification = "This method is meant to invalidate only a specific layer - I just haven't figured out how to do that, yet.")]
        public void InvalidateLayer(KnownLayer knownLayer, DispatcherPriority priority)
        {
            InvalidateMeasure();
        }

        /// <summary>
		/// Gets the height of the document.
		/// </summary>
		public double DocumentHeight
        {
            get
            {
                // return 0 if there is no document = no heightTree
                return heightTree != null ? heightTree.TotalHeight : 0;
            }
        }

        public int CaretIndex
        {
            get { return GetValue(CaretIndexProperty); }
            set { SetValue(CaretIndexProperty, value); }
        }

        public int SelectionStart
        {
            get { return GetValue(SelectionStartProperty); }
            set { SetValue(SelectionStartProperty, value); }
        }

        public int SelectionEnd
        {
            get { return GetValue(SelectionEndProperty); }
            set { SetValue(SelectionEndProperty, value); }
        }

        /// <summary>
        /// Invalidates all visual lines.
        /// The caller of ClearVisualLines() must also call InvalidateMeasure() to ensure
        /// that the visual lines will be recreated.
        /// </summary>
        void ClearVisualLines()
        {
            visibleVisualLines = null;
            if (allVisualLines.Count != 0)
            {
                foreach (VisualLine visualLine in allVisualLines)
                {
                    DisposeVisualLine(visualLine);
                }
                allVisualLines.Clear();
            }
        }

        void DisposeVisualLine(VisualLine visualLine)
        {
            if (newVisualLines != null && newVisualLines.Contains(visualLine))
            {
                throw new ArgumentException("Cannot dispose visual line because it is in construction!");
            }
            visibleVisualLines = null;
            visualLine.Dispose();
            //RemoveInlineObjects(visualLine);
        }

        Size scrollExtent;

        Vector scrollOffset;

        Size scrollViewport;

        void ClearScrollData()
        {
            SetScrollData(new Size(), new Size(), new Vector());
        }

        bool SetScrollData(Size viewport, Size extent, Vector offset)
        {
            if (!(viewport.IsClose(this.scrollViewport)
                  && extent.IsClose(this.scrollExtent)
                  && offset.IsClose(this.scrollOffset)))
            {
                this.scrollViewport = viewport;
                this.scrollExtent = extent;
                SetScrollOffset(offset);
                this.OnScrollChange();
                return true;
            }
            return false;
        }

        void OnScrollChange()
        {
            ScrollViewer scrollOwner = ((IScrollInfo)this).ScrollOwner;
            if (scrollOwner != null)
            {
                scrollOwner.InvalidateVisual();
            }
        }

        public double ExtentWidth
        {
            get
            {
                return scrollExtent.Width;
            }
        }

        public double ViewportWidth
        {
            get
            {
                return scrollViewport.Width;
            }
        }

        public double HorizontalOffset
        {
            get
            {
                return scrollOffset.X;
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        bool canHorizontallyScroll;
        public bool CanHorizontallyScroll
        {
            get
            {
                return canHorizontallyScroll;
            }

            set
            {
                if(canHorizontallyScroll != value)
                {
                    canHorizontallyScroll = value;
                    ClearVisualLines();
                    InvalidateMeasure();
                }
            }
        }

        public double VerticalOffset
        {
            get
            {
                return scrollOffset.Y;
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public double ExtentHeight
        {
            get
            {
                return scrollExtent.Height;
            }
        }

        public double ViewportHeight
        {
            get
            {
                return scrollViewport.Height;
            }
        }

        bool canVerticallyScroll;
        public bool CanVerticallyScroll
        {
            get { return canVerticallyScroll; }
            set
            {
                if (canVerticallyScroll != value)
                {
                    canVerticallyScroll = value;
                    InvalidateMeasure();
                }
            }
        }

        /// <summary>
        /// Gets the scroll offset;
        /// </summary>
        public Vector ScrollOffset
        {
            get { return scrollOffset; }
        }

        /// <summary>
		/// Occurs when the scroll offset has changed.
		/// </summary>
		public event EventHandler ScrollOffsetChanged;

        void SetScrollOffset(Vector vector)
        {
            if (!canHorizontallyScroll)
            {
                vector = new Vector(0, vector.Y);
            }
                
            if (!canVerticallyScroll)
            {
                vector = new Vector(vector.X, 0);
            }
                

            if (!scrollOffset.IsClose(vector))
            {
                scrollOffset = vector;
                if (ScrollOffsetChanged != null)
                    ScrollOffsetChanged(this, EventArgs.Empty);
            }
        }

        public ScrollViewer ScrollOwner
        {
            get; set;
        }

        public int GetCaretIndex(Point point)
        {
            return 1;
            //var hit = FormattedText.HitTestPoint(point);
            //return hit.TextPosition + (hit.IsTrailing ? 1 : 0);
        }

        internal void RenderBackground(DrawingContext drawingContext, KnownLayer layer)
        {
            foreach (IBackgroundRenderer bg in backgroundRenderers)
            {
                if (bg.Layer == layer)
                {
                    bg.Draw(this, drawingContext);
                }
            }
        }

        internal void ArrangeTextLayer(IList<VisualLineDrawingVisual> visuals)
        {
            Point pos = new Point(-scrollOffset.X, -clippedPixelsOnTop);
            foreach (VisualLineDrawingVisual visual in visuals)
            {
                //TranslateTransform t = visual.Transform as TranslateTransform;
                //if (t == null || t.X != pos.X || t.Y != pos.Y)
                //{
                //    visual.Transform = new TranslateTransform(pos.X, pos.Y);
                //    visual.Transform.Freeze();
                //}
                //pos.Y += visual.Height;
            }
        }

        public override void Render(DrawingContext context)
        {
            if (editor.Document != null && editor.Document.LineCount > 0)
            {
                //foreach (var line in editor.TextDocument.Lines)
                //{
                //    var formattedText = new FormattedText(editor.TextDocument.GetText(line.Offset, line.EndOffset), "Consolas", 14, FontStyle.Normal, TextAlignment.Left, FontWeight.Normal);

                //    context.DrawText(Brushes.WhiteSmoke, new Point(0, 0), formattedText);
                //}
            }
        }

        public int GetLine(int caretIndex)
        {
            //var lines = FormattedText.GetLines().ToList();

            //int pos = 0;
            //int i;

            //for (i = 0; i < lines.Count; ++i)
            //{
            //    var line = lines[i];
            //    pos += line.Length;

            //    if (pos > caretIndex)
            //    {
            //        break;
            //    }
            //}

            return 1;
        }

        public void ShowCaret()
        {
            _caretBlink = true;
            _caretTimer.Start();
            InvalidateVisual();
        }

        public void HideCaret()
        {
            _caretBlink = false;
            _caretTimer.Stop();
            InvalidateVisual();
        }

        internal void CaretIndexChanged(int caretIndex)
        {
            if (this.GetVisualParent() != null)
            {
                _caretBlink = true;
                _caretTimer.Stop();
                _caretTimer.Start();
                InvalidateVisual();

                // var rect = FormattedText.HitTestTextPosition(caretIndex);
                // this.BringIntoView(rect);
            }
        }

        #region Layers
        internal readonly TextLayer textLayer;
        readonly LayerCollection layers;

        sealed class LayerCollection : Controls
        {
            readonly TextView textView;

            public LayerCollection(TextView textView)                
            {
                this.textView = textView;
            }

            new public void Clear()
            {
                base.Clear();
                textView.LayersChanged();
            }

            new public void Add(IControl element)
            {
                base.Add(element);
                textView.LayersChanged();
            }

            new public void RemoveAt(int index)
            {
                base.RemoveAt(index);
                textView.LayersChanged();
            }

            new public void RemoveRange(int index, int count)
            {
                base.RemoveRange(index, count);
                textView.LayersChanged();
            }
        }

        void LayersChanged()
        {
            textLayer.index = layers.IndexOf(textLayer);
        }

        /// <summary>
		/// Inserts a new layer at a position specified relative to an existing layer.
		/// </summary>
		/// <param name="layer">The new layer to insert.</param>
		/// <param name="referencedLayer">The existing layer</param>
		/// <param name="position">Specifies whether the layer is inserted above,below, or replaces the referenced layer</param>
		public void InsertLayer(Control layer, KnownLayer referencedLayer, LayerInsertionPosition position)
        {
            if (layer == null)
                throw new ArgumentNullException("layer");
            if (!Enum.IsDefined(typeof(KnownLayer), referencedLayer))
                throw new InvalidEnumArgumentException("referencedLayer", (int)referencedLayer, typeof(KnownLayer));
            if (!Enum.IsDefined(typeof(LayerInsertionPosition), position))
                throw new InvalidEnumArgumentException("position", (int)position, typeof(LayerInsertionPosition));
            if (referencedLayer == KnownLayer.Background && position != LayerInsertionPosition.Above)
                throw new InvalidOperationException("Cannot replace or insert below the background layer.");

            LayerPosition newPosition = new LayerPosition(referencedLayer, position);
            LayerPosition.SetLayerPosition(layer, newPosition);
            for (int i = 0; i < layers.Count; i++)
            {
                LayerPosition p = LayerPosition.GetLayerPosition(layers[i]);
                if (p != null)
                {
                    if (p.KnownLayer == referencedLayer && p.Position == LayerInsertionPosition.Replace)
                    {
                        // found the referenced layer
                        switch (position)
                        {
                            case LayerInsertionPosition.Below:
                                layers.Insert(i, layer);
                                return;
                            case LayerInsertionPosition.Above:
                                layers.Insert(i + 1, layer);
                                return;
                            case LayerInsertionPosition.Replace:
                                layers[i] = layer;
                                return;
                        }
                    }
                    else if (p.KnownLayer == referencedLayer && p.Position == LayerInsertionPosition.Above
                             || p.KnownLayer > referencedLayer)
                    {
                        // we skipped the insertion position (referenced layer does not exist?)
                        layers.Insert(i, layer);
                        return;
                    }
                }
            }
            // inserting after all existing layers:
            layers.Add(layer);
        }
        #endregion

        //protected override FormattedText CreateFormattedText(Size constraint)
        //{
        //    var result = base.CreateFormattedText(constraint);
        //    var selectionStart = SelectionStart;
        //    var selectionEnd = SelectionEnd;
        //    var start = Math.Min(selectionStart, selectionEnd);
        //    var length = Math.Max(selectionStart, selectionEnd) - start;

        //    if (length > 0)
        //    {
        //        result.SetForegroundBrush(Brushes.White, start, length);
        //    }

        //    return result;
        //}
        /// <summary>
        /// Additonal amount that allows horizontal scrolling past the end of the longest line.
        /// This is necessary to ensure the caret always is visible, even when it is at the end of the longest line.
        /// </summary>
        const double AdditionalHorizontalScrollAmount = 3;

        Size lastAvailableSize;
        bool inMeasure;

        protected override Size MeasureOverride(Size availableSize)
        {
            // We don't support infinite available width, so we'll limit it to 32000 pixels.
            if (availableSize.Width > 32000)
                availableSize = new Size(32000, availableSize.Height);

            if (!canHorizontallyScroll && !availableSize.Width.IsClose(lastAvailableSize.Width))
                ClearVisualLines();
            lastAvailableSize = availableSize;

            //foreach (UIElement layer in layers)
            //{
            //    layer.Measure(availableSize);
            //}
            //MeasureInlineObjects();

            InvalidateVisual(); // = InvalidateArrange+InvalidateRender

            double maxWidth;
            if (document == null)
            {
                // no document -> create empty list of lines
                allVisualLines = new List<VisualLine>();
                visibleVisualLines = allVisualLines.AsReadOnly();
                maxWidth = 0;
            }
            else
            {
                inMeasure = true;
                try
                {
                    maxWidth = CreateAndMeasureVisualLines(availableSize);
                }
                finally
                {
                    inMeasure = false;
                }
            }

            // remove inline objects only at the end, so that inline objects that were re-used are not removed from the editor
            //RemoveInlineObjectsNow();

            maxWidth += AdditionalHorizontalScrollAmount;
            double heightTreeHeight = this.DocumentHeight;
            TextEditorOptions options = this.Options;
            if (options.AllowScrollBelowDocument)
            {
                if (!double.IsInfinity(scrollViewport.Height))
                {
                    // HACK: we need to keep at least Caret.MinimumDistanceToViewBorder visible so that we don't scroll back up when the user types after
                    // scrolling to the very bottom.
                    //double minVisibleDocumentHeight = Math.Max(DefaultLineHeight, Editing.Caret.MinimumDistanceToViewBorder);
                    // scrollViewportBottom: bottom of scroll view port, but clamped so that at least minVisibleDocumentHeight of the document stays visible.
                    //double scrollViewportBottom = Math.Min(heightTreeHeight - minVisibleDocumentHeight, scrollOffset.Y) + scrollViewport.Height;
                    // increase the extend height to allow scrolling below the document
                    //heightTreeHeight = Math.Max(heightTreeHeight, scrollViewportBottom);
                }
            }

            textLayer.SetVisualLines(visibleVisualLines);

            SetScrollData(availableSize,
                          new Size(maxWidth, heightTreeHeight),
                          scrollOffset);
            if (VisualLinesChanged != null)
                VisualLinesChanged(this, EventArgs.Empty);

            return new Size(Math.Min(availableSize.Width, maxWidth), Math.Min(availableSize.Height, heightTreeHeight));
        }

        //TextFormatter formatter;

        internal double FontSize
        {
            get
            {
                return (double)GetValue(TextBlock.FontSizeProperty);
            }
        }

        TextRunProperties CreateGlobalTextRunProperties()
        {
            var p = new GlobalTextRunProperties();
            //p.typeface = this.CreateTypeface();
            p.fontRenderingEmSize = FontSize;
            p.foregroundBrush = (Brush)GetValue(TemplatedControl.ForegroundProperty);
            //ExtensionMethods.CheckIsFrozen(p.foregroundBrush);
            p.cultureInfo = CultureInfo.CurrentCulture;
            return p;
        }

        VisualLineTextParagraphProperties CreateParagraphProperties(TextRunProperties defaultTextRunProperties)
        {
            return new VisualLineTextParagraphProperties
            {
                //defaultTextRunProperties = defaultTextRunProperties,
                //textWrapping = canHorizontallyScroll ? TextWrapping.NoWrap : TextWrapping.Wrap,
                //tabSize = Options.IndentationSize * WideSpaceWidth
            };
        }

        /// <summary>
		/// Build all VisualLines in the visible range.
		/// </summary>
		/// <returns>Width the longest line</returns>
		double CreateAndMeasureVisualLines(Size availableSize)
        {
            //throw new Exception("Port to perspex.");

            TextRunProperties globalTextRunProperties = CreateGlobalTextRunProperties();
            VisualLineTextParagraphProperties paragraphProperties = CreateParagraphProperties(globalTextRunProperties);

            Debug.WriteLine("Measure availableSize=" + availableSize + ", scrollOffset=" + scrollOffset);
            var firstLineInView = heightTree.GetLineByVisualPosition(scrollOffset.Y);

            // number of pixels clipped from the first visual line(s)
            clippedPixelsOnTop = scrollOffset.Y - heightTree.GetVisualPosition(firstLineInView);
            // clippedPixelsOnTop should be >= 0, except for floating point inaccurracy.
            Debug.Assert(clippedPixelsOnTop >= -ExtensionMethods.Epsilon);

            newVisualLines = new List<VisualLine>();
            
            //if (VisualLineConstructionStarting != null)
            //    VisualLineConstructionStarting(this, new VisualLineConstructionStartEventArgs(firstLineInView));

            var elementGeneratorsArray = elementGenerators.ToArray();
            //var lineTransformersArray = lineTransformers.ToArray();
            var nextLine = firstLineInView;
            double maxWidth = 0;
            double yPos = -clippedPixelsOnTop;
            while (yPos < availableSize.Height && nextLine != null)
            {
                VisualLine visualLine = GetVisualLine(nextLine.LineNumber);
                if (visualLine == null)
                {
                    visualLine = BuildVisualLine(nextLine,
                                                 globalTextRunProperties, paragraphProperties,
                                                 elementGeneratorsArray, new IVisualLineTransformer[0],
                                                 availableSize);
                }

                visualLine.VisualTop = scrollOffset.Y + yPos;

                nextLine = visualLine.LastDocumentLine.NextLine;

                yPos += visualLine.Height;

                //foreach (TextLine textLine in visualLine.TextLines)
                //{
                //    if (textLine.WidthIncludingTrailingWhitespace > maxWidth)
                //        maxWidth = textLine.WidthIncludingTrailingWhitespace;
                //}

                newVisualLines.Add(visualLine);
            }

            foreach (VisualLine line in allVisualLines)
            {
                Debug.Assert(line.IsDisposed == false);
                if (!newVisualLines.Contains(line))
                    DisposeVisualLine(line);
            }

            allVisualLines = newVisualLines;
            // visibleVisualLines = readonly copy of visual lines
            visibleVisualLines = new ReadOnlyCollection<VisualLine>(newVisualLines.ToArray());
            newVisualLines = null;

            if (allVisualLines.Any(line => line.IsDisposed))
            {
                throw new InvalidOperationException("A visual line was disposed even though it is still in use.\n" +
                                                    "This can happen when Redraw() is called during measure for lines " +
                                                    "that are already constructed.");
            }

            return maxWidth;
        }

        private void CaretTimerTick(object sender, EventArgs e)
        {
            _caretBlink = !_caretBlink;
            InvalidateVisual();
        }

        public void LineLeft()
        {
            HorizontalOffset =(scrollOffset.X - WideSpaceWidth);
        }

        public void LineRight()
        {
            HorizontalOffset = (scrollOffset.X + WideSpaceWidth);
        }

        public void MouseWheelLeft()
        {
            HorizontalOffset = (
                scrollOffset.X - (WheelScrollLines * WideSpaceWidth));
            OnScrollChange();
        }

        public void MouseWheelRight()
        {
            HorizontalOffset =(
                scrollOffset.X + (WheelScrollLines * WideSpaceWidth));
            OnScrollChange();
        }

        public void PageLeft()
        {
            HorizontalOffset = (scrollOffset.X - scrollViewport.Width);
        }

        public void PageRight()
        {
            HorizontalOffset = (scrollOffset.X + scrollViewport.Width);
        }

        public void LineDown()
        {
            VerticalOffset = (scrollOffset.Y + DefaultLineHeight);
        }

        public void LineUp()
        {
            VerticalOffset = (scrollOffset.Y - DefaultLineHeight);
        }

        public void MouseWheelDown()
        {
            VerticalOffset = (
                scrollOffset.Y + (WheelScrollLines * DefaultLineHeight));
            OnScrollChange();
        }

        public void MouseWheelUp()
        {
            VerticalOffset= (
                scrollOffset.Y - (WheelScrollLines * DefaultLineHeight));
            OnScrollChange();
        }

        public void PageDown()
        {
            VerticalOffset = (scrollOffset.Y + scrollViewport.Height);
        }

        public void PageUp()
        {
            VerticalOffset = (scrollOffset.Y - scrollViewport.Height);
        }        

        public Rect MakeVisible(Visual visual, Rect rectangle)
        {
            throw new Exception("Port to perspex.");

            //if (rectangle.IsEmpty || visual == null || visual == this || !this.IsAncestorOf(visual))
            //{
            //    return new Rect();
            //}
            //// Convert rectangle into our coordinate space.
            //GeneralTransform childTransform = visual.TransformToAncestor(this);
            //rectangle = childTransform.TransformBounds(rectangle);

            //MakeVisible(Rect.Offset(rectangle, scrollOffset));

            //return rectangle;
        }


        /// <summary>
		/// Gets the width of a 'wide space' (the space width used for calculating the tab size).
		/// </summary>
		/// <remarks>
		/// This is the width of an 'x' in the current font.
		/// We do not measure the width of an actual space as that would lead to tiny tabs in
		/// some proportional fonts.
		/// For monospaced fonts, this property will return the expected value, as 'x' and ' ' have the same width.
		/// </remarks>
		public double WideSpaceWidth
        {
            get
            {
                CalculateDefaultTextMetrics();
                return wideSpaceWidth;
            }
        }

        /// <summary>
        /// Gets the default line height. This is the height of an empty line or a line containing regular text.
        /// Lines that include formatted text or custom UI elements may have a different line height.
        /// </summary>
        public double DefaultLineHeight
        {
            get
            {
                CalculateDefaultTextMetrics();
                return defaultLineHeight;
            }
        }

        /// <summary>
        /// Gets the default baseline position. This is the difference between <see cref="VisualYPosition.TextTop"/>
        /// and <see cref="VisualYPosition.Baseline"/> for a line containing regular text.
        /// Lines that include formatted text or custom UI elements may have a different baseline.
        /// </summary>
        public double DefaultBaseline
        {
            get
            {
                CalculateDefaultTextMetrics();
                return defaultBaseline;
            }
        }

        void CalculateDefaultTextMetrics()
        {
            //throw new Exception("Port to perspex.");
            //if (defaultTextMetricsValid)
            //    return;
            //defaultTextMetricsValid = true;
            //if (formatter != null)
            //{
            //    var textRunProperties = CreateGlobalTextRunProperties();
            //    using (var line = formatter.FormatLine(
            //        new SimpleTextSource("x", textRunProperties),
            //        0, 32000,
            //        new VisualLineTextParagraphProperties { defaultTextRunProperties = textRunProperties },
            //        null))
            //    {
            //        wideSpaceWidth = Math.Max(1, line.WidthIncludingTrailingWhitespace);
            //        defaultBaseline = Math.Max(1, line.Baseline);
            //        defaultLineHeight = Math.Max(1, line.Height);
            //    }
            //}
            //else
            //{
            //    wideSpaceWidth = FontSize / 2;
            //    defaultBaseline = FontSize;
            //    defaultLineHeight = FontSize + 3;
            //}
            //// Update heightTree.DefaultLineHeight, if a document is loaded.
            //if (heightTree != null)
            //    heightTree.DefaultLineHeight = defaultLineHeight;
        }

        bool defaultTextMetricsValid;
        double wideSpaceWidth; // Width of an 'x'. Used as basis for the tab width, and for scrolling.
        double defaultLineHeight; // Height of a line containing 'x'. Used for scrolling.
        double defaultBaseline; // Baseline of a line containing 'x'. Used for TextTop/TextBottom calculation.

        /// <summary>
        /// Empty line selection width.
        /// </summary>
        public virtual double EmptyLineSelectionWidth
        {
            get { return 1; }
        }
    }


}
