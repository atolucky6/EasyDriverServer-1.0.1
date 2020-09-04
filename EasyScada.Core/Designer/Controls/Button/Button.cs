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
    [
        TemplatePart(Name = "Layout", Type = typeof(Grid)), 
        TemplatePart(Name = "BackgroundOver_Highlight", Type = typeof(Border)), 
        TemplateVisualState(Name = "Focused", GroupName = "FocusStates"), 
        TemplatePart(Name = "BackgroundNormShadow2", Type = typeof(Border)), 
        TemplatePart(Name = "BackgroundNormShadow3", Type = typeof(Border)), 
        TemplatePart(Name = "BackgroundPressedShadow", Type = typeof(Border)), 
        TemplatePart(Name = "BackgroundPressedInnerShadow", Type = typeof(Border)), 
        TemplatePart(Name = "Text", Type = typeof(TextBlock)), 
        TemplatePart(Name = "Image", Type = typeof(Image)), 
        TemplatePart(Name = "BackgroundNorm_highlight", Type = typeof(Border)), 
        TemplatePart(Name = "BackgroundNormShadow1", Type = typeof(Border)), Description("Represents a button control."), 
        TemplateVisualState(Name = "Disabled", GroupName = "CommonStates"), 
        TemplateVisualState(Name = "Normal", GroupName = "CommonStates"), 
        TemplateVisualState(Name = "MouseOver", GroupName = "CommonStates"), 
        TemplateVisualState(Name = "Pressed", GroupName = "CommonStates"), 
        TemplateVisualState(Name = "Unfocused", GroupName = "FocusStates")
    ]
    public class Button : System.Windows.Controls.Button
    {
        static Button()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Button), new FrameworkPropertyMetadata(typeof(Button)));
        }

        // Fields
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
        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(Button), new PropertyMetadata(new CornerRadius(3.0)));
        internal Border pressedShadow1;
        internal Border pressedShadow2;
        internal Border shadowBorder;
        internal Border shadowHighlightBorder;
        internal Border shadowHighlightBorder2;
        internal Border shadowHighlightBorder3;
        internal Border shadowHighlightBorder4;
        public static readonly DependencyProperty BackgroundShadowVisibilityProperty = DependencyProperty.Register("BackgroundShadowVisibility", typeof(Visibility), typeof(Button), new PropertyMetadata(Visibility.Visible));
        private Size imageSize = new Size(0x10, 0x10);
        public static readonly DependencyProperty TextImageRelationProperty = DependencyProperty.Register("TextImageRelation", typeof(TextImageRelation), typeof(Button), new PropertyMetadata(TextImageRelation.ImageBeforeText, TextImageRelationPropertyChanged));
        protected bool isSizeRotated;
        public static readonly DependencyProperty HorizontalTextAlignmentProperty = DependencyProperty.Register("HorizontalTextAlignment", typeof(HorizontalAlignment), typeof(Button), new PropertyMetadata(HorizontalAlignment.Center, HorizontalTextAlignmentPropertyChanged));
        public static readonly DependencyProperty VerticalTextAlignmentProperty = DependencyProperty.Register("VerticalTextAlignment", typeof(VerticalAlignment), typeof(Button), new PropertyMetadata(VerticalAlignment.Center, VerticalTextAlignmentPropertyChanged));
        public static readonly DependencyProperty HorizontalImageAlignmentProperty = DependencyProperty.Register("HorizontalImageAlignment", typeof(HorizontalAlignment), typeof(Button), new PropertyMetadata(HorizontalAlignment.Center, HorizontalImageAlignmentPropertyChanged));
        public static readonly DependencyProperty VerticalImageAlignmentProperty = DependencyProperty.Register("VerticalImageAlignment", typeof(VerticalAlignment), typeof(Button), new PropertyMetadata(VerticalAlignment.Center, VerticalImageAlignmentPropertyChanged));
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(Button), new PropertyMetadata("", TextPropertyChanged));
        public static readonly DependencyProperty ImageSourceProperty = DependencyProperty.Register("ImageSource", typeof(ImageSource), typeof(Button), new PropertyMetadata(null, ImageSourcePropertyChanged));
        public static readonly DependencyProperty BackgroundOpacityProperty = DependencyProperty.Register("BackgroundOpacity", typeof(double), typeof(Button), new PropertyMetadata(1.0));
        public static readonly DependencyProperty ForegroundNormalProperty = DependencyProperty.Register("ForegroundNormal", typeof(Brush), typeof(Button), new PropertyMetadata(null, ForegroundNormalPropertyChanged));
        public static readonly DependencyProperty ForegroundDisabledProperty = DependencyProperty.Register("ForegroundDisabled", typeof(Brush), typeof(Button), new PropertyMetadata(new SolidColorBrush(Colors.Gray), ForegroundDisabledPropertyChanged));
        public static readonly DependencyProperty ForegroundPressedProperty = DependencyProperty.Register("ForegroundPressed", typeof(Brush), typeof(Button), new PropertyMetadata(null, ForegroundPressedPropertyChanged));
        public static readonly DependencyProperty ForegroundHighlightProperty = DependencyProperty.Register("ForegroundHighlight", typeof(Brush), typeof(Button), new PropertyMetadata(null, ForegroundHighlightPropertyChanged));
        public static readonly DependencyProperty BackgroundHighlightProperty = DependencyProperty.Register("BackgroundHighlight", typeof(Brush), typeof(Button), new PropertyMetadata(null, BackgroundHighlightPropertyChanged));
        public static readonly DependencyProperty BorderBrushHighlightProperty = DependencyProperty.Register("BorderBrushHighlight", typeof(Brush), typeof(Button), new PropertyMetadata(null, BorderBrushHighlightPropertyChanged));
        public static readonly DependencyProperty BackgroundPressedProperty = DependencyProperty.Register("BackgroundPressed", typeof(Brush), typeof(Button), new PropertyMetadata(null, BackgroundPressedPropertyChanged));
        public static readonly DependencyProperty BorderBrushPressedProperty = DependencyProperty.Register("BorderBrushPressed", typeof(Brush), typeof(Button), new PropertyMetadata(null, BorderBrushPressedPropertyChanged));

        // Methods
        public Button()
        {
            base.DefaultStyleKey = typeof(Button);
        }

        protected virtual void ApplyTextImageRelation(Button b)
        {
            if ((this.ImageControl != null) && (this.TextBlock != null))
            {
                try
                {
                    if (!this.TextBlock.Text.Equals(this.Text))
                    {
                        this.TextBlock.Text = this.Text;
                    }
                    this.TextBlock.UpdateLayout();
                    this.ImageControl.UpdateLayout();
                    this.ImageControl.Width = this.imageSize.Width;
                    this.ImageControl.Height = this.imageSize.Height;
                    double actualWidth = this.TextBlock.ActualWidth;
                    double actualHeight = this.TextBlock.ActualHeight;
                    double imageWidth = this.imageSize.Width;
                    double imageHeight = this.imageSize.Height;
                    if ((base.ActualWidth > 0.0) && (base.ActualHeight > 0.0))
                    {
                        if (actualHeight > base.ActualHeight)
                        {
                            actualHeight = base.ActualHeight;
                        }
                        if (imageHeight > base.ActualHeight)
                        {
                            imageHeight = base.ActualHeight;
                        }
                        if (actualWidth > base.ActualWidth)
                        {
                            actualWidth = base.ActualWidth;
                        }
                        if (imageWidth > base.ActualWidth)
                        {
                            imageWidth = base.ActualWidth;
                        }
                        if ((this.ImageSource == null) && !string.IsNullOrEmpty(this.Text))
                        {
                            this.PerformTextLayout(ref actualWidth, ref actualHeight);
                        }
                        else if ((this.ImageSource != null) && string.IsNullOrEmpty(this.Text))
                        {
                            this.PerformImageLayout(ref imageWidth, ref imageHeight);
                        }
                        else
                        {
                            Size withinThis = new Size((base.ActualWidth - this.Layout.Margin.Left) - this.Layout.Margin.Right, (base.ActualHeight - this.Layout.Margin.Top) - this.Layout.Margin.Bottom);
                            if (this.isSizeRotated)
                            {
                                double num5 = withinThis.Height;
                                double num6 = withinThis.Width;
                                withinThis = new Size(num5, num6);
                            }
                            Size alignThis = new Size(imageWidth, imageHeight);
                            Size size3 = new Size(actualWidth, actualHeight);
                            double imageY = 0.0;
                            double imageX = 0.0;
                            double textY = 0.0;
                            double textX = 0.0;
                            switch (this.TextImageRelation)
                            {
                                case TextImageRelation.ImageBeforeText:
                                    textY = this.VAlign(size3, withinThis, this.VerticalTextAlignment);
                                    imageY = this.VAlign(alignThis, withinThis, this.VerticalImageAlignment);
                                    this.ImageBeforeText(ref alignThis, ref size3, ref imageX, ref textX);
                                    if (textX < ((imageX + alignThis.Width) + 2.0))
                                    {
                                        textX = (imageX + alignThis.Width) + 2.0;
                                    }
                                    break;

                                case TextImageRelation.ImageAboveText:
                                    textX = this.HAlign(size3, withinThis, this.HorizontalTextAlignment);
                                    imageX = this.HAlign(alignThis, withinThis, this.HorizontalImageAlignment);
                                    this.ImageAboveText(ref alignThis, ref size3, ref imageY, ref textY);
                                    if (textY < ((imageY + 2.0) + alignThis.Height))
                                    {
                                        textY = (imageY + 2.0) + alignThis.Height;
                                    }
                                    break;

                                case TextImageRelation.TextAboveImage:
                                    textX = this.HAlign(size3, withinThis, this.HorizontalTextAlignment);
                                    imageX = this.HAlign(alignThis, withinThis, this.HorizontalImageAlignment);
                                    this.TextAboveImage(ref alignThis, ref size3, ref imageY, ref textY);
                                    if (imageY < ((textY + 2.0) + size3.Height))
                                    {
                                        imageY = (textY + 2.0) + size3.Height;
                                    }
                                    break;

                                case TextImageRelation.TextBeforeImage:
                                    textY = this.VAlign(size3, withinThis, this.VerticalTextAlignment);
                                    imageY = this.VAlign(alignThis, withinThis, this.VerticalImageAlignment);
                                    this.TextBeforeImage(ref alignThis, ref size3, ref imageX, ref textX);
                                    if (imageX < ((textX + size3.Width) + 2.0))
                                    {
                                        imageX = (textX + size3.Width) + 2.0;
                                    }
                                    break;

                                case TextImageRelation.Overlay:
                                    textY = this.VAlign(size3, withinThis, this.VerticalTextAlignment);
                                    imageY = this.VAlign(alignThis, withinThis, this.VerticalImageAlignment);
                                    textX = this.HAlign(size3, withinThis, this.HorizontalTextAlignment);
                                    imageX = this.HAlign(alignThis, withinThis, this.HorizontalImageAlignment);
                                    break;
                            }
                            this.ImageControl.Margin = new Thickness(imageX, imageY, 0.0, 0.0);
                            this.TextBlock.Margin = new Thickness(textX, textY, 0.0, 0.0);
                        }
                    }
                }
                catch (Exception)
                {
                }
            }
        }

        private static void BackgroundHighlightPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Button button = d as Button;
        }

        private static void BackgroundPressedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Button button = d as Button;
        }

        private static void BorderBrushHighlightPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Button button = d as Button;
        }

        private static void BorderBrushPressedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Button button = d as Button;
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
                this.ApplyTextImageRelation(this);
            }
        }

        private static void ForegroundDisabledPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Button button = d as Button;
            if (button != null)
            {
                button.SynchronizeForegrounds();
            }
        }

        private static void ForegroundHighlightPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Button button = d as Button;
            if (button != null)
            {
                button.SynchronizeForegrounds();
            }
        }

        private static void ForegroundNormalPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Button button = d as Button;
            if (button != null)
            {
                button.SynchronizeForegrounds();
            }
        }

        private static void ForegroundPressedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Button button = d as Button;
            if (button != null)
            {
                button.SynchronizeForegrounds();
            }
        }

        private double HAlign(Size alignThis, Size withinThis, HorizontalAlignment align)
        {
            double num = 0.0;
            if (HorizontalAlignment.Right == align)
            {
                return (num + (withinThis.Width - alignThis.Width));
            }
            if (HorizontalAlignment.Center == align)
            {
                num += (withinThis.Width - alignThis.Width) / 2.0;
            }
            return num;
        }

        private static void HorizontalImageAlignmentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Button b = d as Button;
            if (b != null)
            {
                b.ApplyTextImageRelation(b);
            }
        }

        private static void HorizontalTextAlignmentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Button b = d as Button;
            if (b != null)
            {
                b.ApplyTextImageRelation(b);
            }
        }

        private void ImageAboveText(ref Size imageSize, ref Size textSize, ref double imageY, ref double textY)
        {
            double num = (base.ActualHeight - this.Layout.Margin.Top) - this.Layout.Margin.Bottom;
            switch (this.VerticalImageAlignment)
            {
                case VerticalAlignment.Top:
                case VerticalAlignment.Stretch:
                    imageY = 0.0;
                    if ((this.VerticalTextAlignment != VerticalAlignment.Top) && (this.VerticalTextAlignment != VerticalAlignment.Stretch))
                    {
                        if (this.VerticalTextAlignment == VerticalAlignment.Center)
                        {
                            textY = (num / 2.0) - (textSize.Height / 2.0);
                            return;
                        }
                        if (this.VerticalTextAlignment != VerticalAlignment.Bottom)
                        {
                            break;
                        }
                        textY = num - textSize.Height;
                        return;
                    }
                    textY = 2.0 + imageSize.Height;
                    return;

                case VerticalAlignment.Center:
                    if ((this.VerticalTextAlignment != VerticalAlignment.Stretch) && (this.VerticalTextAlignment != VerticalAlignment.Top))
                    {
                        if (this.VerticalTextAlignment == VerticalAlignment.Center)
                        {
                            imageY = ((num / 2.0) - (imageSize.Height / 2.0)) - (textSize.Height / 2.0);
                            if (imageY < 0.0)
                            {
                                imageY = 0.0;
                            }
                            textY = (imageY + 2.0) + imageSize.Height;
                            return;
                        }
                        if (this.VerticalTextAlignment != VerticalAlignment.Bottom)
                        {
                            break;
                        }
                        imageY = (num / 2.0) - (imageSize.Height / 2.0);
                        if (imageY < 0.0)
                        {
                            imageY = 0.0;
                        }
                        textY = num - textSize.Height;
                        return;
                    }
                    imageY = (num / 4.0) - (imageSize.Height / 2.0);
                    if (imageY < 0.0)
                    {
                        imageY = 0.0;
                    }
                    textY = (imageY + 2.0) + imageSize.Height;
                    return;

                case VerticalAlignment.Bottom:
                    if (this.VerticalTextAlignment == VerticalAlignment.Top)
                    {
                        imageY = ((num / 2.0) - (imageSize.Height / 2.0)) - (textSize.Height / 2.0);
                        if (imageY < 0.0)
                        {
                            imageY = 0.0;
                        }
                        textY = (imageY + 2.0) + imageSize.Height;
                    }
                    if (this.VerticalTextAlignment == VerticalAlignment.Center)
                    {
                        imageY = (((num / 2.0) - (imageSize.Height / 2.0)) - textSize.Height) + (num / 4.0);
                        if (imageY < 0.0)
                        {
                            imageY = 0.0;
                        }
                        textY = (imageY + 2.0) + imageSize.Height;
                        return;
                    }
                    if ((this.VerticalTextAlignment == VerticalAlignment.Stretch) || (this.VerticalTextAlignment == VerticalAlignment.Bottom))
                    {
                        imageY = ((num - imageSize.Height) - textSize.Height) - 2.0;
                        if (imageY < 0.0)
                        {
                            imageY = 0.0;
                        }
                        textY = (imageY + 2.0) + imageSize.Height;
                    }
                    break;

                default:
                    return;
            }
        }

        private void ImageBeforeText(ref Size imageSize, ref Size textSize, ref double imageX, ref double textX)
        {
            double num = (base.ActualWidth - this.Layout.Margin.Left) - this.Layout.Margin.Right;
            switch (this.HorizontalImageAlignment)
            {
                case HorizontalAlignment.Left:
                case HorizontalAlignment.Stretch:
                    if ((this.HorizontalTextAlignment != HorizontalAlignment.Left) && (this.HorizontalTextAlignment != HorizontalAlignment.Stretch))
                    {
                        if (this.HorizontalTextAlignment == HorizontalAlignment.Center)
                        {
                            imageX = 0.0;
                            textX = (num / 2.0) - (textSize.Width / 2.0);
                            if (textX < (imageSize.Width + 2.0))
                            {
                                textX = imageSize.Width + 2.0;
                                return;
                            }
                            break;
                        }
                        if (this.HorizontalTextAlignment != HorizontalAlignment.Right)
                        {
                            break;
                        }
                        imageX = 0.0;
                        textX = num - textSize.Width;
                        if (textX >= (imageSize.Width + 2.0))
                        {
                            break;
                        }
                        textX = imageSize.Width + 2.0;
                        return;
                    }
                    imageX = 0.0;
                    textX = imageSize.Width + 2.0;
                    return;

                case HorizontalAlignment.Center:
                    if (this.HorizontalTextAlignment != HorizontalAlignment.Left)
                    {
                        if (this.HorizontalTextAlignment == HorizontalAlignment.Center)
                        {
                            imageX = ((num / 2.0) - (imageSize.Width / 2.0)) - (textSize.Width / 2.0);
                            textX = (num / 2.0) - (textSize.Width / 2.0);
                            if (textX < ((imageX + imageSize.Width) + 2.0))
                            {
                                textX = (imageX + imageSize.Width) + 2.0;
                                return;
                            }
                            break;
                        }
                        imageX = (num / 2.0) - (imageSize.Width / 2.0);
                        if (imageX < 0.0)
                        {
                            imageX = 0.0;
                        }
                        textX = num - textSize.Width;
                        if (textX >= ((imageX + imageSize.Width) + 2.0))
                        {
                            break;
                        }
                        textX = (imageX + imageSize.Width) + 2.0;
                        return;
                    }
                    imageX = (num / 4.0) - (imageSize.Width / 2.0);
                    textX = (imageX + imageSize.Width) + 2.0;
                    return;

                case HorizontalAlignment.Right:
                    if ((this.HorizontalTextAlignment != HorizontalAlignment.Right) && (this.HorizontalTextAlignment != HorizontalAlignment.Stretch))
                    {
                        if (this.HorizontalTextAlignment == HorizontalAlignment.Left)
                        {
                            imageX = ((num / 2.0) - (imageSize.Width / 2.0)) - (textSize.Width / 2.0);
                            textX = (imageX + imageSize.Width) + 2.0;
                            return;
                        }
                        if (this.HorizontalTextAlignment == HorizontalAlignment.Center)
                        {
                            imageX = (((num / 2.0) - (imageSize.Width / 2.0)) - textSize.Width) + (num / 4.0);
                            if (imageX < 0.0)
                            {
                                imageX = 0.0;
                            }
                            textX = (imageX + 2.0) + imageSize.Width;
                        }
                        break;
                    }
                    imageX = ((num - imageSize.Width) - textSize.Width) - 2.0;
                    textX = (imageX + imageSize.Width) + 2.0;
                    return;

                default:
                    return;
            }
        }

        private static void ImageSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Button b = d as Button;
            if (b != null)
            {
                b.ApplyTextImageRelation(b);
            }
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
            this.ApplyTextImageRelation(this);
            base.SizeChanged += new SizeChangedEventHandler(this.Button_SizeChanged);
            base.IsEnabledChanged += Button_IsEnabledChanged;
        }

        protected override void OnClick()
        {
            base.OnClick();
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

        private void PerformImageLayout(ref double imageWidth, ref double imageHeight)
        {
            this.TextBlock.Width = double.NaN;
            this.TextBlock.Height = double.NaN;
            double num = (base.ActualWidth - this.Layout.Margin.Left) - this.Layout.Margin.Right;
            double num2 = (base.ActualHeight - this.Layout.Margin.Top) - this.Layout.Margin.Bottom;
            if (imageWidth > num)
            {
                imageWidth = num;
                this.TextBlock.Width = imageWidth;
            }
            if (imageHeight > num2)
            {
                imageHeight = num2;
                this.TextBlock.Height = imageHeight;
            }
            double num3 = (num2 - this.ImageControl.ActualHeight) / 2.0;
            double num4 = (num - this.ImageControl.ActualWidth) / 2.0;
            if (num3 < 0.0)
            {
                num3 = 0.0;
            }
            if (num4 < 0.0)
            {
                num4 = 0.0;
            }
            double left = 0.0;
            double top = 0.0;
            switch (this.HorizontalImageAlignment)
            {
                case HorizontalAlignment.Left:
                    left = 0.0;
                    break;

                case HorizontalAlignment.Center:
                    left = num4;
                    break;

                case HorizontalAlignment.Right:
                    left = base.ActualWidth - imageWidth;
                    break;

                case HorizontalAlignment.Stretch:
                    left = 0.0;
                    break;
            }
            switch (this.VerticalImageAlignment)
            {
                case VerticalAlignment.Top:
                    top = 0.0;
                    break;

                case VerticalAlignment.Center:
                    top = num3;
                    break;

                case VerticalAlignment.Bottom:
                    top = num2 - imageHeight;
                    break;

                case VerticalAlignment.Stretch:
                    top = 0.0;
                    break;
            }
            this.ImageControl.Margin = new Thickness(left, top, 0.0, 0.0);
        }

        private void PerformTextLayout(ref double textWidth, ref double textHeight)
        {
            this.TextBlock.Width = double.NaN;
            this.TextBlock.Height = double.NaN;
            double num = (base.ActualWidth - this.Layout.Margin.Left) - this.Layout.Margin.Right;
            double num2 = (base.ActualHeight - this.Layout.Margin.Top) - this.Layout.Margin.Bottom;
            if (textWidth > num)
            {
                textWidth = num;
                this.TextBlock.Width = textWidth;
            }
            if (textHeight > num2)
            {
                textHeight = num2;
                this.TextBlock.Height = textHeight;
            }
            double num3 = (num2 - this.TextBlock.ActualHeight) / 2.0;
            double num4 = (num - this.TextBlock.ActualWidth) / 2.0;
            if (num3 < 0.0)
            {
                num3 = 0.0;
            }
            if (num4 < 0.0)
            {
                num4 = 0.0;
            }
            double left = 0.0;
            double top = 0.0;
            switch (this.HorizontalTextAlignment)
            {
                case HorizontalAlignment.Left:
                    left = 0.0;
                    break;

                case HorizontalAlignment.Center:
                    left = num4;
                    break;

                case HorizontalAlignment.Right:
                    left = num - textWidth;
                    break;

                case HorizontalAlignment.Stretch:
                    left = 0.0;
                    break;
            }
            switch (this.VerticalTextAlignment)
            {
                case VerticalAlignment.Top:
                    top = 0.0;
                    break;

                case VerticalAlignment.Center:
                    top = num3;
                    break;

                case VerticalAlignment.Bottom:
                    top = num2 - textHeight;
                    break;

                case VerticalAlignment.Stretch:
                    top = 0.0;
                    break;
            }
            this.TextBlock.Margin = new Thickness(left, top, 0.0, 0.0);
        }

        public static void SplitRegion(Rect bounds, Size specifiedContent, AnchorStyles region1Align, out Rect region1, out Rect region2)
        {
            region1 = region2 = bounds;
            switch (region1Align)
            {
                case AnchorStyles.Top:
                    region1.Height = specifiedContent.Height;
                    region2.Y = region2.Y + specifiedContent.Height;
                    region2.Height = region2.Height - specifiedContent.Height;
                    return;

                case AnchorStyles.Bottom:
                    region1.Y = region1.Y + (bounds.Height - specifiedContent.Height);
                    region1.Height = specifiedContent.Height;
                    region2.Height = region2.Height - specifiedContent.Height;
                    return;

                case (AnchorStyles.Bottom | AnchorStyles.Top):
                case (AnchorStyles.Left | AnchorStyles.Top):
                case (AnchorStyles.Left | AnchorStyles.Bottom):
                case (AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Top):
                    break;

                case AnchorStyles.Left:
                    region1.Width = specifiedContent.Width;
                    region2.X = region2.X + specifiedContent.Width;
                    region2.Width = region2.Width - specifiedContent.Width;
                    return;

                case AnchorStyles.Right:
                    region1.X = region1.X + (bounds.Width - specifiedContent.Width);
                    region1.Width = specifiedContent.Width;
                    region2.Width = region2.Width - specifiedContent.Width;
                    break;

                default:
                    return;
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

        private void TextAboveImage(ref Size imageSize, ref Size textSize, ref double imageY, ref double textY)
        {
            double num = (base.ActualHeight - this.Layout.Margin.Top) - this.Layout.Margin.Bottom;
            switch (this.VerticalImageAlignment)
            {
                case VerticalAlignment.Top:
                case VerticalAlignment.Stretch:
                    textY = 0.0;
                    if ((this.VerticalTextAlignment != VerticalAlignment.Top) && (this.VerticalTextAlignment != VerticalAlignment.Stretch))
                    {
                        if (this.VerticalTextAlignment == VerticalAlignment.Center)
                        {
                            textY = (num / 4.0) - (textSize.Height / 2.0);
                            if (textY < 0.0)
                            {
                                textY = 0.0;
                            }
                            imageY = ((2.0 + (num / 2.0)) + (textSize.Height / 2.0)) - (imageSize.Height / 2.0);
                            return;
                        }
                        if (this.VerticalTextAlignment != VerticalAlignment.Bottom)
                        {
                            break;
                        }
                        textY = ((num / 2.0) - (textSize.Height / 2.0)) - (imageSize.Height / 2.0);
                        if (textY < 0.0)
                        {
                            textY = 0.0;
                        }
                        imageY = (textY + textSize.Height) + 2.0;
                        return;
                    }
                    imageY = 2.0 + textSize.Height;
                    return;

                case VerticalAlignment.Center:
                    if ((this.VerticalTextAlignment != VerticalAlignment.Stretch) && (this.VerticalTextAlignment != VerticalAlignment.Top))
                    {
                        if (this.VerticalTextAlignment == VerticalAlignment.Center)
                        {
                            textY = (num / 4.0) - (textSize.Height / 2.0);
                            if (textY < 0.0)
                            {
                                textY = 0.0;
                            }
                            imageY = ((num / 2.0) + (num / 4.0)) - (imageSize.Height / 2.0);
                            return;
                        }
                        if (this.VerticalTextAlignment != VerticalAlignment.Bottom)
                        {
                            break;
                        }
                        textY = (num / 2.0) - textSize.Height;
                        if (textY < 0.0)
                        {
                            textY = 0.0;
                        }
                        imageY = ((num / 2.0) + (num / 4.0)) - (imageSize.Height / 2.0);
                        return;
                    }
                    textY = 0.0;
                    imageY = (num / 2.0) - (imageSize.Height / 2.0);
                    return;

                case VerticalAlignment.Bottom:
                    if (this.VerticalTextAlignment == VerticalAlignment.Top)
                    {
                        textY = 0.0;
                        imageY = num - imageSize.Height;
                    }
                    if (this.VerticalTextAlignment != VerticalAlignment.Center)
                    {
                        if ((this.VerticalTextAlignment == VerticalAlignment.Stretch) || (this.VerticalTextAlignment == VerticalAlignment.Bottom))
                        {
                            textY = ((num - textSize.Height) - imageSize.Height) - 2.0;
                            if (textY < 0.0)
                            {
                                textY = 0.0;
                            }
                            imageY = (textY + 2.0) + textSize.Height;
                        }
                        break;
                    }
                    textY = (num / 2.0) - (textSize.Height / 2.0);
                    if (textY < 0.0)
                    {
                        textY = 0.0;
                    }
                    imageY = num - imageSize.Height;
                    return;

                default:
                    return;
            }
        }

        private void TextBeforeImage(ref Size imageSize, ref Size textSize, ref double imageX, ref double textX)
        {
            double num = (base.ActualWidth - this.Layout.Margin.Left) - this.Layout.Margin.Right;
            switch (this.HorizontalImageAlignment)
            {
                case HorizontalAlignment.Left:
                case HorizontalAlignment.Stretch:
                    if ((this.HorizontalTextAlignment != HorizontalAlignment.Left) && (this.HorizontalTextAlignment != HorizontalAlignment.Stretch))
                    {
                        if (this.HorizontalTextAlignment == HorizontalAlignment.Center)
                        {
                            textX = (num / 4.0) - (textSize.Width / 2.0);
                            if (textX < 0.0)
                            {
                                textX = 0.0;
                            }
                            imageX = (num / 2.0) - (imageSize.Width / 2.0);
                            return;
                        }
                        if (this.HorizontalTextAlignment != HorizontalAlignment.Right)
                        {
                            break;
                        }
                        textX = (num / 2.0) - (textSize.Width / 2.0);
                        if (textX < 0.0)
                        {
                            textX = 0.0;
                        }
                        imageX = (textX + textSize.Width) + 2.0;
                        return;
                    }
                    textX = 0.0;
                    imageX = textSize.Width + 2.0;
                    return;

                case HorizontalAlignment.Center:
                    if (this.HorizontalTextAlignment != HorizontalAlignment.Left)
                    {
                        if (this.HorizontalTextAlignment == HorizontalAlignment.Center)
                        {
                            textX = (num / 4.0) - (textSize.Width / 2.0);
                            if (textX < 0.0)
                            {
                                textX = 0.0;
                            }
                            imageX = ((num / 2.0) + (num / 4.0)) - (imageSize.Width / 2.0);
                            return;
                        }
                        textX = ((num / 2.0) - (textSize.Width / 2.0)) - imageSize.Width;
                        if (textX < 0.0)
                        {
                            textX = 0.0;
                        }
                        imageX = ((num / 2.0) + (num / 4.0)) - (imageSize.Width / 2.0);
                        return;
                    }
                    textX = 0.0;
                    if (textX < 0.0)
                    {
                        textX = 0.0;
                    }
                    imageX = num / 2.0;
                    return;

                case HorizontalAlignment.Right:
                    if ((this.HorizontalTextAlignment != HorizontalAlignment.Right) && (this.HorizontalTextAlignment != HorizontalAlignment.Stretch))
                    {
                        if (this.HorizontalTextAlignment == HorizontalAlignment.Left)
                        {
                            textX = 0.0;
                            imageX = num - imageSize.Width;
                            return;
                        }
                        if (this.HorizontalTextAlignment == HorizontalAlignment.Center)
                        {
                            textX = ((num / 2.0) - (textSize.Width / 2.0)) - imageSize.Width;
                            if (textX < 0.0)
                            {
                                textX = 0.0;
                            }
                            imageX = num - imageSize.Width;
                        }
                        break;
                    }
                    textX = ((num - imageSize.Width) - textSize.Width) - 2.0;
                    imageX = (textX + textSize.Width) + 2.0;
                    return;

                default:
                    return;
            }
        }

        private static void TextImageRelationPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Button b = d as Button;
            if (((b != null) && (b.ImageControl != null)) && (b.TextBlock != null))
            {
                b.ApplyTextImageRelation(b);
            }
        }

        private static void TextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Button b = d as Button;
            if (b != null)
            {
                b.ApplyTextImageRelation(b);
            }
        }

        public static Size UnionSizes(Size a, Size b) =>
            new Size(Math.Max(a.Width, b.Width), Math.Max(a.Height, b.Height));

        private double VAlign(Size alignThis, Size withinThis, VerticalAlignment align)
        {
            double num = 0.0;
            if (VerticalAlignment.Bottom == align)
            {
                return (num + (withinThis.Height - alignThis.Height));
            }
            if (VerticalAlignment.Center == align)
            {
                num += (withinThis.Height - alignThis.Height) / 2.0;
            }
            return num;
        }

        private static void VerticalImageAlignmentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Button b = d as Button;
            if (b != null)
            {
                b.ApplyTextImageRelation(b);
            }
        }

        private static void VerticalTextAlignmentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Button b = d as Button;
            if (b != null)
            {
                b.ApplyTextImageRelation(b);
            }
        }

        // Properties
        [Description("Gets or sets the background shadow Visibility."), Category("Appearance")]
        public Visibility BackgroundShadowVisibility
        {
            get =>
                ((Visibility)base.GetValue(BackgroundShadowVisibilityProperty));
            set =>
                base.SetValue(BackgroundShadowVisibilityProperty, value);
        }

        public Size ImageSize
        {
            get =>
                this.imageSize;
            set
            {
                this.imageSize = value;
                if (((value.Width > 8.0) && (value.Height > 8.0)) && (this.ImageControl != null))
                {
                    this.ImageControl.Width = value.Width;
                    this.ImageControl.Height = value.Height;
                }
            }
        }

        public Grid Layout { get; internal set; }

        public Image ImageControl { get; internal set; }

        public TextBlock TextBlock { get; internal set; }

        public TextImageRelation TextImageRelation
        {
            get =>
                ((TextImageRelation)base.GetValue(TextImageRelationProperty));
            set =>
                base.SetValue(TextImageRelationProperty, value);
        }

        public HorizontalAlignment HorizontalTextAlignment
        {
            get =>
                ((HorizontalAlignment)base.GetValue(HorizontalTextAlignmentProperty));
            set =>
                base.SetValue(HorizontalTextAlignmentProperty, value);
        }

        public VerticalAlignment VerticalTextAlignment
        {
            get =>
                ((VerticalAlignment)base.GetValue(VerticalTextAlignmentProperty));
            set =>
                base.SetValue(VerticalTextAlignmentProperty, value);
        }

        public HorizontalAlignment HorizontalImageAlignment
        {
            get =>
                ((HorizontalAlignment)base.GetValue(HorizontalImageAlignmentProperty));
            set =>
                base.SetValue(HorizontalImageAlignmentProperty, value);
        }

        public VerticalAlignment VerticalImageAlignment
        {
            get =>
                ((VerticalAlignment)base.GetValue(VerticalImageAlignmentProperty));
            set =>
                base.SetValue(VerticalImageAlignmentProperty, value);
        }

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
