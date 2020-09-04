using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace EasyScada.Core.Designer
{
    [TemplateVisualState(Name = "Focused", GroupName = "FocusStates"),
     TemplateVisualState(Name = "Disabled", GroupName = "CommonStates"),
     TemplateVisualState(Name = "Normal", GroupName = "CommonStates"),
     TemplateVisualState(Name = "MouseOver", GroupName = "CommonStates"),
     TemplateVisualState(Name = "Pressed", GroupName = "CommonStates"),
     TemplateVisualState(Name = "Unfocused", GroupName = "FocusStates")]
    public class TabItemBase : System.Windows.Controls.Button
    {
        internal const string LayoutName = "Layout";
        internal const string TextName = "Text";
        internal const string ImageName = "Image";
        internal const string ShadowBorderName = "BackgroundNorm_highlight";
        internal const string ShadowHighlightBorderName = "BackgroundOver_Highlight";
        internal const string ShadowHighlightBorderName2 = "BackgroundNormShadow1";
        internal const string ShadowHighlightBorderName3 = "BackgroundNormShadow2";
        internal const string ShadowHighlightBorderName4 = "BackgroundNormShadow3";
        internal const string PressedShadowName1 = "BackgroundPressedShadow";
        internal const string PressedShadowName2 = "BackgroundPressedInnerShadow";
        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(TabItemBase), new PropertyMetadata(new CornerRadius(3.0)));
        internal Border pressedShadow1;
        internal Border pressedShadow2;
        internal Border shadowBorder;
        internal Border shadowHighlightBorder;
        internal Border shadowHighlightBorder2;
        internal Border shadowHighlightBorder3;
        internal Border shadowHighlightBorder4;

        public static readonly DependencyProperty TextImageRelationProperty = 
            DependencyProperty.Register("TextImageRelation", typeof(TextImageRelation), typeof(TabItemBase), new PropertyMetadata(TextImageRelation.ImageBeforeText, TextImageRelationPropertyChanged));
        public static readonly DependencyProperty TextProperty = 
            DependencyProperty.Register("Text", typeof(string), typeof(TabItemBase), new PropertyMetadata("", TextPropertyChanged));
        public static readonly DependencyProperty ImageSourceProperty =
            DependencyProperty.Register("ImageSource", typeof(ImageSource), typeof(TabItemBase), new PropertyMetadata(null, ImageSourcePropertyChanged));
        public static readonly DependencyProperty BackgroundOpacityProperty = 
            DependencyProperty.Register("BackgroundOpacity", typeof(double), typeof(TabItemBase), new PropertyMetadata(1.0));
        public static readonly DependencyProperty ForegroundNormalProperty = 
            DependencyProperty.Register("ForegroundNormal", typeof(Brush), typeof(TabItemBase), new PropertyMetadata(null, ForegroundNormalPropertyChanged));
        public static readonly DependencyProperty ForegroundDisabledProperty = 
            DependencyProperty.Register("ForegroundDisabled", typeof(Brush), typeof(TabItemBase), new PropertyMetadata(new SolidColorBrush(Colors.Gray), ForegroundDisabledPropertyChanged));
        public static readonly DependencyProperty ForegroundPressedProperty = 
            DependencyProperty.Register("ForegroundPressed", typeof(Brush), typeof(TabItemBase), new PropertyMetadata(null, ForegroundPressedPropertyChanged));
        public static readonly DependencyProperty ForegroundHighlightProperty = 
            DependencyProperty.Register("ForegroundHighlight", typeof(Brush), typeof(TabItemBase), new PropertyMetadata(null, ForegroundHighlightPropertyChanged));
        public static readonly DependencyProperty BackgroundHighlightProperty = 
            DependencyProperty.Register("BackgroundHighlight", typeof(Brush), typeof(TabItemBase), new PropertyMetadata(null, BackgroundHighlightPropertyChanged));
        public static readonly DependencyProperty BorderBrushHighlightProperty = 
            DependencyProperty.Register("BorderBrushHighlight", typeof(Brush), typeof(TabItemBase), new PropertyMetadata(null, BorderBrushHighlightPropertyChanged));
        public static readonly DependencyProperty BackgroundPressedProperty = 
            DependencyProperty.Register("BackgroundPressed", typeof(Brush), typeof(TabItemBase), new PropertyMetadata(null, BackgroundPressedPropertyChanged));
        public static readonly DependencyProperty BorderBrushPressedProperty = 
            DependencyProperty.Register("BorderBrushPressed", typeof(Brush), typeof(TabItemBase), new PropertyMetadata(null, BorderBrushPressedPropertyChanged));

        // Events
        [Description("Occurs when the TextImageRelation property is changed."), Category("Action")]
        public event EventHandler TextImageRelationChanged;

        // Methods
        public TabItemBase()
        {
            base.DefaultStyleKey = typeof(TabItemBase);
        }

        private static void BackgroundHighlightPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabItemBase base2 = d as TabItemBase;
        }

        private static void BackgroundPressedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabItemBase base2 = d as TabItemBase;
        }

        private static void BorderBrushHighlightPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabItemBase base2 = d as TabItemBase;
        }

        private static void BorderBrushPressedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabItemBase base2 = d as TabItemBase;
        }

        private void Button_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.Layout != null)
            {
                if (base.IsEnabled)
                {
                    this.Layout.Opacity = 1.0;
                }
                else
                {
                    this.Layout.Opacity = 0.5;
                }
            }
        }

        private void Button_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (this.Layout != null)
            {
                RectangleGeometry geometry = new RectangleGeometry
                {
                    Rect = new Rect(0.0, 0.0, base.ActualWidth, base.ActualHeight)
                };
                if ((geometry.Bounds.Width > 0.0) && (geometry.Bounds.Height > 0.0))
                {
                    base.Clip = geometry;
                }
            }
        }

        private static void ForegroundDisabledPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabItemBase base2 = d as TabItemBase;
            if (base2 != null)
            {
                base2.SynchronizeForegrounds();
            }
        }

        private static void ForegroundHighlightPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabItemBase base2 = d as TabItemBase;
            if (base2 != null)
            {
                base2.SynchronizeForegrounds();
            }
        }

        private static void ForegroundNormalPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabItemBase base2 = d as TabItemBase;
            if (base2 != null)
            {
                base2.SynchronizeForegrounds();
            }
        }

        private static void ForegroundPressedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabItemBase base2 = d as TabItemBase;
            if (base2 != null)
            {
                base2.SynchronizeForegrounds();
            }
        }

        private static void ImageSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabItemBase base2 = d as TabItemBase;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.Layout = base.GetTemplateChild("Layout") as Grid;
            this.TextBlock = base.GetTemplateChild("Text") as TextBlock;
            this.ImageControl = base.GetTemplateChild("Image") as Image;
            this.shadowBorder = base.GetTemplateChild("BackgroundNorm_highlight") as Border;
            this.shadowHighlightBorder = base.GetTemplateChild("BackgroundOver_Highlight") as Border;
            this.shadowHighlightBorder2 = base.GetTemplateChild("BackgroundNormShadow1") as Border;
            this.shadowHighlightBorder3 = base.GetTemplateChild("BackgroundNormShadow2") as Border;
            this.shadowHighlightBorder4 = base.GetTemplateChild("BackgroundNormShadow3") as Border;
            this.pressedShadow1 = base.GetTemplateChild("BackgroundPressedShadow") as Border;
            this.pressedShadow2 = base.GetTemplateChild("BackgroundPressedInnerShadow") as Border;
            base.SizeChanged += new SizeChangedEventHandler(this.Button_SizeChanged);
            base.IsEnabledChanged += Button_IsEnabledChanged;
        }

        protected override void OnClick()
        {
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                base.OnClick();
            }
            else
            {
                base.OnClick();
            }
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            this.SynchronizeForegrounds();
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            this.SynchronizeForegrounds();
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            this.SynchronizeForegrounds();
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);
            this.SynchronizeForegrounds();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            this.SynchronizeForegrounds();
        }

        protected override void OnMouseRightButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseRightButtonDown(e);
            this.SynchronizeForegrounds();
        }

        protected override void OnMouseRightButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseRightButtonUp(e);
            this.SynchronizeForegrounds();
        }

        protected virtual void OnTextImageRelationChanged()
        {
            if (this.TextImageRelationChanged != null)
            {
                this.TextImageRelationChanged(this, EventArgs.Empty);
            }
        }

        protected virtual void SynchronizeForegrounds()
        {
            if (base.IsPressed)
            {
                base.Foreground = this.ForegroundPressed;
            }
            else if (base.IsMouseOver)
            {
                base.Foreground = this.ForegroundHighlight;
            }
            else if (base.IsEnabled)
            {
                base.Foreground = this.ForegroundNormal;
            }
            else if (!base.IsEnabled)
            {
                base.Foreground = this.ForegroundDisabled;
            }
        }

        private static void TextImageRelationPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabItemBase base2 = d as TabItemBase;
            if (base2 != null)
            {
                base2.OnTextImageRelationChanged();
            }
        }

        private static void TextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabItemBase control = d as TabItemBase;
        }

        public virtual void UnwireEvents()
        {
            base.SizeChanged -= new SizeChangedEventHandler(this.Button_SizeChanged);
            base.IsEnabledChanged -= Button_IsEnabledChanged;
        }

        // Properties
        public TextImageRelation TextImageRelation
        {
            get =>
                ((TextImageRelation)base.GetValue(TextImageRelationProperty));
            set =>
                base.SetValue(TextImageRelationProperty, value);
        }

        public Grid Layout { get; internal set; }

        public Image ImageControl { get; internal set; }

        public TextBlock TextBlock { get; internal set; }

        public string Text
        {
            get =>
                ((string)base.GetValue(TextProperty));
            set =>
                base.SetValue(TextProperty, value);
        }

        public ImageSource ImageSource
        {
            get =>
                ((ImageSource)base.GetValue(ImageSourceProperty));
            set =>
                base.SetValue(ImageSourceProperty, value);
        }

        public CornerRadius CornerRadius
        {
            get =>
                ((CornerRadius)base.GetValue(CornerRadiusProperty));
            set =>
                base.SetValue(CornerRadiusProperty, value);
        }

        public double BackgroundOpacity
        {
            get =>
                ((double)base.GetValue(BackgroundOpacityProperty));
            set =>
                base.SetValue(BackgroundOpacityProperty, value);
        }

        public Brush ForegroundNormal
        {
            get =>
                ((Brush)base.GetValue(ForegroundNormalProperty));
            set =>
                base.SetValue(ForegroundNormalProperty, value);
        }

        public Brush ForegroundDisabled
        {
            get =>
                ((Brush)base.GetValue(ForegroundDisabledProperty));
            set =>
                base.SetValue(ForegroundDisabledProperty, value);
        }

        public Brush ForegroundPressed
        {
            get =>
                ((Brush)base.GetValue(ForegroundPressedProperty));
            set =>
                base.SetValue(ForegroundPressedProperty, value);
        }

        public Brush ForegroundHighlight
        {
            get =>
                ((Brush)base.GetValue(ForegroundHighlightProperty));
            set =>
                base.SetValue(ForegroundHighlightProperty, value);
        }

        public Brush BackgroundHighlight
        {
            get =>
                ((Brush)base.GetValue(BackgroundHighlightProperty));
            set =>
                base.SetValue(BackgroundHighlightProperty, value);
        }

        public Brush BorderBrushHighlight
        {
            get =>
                ((Brush)base.GetValue(BorderBrushHighlightProperty));
            set =>
                base.SetValue(BorderBrushHighlightProperty, value);
        }

        public Brush BackgroundPressed
        {
            get =>
                ((Brush)base.GetValue(BackgroundPressedProperty));
            set =>
                base.SetValue(BackgroundPressedProperty, value);
        }

        public Brush BorderBrushPressed
        {
            get =>
                ((Brush)base.GetValue(BorderBrushPressedProperty));
            set =>
                base.SetValue(BorderBrushPressedProperty, value);
        }
    }
}



