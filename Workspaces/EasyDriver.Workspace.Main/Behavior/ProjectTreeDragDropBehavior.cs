using DevExpress.Mvvm.UI.Interactivity;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Grid;
using EasyDriver.RemoteConnectionPlugin;
using EasyDriver.Service.Reversible;
using EasyDriverPlugin;
using EasyScada.WorkspaceManager;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace EasyDriver.Workspace.Main
{
    public class ProjectTreeDragDropBehavior : Behavior<TreeListView>
    {
        private int maxLevel;
        private ICoreItem parentItem = null;
        private IGroupItem targetItem = null;
        private IGroupItem oldParent = null;
        private int oldIndex = 0;
        object[] dragRecordData = null;

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.ShowTargetInfoInDragDropHint = true;
            AssociatedObject.StartRecordDrag += AssociatedObject_StartRecordDrag;
            AssociatedObject.DragRecordOver += AssociatedObject_DragRecordOver;
            AssociatedObject.CompleteRecordDragDrop += AssociatedObject_CompleteRecordDragDrop;
            AssociatedObject.DropRecord += AssociatedObject_DropRecord;
        }

        private void AssociatedObject_DropRecord(object sender, DropRecordEventArgs e)
        {
        }

        private void AssociatedObject_CompleteRecordDragDrop(object sender, CompleteRecordDragDropEventArgs e)
        {
            foreach (var item in e.Records)
            {
                if (item is ICoreItem coreItem && targetItem != null && e.Effects != DragDropEffects.None)
                {
                    oldIndex = coreItem.Parent.Childs.IndexOf(coreItem);
                    oldParent = coreItem.Parent;
                    coreItem.Added += OnItemAdded;
                }
            }
        }

        private void OnItemAdded(object sender, EventArgs e)
        {
            if (sender is ICoreItem coreItem)
            {
                using (var transaction = ServiceLocator.ReversibleService.Begin($"Drag {coreItem.Name}"))
                {
                    IGroupItem previousParent = oldParent;
                    int previousIndex = oldIndex;
                    coreItem.Added -= OnItemAdded;

                    string oldName = coreItem.Name;
                    string newName = coreItem.Name;
                    if (!coreItem.Name.IsUniqueNameInGroup(coreItem.Parent, coreItem))
                    {
                        newName = coreItem.Parent.GetUniqueNameInGroup(coreItem.Name, true);
                        coreItem.SetPropertyReversible(x => x.Name, newName);
                    }

                    transaction.AddChange(new ReversibleCollectionChange<object>(CollectionAction.Remove, previousParent.Childs, coreItem, previousIndex));
                    transaction.AddChange(new ReversibleCollectionChange<object>(CollectionAction.Add, coreItem.Parent.Childs, coreItem));
                    ServiceLocator.WorkspaceManagerService.RemovePanel(x => x.WorkspaceContext == coreItem);
                    transaction.Commit();
                }
            }
        }

        private void AssociatedObject_DragRecordOver(object sender, DevExpress.Xpf.Core.DragRecordOverEventArgs e)
        {
            targetItem = null;
            if (dragRecordData == null || parentItem == null || e.IsFromOutside)
            {
                e.Handled = true;
                return;
            }

            if (parentItem is IChannelCore channelCore)
            {
                if (e.TargetRecord is IChannelCore targetChannel)
                {
                    if (e.DropPosition == DropPosition.After ||
                        e.DropPosition == DropPosition.Before)
                    {
                        targetItem = e.TargetRecord as IGroupItem;
                        e.Effects = DragDropEffects.Move;
                        return;
                    }
                }
            }
            else if (parentItem is IDeviceCore deviceCore)
            {
                IChannelCore channelParent = deviceCore.FindParent<IChannelCore>(x => x is IChannelCore);
                if (channelParent != null)
                {
                    if (e.TargetRecord is IChannelCore targetChannel &&
                        targetChannel == channelParent &&
                        e.DropPosition == DropPosition.Inside)
                    {
                        e.Effects = DragDropEffects.Move;
                        return;
                    }
                    else if (e.TargetRecord is IDeviceCore targetDevice &&
                        targetDevice.FindParent<IChannelCore>(x => x is IChannelCore) == channelParent && 
                        (e.DropPosition == DropPosition.After ||
                        e.DropPosition == DropPosition.Before))
                    {
                        targetItem = e.TargetRecord as IGroupItem;
                        e.Effects = DragDropEffects.Move;
                        return;
                    }
                    else if (e.TargetRecord is GroupCore groupCore)
                    {
                        IDeviceCore deviceParent = groupCore.FindParent<IDeviceCore>(x => x is IDeviceCore);
                        if (deviceParent == null)
                        {
                            if (groupCore.FindParent<IChannelCore>(x => x is IChannelCore) == channelParent &&
                                (e.TargetRecord as ICoreItem).FindParent<IDeviceCore>(x => x is IDeviceCore) != deviceCore)
                            {
                                targetItem = e.TargetRecord as IGroupItem;
                                e.Effects = DragDropEffects.Move;
                                return;
                            }
                        }
                    }
                }
            }
            else if (parentItem is GroupCore groupCore)
            {
                IDeviceCore deviceParent = groupCore.FindParent<IDeviceCore>(x => x is IDeviceCore);
                if (deviceParent != null)
                {
                    IChannelCore channelParent = groupCore.FindParent<IChannelCore>(x => x is IChannelCore);
                    
                    if (e.TargetRecord is ICoreItem && channelParent != null)
                    {
                        if ((e.TargetRecord as ICoreItem).FindParent<ICoreItem>(x => x == groupCore) == null &&
                            (e.TargetRecord as ICoreItem).FindParent<IDeviceCore>(x => x is IDeviceCore) == deviceParent &&
                            (e.TargetRecord as ICoreItem).Level >= deviceParent.Level &&
                            (e.TargetRecord as ICoreItem).FindParent<IChannelCore>(x => x is IChannelCore) == channelParent)
                        {
                            if (e.TargetRecord == deviceParent)
                            {
                                if (e.DropPosition == DropPosition.Inside)
                                {
                                    targetItem = e.TargetRecord as IGroupItem;
                                    e.Effects = DragDropEffects.Move;
                                    return;
                                }
                            }
                            else
                            {
                                targetItem = e.TargetRecord as IGroupItem;
                                e.Effects = DragDropEffects.Move;
                                return;
                            }
                        }
                    }
                }
                else
                {
                    IChannelCore channelParent = groupCore.FindParent<IChannelCore>(x => x is IChannelCore);
                    if (e.TargetRecord is ICoreItem && channelParent != null)
                    {
                        if ((e.TargetRecord as ICoreItem).FindParent<ICoreItem>(x => x == groupCore) == null &&
                            (e.TargetRecord as ICoreItem).Level >= channelParent.Level &&
                            (e.TargetRecord as ICoreItem).FindParent<IChannelCore>(x => x is IChannelCore) == channelParent &&
                            (e.TargetRecord as ICoreItem).FindParent<IDeviceCore>(x => x is IDeviceCore) == null)
                        {
                            if (e.TargetRecord == channelParent)
                            {
                                if (e.DropPosition == DropPosition.Inside)
                                {
                                    targetItem = e.TargetRecord as IGroupItem;
                                    e.Effects = DragDropEffects.Move;
                                    return;
                                }
                            }
                            else if (e.TargetRecord is IDeviceCore)
                            {
                                if (e.DropPosition == DropPosition.After ||
                                    e.DropPosition == DropPosition.Before)
                                {
                                    targetItem = e.TargetRecord as IGroupItem;
                                    e.Effects = DragDropEffects.Move;
                                    return;
                                }
                            }
                            else
                            {
                                targetItem = e.TargetRecord as IGroupItem;
                                e.Effects = DragDropEffects.Move;
                                return;
                            }
                        }
                    }
                }
            }
            e.Effects = DragDropEffects.None;
            e.Handled = true;
        }

        private void AssociatedObject_StartRecordDrag(object sender, DevExpress.Xpf.Core.StartRecordDragEventArgs e)
        {
            if (e.Records != null && e.Records.Length > 0)
            {
                maxLevel = 0;
                foreach (var item in e.Records)
                {
                    if (item is ICoreItem coreItem)
                    {
                        if (coreItem.IsReadOnly)
                        {
                            e.AllowDrag = false;
                            e.Handled = true;
                            return;
                        }
                        if (maxLevel < coreItem.Level)
                        {
                            maxLevel = coreItem.Level;
                            parentItem = coreItem;
                        }
                    }
                }

                dragRecordData = e.Records;
                return;
            }
            else
            {
                e.AllowDrag = false;
                dragRecordData = null;
                e.Handled = true;
            }
        }
    }
}
