using Microsoft.Windows.Design.Features;
using Microsoft.Windows.Design.Interaction;
using Microsoft.Windows.Design.Metadata;
using Microsoft.Windows.Design.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

[assembly: ProvideMetadata(typeof(EasyScada.Wpf.Connector.VisualStudio.Design.VisualStudioMetadata))]
namespace EasyScada.Wpf.Connector.VisualStudio.Design
{
    public class VisualStudioMetadata : IProvideAttributeTable
    {
        public AttributeTable AttributeTable { get; set; }

        public VisualStudioMetadata()
        {
            AttributeTableBuilder builder = new AttributeTableBuilder();
            // Add menu items
            builder.AddCallback(typeof(EasyDriverConnector), CreateMenu);
            // Add design adorner
            builder.AddCustomAttributes(typeof(EasyDriverConnector), new FeatureAttribute(typeof(ConnectorDesignAdorner)));
            builder.AddCustomAttributes(typeof(Window), new FeatureAttribute(typeof(ConnectorDesignAdorner)));
            AttributeTable = builder.CreateTable();
        }

        private void CreateMenu(AttributeCallbackBuilder builder)
        {
            builder.AddCustomAttributes(new FeatureAttribute(typeof(ConnectorDesignMenuProvider)));
        }
    }

    public class ConnectorDesignMenuProvider : PrimarySelectionContextMenuProvider
    {
        readonly MenuAction updateMenuAction;
        readonly EasyDriverConnector easyDriverConnector;

        public ConnectorDesignMenuProvider(EasyDriverConnector easyDriverConnector)
        {
            this.easyDriverConnector = easyDriverConnector;
            updateMenuAction = new MenuAction("Update Connection Schema");
            updateMenuAction.Execute += OnUpdateMenuActionExecute;
            Items.Add(updateMenuAction);
        }

        private void OnUpdateMenuActionExecute(object sender, MenuActionEventArgs e)
        {

        }
    }

    public class ConnectorDesignAdorner : PrimarySelectionAdornerProvider
    {
        EasyDriverConnectorDesignUI designUI;
        AdornerPanel adornerPanel;
        ModelItem modelItem;

        public ConnectorDesignAdorner()
        {
            designUI = new EasyDriverConnectorDesignUI();
        }

        protected override void Activate(ModelItem item)
        {
            modelItem = item;

            // Create adorner panel
            if (adornerPanel == null)
            {
                adornerPanel = new AdornerPanel();
                adornerPanel.IsContentFocusable = true;
                adornerPanel.Children.Add(designUI);
                Adorners.Add(adornerPanel);
            }

            // Locate adorner panel
            AdornerPanel.SetHorizontalStretch(designUI, AdornerStretch.Stretch);
            AdornerPanel.SetVerticalStretch(designUI, AdornerStretch.Stretch);
            var placements = new AdornerPlacementCollection();
            placements.PositionRelativeToContentHeight(0, 0);
            placements.PositionRelativeToContentWidth(1, 0);
            placements.SizeRelativeToAdornerDesiredHeight(1, 0);
            placements.SizeRelativeToAdornerDesiredWidth(1, 0);
            AdornerPanel.SetPlacements(designUI, placements);

            // Subscribe design ui events
            designUI.Loaded += OnDesignUILoaded;
            if (designUI is INotifyPropertyChanged notify)
                notify.PropertyChanged += OnDesignUIPropertyChanged;

            base.Activate(item);
        }

        protected override void Deactivate()
        {
            // Unsubscribe design ui events
            designUI.Loaded -= OnDesignUILoaded;
            if (designUI is INotifyPropertyChanged notify)
                notify.PropertyChanged -= OnDesignUIPropertyChanged;
            base.Deactivate();
        }

        private void OnDesignUIPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e is DesignerPropertyChangedEventArgs args)
                modelItem.Properties[e.PropertyName]?.SetValue(args.Value);
        }

        private void OnDesignUILoaded(object sender, System.Windows.RoutedEventArgs e)
        {

        }
    }
}
