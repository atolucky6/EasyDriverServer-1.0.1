using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace EasyScada.Core.Designer
{
    public sealed class ItemsControlHelper
    {
        // Fields
        private Panel itemsHost;
        private ScrollViewer _scrollHost;

        // Methods
        public ItemsControlHelper(ItemsControl control)
        {
            this.ItemsControl = control;
        }

        public void OnApplyTemplate()
        {
            this.itemsHost = null;
        }

        public static void PrepareContainerForItemOverride(DependencyObject element, Style parentItemContainerStyle)
        {
            Control control = element as Control;
            if (((parentItemContainerStyle != null) && (control != null)) && (control.Style == null))
            {
                control.SetValue(FrameworkElement.StyleProperty, parentItemContainerStyle);
            }
        }

        public void ScrollIntoView(FrameworkElement element)
        {
            ScrollViewer scrollHost = this.ScrollHost;
            if (scrollHost != null)
            {
                GeneralTransform transform = null;
                try
                {
                    transform = element.TransformToVisual(scrollHost);
                }
                catch (ArgumentException)
                {
                    return;
                }
                Rect rect = new Rect(transform.Transform(new Point()), transform.Transform(new Point(element.ActualWidth, element.ActualHeight)));
                double verticalOffset = scrollHost.VerticalOffset;
                double num2 = 0.0;
                double viewportHeight = scrollHost.ViewportHeight;
                double num4 = rect.Bottom;
                if (viewportHeight < num4)
                {
                    num2 = num4 - viewportHeight;
                    verticalOffset += num2;
                }
                double num5 = rect.Top;
                if ((num5 - num2) < 0.0)
                {
                    verticalOffset -= num2 - num5;
                }
                scrollHost.ScrollToVerticalOffset(verticalOffset);
                double horizontalOffset = scrollHost.HorizontalOffset;
                double num7 = 0.0;
                double viewportWidth = scrollHost.ViewportWidth;
                double num9 = rect.Right;
                if (viewportWidth < num9)
                {
                    num7 = num9 - viewportWidth;
                    horizontalOffset += num7;
                }
                double num10 = rect.Left;
                if ((num10 - num7) < 0.0)
                {
                    horizontalOffset -= num7 - num10;
                }
                scrollHost.ScrollToHorizontalOffset(horizontalOffset);
            }
        }

        public void UpdateItemContainerStyle(Style itemContainerStyle)
        {
            if (itemContainerStyle != null)
            {
                Panel itemsHost = this.ItemsHost;
                if ((itemsHost != null) && (itemsHost.Children != null))
                {
                    foreach (UIElement element in itemsHost.Children)
                    {
                        FrameworkElement element2 = element as FrameworkElement;
                        if (element2.Style == null)
                        {
                            element2.Style = itemContainerStyle;
                        }
                    }
                }
            }
        }

        // Properties
        private ItemsControl ItemsControl { get; set; }

        internal Panel ItemsHost
        {
            get
            {
                if (((this.itemsHost == null) && (this.ItemsControl != null)) && (this.ItemsControl.ItemContainerGenerator != null))
                {
                    DependencyObject reference = this.ItemsControl.ItemContainerGenerator.ContainerFromIndex(0);
                    if (reference != null)
                    {
                        this.itemsHost = VisualTreeHelper.GetParent(reference) as Panel;
                    }
                }
                return this.itemsHost;
            }
        }

        internal ScrollViewer ScrollHost
        {
            get
            {
                if (this._scrollHost == null)
                {
                    Panel itemsHost = this.ItemsHost;
                    if (itemsHost != null)
                    {
                        for (DependencyObject obj2 = itemsHost; (obj2 != this.ItemsControl) && (obj2 != null); obj2 = VisualTreeHelper.GetParent(obj2))
                        {
                            ScrollViewer viewer = obj2 as ScrollViewer;
                            if (viewer != null)
                            {
                                this._scrollHost = viewer;
                                break;
                            }
                        }
                    }
                }
                return this._scrollHost;
            }
        }
    }
}
