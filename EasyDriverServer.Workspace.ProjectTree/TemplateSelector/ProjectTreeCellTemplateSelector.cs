﻿using DevExpress.Xpf.Grid;
using EasyDriverPlugin;
using System.Windows;
using System.Windows.Controls;
using EasyDriver.Core;

namespace EasyDriverServer.Workspace.ProjectTree
{
    public class ProjectTreeCellTemplateSelector : DataTemplateSelector
    {
        public DataTemplate DefaultTemplate { get; set; }
        public DataTemplate ChannelTemplate { get; set; }
        public DataTemplate DeviceTemplate { get; set; }
        public DataTemplate TagTemplate { get; set; }
        public DataTemplate HubTemplate { get; set; }
        public DataTemplate LocalStationTemplate { get; set; }
        public DataTemplate RemoteStationTemplate { get; set; }
        public DataTemplate RemoteOpcDaStationTemplate { get; set; }
        public DataTemplate GroupTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item != null)
            {
                var row = (item as GridCellData).Row;
                if (row is IClientObject clientObject)
                {
                    switch (clientObject.ItemType)
                    {
                        case ItemType.LocalStation:
                            return LocalStationTemplate;
                        case ItemType.RemoteStation:
                            return RemoteStationTemplate;
                        case ItemType.Channel:
                            return ChannelTemplate;
                        case ItemType.Device:
                            return DeviceTemplate;
                        case ItemType.Tag:
                            return TagTemplate;
                        case ItemType.Group:
                        case ItemType.ConnectionSchema:
                            return DefaultTemplate;
                        default:
                            break;
                    }
                }
            }
            return DefaultTemplate;
        }
    }
}