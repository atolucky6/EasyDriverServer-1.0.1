using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace EasyScada.Core.Designer
{
    public interface IAnimatedPanelScrollInfo
    {
        // Methods
        void LineDown();
        void LineLeft();
        void LineRight();
        void LineUp();
        Rect MakeVisible(UIElement visual, Rect rectangle);
        void MouseWheelDown();
        void MouseWheelLeft();
        void MouseWheelRight();
        void MouseWheelUp();
        void PageDown();
        void PageLeft();
        void PageRight();
        void PageUp();
        void SetHorizontalOffset(double offset);
        void SetVerticalOffset(double offset);

        // Properties
        bool CanHorizontallyScroll { get; set; }
        bool CanVerticallyScroll { get; set; }
        double ExtentHeight { get; }
        double ExtentWidth { get; }
        double HorizontalOffset { get; }
        AnimatedScrollViewer ScrollOwner { get; set; }
        double VerticalOffset { get; }
        double ViewportHeight { get; }
        double ViewportWidth { get; }
    }

    public sealed class AnimatedScrollViewer : ContentControl
    {
        // Fields
        internal const double mouseWheelDelta = 0x30;
        internal const double scrollLineDelta = 0x10;
        public const string ElementLeftHorizontalScrollBarName = "HorizontalLeftScrollBar";
        public const string ElementRightHorizontalScrollBarName = "HorizontalRightScrollBar";
        public const string ElementScrollContentPresenterName = "ScrollContentPresenter";
        public const string ElementTopVerticalScrollBarName = "VerticalTopScrollBar";
        public const string ElementBottomVerticalScrollBarName = "VerticalBottomScrollBar";
        private bool inChildInvalidateMeasure;
        private bool inMeasure;
        private IAnimatedPanelScrollInfo scrollInfo;
        private Visibility scrollVisibilityX;
        private Visibility scrollVisibilityY;
        private bool templatedParentHandlesScrolling;
        private double xExtent;
        private double xOffset;
        private double xViewport;
        private double yExtent;
        private double yOffset;
        private double yViewport;
        public static readonly DependencyProperty ComputedHorizontalScrollBarVisibilityProperty = 
            DependencyProperty.Register("ComputedHorizontalScrollBarVisibility", typeof(Visibility), typeof(AnimatedScrollViewer), null);
        public static readonly DependencyProperty ComputedVerticalScrollBarVisibilityProperty = 
            DependencyProperty.Register("ComputedVerticalScrollBarVisibility", typeof(Visibility), typeof(AnimatedScrollViewer), null);
        public static readonly DependencyProperty ExtentHeightProperty = 
            DependencyProperty.Register("ExtentHeight", typeof(double), typeof(AnimatedScrollViewer), null);
        public static readonly DependencyProperty ExtentWidthProperty =
            DependencyProperty.Register("ExtentWidth", typeof(double), typeof(AnimatedScrollViewer), null);
        public static readonly DependencyProperty HorizontalOffsetProperty = 
            DependencyProperty.Register("HorizontalOffset", typeof(double), typeof(AnimatedScrollViewer), null);
        public static readonly DependencyProperty HorizontalScrollBarVisibilityProperty = 
            DependencyProperty.RegisterAttached("HorizontalScrollBarVisibility", typeof(ScrollBarVisibility), typeof(AnimatedScrollViewer), new PropertyMetadata(ScrollBarVisibility.Auto, OnScrollBarVisibilityChanged));
        public static readonly DependencyProperty ScrollableHeightProperty = 
            DependencyProperty.Register("ScrollableHeight", typeof(double), typeof(AnimatedScrollViewer), null);
        public static readonly DependencyProperty ScrollableWidthProperty = 
            DependencyProperty.Register("ScrollableWidth", typeof(double), typeof(AnimatedScrollViewer), null);
        public static readonly DependencyProperty VerticalOffsetProperty =
            DependencyProperty.Register("VerticalOffset", typeof(double), typeof(AnimatedScrollViewer), null);
        public static readonly DependencyProperty VerticalScrollBarVisibilityProperty =
            DependencyProperty.RegisterAttached("VerticalScrollBarVisibility", typeof(ScrollBarVisibility), typeof(AnimatedScrollViewer), new PropertyMetadata(ScrollBarVisibility.Auto, OnScrollBarVisibilityChanged));
        public static readonly DependencyProperty ViewportHeightProperty = 
            DependencyProperty.Register("ViewportHeight", typeof(double), typeof(AnimatedScrollViewer), null);
        public static readonly DependencyProperty ViewportWidthProperty = 
            DependencyProperty.Register("ViewportWidth", typeof(double), typeof(AnimatedScrollViewer), null);
        public static readonly DependencyProperty VerticalScrollbarHorizontalAlignmentProperty =
            DependencyProperty.Register("VerticalScrollbarHorizontalAlignment", typeof(HorizontalAlignment), typeof(AnimatedScrollViewer), new PropertyMetadata(HorizontalAlignment.Center));
        public static readonly DependencyProperty HorizontalScrollbarVerticalAlignmentProperty = 
            DependencyProperty.Register("HorizontalScrollbarVerticalAlignment", typeof(VerticalAlignment), typeof(AnimatedScrollViewer), new PropertyMetadata(VerticalAlignment.Center));
        public static readonly DependencyProperty AccelerationMaxStepProperty = 
            DependencyProperty.Register("AccelerationMaxStep", typeof(double), typeof(AnimatedScrollViewer), new PropertyMetadata(10.0));
        public static readonly DependencyProperty LineStepProperty = 
            DependencyProperty.Register("LineStep", typeof(double), typeof(AnimatedScrollViewer), new PropertyMetadata(60.0));

        // Events
        internal event ScrollChangedDelegate ScrollChanged;

        // Methods
        public AnimatedScrollViewer()
        {
            base.DefaultStyleKey = typeof(AnimatedScrollViewer);
        }

        internal bool CanForwardPropertyValue(DependencyProperty property) =>
            (base.ReadLocalValue(property) == DependencyProperty.UnsetValue);

        private void CustomScrollViewer_Loaded(object sender, RoutedEventArgs e)
        {
            this.ElementScrollContentPresenter.HookupScrollingComponents(this);
            base.InvalidateMeasure();
            base.UpdateLayout();
            this.InvalidateScrollInfo();
        }

        private void ElementBottomVerticalScrollBar_IsButtonMouseOverChanged(object sender, EventArgs e)
        {
            this.ElementScrollContentPresenter.IsHovered = this.ElementBottomVerticalScrollBar.IsButtonMouseOver;
        }

        private void ElementLeftHorizontalScrollBar_IsButtonMouseOverChanged(object sender, EventArgs e)
        {
            this.ElementScrollContentPresenter.IsHovered = this.ElementLeftHorizontalScrollBar.IsButtonMouseOver;
        }

        private void ElementRightHorizontalScrollBar_IsButtonMouseOverChanged(object sender, EventArgs e)
        {
            this.ElementScrollContentPresenter.IsHovered = this.ElementRightHorizontalScrollBar.IsButtonMouseOver;
        }

        private void ElementTopVerticalScrollBar_IsButtonMouseOverChanged(object sender, EventArgs e)
        {
            this.ElementScrollContentPresenter.IsHovered = this.ElementTopVerticalScrollBar.IsButtonMouseOver;
        }

        public static ScrollBarVisibility GetHorizontalScrollBarVisibility(DependencyObject element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            return (ScrollBarVisibility)element.GetValue(HorizontalScrollBarVisibilityProperty);
        }

        public static ScrollBarVisibility GetVerticalScrollBarVisibility(DependencyObject element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            return (ScrollBarVisibility)element.GetValue(VerticalScrollBarVisibilityProperty);
        }

        private void HandleHorizontalScroll(ScrollEventArgs e)
        {
            if (this.ScrollInfo != null)
            {
                double horizontalOffset = this.ScrollInfo.HorizontalOffset;
                double minValue = horizontalOffset;
                switch (e.ScrollEventType)
                {
                    case ScrollEventType.First:
                        minValue = double.MinValue;
                        break;

                    case ScrollEventType.LargeDecrement:
                        this.ScrollInfo.PageLeft();
                        break;

                    case ScrollEventType.LargeIncrement:
                        this.ScrollInfo.PageRight();
                        break;

                    case ScrollEventType.Last:
                        minValue = double.MaxValue;
                        break;

                    case ScrollEventType.SmallDecrement:
                        this.ScrollInfo.LineLeft();
                        break;

                    case ScrollEventType.SmallIncrement:
                        this.ScrollInfo.LineRight();
                        break;

                    case ScrollEventType.ThumbPosition:
                    case ScrollEventType.ThumbTrack:
                        minValue = e.NewValue;
                        break;
                }
                minValue = Math.Max(minValue, 0.0);
                minValue = Math.Min(this.ScrollableWidth, minValue);
                if (horizontalOffset == minValue)
                    ScrollInfo.SetHorizontalOffset(minValue);
            }
        }

        private void HandleScroll(Orientation orientation, ScrollEventArgs e)
        {
            if (orientation == Orientation.Horizontal)
            {
                this.HandleHorizontalScroll(e);
            }
            else
            {
                this.HandleVerticalScroll(e);
            }
        }

        private void HandleVerticalScroll(ScrollEventArgs e)
        {
            if (this.ScrollInfo != null)
            {
                double verticalOffset = this.ScrollInfo.VerticalOffset;
                double minValue = verticalOffset;
                switch (e.ScrollEventType)
                {
                    case ScrollEventType.First:
                        minValue = double.MinValue;
                        break;

                    case ScrollEventType.LargeDecrement:
                        this.ScrollInfo.PageUp();
                        break;

                    case ScrollEventType.LargeIncrement:
                        this.ScrollInfo.PageDown();
                        break;

                    case ScrollEventType.Last:
                        minValue = double.MaxValue;
                        break;

                    case ScrollEventType.SmallDecrement:
                        this.ScrollInfo.LineUp();
                        break;

                    case ScrollEventType.SmallIncrement:
                        this.ScrollInfo.LineDown();
                        break;

                    case ScrollEventType.ThumbPosition:
                    case ScrollEventType.ThumbTrack:
                        minValue = e.NewValue;
                        break;
                }
                minValue = Math.Max(minValue, 0.0);
                minValue = Math.Min(this.ScrollableHeight, minValue);
                if (verticalOffset == minValue)
                    ScrollInfo.SetVerticalOffset(minValue);
            }
        }

        public void InvalidateScrollInfo()
        {
            if (this.ScrollInfo != null)
            {
                if (!this.inMeasure)
                {
                    double num = this.ScrollInfo.ExtentWidth;
                    double num2 = this.ScrollInfo.ViewportWidth;
                    if ((this.HorizontalScrollBarVisibility == ScrollBarVisibility.Auto) && (((this.scrollVisibilityX == Visibility.Collapsed) && (num > num2)) || ((this.scrollVisibilityX == Visibility.Visible) && (num < num2))))
                    {
                        base.InvalidateMeasure();
                    }
                    else
                    {
                        num = this.ScrollInfo.ExtentHeight;
                        num2 = this.ScrollInfo.ViewportHeight;
                        if ((this.VerticalScrollBarVisibility == ScrollBarVisibility.Auto) && (((this.scrollVisibilityY == Visibility.Collapsed) && (num > num2)) || ((this.scrollVisibilityY == Visibility.Visible) && (num < num2))))
                        {
                            base.InvalidateMeasure();
                        }
                    }
                }
                double horizontalOffset = this.ScrollInfo.HorizontalOffset;
                double verticalOffset = this.ScrollInfo.VerticalOffset;
                double viewportWidth = this.ScrollInfo.ViewportWidth;
                double viewportHeight = this.ScrollInfo.ViewportHeight;
                double extentWidth = this.ScrollInfo.ExtentWidth;
                double extentHeight = this.ScrollInfo.ExtentHeight;
                if (xOffset != horizontalOffset || yOffset != verticalOffset || 
                    this.xViewport != viewportWidth || this.yViewport != viewportHeight || 
                    this.xExtent != extentWidth || this.yExtent != extentHeight)
                {
                    double xOffset = this.xOffset;
                    double yOffset = this.yOffset;
                    double xViewport = this.xViewport;
                    double yViewport = this.yViewport;
                    double xExtent = this.xExtent;
                    double yExtent = this.yExtent;
                    double scrollableWidth = this.ScrollableWidth;
                    double scrollableHeight = this.ScrollableHeight;
                    bool flag = false;
                    try
                    {
                        if (xOffset != horizontalOffset)
                        {
                            this.HorizontalOffset = horizontalOffset;
                            flag = true;
                        }
                        if (yOffset != verticalOffset)
                        {
                            this.VerticalOffset = verticalOffset;
                            flag = true;
                        }
                        if (xViewport != viewportWidth)
                        {
                            this.ViewportWidth = viewportWidth;
                            flag = true;
                        }
                        if (yViewport != viewportHeight)
                        {
                            this.ViewportHeight = viewportHeight;
                            flag = true;
                        }
                        if (xExtent != extentWidth)
                        {
                            this.ExtentWidth = extentWidth;
                            flag = true;
                        }
                        if (yExtent != extentHeight)
                        {
                            this.ExtentHeight = extentHeight;
                            flag = true;
                        }
                        double num17 = this.ScrollableWidth;
                        if (scrollableWidth != num17)
                        {
                            this.ScrollableWidth = num17;
                            flag = true;
                        }
                        double num18 = this.ScrollableHeight;
                        if (scrollableHeight != num18)
                        {
                            this.ScrollableHeight = num18;
                            flag = true;
                        }
                    }
                    finally
                    {
                        if (flag)
                        {
                            if (xOffset != this.xOffset && (this.ElementLeftHorizontalScrollBar != null))
                            {
                                this.ElementLeftHorizontalScrollBar.Value = this.xOffset;
                            }
                            if (xOffset != this.xOffset && (this.ElementRightHorizontalScrollBar != null))
                            {
                                this.ElementRightHorizontalScrollBar.Value = this.xOffset;
                            }
                            if (yOffset != this.yOffset && (this.ElementTopVerticalScrollBar != null))
                            {
                                this.ElementTopVerticalScrollBar.Value = this.yOffset;
                            }
                            if (yOffset != this.yOffset && (this.ElementBottomVerticalScrollBar != null))
                            {
                                this.ElementBottomVerticalScrollBar.Value = this.yOffset;
                            }
                            if (this.ScrollChanged != null)
                            {
                                this.ScrollChanged(this.HorizontalOffset, this.VerticalOffset);
                            }
                        }
                    }
                }
            }
        }

        internal void LineDown()
        {
            this.HandleVerticalScroll(new ScrollEventArgs(ScrollEventType.SmallIncrement, 0.0));
        }

        internal void LineLeft()
        {
            this.HandleHorizontalScroll(new ScrollEventArgs(ScrollEventType.SmallDecrement, 0.0));
        }

        internal void LineRight()
        {
            this.HandleHorizontalScroll(new ScrollEventArgs(ScrollEventType.SmallIncrement, 0.0));
        }

        internal void LineUp()
        {
            this.HandleVerticalScroll(new ScrollEventArgs(ScrollEventType.SmallDecrement, 0.0));
        }

        protected override Size MeasureOverride(Size constraint)
        {
            this.inChildInvalidateMeasure = false;
            UIElement element = (VisualTreeHelper.GetChildrenCount(this) == 0) ? null : (VisualTreeHelper.GetChild(this, 0) as UIElement);
            if (element == null)
            {
                return new Size();
            }
            IAnimatedPanelScrollInfo scrollInfo = this.ScrollInfo;
            ScrollBarVisibility verticalScrollBarVisibility = this.VerticalScrollBarVisibility;
            ScrollBarVisibility horizontalScrollBarVisibility = this.HorizontalScrollBarVisibility;
            bool flag = verticalScrollBarVisibility == ScrollBarVisibility.Auto;
            bool flag2 = horizontalScrollBarVisibility == ScrollBarVisibility.Auto;
            Visibility visibility3 = (verticalScrollBarVisibility == ScrollBarVisibility.Visible) ? Visibility.Visible : Visibility.Collapsed;
            Visibility visibility4 = (horizontalScrollBarVisibility == ScrollBarVisibility.Visible) ? Visibility.Visible : Visibility.Collapsed;
            try
            {
                this.inMeasure = true;
                if (this.scrollVisibilityY != visibility3)
                {
                    this.scrollVisibilityY = visibility3;
                    this.ComputedVerticalScrollBarVisibility = this.scrollVisibilityY;
                }
                if (this.scrollVisibilityX != visibility4)
                {
                    this.scrollVisibilityX = visibility4;
                    this.ComputedHorizontalScrollBarVisibility = this.scrollVisibilityX;
                }
                if (scrollInfo != null)
                {
                    scrollInfo.CanHorizontallyScroll = horizontalScrollBarVisibility != ScrollBarVisibility.Disabled;
                    scrollInfo.CanVerticallyScroll = verticalScrollBarVisibility != ScrollBarVisibility.Disabled;
                }
                element.Measure(constraint);
                scrollInfo = this.ScrollInfo;
                if ((scrollInfo != null) && (flag || flag2))
                {
                    bool flag3 = flag2 && (scrollInfo.ExtentWidth > scrollInfo.ViewportWidth);
                    bool flag4 = flag && (scrollInfo.ExtentHeight > scrollInfo.ViewportHeight);
                    if (flag3 && (this.scrollVisibilityX != Visibility.Visible))
                    {
                        this.scrollVisibilityX = Visibility.Visible;
                        this.ComputedHorizontalScrollBarVisibility = this.scrollVisibilityX;
                    }
                    if (flag4 && (this.scrollVisibilityY != Visibility.Visible))
                    {
                        this.scrollVisibilityY = Visibility.Visible;
                        this.ComputedVerticalScrollBarVisibility = this.scrollVisibilityY;
                    }
                    if (flag3 || flag4)
                    {
                        this.inChildInvalidateMeasure = true;
                        element.InvalidateMeasure();
                        element.Measure(constraint);
                    }
                    if ((flag2 && flag) && (flag3 != flag4))
                    {
                        bool flag5 = !flag3 && (scrollInfo.ExtentWidth > scrollInfo.ViewportWidth);
                        bool flag6 = !flag4 && (scrollInfo.ExtentHeight > scrollInfo.ViewportHeight);
                        if (flag5)
                        {
                            if (this.scrollVisibilityX != Visibility.Visible)
                            {
                                this.scrollVisibilityX = Visibility.Visible;
                                this.ComputedHorizontalScrollBarVisibility = this.scrollVisibilityX;
                            }
                        }
                        else if (flag6 && (this.scrollVisibilityY != Visibility.Visible))
                        {
                            this.scrollVisibilityY = Visibility.Visible;
                            this.ComputedVerticalScrollBarVisibility = this.scrollVisibilityY;
                        }
                        if (flag5 || flag6)
                        {
                            this.inChildInvalidateMeasure = true;
                            element.InvalidateMeasure();
                            element.Measure(constraint);
                        }
                    }
                }
            }
            finally
            {
                this.inMeasure = false;
            }
            return element.DesiredSize;
        }

        public override void OnApplyTemplate()
        {
            ScrollEventHandler handler = null;
            ScrollEventHandler handler2 = null;
            ScrollEventHandler handler3 = null;
            ScrollEventHandler handler4 = null;
            ScrollEventHandler handler5 = null;
            ScrollEventHandler handler6 = null;
            ScrollEventHandler handler7 = null;
            ScrollEventHandler handler8 = null;
            base.OnApplyTemplate();
            this.ElementScrollContentPresenter = base.GetTemplateChild("ScrollContentPresenter") as AnimatedPanelContentPresenter;
            this.ElementLeftHorizontalScrollBar = base.GetTemplateChild("HorizontalLeftScrollBar") as AnimatedScrollElement;
            this.ElementRightHorizontalScrollBar = base.GetTemplateChild("HorizontalRightScrollBar") as AnimatedScrollElement;
            this.ElementScrollContentPresenter.ScrollOwner = this;
            if (this.ElementRightHorizontalScrollBar != null)
            {
                this.ElementRightHorizontalScrollBar.IsButtonMouseOverChanged += new EventHandler(this.ElementRightHorizontalScrollBar_IsButtonMouseOverChanged);
                if (handler == null)
                {
                    if (handler5 == null)
                    {
                        handler5 = (sender, e) => this.HandleHorizontalScroll(e);
                    }
                    handler = handler5;
                }
                this.ElementRightHorizontalScrollBar.Scroll += handler;
            }
            if (this.ElementLeftHorizontalScrollBar != null)
            {
                this.ElementLeftHorizontalScrollBar.IsButtonMouseOverChanged += new EventHandler(this.ElementLeftHorizontalScrollBar_IsButtonMouseOverChanged);
                if (handler2 == null)
                {
                    if (handler6 == null)
                    {
                        handler6 = (sender, e) => this.HandleHorizontalScroll(e);
                    }
                    handler2 = handler6;
                }
                this.ElementLeftHorizontalScrollBar.Scroll += handler2;
            }
            this.ElementTopVerticalScrollBar = base.GetTemplateChild("VerticalTopScrollBar") as AnimatedScrollElement;
            this.ElementBottomVerticalScrollBar = base.GetTemplateChild("VerticalBottomScrollBar") as AnimatedScrollElement;
            if (this.ElementTopVerticalScrollBar != null)
            {
                this.ElementTopVerticalScrollBar.IsButtonMouseOverChanged += new EventHandler(this.ElementTopVerticalScrollBar_IsButtonMouseOverChanged);
                if (handler3 == null)
                {
                    if (handler7 == null)
                    {
                        handler7 = (sender, e) => this.HandleVerticalScroll(e);
                    }
                    handler3 = handler7;
                }
                this.ElementTopVerticalScrollBar.Scroll += handler3;
            }
            if (this.ElementBottomVerticalScrollBar != null)
            {
                this.ElementBottomVerticalScrollBar.IsButtonMouseOverChanged += new EventHandler(this.ElementBottomVerticalScrollBar_IsButtonMouseOverChanged);
                if (handler4 == null)
                {
                    if (handler8 == null)
                    {
                        handler8 = (sender, e) => this.HandleVerticalScroll(e);
                    }
                    handler4 = handler8;
                }
                this.ElementBottomVerticalScrollBar.Scroll += handler4;
            }
            if (this.ElementScrollContentPresenter != null)
            {
                this.ElementScrollContentPresenter.HookupScrollingComponents(this);
            }
            base.Loaded += new RoutedEventHandler(this.CustomScrollViewer_Loaded);
            base.InvalidateMeasure();
            base.UpdateLayout();
            this.InvalidateScrollInfo();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (!e.Handled && !this.TemplatedParentHandlesScrolling)
            {
                bool flag = 2 == ((int)Keyboard.Modifiers & 2);
                Orientation vertical = Orientation.Vertical;
                ScrollEventType thumbTrack = ScrollEventType.ThumbTrack;
                switch ((int)e.Key)
                {
                    case 0x13:
                        thumbTrack = ScrollEventType.LargeDecrement;
                        break;

                    case 20:
                        thumbTrack = ScrollEventType.LargeIncrement;
                        break;

                    case 0x15:
                        if (!flag)
                        {
                            vertical = Orientation.Horizontal;
                        }
                        thumbTrack = ScrollEventType.Last;
                        break;

                    case 0x16:
                        if (!flag)
                        {
                            vertical = Orientation.Horizontal;
                        }
                        thumbTrack = ScrollEventType.First;
                        break;

                    case 0x17:
                        vertical = Orientation.Horizontal;
                        thumbTrack = ScrollEventType.SmallDecrement;
                        break;

                    case 0x18:
                        thumbTrack = ScrollEventType.SmallDecrement;
                        break;

                    case 0x19:
                        vertical = Orientation.Horizontal;
                        thumbTrack = ScrollEventType.SmallIncrement;
                        break;

                    case 0x1a:
                        thumbTrack = ScrollEventType.SmallIncrement;
                        break;
                }
                if (ScrollEventType.ThumbTrack != thumbTrack)
                {
                    this.HandleScroll(vertical, new ScrollEventArgs(thumbTrack, 0.0));
                    e.Handled = true;
                }
            }
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            if (!e.Handled && base.Focus())
            {
                e.Handled = true;
            }
        }

        private static void OnScrollBarVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AnimatedScrollViewer viewer = d as AnimatedScrollViewer;
            if (viewer != null)
            {
                if (viewer.ScrollInfo != null)
                {
                    viewer.ScrollInfo.CanHorizontallyScroll = viewer.HorizontalScrollBarVisibility != ScrollBarVisibility.Disabled;
                    viewer.ScrollInfo.CanVerticallyScroll = viewer.VerticalScrollBarVisibility != ScrollBarVisibility.Disabled;
                }
                viewer.InvalidateMeasure();
            }
        }

        internal void PageDown()
        {
            this.HandleVerticalScroll(new ScrollEventArgs(ScrollEventType.LargeIncrement, 0.0));
        }

        internal void PageEnd()
        {
            this.HandleHorizontalScroll(new ScrollEventArgs(ScrollEventType.Last, 0.0));
        }

        internal void PageHome()
        {
            this.HandleHorizontalScroll(new ScrollEventArgs(ScrollEventType.First, 0.0));
        }

        internal void PageLeft()
        {
            this.HandleHorizontalScroll(new ScrollEventArgs(ScrollEventType.LargeDecrement, 0.0));
        }

        internal void PageRight()
        {
            this.HandleHorizontalScroll(new ScrollEventArgs(ScrollEventType.LargeIncrement, 0.0));
        }

        internal void PageUp()
        {
            this.HandleVerticalScroll(new ScrollEventArgs(ScrollEventType.LargeDecrement, 0.0));
        }

        internal void ScrollInDirection(Key key)
        {
            switch ((int)key)
            {
                case 0x13:
                    this.PageUp();
                    return;

                case 20:
                    this.PageDown();
                    return;

                case 0x15:
                    this.PageEnd();
                    return;

                case 0x16:
                    this.PageHome();
                    return;

                case 0x17:
                    this.LineLeft();
                    return;

                case 0x18:
                    this.LineUp();
                    return;

                case 0x19:
                    this.LineRight();
                    return;

                case 0x1a:
                    this.LineDown();
                    return;
            }
        }

        public void ScrollToHorizontalOffset(double offset)
        {
            this.HandleHorizontalScroll(new ScrollEventArgs(ScrollEventType.ThumbPosition, offset));
        }

        public void ScrollToVerticalOffset(double offset)
        {
            this.HandleVerticalScroll(new ScrollEventArgs(ScrollEventType.ThumbPosition, offset));
        }

        public static void SetHorizontalScrollBarVisibility(DependencyObject element, ScrollBarVisibility horizontalScrollBarVisibility)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            element.SetValue(HorizontalScrollBarVisibilityProperty, horizontalScrollBarVisibility);
        }

        public static void SetVerticalScrollBarVisibility(DependencyObject element, ScrollBarVisibility verticalScrollBarVisibility)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            element.SetValue(VerticalScrollBarVisibilityProperty, verticalScrollBarVisibility);
        }

        // Properties
        [Browsable(false)]
        public HorizontalAlignment VerticalScrollbarHorizontalAlignment
        {
            get =>
                ((HorizontalAlignment)base.GetValue(VerticalScrollbarHorizontalAlignmentProperty));
            set =>
                base.SetValue(VerticalScrollbarHorizontalAlignmentProperty, value);
        }

        public VerticalAlignment HorizontalScrollbarVerticalAlignment
        {
            get =>
                ((VerticalAlignment)base.GetValue(HorizontalScrollbarVerticalAlignmentProperty));
            set =>
                base.SetValue(HorizontalScrollbarVerticalAlignmentProperty, value);
        }

        internal double AccelerationMaxStep
        {
            get =>
                ((double)base.GetValue(AccelerationMaxStepProperty));
            set =>
                base.SetValue(AccelerationMaxStepProperty, value);
        }

        public double LineStep
        {
            get =>
                ((double)base.GetValue(LineStepProperty));
            set =>
                base.SetValue(LineStepProperty, value);
        }

        public Visibility ComputedHorizontalScrollBarVisibility
        {
            get =>
                ((Visibility)base.GetValue(ComputedHorizontalScrollBarVisibilityProperty));
            internal set =>
                base.SetValue(ComputedHorizontalScrollBarVisibilityProperty, value);
        }

        public Visibility ComputedVerticalScrollBarVisibility
        {
            get =>
                ((Visibility)base.GetValue(ComputedVerticalScrollBarVisibilityProperty));
            internal set =>
                base.SetValue(ComputedVerticalScrollBarVisibilityProperty, value);
        }

        public AnimatedScrollElement ElementLeftHorizontalScrollBar { get; set; }

        public AnimatedScrollElement ElementRightHorizontalScrollBar { get; set; }

        internal AnimatedPanelContentPresenter ElementScrollContentPresenter { get; set; }

        public AnimatedScrollElement ElementTopVerticalScrollBar { get; set; }

        public AnimatedScrollElement ElementBottomVerticalScrollBar { get; set; }

        public double ExtentHeight
        {
            get =>
                this.yExtent;
            internal set
            {
                this.yExtent = value;
                base.SetValue(ExtentHeightProperty, value);
            }
        }

        public double ExtentWidth
        {
            get =>
                this.xExtent;
            internal set
            {
                this.xExtent = value;
                base.SetValue(ExtentWidthProperty, value);
            }
        }

        public double HorizontalOffset
        {
            get =>
                this.xOffset;
            internal set
            {
                this.xOffset = value;
                base.SetValue(HorizontalOffsetProperty, value);
            }
        }

        public ScrollBarVisibility HorizontalScrollBarVisibility
        {
            get =>
                ((ScrollBarVisibility)base.GetValue(HorizontalScrollBarVisibilityProperty));
            set =>
                base.SetValue(HorizontalScrollBarVisibilityProperty, value);
        }

        internal bool InChildInvalidateMeasure
        {
            get => this.inChildInvalidateMeasure;
            set => this.inChildInvalidateMeasure = value;
        }

        public double ScrollableHeight
        {
            get =>
                Math.Max((double)0.0, (double)(this.ExtentHeight - this.ViewportHeight));
            internal set =>
                base.SetValue(ScrollableHeightProperty, value);
        }

        public double ScrollableWidth
        {
            get =>
                Math.Max((double)0.0, (double)(this.ExtentWidth - this.ViewportWidth));
            internal set =>
                base.SetValue(ScrollableWidthProperty, value);
        }

        internal IAnimatedPanelScrollInfo ScrollInfo
        {
            get =>
                this.scrollInfo;
            set
            {
                this.scrollInfo = value;
                if (this.scrollInfo != null)
                {
                    this.scrollInfo.CanHorizontallyScroll = this.HorizontalScrollBarVisibility != ScrollBarVisibility.Disabled;
                    this.scrollInfo.CanVerticallyScroll = this.VerticalScrollBarVisibility != ScrollBarVisibility.Disabled;
                }
            }
        }

        internal bool TemplatedParentHandlesScrolling
        {
            get =>
                this.templatedParentHandlesScrolling;
            set
            {
                this.templatedParentHandlesScrolling = value;
                base.IsTabStop = !this.templatedParentHandlesScrolling;
            }
        }

        public double VerticalOffset
        {
            get =>
                this.yOffset;
            internal set
            {
                this.yOffset = value;
                base.SetValue(VerticalOffsetProperty, value);
            }
        }

        public ScrollBarVisibility VerticalScrollBarVisibility
        {
            get =>
                ((ScrollBarVisibility)base.GetValue(VerticalScrollBarVisibilityProperty));
            set =>
                base.SetValue(VerticalScrollBarVisibilityProperty, value);
        }

        public double ViewportHeight
        {
            get =>
                this.yViewport;
            internal set
            {
                this.yViewport = value;
                base.SetValue(ViewportHeightProperty, value);
            }
        }

        public double ViewportWidth
        {
            get =>
                this.xViewport;
            internal set
            {
                this.xViewport = value;
                base.SetValue(ViewportWidthProperty, value);
            }
        }

        // Nested Types
        internal delegate void ScrollChangedDelegate(double xOffset, double yOffset);
    }
}
