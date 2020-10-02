using EasyDriverPlugin;
using System.Collections.Generic;

namespace EasyDriver.Core
{
    public static class Helper
    {
        public static List<IClientObject> GetClientObjects(this IGroupItem groupItem)
        {
            List<IClientObject> clientObjects = new List<IClientObject>();
            if (groupItem != null)
            {
                if (groupItem.Childs != null)
                {
                    foreach (var item in groupItem.Childs)
                    {
                        if (item is IClientObject obj)
                            clientObjects.Add(obj);
                    }
                }

                if (groupItem is IHaveTag haveTag)
                {
                    if (haveTag.HaveTags && haveTag.Tags != null)
                    {
                        foreach (var item in haveTag.Tags)
                        {
                            if (item is IClientObject obj)
                                clientObjects.Add(obj);
                        }
                    }
                }
            }
            return clientObjects;
        }

        public static List<IClientTag> GetClientTags(this ITagCore tagCore)
        {
            List<IClientTag> tagClients = new List<IClientTag>();
            if (tagCore != null)
            {
                if (tagCore is IHaveTag objHaveTag)
                {
                    if (objHaveTag.HaveTags)
                    {
                        foreach (var item in objHaveTag.Tags)
                        {
                            if (item is IClientTag childTag)
                            {
                                tagClients.Add(childTag);
                            }
                        }
                    }
                }
            }
            return tagClients;
        }

        public static IGroupItem ToCoreItem(this IClientObject clientObject, IGroupItem parent, bool isReadOnly)
        {
            if (clientObject == null)
                return null;
            IGroupItem result = null;
            switch (clientObject.ItemType)
            {
                case ItemType.LocalStation:
                    result = new LocalStation(parent);
                    break;
                case ItemType.RemoteStation:
                    result = new RemoteStation(parent);
                    (result as RemoteStation).ConnectionStatus = ConnectionStatus.Connected;
                    (result as RemoteStation).OpcDaServerName = clientObject.DisplayInfo;
                    break;
                case ItemType.Channel:
                    result = new ChannelCore(parent, isReadOnly);
                    (result as ChannelCore).DriverPath = clientObject.DisplayInfo;
                    break;
                case ItemType.Device:
                    result = new DeviceCore(parent, isReadOnly);
                    break;
                case ItemType.Group:
                    result = new GroupCore(parent, true, isReadOnly);
                    break;
                case ItemType.Tag:
                    result = new TagCore(parent, isReadOnly);
                    var tag = result as TagCore;
                    foreach (var kvp in clientObject.Properties)
                    {
                        if (kvp.Key == "DataType")
                            tag.DataTypeName = kvp.Value;
                        else if (kvp.Key == "Address")
                            tag.Address = kvp.Value;
                    }
                    break;
                case ItemType.ConnectionSchema:
                default:
                    return null;
            }

            if (result != null)
            {
                result.Name = clientObject.Name;
                result.Description = clientObject.Description;
                foreach (var item in clientObject.Childs)
                {
                    if (item.ItemType == ItemType.Tag)
                    {
                        if (result is IHaveTag objHaveTag)
                        {
                            if (objHaveTag.HaveTags)
                                objHaveTag.Tags.Add(item.ToCoreItem(result, isReadOnly));
                        }
                    }
                    else
                    {
                        result.Childs.Add(item.ToCoreItem(result, isReadOnly));
                    }
                }
            }

            return result;
        }

        public static void UpdateParent(this IGroupItem parent, bool updateDeep = true)
        {
            if (parent != null && parent.Childs != null)
            {
                if (parent is IHaveTag haveTagObject)
                {
                    if (haveTagObject.Tags != null)
                    {

                        foreach (var item in haveTagObject.Tags)
                        {
                            if (item is IGroupItem childGroup)
                            {
                                childGroup.Parent = parent;
                                if (updateDeep)
                                    childGroup.UpdateParent();
                            }
                            else if (item is ICoreItem childItem)
                            {
                                childItem.Parent = parent;
                            }
                        }
                    }
                }

                if (parent.Childs != null)
                {
                    foreach (var item in parent.Childs)
                    {
                        if (item is IGroupItem childGroup)
                        {
                            childGroup.Parent = parent;
                            if (updateDeep)
                                childGroup.UpdateParent();
                        }
                        else if (item is ICoreItem childItem)
                        {
                            childItem.Parent = parent;
                        }
                    }
                }
            }
        }
    }
}
