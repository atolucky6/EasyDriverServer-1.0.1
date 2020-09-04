using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Threading;

namespace EasyScada.Core.Designer
{
    [
        TemplatePart(Name = "TabControlHeaderLayout", Type = typeof(Grid)), Description("Represents a control that enables the arrangement of a visual content in a compacted and organized form."), 
        TemplatePart(Name = "TabControlDockPanel", Type = typeof(DockPanel)), 
        TemplatePart(Name = "HeaderBackground", Type = typeof(Border)), TemplatePart(Name = "ScrollViewer", Type = typeof(AnimatedScrollViewer)), 
        TemplatePart(Name = "ControlBorder", Type = typeof(Border))
    ]
    [ContentProperty("Items")]
    [DefaultEvent("OnItemsChanged")]
    [DefaultProperty("Items")]
    [Localizability(LocalizationCategory.None, Readability = Readability.Unreadable)]
    [StyleTypedProperty(Property = "ItemContainerStyle", StyleTargetType = typeof(FrameworkElement))]
    public class TabControl : ItemsControl
    {
        static TabControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TabControl), new FrameworkPropertyMetadata(typeof(TabControl)));
        }


        // Fields
        private const string DockPanelPrimaryName = "TabControlDockPanel";
        private const string HeaderBackgroundName = "HeaderBackground";
        private const string ScrollViewerName = "ScrollViewer";
        private const string TabControlBorderName = "ControlBorder";
        private const string TabControlHeaderLayoutName = "TabControlHeaderLayout";
        private StackPanel layoutPanel;
        private AnimatedScrollViewer scrollViewer;
        private Border headerBackground;
        internal DockPanel dockPanel;
        private Border tabControlBorder;
        private bool allowFadeEffect;
        private Grid tabControlHeaderLayout;
        private bool isKeyboardNavigationEnabled = true;
        public static readonly DependencyProperty VerticalScrollbarHorizontalAlignmentProperty = DependencyProperty.Register("VerticalScrollbarHorizontalAlignment", typeof(HorizontalAlignment), typeof(TabControl), new PropertyMetadata(HorizontalAlignment.Center));
        public static readonly DependencyProperty HorizontalScrollbarVerticalAlignmentProperty = DependencyProperty.Register("HorizontalScrollbarVerticalAlignment", typeof(VerticalAlignment), typeof(TabControl), new PropertyMetadata(VerticalAlignment.Center));
        public static readonly DependencyProperty HeaderBackgroundProperty = 
            DependencyProperty.Register("HeaderBackground", typeof(Brush), typeof(TabControl), new PropertyMetadata(null, HeaderBackgroundPropertyChanged));
        public static readonly DependencyProperty HeaderBorderBrushProperty = 
            DependencyProperty.Register("HeaderBorderBrush", typeof(Brush), typeof(TabControl), new PropertyMetadata(null, HeaderBorderBrushPropertyChanged));
        public static readonly DependencyProperty ContentBorderBrushProperty = 
            DependencyProperty.Register("ContentBorderBrush", typeof(Brush), typeof(TabControl), new PropertyMetadata(null, ContentBorderBrushPropertyChanged));
        public static readonly DependencyProperty ContentBorderThicknessProperty = 
            DependencyProperty.Register("ContentBorderThickness", typeof(Thickness), typeof(TabControl), new PropertyMetadata(new Thickness(0.0), ContentBorderThicknessPropertyChanged));
        public static readonly DependencyProperty HeaderBorderThicknessProperty = 
            DependencyProperty.Register("HeaderBorderThickness", typeof(Thickness), typeof(TabControl), new PropertyMetadata(new Thickness(0.0), HeaderBorderThicknessPropertyChanged));
        private DispatcherTimer opacityTimer = new DispatcherTimer();
        public static readonly DependencyProperty ScrollStepProperty = 
            DependencyProperty.Register("ScrollStep", typeof(double), typeof(TabControl), new PropertyMetadata(60.0));
        public static readonly DependencyProperty SelectedItemProperty = 
            DependencyProperty.Register("SelectedItem", typeof(object), typeof(TabControl), new PropertyMetadata(null, OnSelectedItemPropertyChanged));
        public static readonly DependencyProperty SelectedIndexProperty = 
            DependencyProperty.Register("SelectedIndex", typeof(int), typeof(TabControl), new PropertyMetadata(-1, OnSelectedIndexPropertyChanged));
        public static readonly DependencyProperty TabItemTemplateTemplateProperty = 
            DependencyProperty.Register("TabItemTemplate", typeof(DataTemplate), typeof(TabControl), new PropertyMetadata(null));
        public new static readonly DependencyProperty ItemContainerStyleProperty = 
            DependencyProperty.Register("ItemContainerStyle", typeof(Style), typeof(TabControl), new PropertyMetadata(null, OnItemContainerStylePropertyChanged));
        public static readonly DependencyProperty TabAlignmentProperty = 
            DependencyProperty.Register("TabAlignment", typeof(Dock), typeof(TabControl), new PropertyMetadata(Dock.Top, OnTabAlignmentPropertyChanged));
        public static readonly DependencyProperty TabOrientationProperty = 
            DependencyProperty.Register("TabOrientation", typeof(Orientation), typeof(TabControl), new PropertyMetadata(Orientation.Horizontal, OnTabOrientationPropertyChanged));
        public static readonly DependencyProperty CornerRadiusProperty = 
            DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(TabControl), new PropertyMetadata(new CornerRadius(3.0)));
        public static readonly DependencyProperty ContentTemplateProperty = 
            DependencyProperty.Register("ContentTemplate", typeof(DataTemplate), typeof(TabControl), new PropertyMetadata(null, OnContentTemplatePropertyChanged));
        public static readonly DependencyProperty ContentProperty = 
            DependencyProperty.Register("Content", typeof(object), typeof(TabControl), new PropertyMetadata(null, OnContentPropertyChanged));
        private bool startOpacityTimer;
        private double opacity = 0.7;
        private double faceOpacity = 0.7;

        // Events
        [Description("Occurs when the selection is changed."), Category("Action")]
        public event SelectionChangedEventHandler SelectionChanged;

        [Category("Action"), Description("Occurs when tab alignment is changed.")]
        public event EventHandler TabAlignmentChanged;

        [Description("Occurs when tab orientation is changed."), Category("Action")]
        public event EventHandler TabOrientationChanged;

        // Methods
        public TabControl()
        {
            this.isKeyboardNavigationEnabled = true;
            this.opacityTimer = new DispatcherTimer();
            this.opacity = 0.7;
            this.faceOpacity = 0.7;
            base.DefaultStyleKey = typeof(TabControl);
            this.ItemsControlHelper = new ItemsControlHelper(this);
            this.opacityTimer.Interval = new TimeSpan(0, 0, 0, 0, 1);
            this.opacityTimer.Tick += opacityTimer_Tick;
        }

        public static void ApplyContent(TabControl tabControl)
        {
            if (tabControl != null)
            {
                TabItem selectedItem = tabControl.SelectedItem as TabItem;
                for (int i = 0; i < tabControl.Items.Count; i++)
                {
                    TabItem container = tabControl.ItemContainerGenerator.ContainerFromIndex(i) as TabItem;
                    if (container != null)
                    {
                        if (container != selectedItem)
                        {
                            if ((container.IsSelected.HasValue && container.IsSelected.Value) && (container.LayoutRoot != null))
                            {
                                if (tabControl.Content == container.contentPresenter)
                                {
                                    tabControl.Content = null;
                                }
                                container.LayoutRoot.Children.Add(container.contentPresenter);
                                container.contentPresenter.Visibility = Visibility.Collapsed;
                            }
                            container.IsSelected = false;
                        }
                        else if (container.LayoutRoot != null)
                        {
                            container.LayoutRoot.Children.Remove(container.contentPresenter);
                            container.IsSelected = true;
                            tabControl.Content = container.contentPresenter;
                            container.contentPresenter.Visibility = Visibility.Visible;
                            int num2 = tabControl.ItemContainerGenerator.IndexFromContainer(container);
                            if (num2 != tabControl.SelectedIndex)
                            {
                                tabControl.SelectedIndex = num2;
                            }
                        }
                    }
                }
            }
        }

        public static void ApplySelectedIndex(TabControl tabControl)
        {
            if ((tabControl != null) && (tabControl.ItemContainerGenerator != null))
            {
                if ((tabControl.SelectedIndex >= 0) && (tabControl.SelectedIndex < tabControl.Items.Count))
                {
                    TabItem item = tabControl.ItemContainerGenerator.ContainerFromIndex(tabControl.SelectedIndex) as TabItem;
                    if ((item != null) && (tabControl.SelectedItem != item))
                    {
                        tabControl.SelectedItem = item;
                    }
                }
                else
                {
                    tabControl.SelectedItem = null;
                }
            }
        }

        private static void ApplyTabAlignment(TabControl tabControl)
        {
            if ((tabControl != null) && (tabControl.layoutPanel != null))
            {
                if ((tabControl.TabAlignment == Dock.Top) || (tabControl.TabAlignment == Dock.Bottom))
                {
                    tabControl.layoutPanel.Orientation = Orientation.Horizontal;
                }
                else
                {
                    tabControl.layoutPanel.Orientation = Orientation.Vertical;
                }
                switch (tabControl.TabAlignment)
                {
                    case Dock.Left:
                    case Dock.Right:
                        tabControl.ScrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
                        tabControl.ScrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
                        return;

                    case Dock.Top:
                    case Dock.Bottom:
                        tabControl.ScrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
                        tabControl.ScrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
                        return;
                }
            }
        }

        protected override void ClearContainerForItemOverride(DependencyObject element, object item)
        {
            TabItem item2 = element as TabItem;
            if (item2 != null)
            {
                item2.ParentItemsControl = null;
            }
            base.ClearContainerForItemOverride(element, item);
        }

        private static void ContentBorderBrushPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabControl control = d as TabControl;
        }

        private static void ContentBorderThicknessPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabControl control = d as TabControl;
        }

        public void EnsureVisible(FrameworkElement element)
        {
            if (this.ScrollViewer != null)
            {
                GeneralTransform transform = null;
                try
                {
                    transform = element.TransformToVisual(this.ScrollViewer);
                }
                catch (ArgumentException)
                {
                    return;
                }
                Rect rect = new Rect(transform.Transform(new Point()), transform.Transform(new Point(element.ActualWidth, element.ActualHeight)));
                double verticalOffset = this.ScrollViewer.VerticalOffset;
                double num2 = 0.0;
                double viewportHeight = this.ScrollViewer.ViewportHeight;
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
                this.ScrollViewer.ElementScrollContentPresenter.StopTimers();
                this.ScrollViewer.ScrollToVerticalOffset(verticalOffset);
                double horizontalOffset = this.ScrollViewer.HorizontalOffset;
                double num7 = 0.0;
                double viewportWidth = this.ScrollViewer.ViewportWidth;
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
                this.ScrollViewer.ScrollToHorizontalOffset(horizontalOffset);
            }
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            TabItem item = new TabItem();
            if (this.ItemContainerStyle != null)
            {
                item.Style = this.ItemContainerStyle;
            }
            return item;
        }

        private static void HeaderBackgroundPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabControl control = d as TabControl;
        }

        private static void HeaderBorderBrushPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabControl control = d as TabControl;
        }

        private static void HeaderBorderThicknessPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabControl control = d as TabControl;
        }

        protected override bool IsItemItsOwnContainerOverride(object item) =>
            (item is TabItem);

        private void itemsControl_Loaded(object sender, RoutedEventArgs e)
        {
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.dockPanel = base.GetTemplateChild("TabControlDockPanel") as DockPanel;
            this.headerBackground = base.GetTemplateChild("HeaderBackground") as Border;
            this.tabControlBorder = base.GetTemplateChild("ControlBorder") as Border;
            this.scrollViewer = base.GetTemplateChild("ScrollViewer") as AnimatedScrollViewer;
            this.tabControlHeaderLayout = base.GetTemplateChild("TabControlHeaderLayout") as Grid;
            if (this.scrollViewer != null)
            {
                this.scrollViewer.MouseEnter += scrollViewer_MouseEnter;
                this.scrollViewer.MouseLeave += scrollViewer_MouseLeave;
            }
            base.AddHandler(UIElement.KeyDownEvent, new KeyEventHandler(this.TabControl_KeyDown), true);
            base.KeyDown += TabControl_KeyDown;
            ApplyTabAlignment(this);
            if (this.AllowFadeEffect)
            {
                this.SetOpacity(this.faceOpacity);
            }
            if (this.dockPanel != null)
            {
                this.dockPanel.InvalidateMeasure();
                this.dockPanel.InvalidateArrange();
                this.dockPanel.UpdateLayout();
            }
        }

        private static void OnContentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabControl control = d as TabControl;
        }

        private static void OnContentTemplatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabControl control = d as TabControl;
        }

        private static void OnItemContainerStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabControl control = d as TabControl;
            Style itemContainerStyle = e.NewValue as Style;
            control.ItemsControlHelper.UpdateItemContainerStyle(itemContainerStyle);
        }

        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);
            if (e.NewItems != null)
            {
                int count = e.NewItems.Count;
                for (int i = 0; i < count; i++)
                {
                    TabItem item = e.NewItems[i] as TabItem;
                    if (item != null)
                    {
                        item.ParentItemsControl = this;
                    }
                }
            }
        }

        private static void OnSelectedIndexPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabControl control = d as TabControl;
            if (control != null)
            {
                ApplySelectedIndex(control);
            }
        }

        private static void OnSelectedItemPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabControl tabControl = d as TabControl;
            if (tabControl != null)
            {
                ApplyContent(tabControl);
                List<TabItem> removedItems = new List<TabItem>();
                List<TabItem> addedItems = new List<TabItem>();
                removedItems.Add(e.OldValue as TabItem);
                addedItems.Add(e.NewValue as TabItem);
                tabControl.OnSelectionChanged(new SelectionChangedEventArgs(UIElement.MouseDownEvent, removedItems, addedItems));
            }
        }

        protected virtual void OnSelectionChanged(SelectionChangedEventArgs args)
        {
            if (this.SelectionChanged != null)
            {
                this.SelectionChanged(this, args);
            }
        }

        protected virtual void OnTabAlignmentChanged()
        {
            if (this.TabAlignmentChanged != null)
            {
                this.TabAlignmentChanged(this, EventArgs.Empty);
            }
        }

        private static void OnTabAlignmentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabControl tabControl = d as TabControl;
            if (tabControl != null)
            {
                ApplyTabAlignment(tabControl);
                tabControl.OnTabAlignmentChanged();
            }
        }

        protected virtual void OnTabOrientationChanged()
        {
            if (this.TabOrientationChanged != null)
            {
                this.TabOrientationChanged(this, EventArgs.Empty);
            }
        }

        private static void OnTabOrientationPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabControl control = d as TabControl;
            if (control != null)
            {
                control.OnTabOrientationChanged();
            }
        }

        private void opacityTimer_Tick(object sender, EventArgs e)
        {
            if (!this.AllowFadeEffect)
            {
                this.opacityTimer.Stop();
            }
            if (this.startOpacityTimer)
            {
                if (this.opacity < 1.0)
                {
                    this.opacity += 0.01;
                    this.SetOpacity(this.opacity);
                }
                else
                {
                    this.opacityTimer.Stop();
                }
            }
            else if (this.opacity >= this.faceOpacity)
            {
                this.opacity -= 0.01;
                this.SetOpacity(this.opacity);
            }
            else
            {
                this.opacityTimer.Stop();
            }
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            TabItem item2 = element as TabItem;
            if (item2 != null)
            {
                item2.ParentItemsControl = this;
                if (item.GetType() != typeof(TabItem))
                {
                    item2.Header = item;
                    item2.HeaderTemplate = this.TabItemTemplate;
                }
            }
            if (this.layoutPanel == null)
            {
                this.layoutPanel = VisualTreeHelper.GetParent(element) as StackPanel;
                ApplyTabAlignment(this);
            }
            ItemsControlHelper.PrepareContainerForItemOverride(element, this.ItemContainerStyle);
            if (base.ItemTemplate is HierarchicalDataTemplate)
            {
                HeaderedItemsControl control = null;
                if (item2 != null)
                {
                    control = new HeaderedItemsControl();
                    item2.Content = control;
                    control.Loaded += new RoutedEventHandler(this.itemsControl_Loaded);
                }
            }
            ApplySelectedIndex(this);
            ApplyContent(this);
        }

        private void scrollViewer_MouseEnter(object sender, MouseEventArgs e)
        {
            this.startOpacityTimer = true;
            this.opacityTimer.Start();
        }

        private void scrollViewer_MouseLeave(object sender, MouseEventArgs e)
        {
            this.startOpacityTimer = false;
            this.opacityTimer.Start();
        }

        public bool SelectNextItem()
        {
            for (int i = this.SelectedIndex + 1; i < base.Items.Count; i++)
            {
                TabItem element = base.ItemContainerGenerator.ContainerFromIndex(i) as TabItem;
                if ((element == null) && (i < base.Items.Count))
                {
                    element = base.Items[i] as TabItem;
                }
                if (((element != null) && element.IsEnabled) && (element.Visibility == Visibility.Visible))
                {
                    this.SelectedItem = element;
                    this.EnsureVisible(element);
                    return true;
                }
            }
            return false;
        }

        public bool SelectPrevItem()
        {
            for (int i = this.SelectedIndex - 1; i >= 0; i--)
            {
                TabItem element = base.ItemContainerGenerator.ContainerFromIndex(i) as TabItem;
                if ((element == null) && (i < base.Items.Count))
                {
                    element = base.Items[i] as TabItem;
                }
                if (((element != null) && element.IsEnabled) && (element.Visibility == Visibility.Visible))
                {
                    this.SelectedItem = element;
                    this.EnsureVisible(element);
                    break;
                }
            }
            return false;
        }

        public void SetCloseButtonVisibility(Visibility visibility)
        {
            for (int i = 0; i < base.Items.Count; i++)
            {
                TabItem item = base.ItemContainerGenerator.ContainerFromIndex(i) as TabItem;
                if (item != null)
                {
                    item.CloseButtonVisibility = visibility;
                }
            }
        }

        protected virtual void SetOpacity(double opacity)
        {
            if (this.AllowFadeEffect)
            {
                for (int i = 0; i < base.Items.Count; i++)
                {
                    TabItem item = base.ItemContainerGenerator.ContainerFromIndex(i) as TabItem;
                    if (((item != null) && (!item.IsSelected.HasValue || !item.IsSelected.Value)) && (item != null))
                    {
                        item.Opacity = opacity;
                    }
                }
            }
        }

        private void TabControl_KeyDown(object sender, KeyEventArgs e)
        {
            TabItem focusedElement = FocusManager.GetFocusedElement(Window.GetWindow(this)) as TabItem;
            TabControl control = FocusManager.GetFocusedElement(Window.GetWindow(this)) as TabControl;
            if (((focusedElement != null) || (control != null)) && this.isKeyboardNavigationEnabled)
            {
                switch (this.TabAlignment)
                {
                    case Dock.Left:
                    case Dock.Right:
                        if (e.Key != (Key)0x1a)
                        {
                            if (e.Key == (Key)0x18)
                            {
                                this.SelectPrevItem();
                            }
                            return;
                        }
                        this.SelectNextItem();
                        return;

                    case Dock.Top:
                    case Dock.Bottom:
                        if (e.Key != (Key)0x19)
                        {
                            if (e.Key == (Key)0x17)
                            {
                                this.SelectPrevItem();
                            }
                            return;
                        }
                        this.SelectNextItem();
                        return;
                }
            }
        }

        // Properties
        [Category("Behavior"), Description("Gets or sets a value indicating whether this instance is keyboard navigation enabled.")]
        public bool IsKeyboardNavigationEnabled
        {
            get => this.isKeyboardNavigationEnabled;
            set => this.isKeyboardNavigationEnabled = value;
        }

        [Browsable(false)]
        public HorizontalAlignment VerticalScrollbarHorizontalAlignment
        {
            get =>
                ((HorizontalAlignment)base.GetValue(VerticalScrollbarHorizontalAlignmentProperty));
            set =>
                base.SetValue(VerticalScrollbarHorizontalAlignmentProperty, value);
        }

        [Browsable(false)]
        public VerticalAlignment HorizontalScrollbarVerticalAlignment
        {
            get =>
                ((VerticalAlignment)base.GetValue(HorizontalScrollbarVerticalAlignmentProperty));
            set =>
                base.SetValue(HorizontalScrollbarVerticalAlignmentProperty, value);
        }

        [Category("Appearance"), Description("Gets or sets the header background.")]
        public Brush HeaderBackground
        {
            get =>
                ((Brush)base.GetValue(HeaderBackgroundProperty));
            set =>
                base.SetValue(HeaderBackgroundProperty, value);
        }

        [Description("Gets or sets the header border brush."), Category("Appearance")]
        public Brush HeaderBorderBrush
        {
            get =>
                ((Brush)base.GetValue(HeaderBorderBrushProperty));
            set =>
                base.SetValue(HeaderBorderBrushProperty, value);
        }

        [Description("Gets or sets the Content border brush."), Category("Appearance")]
        public Brush ContentBorderBrush
        {
            get =>
                ((Brush)base.GetValue(ContentBorderBrushProperty));
            set =>
                base.SetValue(ContentBorderBrushProperty, value);
        }

        [Description("Gets or sets the Content border thickness."), Category("Appearance")]
        public Thickness ContentBorderThickness
        {
            get =>
                ((Thickness)base.GetValue(ContentBorderThicknessProperty));
            set =>
                base.SetValue(ContentBorderThicknessProperty, value);
        }

        [Description("Gets or sets the header border thickness."), Category("Appearance")]
        public Thickness HeaderBorderThickness
        {
            get =>
                ((Thickness)base.GetValue(HeaderBorderThicknessProperty));
            set =>
                base.SetValue(HeaderBorderThicknessProperty, value);
        }

        [Description("Gets or sets a value indicating whether the fade effect is allowed."), Category("Behavior")]
        public bool AllowFadeEffect
        {
            get =>
                this.allowFadeEffect;
            set
            {
                this.allowFadeEffect = value;
                if (!value)
                {
                    this.SetOpacity(1.0);
                }
            }
        }

        [Browsable(false)]
        public AnimatedScrollViewer ScrollViewer =>
            this.scrollViewer;

        [Description("Gets or sets the scroll step."), Browsable(false)]
        public double ScrollStep
        {
            get =>
                ((double)base.GetValue(ScrollStepProperty));
            set =>
                base.SetValue(ScrollStepProperty, value);
        }

        [Browsable(false), Description("Gets or sets the selected item."), Category("Behavior")]
        public object SelectedItem
        {
            get =>
                base.GetValue(SelectedItemProperty);
            set =>
                base.SetValue(SelectedItemProperty, value);
        }

        [Description("Gets or sets the selected index."), Category("Behavior"), Browsable(false)]
        public int SelectedIndex
        {
            get =>
                ((int)base.GetValue(SelectedIndexProperty));
            set =>
                base.SetValue(SelectedIndexProperty, value);
        }

        [Browsable(false)]
        public virtual DataTemplate TabItemTemplate
        {
            get =>
                ((DataTemplate)base.GetValue(TabItemTemplateTemplateProperty));
            set =>
                base.SetValue(TabItemTemplateTemplateProperty, value);
        }

        internal ItemsControlHelper ItemsControlHelper { get; private set; }

        [Browsable(false)]
        public Style ItemContainerStyle
        {
            get =>
                (base.GetValue(ItemContainerStyleProperty) as Style);
            set =>
                base.SetValue(ItemContainerStyleProperty, value);
        }

        [Category("Behavior"), Browsable(false), Description("Gets or sets the tab alignment.")]
        public Dock TabAlignment
        {
            get =>
                ((Dock)base.GetValue(TabAlignmentProperty));
            set =>
                base.SetValue(TabAlignmentProperty, value);
        }

        [Description("Gets or sets the tab orientation."), Browsable(false), Category("Behavior")]
        public Orientation TabOrientation
        {
            get =>
                ((Orientation)base.GetValue(TabOrientationProperty));
            set =>
                base.SetValue(TabOrientationProperty, value);
        }

        [Category("Behavior"), Description("Gets or sets the corner radius.")]
        public CornerRadius CornerRadius
        {
            get =>
                ((CornerRadius)base.GetValue(CornerRadiusProperty));
            set =>
                base.SetValue(CornerRadiusProperty, value);
        }

        [Browsable(false)]
        public DataTemplate ContentTemplate
        {
            get =>
                ((DataTemplate)base.GetValue(ContentTemplateProperty));
            set =>
                base.SetValue(ContentTemplateProperty, value);
        }

        [Browsable(false)]
        public object Content
        {
            get =>
                base.GetValue(ContentProperty);
            set =>
                base.SetValue(ContentProperty, value);
        }

        [Description("Gets or sets the fade effect opacity."), Browsable(false)]
        public double FadeOpacity
        {
            get => this.faceOpacity;
            set => this.faceOpacity = value;
        }
    }
}
