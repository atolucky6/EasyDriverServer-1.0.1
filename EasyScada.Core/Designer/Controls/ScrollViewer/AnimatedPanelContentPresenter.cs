using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace EasyScada.Core.Designer
{
public sealed class AnimatedPanelContentPresenter : ContentPresenter, IAnimatedPanelScrollInfo
    {
        // Fields
        private RectangleGeometry clipRect;
        private bool isClipPropertySet;
        private ScrollData scrollData = new ScrollData();
        private IAnimatedPanelScrollInfo scrollInfo;
        private DispatcherTimer incrementTimer = new DispatcherTimer();
        private DispatcherTimer decrementTimer = new DispatcherTimer();
        public static readonly DependencyProperty LineStepProperty = DependencyProperty.Register("LineStep", typeof(double), typeof(AnimatedPanelContentPresenter), new PropertyMetadata(60.0));
        public static readonly DependencyProperty AccelerationLineStepProperty = DependencyProperty.Register("AccelerationLineStep", typeof(double), typeof(AnimatedPanelContentPresenter), new PropertyMetadata(6.0));
        private bool isHovered;
        private double currentStep;
        private double lastIncrementHOffset;
        private double startOffset;
        private bool cachedDownScrolling;
        private bool isHScrolling;
        private bool cachedLeftScrolling;
        private double startHOffset;
        private bool cachedRightScrolling;
        private bool cachedUpScrolling;

        // Methods
        public AnimatedPanelContentPresenter()
        {
            this.incrementTimer.Tick += incrementTimer_Tick;
            this.decrementTimer.Tick += decrementTimer_Tick;
            this.incrementTimer.Interval = new TimeSpan(0, 0, 0, 0, 1);
            this.decrementTimer.Interval = new TimeSpan(0, 0, 0, 0, 1);
        }

        protected override Size ArrangeOverride(Size arrangeSize)
        {
            this.UpdateClip(arrangeSize);
            UIElement element = (VisualTreeHelper.GetChildrenCount(this) == 0) ? null : (VisualTreeHelper.GetChild(this, 0) as UIElement);
            if (this.IsScrollClient)
            {
                this.VerifyScrollData(arrangeSize, this.scrollData.extent);
            }
            if (element != null)
            {
                Rect finalRect = new Rect(0.0, 0.0, element.DesiredSize.Width, element.DesiredSize.Height);
                if (this.IsScrollClient)
                {
                    finalRect.X = -this.scrollData.computedOffset.X;
                    finalRect.Y = -this.scrollData.computedOffset.Y;
                }
                finalRect.Width = Math.Max(finalRect.Width, arrangeSize.Width);
                finalRect.Height = Math.Max(finalRect.Height, arrangeSize.Height);
                element.Arrange(finalRect);
            }
            return arrangeSize;
        }

        internal static double CoerceOffset(double offset, double extent, double viewport)
        {
            if (offset > (extent - viewport))
            {
                offset = extent - viewport;
            }
            if (offset < 0.0)
            {
                offset = 0.0;
            }
            return offset;
        }

        private bool CoerceOffsets()
        {
            double x = CoerceOffset(this.scrollData.offset.X, this.scrollData.extent.Width, this.scrollData.viewport.Width);
            Vector vector = new Vector(x, CoerceOffset(this.scrollData.offset.Y, this.scrollData.extent.Height, this.scrollData.viewport.Height));
            bool flag = scrollData.computedOffset == vector;
            this.scrollData.computedOffset = vector;
            return flag;
        }

        internal static double ComputeScrollOffsetWithMinimalScroll(double topView, double bottomView, double topChild, double bottomChild)
        {
            bool flag = topChild < topView && bottomChild < bottomView;
            bool flag2 = bottomChild > bottomView && topChild > topView;
            bool flag3 = (bottomChild - topChild) > (bottomView - topView);
            if (flag && !flag3)
            {
                return topChild;
            }
            if (flag2 && flag3)
            {
                return topChild;
            }
            if (!flag && !flag2)
            {
                return topView;
            }
            return (bottomChild - (bottomView - topView));
        }

        private void decrementTimer_Tick(object sender, EventArgs e)
        {
            if (!this.isHScrolling)
            {
                if (this.VerticalOffset > (this.startOffset - this.LineStep))
                {
                    double lineStep = this.LineStep;
                    if ((this.startOffset - this.LineStep) < 0.0)
                    {
                        lineStep = this.startOffset;
                    }
                    double num2 = this.GetCalculatedValue(new ExponentialInterpolation(), 0.0, lineStep, this.currentStep);
                    this.SetVerticalOffset(this.startOffset - num2);
                    this.currentStep += 3.0 / this.LineStep;
                }
                else
                {
                    this.decrementTimer.Stop();
                    if (this.cachedUpScrolling)
                    {
                        if (this.ScrollOwner.ElementTopVerticalScrollBar.ElementVerticalSmallDecrease.IsPressed)
                        {
                            this.LineUp();
                        }
                        this.cachedUpScrolling = false;
                    }
                }
            }
            else if (this.isHScrolling)
            {
                if (this.HorizontalOffset > (this.startHOffset - this.LineStep))
                {
                    double lineStep = this.LineStep;
                    if ((this.startHOffset - this.LineStep) < 0.0)
                    {
                        lineStep = this.startHOffset;
                    }
                    double num4 = this.GetCalculatedValue(new ExponentialInterpolation(), 0.0, lineStep, this.currentStep);
                    this.SetHorizontalOffset(this.startHOffset - num4);
                    this.currentStep += 3.0 / this.LineStep;
                }
                else
                {
                    this.decrementTimer.Stop();
                    if (this.cachedLeftScrolling)
                    {
                        if (this.ScrollOwner.ElementLeftHorizontalScrollBar.ElementHorizontalSmallDecrease.IsPressed)
                        {
                            this.LineLeft();
                        }
                        this.cachedLeftScrolling = false;
                    }
                }
            }
        }

        private double GetCalculatedValue(Interpolation interpolation, double from, double to, double progress)
        {
            double alpha = interpolation.GetAlpha(progress);
            return (from + (alpha * (to - from)));
        }

        public void HookupScrollingComponents()
        {
        }

        public void HookupScrollingComponents(AnimatedScrollViewer templatedParent)
        {
            if (templatedParent != null)
            {
                IAnimatedPanelScrollInfo content = null;
                content = base.Content as IAnimatedPanelScrollInfo;
                if (content == null)
                {
                    ItemsPresenter reference = base.Content as ItemsPresenter;
                    if ((reference != null) && (VisualTreeHelper.GetChildrenCount(reference) > 0))
                    {
                        content = VisualTreeHelper.GetChild(reference, 0) as IAnimatedPanelScrollInfo;
                    }
                }
                if (content == null)
                {
                    content = this;
                }
                if ((content != this.scrollInfo) && (this.scrollInfo != null))
                {
                    if (this.IsScrollClient)
                    {
                        this.scrollData = new ScrollData();
                    }
                    else
                    {
                        this.scrollInfo.ScrollOwner = null;
                    }
                }
                if (content != null)
                {
                    this.scrollInfo = content;
                    content.ScrollOwner = templatedParent;
                    templatedParent.ScrollInfo = content;
                }
            }
            else if (this.scrollInfo != null)
            {
                if (this.scrollInfo.ScrollOwner != null)
                {
                    this.scrollInfo.ScrollOwner.ScrollInfo = null;
                }
                this.scrollInfo.ScrollOwner = null;
                this.scrollInfo = null;
                this.scrollData = new ScrollData();
            }
        }

        private void incrementTimer_Tick(object sender, EventArgs e)
        {
            if (!this.isHScrolling)
            {
                if (((this.ScrollOwner != null) && (this.ScrollOwner.ElementBottomVerticalScrollBar != null)) && (this.ScrollOwner.ElementBottomVerticalScrollBar.Maximum == this.ScrollOwner.ElementBottomVerticalScrollBar.Value))
                {
                    this.incrementTimer.Stop();
                    if (this.cachedDownScrolling)
                    {
                        if (this.ScrollOwner.ElementBottomVerticalScrollBar.ElementVerticalSmallIncrease.IsPressed)
                        {
                            this.LineDown();
                        }
                        this.cachedDownScrolling = false;
                    }
                }
                else if (this.VerticalOffset < (this.startOffset + this.LineStep))
                {
                    double lineStep = this.LineStep;
                    if (((this.startOffset + this.LineStep) + this.scrollInfo.ViewportHeight) > this.scrollInfo.ExtentHeight)
                    {
                        lineStep = ((this.startOffset + this.LineStep) + this.scrollInfo.ViewportHeight) - this.scrollInfo.ExtentHeight;
                        lineStep = this.LineStep - lineStep;
                    }
                    double num2 = this.GetCalculatedValue(new ExponentialInterpolation(), 0.0, lineStep, this.currentStep);
                    this.SetVerticalOffset(this.startOffset + num2);
                    this.currentStep += 3.0 / this.LineStep;
                }
            }
            else if (this.isHScrolling)
            {
                if (((this.ScrollOwner != null) && (this.ScrollOwner.ElementRightHorizontalScrollBar != null)) && (this.ScrollOwner.ElementRightHorizontalScrollBar.Maximum == this.ScrollOwner.ElementRightHorizontalScrollBar.Value))
                {
                    this.incrementTimer.Stop();
                    this.cachedRightScrolling = false;
                }
                else if (this.HorizontalOffset < (this.startHOffset + this.LineStep))
                {
                    double lineStep = this.LineStep;
                    if (((this.startHOffset + this.LineStep) + this.scrollInfo.ViewportWidth) > this.scrollInfo.ExtentWidth)
                    {
                        lineStep = ((this.startHOffset + this.LineStep) + this.scrollInfo.ViewportWidth) - this.scrollInfo.ExtentWidth;
                        lineStep = this.LineStep - lineStep;
                    }
                    double num4 = this.GetCalculatedValue(new ExponentialInterpolation(), 0.0, lineStep, this.currentStep);
                    this.SetHorizontalOffset(this.startHOffset + num4);
                    this.lastIncrementHOffset = this.HorizontalOffset;
                    this.currentStep += 3.0 / this.LineStep;
                }
                else
                {
                    this.incrementTimer.Stop();
                    if (this.cachedRightScrolling)
                    {
                        if (this.ScrollOwner.ElementRightHorizontalScrollBar.ElementHorizontalSmallIncrease.IsPressed)
                        {
                            this.LineRight();
                        }
                        this.cachedRightScrolling = false;
                    }
                }
            }
        }

        public void LineDown()
        {
            if (this.IsScrollClient)
            {
                if (!this.incrementTimer.IsEnabled)
                {
                    this.decrementTimer.Stop();
                    this.incrementTimer.Stop();
                    this.incrementTimer.Start();
                    this.currentStep = 0.0;
                    this.startOffset = this.VerticalOffset;
                    this.isHScrolling = false;
                }
                else
                {
                    this.cachedDownScrolling = true;
                }
            }
        }

        public void LineLeft()
        {
            if (this.IsScrollClient)
            {
                if (!this.decrementTimer.IsEnabled)
                {
                    this.decrementTimer.Stop();
                    this.incrementTimer.Stop();
                    this.decrementTimer.Start();
                    this.currentStep = 0.0;
                    this.startHOffset = this.HorizontalOffset;
                    this.isHScrolling = true;
                }
                else
                {
                    this.cachedLeftScrolling = true;
                }
            }
        }

        public void LineRight()
        {
            if (this.IsScrollClient)
            {
                if (!this.incrementTimer.IsEnabled)
                {
                    this.decrementTimer.Stop();
                    this.incrementTimer.Stop();
                    this.incrementTimer.Start();
                    this.currentStep = 0.0;
                    this.startHOffset = this.HorizontalOffset;
                    this.isHScrolling = true;
                }
                else
                {
                    this.cachedRightScrolling = true;
                }
            }
        }

        public void LineUp()
        {
            if (this.IsScrollClient)
            {
                if (!this.decrementTimer.IsEnabled)
                {
                    this.decrementTimer.Stop();
                    this.incrementTimer.Stop();
                    this.decrementTimer.Start();
                    this.currentStep = 0.0;
                    this.startOffset = this.VerticalOffset;
                    this.isHScrolling = false;
                }
                else
                {
                    this.cachedUpScrolling = true;
                }
            }
        }

        public Rect MakeVisible(UIElement visual, Rect rectangle)
        {
            if ((rectangle.IsEmpty || (visual == null)) || (visual == this))
            {
                return Rect.Empty;
            }
            Point point = visual.TransformToVisual(this).Transform(new Point(rectangle.X, rectangle.Y));
            rectangle.X = point.X;
            rectangle.Y = point.Y;
            if (this.IsScrollClient)
            {
                Rect rect = new Rect(this.HorizontalOffset, this.VerticalOffset, this.ViewportWidth, this.ViewportHeight);
                rectangle.X = rectangle.X + rect.X;
                rectangle.Y = rectangle.Y + rect.Y;
                double topView = rect.Left;
                double offset = ComputeScrollOffsetWithMinimalScroll(topView, rect.Right, rectangle.Left, rectangle.Right);
                double introduced5 = rect.Top;
                double num2 = ComputeScrollOffsetWithMinimalScroll(introduced5, rect.Bottom, rectangle.Top, rectangle.Bottom);
                this.SetHorizontalOffset(offset);
                this.SetVerticalOffset(num2);
                rect.X = offset;
                rect.Y = num2;
                rectangle.Intersect(rect);
                if (!rectangle.IsEmpty)
                {
                    rectangle.X = rectangle.X - rect.X;
                    rectangle.Y = rectangle.Y - rect.Y;
                }
            }
            return rectangle;
        }

        protected override Size MeasureOverride(Size constraint)
        {
            UIElement element = (VisualTreeHelper.GetChildrenCount(this) == 0) ? null : (VisualTreeHelper.GetChild(this, 0) as UIElement);
            Size desiredSize = new Size(0.0, 0.0);
            Size extent = new Size(0.0, 0.0);
            if (element != null)
            {
                if (!this.IsScrollClient)
                {
                    desiredSize = base.MeasureOverride(constraint);
                }
                else
                {
                    Size availableSize = constraint;
                    if (this.scrollData.canHorizontallyScroll)
                    {
                        availableSize.Width = double.PositiveInfinity;
                    }
                    if (this.scrollData.canVerticallyScroll)
                    {
                        availableSize.Height = double.PositiveInfinity;
                    }
                    element.Measure(availableSize);
                    desiredSize = element.DesiredSize;
                }
                extent = element.DesiredSize;
            }
            if (this.IsScrollClient)
            {
                this.VerifyScrollData(constraint, extent);
            }
            desiredSize.Width = Math.Min(constraint.Width, desiredSize.Width);
            desiredSize.Height = Math.Min(constraint.Height, desiredSize.Height);
            return desiredSize;
        }

        public void MouseWheelDown()
        {
            if (this.IsScrollClient)
            {
                this.SetVerticalOffset(this.VerticalOffset + 0x30);
            }
        }

        public void MouseWheelLeft()
        {
            if (this.IsScrollClient)
            {
                this.SetHorizontalOffset(this.HorizontalOffset - 0x30);
            }
        }

        public void MouseWheelRight()
        {
            if (this.IsScrollClient)
            {
                this.SetHorizontalOffset(this.HorizontalOffset + 0x30);
            }
        }

        public void MouseWheelUp()
        {
            if (this.IsScrollClient)
            {
                this.SetVerticalOffset(this.VerticalOffset - 0x30);
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.HookupScrollingComponents();
        }

        public void PageDown()
        {
            if (this.IsScrollClient)
            {
                this.SetVerticalOffset(this.VerticalOffset + this.ViewportHeight);
            }
        }

        public void PageLeft()
        {
            if (this.IsScrollClient)
            {
                this.SetHorizontalOffset(this.HorizontalOffset - this.ViewportWidth);
            }
        }

        public void PageRight()
        {
            if (this.IsScrollClient)
            {
                this.SetHorizontalOffset(this.HorizontalOffset + this.ViewportWidth);
            }
        }

        public void PageUp()
        {
            if (this.IsScrollClient)
            {
                this.SetVerticalOffset(this.VerticalOffset - this.ViewportHeight);
            }
        }

        public void SetHorizontalOffset(double offset)
        {
            if (this.CanHorizontallyScroll)
            {
                double num = ValidateInputOffset(offset);
                if (scrollData.offset.X != num)
                {
                    this.scrollData.offset.X = num;
                    base.InvalidateArrange();
                }
            }
        }

        public void SetVerticalOffset(double offset)
        {
            if (this.CanVerticallyScroll)
            {
                double num = ValidateInputOffset(offset);
                if (this.scrollData.offset.Y != num)
                {
                    this.scrollData.offset.Y = num;
                    base.InvalidateArrange();
                }
            }
        }

        public void StopTimers()
        {
            this.incrementTimer.Stop();
            this.decrementTimer.Stop();
        }

        private void UpdateClip(Size arrangeSize)
        {
            if (!this.isClipPropertySet)
            {
                this.clipRect = new RectangleGeometry();
                base.Clip = this.clipRect;
                this.isClipPropertySet = true;
            }
            this.clipRect.Rect = new Rect(0.0, 0.0, arrangeSize.Width, arrangeSize.Height);
        }

        internal static double ValidateInputOffset(double offset)
        {
            if (double.IsNaN(offset))
            {
                throw new ArgumentOutOfRangeException("offset");
            }
            return Math.Max(0.0, offset);
        }

        private void VerifyScrollData(Size viewport, Size extent)
        {
            bool flag = viewport == this.scrollData.viewport;
            flag &= extent == this.scrollData.extent;
            this.scrollData.viewport = viewport;
            this.scrollData.extent = extent;
            if (!(flag & this.CoerceOffsets()) && (this.ScrollOwner != null))
            {
                this.ScrollOwner.InvalidateScrollInfo();
            }
        }

        // Properties
        public double LineSmallStep
        {
            get => this.LineStep;
            set => this.LineStep = value;
        }

        public double LineStep
        {
            get =>
                ((double)base.GetValue(LineStepProperty));
            set =>
                base.SetValue(LineStepProperty, value);
        }

        public double AccelerationLineStep
        {
            get =>
                ((double)base.GetValue(AccelerationLineStepProperty));
            set =>
                base.SetValue(AccelerationLineStepProperty, value);
        }

        public bool IsHovered
        {
            get =>
                this.isHovered;
            set
            {
                if (value != this.isHovered)
                {
                    this.isHovered = value;
                }
            }
        }

        public bool CanHorizontallyScroll
        {
            get
            {
                if (!this.IsScrollClient)
                {
                    return false;
                }
                return this.scrollData.canHorizontallyScroll;
            }
            set
            {
                if (this.IsScrollClient && (this.scrollData.canHorizontallyScroll != value))
                {
                    this.scrollData.canHorizontallyScroll = value;
                    base.InvalidateMeasure();
                }
            }
        }

        public bool CanVerticallyScroll
        {
            get
            {
                if (!this.IsScrollClient)
                {
                    return false;
                }
                return this.scrollData.canVerticallyScroll;
            }
            set
            {
                if (this.IsScrollClient && (this.scrollData.canVerticallyScroll != value))
                {
                    this.scrollData.canVerticallyScroll = value;
                    base.InvalidateMeasure();
                }
            }
        }

        public double ExtentHeight
        {
            get
            {
                if (!this.IsScrollClient)
                {
                    return 0.0;
                }
                return this.scrollData.extent.Height;
            }
        }

        public double ExtentWidth
        {
            get
            {
                if (!this.IsScrollClient)
                {
                    return 0.0;
                }
                return this.scrollData.extent.Width;
            }
        }

        public double HorizontalOffset
        {
            get
            {
                if (!this.IsScrollClient)
                {
                    return 0.0;
                }
                return this.scrollData.computedOffset.X;
            }
        }

        private bool IsScrollClient =>
            (this.scrollInfo == this);

        public AnimatedScrollViewer ScrollOwner
        {
            get
            {
                if (!this.IsScrollClient)
                {
                    return null;
                }
                return this.scrollData._scrollOwner;
            }
            set
            {
                if (this.IsScrollClient)
                {
                    this.scrollData._scrollOwner = value;
                }
            }
        }

        public double VerticalOffset
        {
            get
            {
                if (!this.IsScrollClient)
                {
                    return 0.0;
                }
                return this.scrollData.computedOffset.Y;
            }
        }

        public double ViewportHeight
        {
            get
            {
                if (!this.IsScrollClient)
                {
                    return 0.0;
                }
                return this.scrollData.viewport.Height;
            }
        }

        public double ViewportWidth
        {
            get
            {
                if (!this.IsScrollClient)
                {
                    return 0.0;
                }
                return this.scrollData.viewport.Width;
            }
        }
    }
}
