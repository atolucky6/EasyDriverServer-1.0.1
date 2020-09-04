using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace EasyScada.Core.Designer
{
    public sealed class AnimatedScrollElement : RangeBase
    {
        // Fields
        internal const string ElementHorizontalSmallDecreaseName = "HorizontalSmallDecrease";
        internal const string ElementHorizontalSmallIncreaseName = "HorizontalSmallIncrease";
        internal const string ElementHorizontalTemplateName = "HorizontalRoot";
        internal const string ElementVerticalSmallDecreaseName = "VerticalSmallDecrease";
        internal const string ElementVerticalSmallIncreaseName = "VerticalSmallIncrease";
        internal const string ElementVerticalTemplateName = "VerticalRoot";
        internal const string GroupCommon = "CommonStates";
        internal const string StateDisabled = "Disabled";
        internal const string StateMouseOver = "MouseOver";
        internal const string StateNormal = "Normal";
        public static readonly DependencyProperty OrientationProperty = 
            DependencyProperty.Register("Orientation", typeof(Orientation), typeof(AnimatedScrollElement), new PropertyMetadata(Orientation.Vertical, OnOrientationPropertyChanged));
        public static readonly DependencyProperty ViewportSizeProperty = 
            DependencyProperty.Register("ViewportSize", typeof(double), typeof(AnimatedScrollElement), new PropertyMetadata(0.0, OnViewportSizeChanged));
        private bool isButtonMouseOver;

        // Events
        [Category("Action")]
        public event EventHandler IsButtonMouseOverChanged;

        public event ScrollEventHandler Scroll;

        // Methods
        public AnimatedScrollElement()
        {
            base.DefaultStyleKey = typeof(AnimatedScrollElement);
        }

        private double ConvertViewportSizeToDisplayUnits(double trackLength)
        {
            double num = base.Maximum - base.Minimum;
            return ((trackLength * this.ViewportSize) / (this.ViewportSize + num));
        }

        private void ElementVerticalSmallIncrease_MouseEnter(object sender, MouseEventArgs e)
        {
            this.IsButtonMouseOver = true;
        }

        private void ElementVerticalSmallIncrease_MouseLeave(object sender, MouseEventArgs e)
        {
            this.IsButtonMouseOver = false;
        }

        internal double GetTrackLength()
        {
            double naN = double.NaN;
            if (this.Orientation == Orientation.Horizontal)
            {
                naN = base.ActualWidth;
                if (this.ElementHorizontalSmallDecrease != null)
                {
                    naN -= (this.ElementHorizontalSmallDecrease.ActualWidth + this.ElementHorizontalSmallDecrease.Margin.Left) + this.ElementHorizontalSmallDecrease.Margin.Right;
                }
                if (this.ElementHorizontalSmallIncrease != null)
                {
                    naN -= (this.ElementHorizontalSmallIncrease.ActualWidth + this.ElementHorizontalSmallIncrease.Margin.Left) + this.ElementHorizontalSmallIncrease.Margin.Right;
                }
                return naN;
            }
            naN = base.ActualHeight;
            if (this.ElementVerticalSmallDecrease != null)
            {
                naN -= (this.ElementVerticalSmallDecrease.ActualHeight + this.ElementVerticalSmallDecrease.Margin.Top) + this.ElementVerticalSmallDecrease.Margin.Bottom;
            }
            if (this.ElementVerticalSmallIncrease != null)
            {
                naN -= (this.ElementVerticalSmallIncrease.ActualHeight + this.ElementVerticalSmallIncrease.Margin.Top) + this.ElementVerticalSmallIncrease.Margin.Bottom;
            }
            return naN;
        }

        public override void OnApplyTemplate()
        {
            RoutedEventHandler handler = null;
            RoutedEventHandler handler2 = null;
            RoutedEventHandler handler3 = null;
            RoutedEventHandler handler4 = null;
            RoutedEventHandler handler5 = null;
            RoutedEventHandler handler6 = null;
            RoutedEventHandler handler7 = null;
            RoutedEventHandler handler8 = null;
            base.OnApplyTemplate();
            this.ElementHorizontalTemplate = base.GetTemplateChild("HorizontalRoot") as FrameworkElement;
            this.ElementHorizontalSmallIncrease = base.GetTemplateChild("HorizontalSmallIncrease") as Button;
            this.ElementHorizontalSmallDecrease = base.GetTemplateChild("HorizontalSmallDecrease") as Button;
            this.ElementVerticalTemplate = base.GetTemplateChild("VerticalRoot") as FrameworkElement;
            this.ElementVerticalSmallIncrease = base.GetTemplateChild("VerticalSmallIncrease") as Button;
            this.ElementVerticalSmallDecrease = base.GetTemplateChild("VerticalSmallDecrease") as Button;
            if (this.ElementHorizontalSmallDecrease != null)
            {
                this.ElementHorizontalSmallDecrease.MouseEnter += new MouseEventHandler(this.ElementVerticalSmallIncrease_MouseEnter);
                this.ElementHorizontalSmallDecrease.MouseLeave += new MouseEventHandler(this.ElementVerticalSmallIncrease_MouseLeave);
                if (handler == null)
                {
                    if (handler5 == null)
                    {
                        handler5 = (sender, e) => this.SmallDecrement();
                    }
                    handler = handler5;
                }
                this.ElementHorizontalSmallDecrease.Click += handler;
            }
            if (this.ElementHorizontalSmallIncrease != null)
            {
                this.ElementHorizontalSmallIncrease.MouseEnter += new MouseEventHandler(this.ElementVerticalSmallIncrease_MouseEnter);
                this.ElementHorizontalSmallIncrease.MouseLeave += new MouseEventHandler(this.ElementVerticalSmallIncrease_MouseLeave);
                if (handler2 == null)
                {
                    if (handler6 == null)
                    {
                        handler6 = (sender, e) => this.SmallIncrement();
                    }
                    handler2 = handler6;
                }
                this.ElementHorizontalSmallIncrease.Click += handler2;
            }
            if (this.ElementVerticalSmallDecrease != null)
            {
                this.ElementVerticalSmallDecrease.MouseEnter += new MouseEventHandler(this.ElementVerticalSmallIncrease_MouseEnter);
                this.ElementVerticalSmallDecrease.MouseLeave += new MouseEventHandler(this.ElementVerticalSmallIncrease_MouseLeave);
                if (handler3 == null)
                {
                    if (handler7 == null)
                    {
                        handler7 = (sender, e) => this.SmallDecrement();
                    }
                    handler3 = handler7;
                }
                this.ElementVerticalSmallDecrease.Click += handler3;
            }
            if (this.ElementVerticalSmallIncrease != null)
            {
                this.ElementVerticalSmallIncrease.MouseEnter += new MouseEventHandler(this.ElementVerticalSmallIncrease_MouseEnter);
                this.ElementVerticalSmallIncrease.MouseLeave += new MouseEventHandler(this.ElementVerticalSmallIncrease_MouseLeave);
                if (handler4 == null)
                {
                    if (handler8 == null)
                    {
                        handler8 = (sender, e) => this.SmallIncrement();
                    }
                    handler4 = handler8;
                }
                this.ElementVerticalSmallIncrease.Click += handler4;
            }
            this.OnOrientationChanged();
            this.UpdateVisualState(false);
            base.IsEnabledChanged += ScrollElement_IsEnabledChanged;
        }

        protected void OnIsButtonMouseOverChanged()
        {
            if (this.IsButtonMouseOverChanged != null)
            {
                this.IsButtonMouseOverChanged(this, EventArgs.Empty);
            }
        }

        protected override void OnLostMouseCapture(MouseEventArgs e)
        {
            this.UpdateVisualState();
        }

        protected override void OnMaximumChanged(double oldMaximum, double newMaximum)
        {
            this.GetTrackLength();
            base.OnMaximumChanged(oldMaximum, newMaximum);
        }

        protected override void OnMinimumChanged(double oldMinimum, double newMinimum)
        {
            this.GetTrackLength();
            base.OnMinimumChanged(oldMinimum, newMinimum);
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            this.IsMouseOver = true;
            if ((this.Orientation == Orientation.Horizontal) || (this.Orientation == Orientation.Vertical))
            {
                this.UpdateVisualState();
            }
            this.IsButtonMouseOver = true;
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            this.IsMouseOver = false;
            if ((this.Orientation == Orientation.Horizontal) || (this.Orientation == Orientation.Vertical))
            {
                this.UpdateVisualState();
            }
            this.IsButtonMouseOver = false;
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            if (!e.Handled)
            {
                e.Handled = true;
                base.CaptureMouse();
            }
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);
            if (!e.Handled)
            {
                e.Handled = true;
            }
        }

        private void OnOrientationChanged()
        {
            this.GetTrackLength();
            if (this.ElementHorizontalTemplate != null)
            {
                this.ElementHorizontalTemplate.Visibility = (this.Orientation == Orientation.Horizontal) ? Visibility.Visible : Visibility.Collapsed;
            }
            if (this.ElementVerticalTemplate != null)
            {
                this.ElementVerticalTemplate.Visibility = (this.Orientation == Orientation.Horizontal) ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        private static void OnOrientationPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as AnimatedScrollElement).OnOrientationChanged();
        }

        protected override void OnValueChanged(double oldValue, double newValue)
        {
            this.GetTrackLength();
            base.OnValueChanged(oldValue, newValue);
        }

        private static void OnViewportSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        [SecuritySafeCritical]
        internal void RaiseScrollEvent(ScrollEventType scrollEventType)
        {
            ScrollEventArgs e = new ScrollEventArgs(scrollEventType, base.Value);
            if (this.Scroll != null)
            {
                this.Scroll(this, e);
            }
        }

        private void ScrollElement_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!base.IsEnabled)
            {
                this.IsMouseOver = false;
            }
            this.UpdateVisualState();
        }

        private void SmallDecrement()
        {
            double num = Math.Max(base.Value - base.SmallChange, base.Minimum);
            if (base.Value != num)
            {
                base.Value = num;
                this.RaiseScrollEvent(ScrollEventType.SmallDecrement);
            }
        }

        private void SmallIncrement()
        {
            double num = Math.Min(base.Value + base.SmallChange, base.Maximum);
            if (base.Value != num)
            {
                base.Value = num;
                this.RaiseScrollEvent(ScrollEventType.SmallIncrement);
            }
        }

        internal void UpdateVisualState()
        {
            this.UpdateVisualState(true);
        }

        internal void UpdateVisualState(bool useTransitions)
        {
            if (!base.IsEnabled)
            {
                VisualStateManager.GoToState(this, "Disabled", useTransitions);
            }
            else if (this.IsMouseOver)
            {
                VisualStateManager.GoToState(this, "MouseOver", useTransitions);
            }
            else
            {
                VisualStateManager.GoToState(this, "Normal", useTransitions);
            }
        }

        // Properties
        public bool IsButtonMouseOver
        {
            get =>
                this.isButtonMouseOver;
            set
            {
                if (this.isButtonMouseOver != value)
                {
                    this.isButtonMouseOver = value;
                    this.OnIsButtonMouseOverChanged();
                }
            }
        }

        internal Button ElementHorizontalSmallDecrease { get; set; }

        internal Button ElementHorizontalSmallIncrease { get; set; }

        internal FrameworkElement ElementHorizontalTemplate { get; set; }

        internal Button ElementVerticalSmallDecrease { get; set; }

        internal Button ElementVerticalSmallIncrease { get; set; }

        internal FrameworkElement ElementVerticalTemplate { get; set; }

        internal bool IsMouseOver { get; set; }

        public Orientation Orientation
        {
            get =>
                ((Orientation)base.GetValue(OrientationProperty));
            set =>
                base.SetValue(OrientationProperty, value);
        }

        public double ViewportSize
        {
            get =>
                ((double)base.GetValue(ViewportSizeProperty));
            set =>
                base.SetValue(ViewportSizeProperty, value);
        }
    }
}
