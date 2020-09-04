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
        TemplatePart(Name = "CloseButton", Type = typeof(Button)),
        TemplatePart(Name = "TabPositionElement", Type = typeof(TabPositionElement)), 
        TemplatePart(Name = "InnerLayoutRoot", Type = typeof(Grid)), 
        TemplatePart(Name = "LayoutRoot", Type = typeof(Grid)), 
        TemplatePart(Name = "LayoutStackPanel", Type = typeof(StackPanel))
    ]
    public class TabItem : TabItemBase
    {
        static TabItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TabItem), new FrameworkPropertyMetadata(typeof(TabItem)));
        }

        // Fields
        internal const string GroupSelect = "SelectStates";
        internal const string StateSelected = "Selected";
        internal const string StateUnselected = "Unselected";
        internal const string selectedHighlightName = "Backgoundselected_Highlight";
        internal const string selectedShadowName1 = "BackgroundselectedShadow";
        internal const string selectedShadowName2 = "BackgroundselectedInnerShadow";
        internal const string ItemContentPresenterName = "ItemContentPresenter";
        internal const string LayoutRootName = "LayoutRoot";
        internal const string LayoutStackPanelName = "LayoutStackPanel";
        internal const string CloseButtonName = "CloseButton";
        internal const string InnerLayoutRootName = "InnerLayoutRoot";
        internal const string HeaderPresenterName = "contentPresenter";
        internal const string HeaderLayoutName = "Layout";
        public const string TabPositionElementName = "TabPositionElement";
        internal ContentPresenter headerPresenter;
        internal Grid headerLayout;
        internal Border selectedShadow1;
        internal Border selectedShadow2;
        internal Border selectedHighlight;
        internal ContentPresenter contentPresenter;
        internal Grid LayoutRoot;
        internal StackPanel layoutStackPanel;
        internal Button closeButton;
        internal Grid InnerLayoutRoot;
        internal TabPositionElement tabPositionElement;
        public static readonly DependencyProperty ContentMarginProperty = 
            DependencyProperty.Register("ContentMargin", typeof(Thickness), typeof(TabItem), new PropertyMetadata(new Thickness(1.0), ContentMarginPropertyChanged));
        private Thickness borderThickness;
        private CornerRadius cornerRadius;
        public static readonly DependencyProperty HeaderTemplateProperty = 
            DependencyProperty.Register("HeaderTemplate", typeof(DataTemplate), typeof(TabItem), new PropertyMetadata(null));
        public static readonly DependencyProperty HeaderProperty = 
            DependencyProperty.Register("Header", typeof(object), typeof(TabItem), new PropertyMetadata(null));
        private ItemsControl parentItemsControl;
        public static readonly DependencyProperty AutoAdjustBorderThicknessProperty = 
            DependencyProperty.Register("AutoAdjustBorderThickness", typeof(bool), typeof(TabItem), new PropertyMetadata(true));
        public static readonly DependencyProperty CloseButtonVisibilityProperty = 
            DependencyProperty.Register("CloseButtonVisibility", typeof(Visibility), typeof(TabItem), new PropertyMetadata(Visibility.Collapsed, OnCloseButtonVisibilityPropertyChanged));
        private bool showCloseButtonInDefaultTabItemState;
        private Action closeButtonAction;
        public static readonly DependencyProperty TabOrientationProperty = 
            DependencyProperty.Register("TabOrientation", typeof(Orientation), typeof(TabItem), new PropertyMetadata(Orientation.Horizontal, OnTabOrientationPropertyChanged));
        public static readonly 
            DependencyProperty IsSelectedProperty = DependencyProperty.Register("IsSelected", typeof(bool?), typeof(TabItem), new PropertyMetadata(false, OnIsSelectedPropertyChanged));

        // Events
        [Category("Action")]
        public event EventHandler CloseButtonClick;

        [Category("Action")]
        public event EventHandler Selected;

        [Category("Action")]
        public event EventHandler TabItemStateChanged;

        [Category("Action")]
        public event EventHandler Unselected;

        // Methods
        public TabItem(object header) : this()
        {
            this.Header = header;
        }

        public TabItem(string text) : this()
        {
            base.Text = text;
        }

        public TabItem()
        {
            this.borderThickness = new Thickness();
            this.cornerRadius = new CornerRadius();
            base.DefaultStyleKey = typeof(TabItem);
            base.Loaded += new RoutedEventHandler(this.TabItem_Loaded);
        }

        private void ApplyTabAlignmentBorder()
        {
            if ((this.TabControl != null) && this.AutoAdjustBorderThickness)
            {
                double left = base.BorderThickness.Left;
                double right = base.BorderThickness.Right;
                double top = base.BorderThickness.Top;
                double bottom = base.BorderThickness.Bottom;
                switch (this.TabControl.TabAlignment)
                {
                    case Dock.Left:
                        if (this.TabOrientation != Orientation.Horizontal)
                        {
                            base.CornerRadius = new CornerRadius(this.cornerRadius.TopLeft, 0.0, 0.0, this.cornerRadius.BottomLeft);
                            base.BorderThickness = new Thickness(this.borderThickness.Left, this.borderThickness.Top, 0.0, this.borderThickness.Bottom);
                            return;
                        }
                        base.CornerRadius = new CornerRadius(this.cornerRadius.TopLeft, this.cornerRadius.TopRight, 0.0, 0.0);
                        base.BorderThickness = new Thickness(this.borderThickness.Left, this.borderThickness.Top, this.borderThickness.Right, 0.0);
                        return;

                    case Dock.Top:
                        if (this.TabOrientation != Orientation.Horizontal)
                        {
                            base.CornerRadius = new CornerRadius(this.cornerRadius.TopLeft, 0.0, 0.0, this.cornerRadius.BottomLeft);
                            base.BorderThickness = new Thickness(this.borderThickness.Left, this.borderThickness.Top, 0.0, this.borderThickness.Bottom);
                            return;
                        }
                        base.BorderThickness = new Thickness(this.borderThickness.Left, this.borderThickness.Top, this.borderThickness.Right, 0.0);
                        base.CornerRadius = new CornerRadius(this.cornerRadius.TopLeft, this.cornerRadius.TopRight, 0.0, 0.0);
                        return;

                    case Dock.Right:
                        if (this.TabOrientation != Orientation.Horizontal)
                        {
                            base.CornerRadius = new CornerRadius(0.0, this.cornerRadius.TopRight, this.cornerRadius.BottomRight, 0.0);
                            base.BorderThickness = new Thickness(0.0, this.borderThickness.Top, this.borderThickness.Right, this.borderThickness.Bottom);
                            return;
                        }
                        base.CornerRadius = new CornerRadius(this.cornerRadius.TopLeft, this.cornerRadius.TopRight, 0.0, 0.0);
                        base.BorderThickness = new Thickness(this.borderThickness.Left, this.borderThickness.Top, this.borderThickness.Right, 0.0);
                        return;

                    case Dock.Bottom:
                        if (this.TabOrientation != Orientation.Horizontal)
                        {
                            base.CornerRadius = new CornerRadius(this.cornerRadius.TopLeft, 0.0, 0.0, this.cornerRadius.BottomLeft);
                            base.BorderThickness = new Thickness(this.borderThickness.Left, this.borderThickness.Top, 0.0, this.borderThickness.Bottom);
                            return;
                        }
                        base.CornerRadius = new CornerRadius(0.0, 0.0, this.cornerRadius.BottomRight, this.cornerRadius.BottomLeft);
                        base.BorderThickness = new Thickness(this.borderThickness.Left, 0.0, this.borderThickness.Right, this.borderThickness.Bottom);
                        return;
                }
            }
        }

        protected virtual void ApplyTextImageRelation()
        {
            if (((base.ImageControl != null) && (base.TextBlock != null)) && (this.layoutStackPanel != null))
            {
                if (this.layoutStackPanel.Children.Contains(base.TextBlock))
                {
                    this.layoutStackPanel.Children.Remove(base.TextBlock);
                }
                else
                {
                    this.LayoutRoot.Children.Remove(base.TextBlock);
                }
                if (this.layoutStackPanel.Children.Contains(base.ImageControl))
                {
                    this.layoutStackPanel.Children.Remove(base.ImageControl);
                }
                else
                {
                    this.LayoutRoot.Children.Remove(base.ImageControl);
                }
                this.layoutStackPanel.VerticalAlignment = VerticalAlignment.Center;
                this.layoutStackPanel.HorizontalAlignment = HorizontalAlignment.Center;
                switch (base.TextImageRelation)
                {
                    case TextImageRelation.ImageBeforeText:
                        this.layoutStackPanel.Orientation = Orientation.Horizontal;
                        this.layoutStackPanel.Children.Add(base.ImageControl);
                        this.layoutStackPanel.Children.Add(base.TextBlock);
                        return;

                    case TextImageRelation.ImageAboveText:
                        this.layoutStackPanel.Orientation = Orientation.Vertical;
                        this.layoutStackPanel.Children.Add(base.ImageControl);
                        this.layoutStackPanel.Children.Add(base.TextBlock);
                        return;

                    case TextImageRelation.TextAboveImage:
                        this.layoutStackPanel.Orientation = Orientation.Vertical;
                        this.layoutStackPanel.Children.Add(base.TextBlock);
                        this.layoutStackPanel.Children.Add(base.ImageControl);
                        return;

                    case TextImageRelation.TextBeforeImage:
                        this.layoutStackPanel.Orientation = Orientation.Horizontal;
                        this.layoutStackPanel.Children.Add(base.TextBlock);
                        this.layoutStackPanel.Children.Add(base.ImageControl);
                        return;

                    case TextImageRelation.Overlay:
                        this.LayoutRoot.Children.Add(base.TextBlock);
                        this.LayoutRoot.Children.Add(base.ImageControl);
                        break;

                    default:
                        return;
                }
            }
        }

        protected void ApplyTextImageRelation(TabItemBase b)
        {
            if (base.Layout != null)
            {
                if ((base.ImageSource == null) && string.IsNullOrEmpty(base.Text))
                {
                    base.Layout.Visibility = Visibility.Collapsed;
                }
                else
                {
                    base.Layout.Visibility = Visibility.Visible;
                }
            }
        }

        private void ApplyTransformAngle(RotateTransform transform, Dock position)
        {
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                if (this.TabControl.TabOrientation == Orientation.Horizontal)
                {
                    switch (position)
                    {
                        case Dock.Left:
                            transform.Angle = -90.0;
                            return;

                        case Dock.Top:
                            transform.Angle = 0.0;
                            return;

                        case Dock.Right:
                            transform.Angle = 90.0;
                            return;

                        case Dock.Bottom:
                            transform.Angle = 0.0;
                            return;
                    }
                }
                else
                {
                    switch (position)
                    {
                        case Dock.Left:
                            transform.Angle = 0.0;
                            return;

                        case Dock.Top:
                            transform.Angle = 90.0;
                            return;

                        case Dock.Right:
                            transform.Angle = 0.0;
                            return;

                        case Dock.Bottom:
                            transform.Angle = -90.0;
                            return;
                    }
                }
            }
        }

        protected virtual void ApplyTransformation()
        {
            if ((!DesignerProperties.GetIsInDesignMode(this) && (this.tabPositionElement != null)) && (this.TabControl != null))
            {
                if (!string.IsNullOrEmpty(base.Text) || (base.ImageSource != null))
                {
                    switch (this.TabControl.TabAlignment)
                    {
                        case Dock.Left:
                        case Dock.Right:
                            this.ApplyTextImageRelation(this);
                            break;

                        case Dock.Top:
                        case Dock.Bottom:
                            this.ApplyTextImageRelation(this);
                            break;
                    }
                }
                RotateTransform transform = new RotateTransform();
                this.ApplyTransformAngle(transform, this.TabControl.TabAlignment);
                if (this.tabPositionElement != null)
                {
                    this.tabPositionElement.LayoutTransform = transform;
                }
            }
        }

        internal void ChangeVisualState(bool useTransitions)
        {
            bool? isSelected = this.IsSelected;
            bool isEnabled = base.IsEnabled;
            this.OnTabItemStateChanged();
            if (!isEnabled)
            {
                VisualStateManager.GoToState(this, "Disabled", useTransitions);
            }
            else if (base.IsPressed)
            {
                VisualStateManager.GoToState(this, "Selected", useTransitions);
            }
            else if ((base.IsMouseOver && (this.contentPresenter != null)) && !this.contentPresenter.IsMouseOver)
            {
                VisualStateManager.GoToState(this, "MouseOver", useTransitions);
            }
            else
            {
                VisualStateManager.GoToState(this, "Normal", useTransitions);
            }
            if (isSelected == true)
            {
                VisualStateManager.GoToState(this, "Selected", useTransitions);
            }
            else if (isSelected == false)
            {
                VisualStateManager.GoToState(this, "Unselected", useTransitions);
            }
            if (((isSelected == true) || base.IsMouseOver) || (base.IsPressed || this.ShowCloseButtonInDefaultTabItemState))
            {
                VisualStateManager.GoToState(this, "ShowButton", useTransitions);
            }
            else
            {
                VisualStateManager.GoToState(this, "HideButton", useTransitions);
            }
            if (base.IsFocused && isEnabled)
            {
                VisualStateManager.GoToState(this, "Focused", useTransitions);
            }
            else
            {
                VisualStateManager.GoToState(this, "UnFocused", useTransitions);
            }
            this.SynchronizeForegrounds();
        }

        public void Close()
        {
            this.UnwireEvents();
            this.TabControl.Items.Remove(this);
        }

        private void closeButton_Click(object sender, RoutedEventArgs e)
        {
            this.CloseButtonAction();
            this.OnCloseButtonClick();
        }

        private static void ContentMarginPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabControl control = d as TabControl;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.borderThickness = base.BorderThickness;
            this.cornerRadius = base.CornerRadius;
            this.tabPositionElement = base.GetTemplateChild("TabPositionElement") as TabPositionElement;
            this.selectedHighlight = base.GetTemplateChild("Backgoundselected_Highlight") as Border;
            this.selectedShadow1 = base.GetTemplateChild("BackgroundselectedShadow") as Border;
            this.selectedShadow2 = base.GetTemplateChild("BackgroundselectedInnerShadow") as Border;
            this.contentPresenter = base.GetTemplateChild("ItemContentPresenter") as ContentPresenter;
            this.LayoutRoot = base.GetTemplateChild("LayoutRoot") as Grid;
            this.layoutStackPanel = base.GetTemplateChild("LayoutStackPanel") as StackPanel;
            this.closeButton = base.GetTemplateChild("CloseButton") as Button;
            this.InnerLayoutRoot = base.GetTemplateChild("InnerLayoutRoot") as Grid;
            this.headerPresenter = base.GetTemplateChild("contentPresenter") as ContentPresenter;
            this.headerLayout = base.GetTemplateChild("Layout") as Grid;
            if ((this.closeButton != null) && this.ShowCloseButtonInDefaultTabItemState)
            {
                this.closeButton.Opacity = 1.0;
            }
            this.ApplyTransformation();
            TabControl.ApplySelectedIndex(this.TabControl);
            TabControl.ApplyContent(this.TabControl);
            base.TextImageRelationChanged += new EventHandler(this.TabItem_TextImageRelationChanged);
            if (this.closeButton != null)
            {
                this.closeButton.Click += new RoutedEventHandler(this.closeButton_Click);
            }
            this.closeButtonAction = new Action(this.Close);
            this.UpdateVisualState(false);
            if (this.contentPresenter != null)
            {
                this.contentPresenter.Visibility = Visibility.Visible;
                this.contentPresenter.Visibility = Visibility.Collapsed;
            }
            this.ApplyTabAlignmentBorder();
            this.SynchronizeForegrounds();
            if (this.tabPositionElement != null)
            {
                this.tabPositionElement.UpdateLayout();
            }
        }

        protected override void OnClick()
        {
            this.TabControl.SelectedItem = this;
            base.OnClick();
        }

        protected virtual void OnCloseButtonClick()
        {
            if (this.CloseButtonClick != null)
            {
                this.CloseButtonClick(this, EventArgs.Empty);
            }
        }

        private static void OnCloseButtonVisibilityPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabItem item = d as TabItem; ;
        }

        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);
            if ((this.IsSelected.HasValue && this.IsSelected.Value) && (this.TabControl != null))
            {
                TabControl.ApplyContent(this.TabControl);
            }
            this.UpdateVisualState(false);
        }

        private static void OnIsSelectedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabItem item = d as TabItem;
            bool? nullable = (bool?)e.NewValue;
            if ((((item != null) && (item.TabControl != null)) && (item.IsSelected.HasValue && item.IsSelected.Value)) && (item.TabControl.SelectedItem != item))
            {
                item.TabControl.SelectedItem = item;
            }
            if (nullable == true)
            {
                item.OnSelected(EventArgs.Empty);
            }
            else if (nullable == false)
            {
                item.OnUnselected(EventArgs.Empty);
            }
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            this.UpdateVisualState(true);
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            this.UpdateVisualState(true);
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            this.ChangeVisualState(true);
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);
            this.ChangeVisualState(true);
        }

        internal virtual void OnSelected(EventArgs e)
        {
            this.UpdateVisualState(true);
            EventHandler selected = this.Selected;
            if (selected != null)
            {
                selected(this, e);
            }
        }

        protected virtual void OnTabItemStateChanged()
        {
            if (this.TabItemStateChanged != null)
            {
                this.TabItemStateChanged(this, EventArgs.Empty);
            }
        }

        private static void OnTabOrientationPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabItem item = d as TabItem;
            if (item != null)
            {
                item.ApplyTransformation();
            }
        }

        internal void OnToggle()
        {
            bool? isSelected = this.IsSelected;
            if (isSelected == true)
            {
                this.IsSelected = false;
            }
            else
            {
                this.IsSelected = new bool?(isSelected.HasValue);
            }
        }

        internal virtual void OnUnselected(EventArgs e)
        {
            this.UpdateVisualState(true);
            EventHandler unselected = this.Unselected;
            if (unselected != null)
            {
                unselected(this, e);
            }
        }

        public void SetBorderThickness(Thickness borderThickness)
        {
            base.BorderThickness = borderThickness;
            this.borderThickness = base.BorderThickness;
            this.ApplyTabAlignmentBorder();
        }

        public void SetCornerRadius(CornerRadius cornerRadius)
        {
            base.CornerRadius = cornerRadius;
            this.cornerRadius = base.CornerRadius;
            this.ApplyTabAlignmentBorder();
        }

        protected override void SynchronizeForegrounds()
        {
            base.SynchronizeForegrounds();
            if (this.IsSelected.HasValue && this.IsSelected.Value)
            {
                base.Foreground = base.ForegroundPressed;
            }
        }

        private void TabControl_TabAlignmentChanged(object sender, EventArgs e)
        {
            this.TabOrientation = this.TabControl.TabOrientation;
            this.ApplyTransformation();
            this.ApplyTextImageRelation();
            this.ApplyTabAlignmentBorder();
        }

        private void TabControl_TabOrientationChanged(object sender, EventArgs e)
        {
            this.TabOrientation = this.TabControl.TabOrientation;
            this.ApplyTransformation();
            this.ApplyTextImageRelation();
            this.ApplyTabAlignmentBorder();
        }

        private void TabItem_Loaded(object sender, RoutedEventArgs e)
        {
            TabItem item = this;
            if (item.IsSelected.HasValue && item.IsSelected.Value)
            {
                if (((item != null) && (item.TabControl != null)) && (item.TabControl.SelectedItem != item))
                {
                    item.TabControl.SelectedItem = item;
                }
                TabControl.ApplySelectedIndex(this.TabControl);
                TabControl.ApplyContent(this.TabControl);
            }
            if ((this.TabControl != null) && this.TabControl.AllowFadeEffect)
            {
                if (this.IsSelected.HasValue && !this.IsSelected.Value)
                {
                    base.Opacity = this.TabControl.FadeOpacity;
                }
                else if (!this.IsSelected.HasValue)
                {
                    base.Opacity = this.TabControl.FadeOpacity;
                }
            }
        }

        private void TabItem_TextImageRelationChanged(object sender, EventArgs e)
        {
            this.ApplyTextImageRelation();
        }

        public override string ToString() =>
            "VIBlend TabItem for WPF";

        public override void UnwireEvents()
        {
            base.UnwireEvents();
            base.Loaded -= new RoutedEventHandler(this.TabItem_Loaded);
            base.TextImageRelationChanged -= new EventHandler(this.TabItem_TextImageRelationChanged);
            if (this.TabControl != null)
            {
                this.TabControl.TabAlignmentChanged -= new EventHandler(this.TabControl_TabAlignmentChanged);
                this.TabControl.TabOrientationChanged -= new EventHandler(this.TabControl_TabOrientationChanged);
            }
        }

        private void UpdateVisualState(bool useTransitions)
        {
            this.ChangeVisualState(useTransitions);
        }

        // Properties
        public Button CloseButton =>
            this.closeButton;

        [Category("Appearance")]
        public Thickness ContentMargin
        {
            get =>
                ((Thickness)base.GetValue(ContentMarginProperty));
            set =>
                base.SetValue(ContentMarginProperty, value);
        }

        [Browsable(false)]
        public DataTemplate HeaderTemplate
        {
            get =>
                ((DataTemplate)base.GetValue(HeaderTemplateProperty));
            set =>
                base.SetValue(HeaderTemplateProperty, value);
        }

        [Browsable(false)]
        public object Header
        {
            get =>
                base.GetValue(HeaderProperty);
            set =>
                base.SetValue(HeaderProperty, value);
        }

        [Category("Behavior"), Description("Gets the parent TabControl."), Browsable(false)]
        public TabControl TabControl
        {
            get
            {
                TabItem item = this;
                TabControl parentItemsControl = item.ParentItemsControl as TabControl;
                if (parentItemsControl != null)
                {
                    return parentItemsControl;
                }
                return null;
            }
        }

        internal ItemsControl ParentItemsControl
        {
            get =>
                this.parentItemsControl;
            set
            {
                if (this.parentItemsControl != value)
                {
                    this.parentItemsControl = value;
                    if (this.TabControl != null)
                    {
                        this.TabControl.TabAlignmentChanged += new EventHandler(this.TabControl_TabAlignmentChanged);
                        this.TabControl.TabOrientationChanged += new EventHandler(this.TabControl_TabOrientationChanged);
                        this.ApplyTransformation();
                    }
                }
            }
        }

        [Browsable(false), Category("Behavior")]
        public bool AutoAdjustBorderThickness
        {
            get =>
                ((bool)base.GetValue(AutoAdjustBorderThicknessProperty));
            set =>
                base.SetValue(AutoAdjustBorderThicknessProperty, value);
        }

        [Category("Behavior"), Description("Gets or sets the close button visibility.")]
        public Visibility CloseButtonVisibility
        {
            get =>
                ((Visibility)base.GetValue(CloseButtonVisibilityProperty));
            set =>
                base.SetValue(CloseButtonVisibilityProperty, value);
        }

        [Category("Behavior"), Description(" or sets a value indicating whether to show the close button in the default state."), Browsable(false)]
        public bool ShowCloseButtonInDefaultTabItemState
        {
            get =>
                this.showCloseButtonInDefaultTabItemState;
            set => this.showCloseButtonInDefaultTabItemState = value;
        }

        [Browsable(false), Category("Behavior")]
        public Action CloseButtonAction
        {
            get => this.closeButtonAction;
            set => this.closeButtonAction = value;
        }

        [Description("Gets or sets the tab orientation."), Category("Behavior")]
        public Orientation TabOrientation
        {
            get =>
                ((Orientation)base.GetValue(TabOrientationProperty));
            set =>
                base.SetValue(TabOrientationProperty, value);
        }

        [Browsable(false), TypeConverter(typeof(NullableBoolConverter))]
        public bool? IsSelected
        {
            get =>
                (base.GetValue(IsSelectedProperty) as bool?);
            internal set =>
                base.SetValue(IsSelectedProperty, value);
        }
    }
}
