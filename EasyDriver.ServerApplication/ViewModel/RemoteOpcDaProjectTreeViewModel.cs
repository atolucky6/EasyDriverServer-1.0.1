using DevExpress.Mvvm;
using DevExpress.Mvvm.POCO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using EasyDriver.Core;
using EasyDriver.Opc.Client.Da;
using EasyDriver.Opc.Client.Da.Browsing;
using System.Threading.Tasks;
using EasyDriverPlugin;
using System;

namespace EasyScada.ServerApplication
{
    public class RemoteOpcDaProjectTreeViewModel : ISupportParameter, ISupportParentViewModel
    {
        #region Constructors

        public RemoteOpcDaProjectTreeViewModel(
            IRemoteConnectionManagerService remoteConnectionManagerService,
            IProjectManagerService projectManagerService)
        {
            Title = "Easy Driver Server";
            SizeToContent = SizeToContent.Manual;
            Height = 600;
            Width = 800;
            RemoteConnectionManagerService = remoteConnectionManagerService;
            ProjectManagerService = projectManagerService;
            Source = new List<IGroupItem>();
        }

        #endregion

        #region Injected services

        protected IProjectManagerService ProjectManagerService { get; set; }
        protected IRemoteConnectionManagerService RemoteConnectionManagerService { get; set; }

        #endregion

        #region UI services

        protected ICurrentWindowService CurrentWindowService { get => this.GetService<ICurrentWindowService>(); }
        protected ITreeListViewUtilities TreeListViewUtilities { get => this.GetService<ITreeListViewUtilities>(); }
        protected IMessageBoxService MessageBoxService { get => this.GetService<IMessageBoxService>(); }
        protected ISaveFileDialogService SaveFileDialogService { get => this.GetService<ISaveFileDialogService>(); }
        protected IDispatcherService DispatcherService { get => this.GetService<IDispatcherService>(); }

        #endregion

        #region Public members
        int tagCount = 0;
        public virtual string TotalTags { get => $"Total tags: {tagCount}"; }
        public string Title { get; set; }
        public SizeToContent SizeToContent { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public virtual bool IsBusy { get; set; }
        public object ParentViewModel { get; set; }
        public object Parameter { get; set; }
        public virtual object SelectedItem { get; set; }
        public virtual ObservableCollection<object> SelectedItems { get; set; }
        public bool IsCreateStationMode { get; private set; }
        public List<IGroupItem> Source { get; set; }
        public OpcDaServer OpcDaServer { get; private set; }

        #endregion

        #region Commands

        public void Refresh()
        {
            IsBusy = true;
            try
            {
                DispatcherService.BeginInvoke(async () =>
                {
                    if (OpcDaServer.IsConnected)
                    {
                        try
                        {
                            // Create root object
                            RemoteStation station = new RemoteStation(ProjectManagerService.CurrentProject);
                            station.StationType = "OPC_DA";
                            station.Name = OpcDaServer.Uri.ToString();
                            station.IsChecked = true;
                            station.ConnectionStatus = ConnectionStatus.Connected;
                            tagCount = 0;
                            OpcDaBrowserAuto browser = new OpcDaBrowserAuto(OpcDaServer);
                            var childs = await GetChildElementsAsync(browser, null, station);
                            childs.ForEach(x => station.Childs.Add(x));
                            Source = new List<IGroupItem>() { station };
                            this.RaisePropertyChanged(x => x.Source);
                            this.RaisePropertyChanged(x => x.TotalTags);
                        }   
                        catch
                        {
                            MessageBoxService.ShowMessage($"Can't connect to server OPC server '{OpcDaServer.Uri.ToString()}'", "Easy Driver Server", MessageButton.OK, MessageIcon.Warning);
                        }
                        finally
                        {
                            IsBusy = false;
                            this.RaisePropertyChanged(x => x.IsBusy);
                        }
                    }
                });
            }
            catch
            {
                IsBusy = false;
                MessageBoxService.ShowMessage($"Can't connect to server OPC server '{OpcDaServer.Uri.ToString()}'", "Easy Driver Server", MessageButton.OK, MessageIcon.Warning);
            }
        }

        Task<List<IGroupItem>> GetChildElementsAsync(IOpcDaBrowser browser, string itemId, IGroupItem parent)
        {
            return Task.Run(() => GetChildElements(browser, itemId, parent));
        }

        List<IGroupItem> GetChildElements(IOpcDaBrowser browser, string itemId, IGroupItem parent)
        {
            List<IGroupItem> result = new List<IGroupItem>();
            OpcDaBrowseElement[] elements = browser.GetElements(itemId);
            foreach (OpcDaBrowseElement element in elements)
            {
                if (element.Name.StartsWith("_"))
                    continue;
                IGroupItem groupItem = null;
                if (element.IsItem)
                {
                    groupItem = new TagCore(parent, true);
                    tagCount++;
                }
                else
                {
                    groupItem = new GroupCore(parent, true, true);
                }

                groupItem.Name = element.Name;
                if (groupItem is ISupportParameters supportParameters)
                    supportParameters.ParameterContainer.Parameters["ItemId"] = element.ItemId;

                if (groupItem != null)
                {
                    groupItem.IsChecked = true;
                    if (element.HasChildren)
                    {
                        GetChildElements(browser, element.ItemId, groupItem)?.ForEach(x =>
                        {
                            if (x is TagCore && groupItem is IHaveTag itemHaveTags)
                            {
                                if (itemHaveTags.HaveTags)
                                {
                                    itemHaveTags.Tags.Add(x);
                                }
                            }
                            else
                            {
                                groupItem.Childs.Add(x);
                            }
                        });
                    }
                    result.Add(groupItem);
                }
            }
            return result;
        }

        public bool CanRefresh()
        {
            return !IsBusy && OpcDaServer != null;
        }

        public void Comfirm()
        {
            try
            {
                if (IsCreateStationMode)
                {
                    IsBusy = true;
                    IGroupItem groupItem = Source.FirstOrDefault();
                    RemoveUncheckedItem(groupItem);
                    Messenger.Default.Send(new CreateRemoteOpcDaStationSuccessMessage(OpcDaServer, groupItem));
                    CurrentWindowService.Close();
                    IsBusy = false;
                }
            }
            catch (Exception ex)
            {
            }
            finally { IsBusy = false; }
        }

        public bool CanComfirm()
        {
            return !IsBusy && OpcDaServer != null;
        }

        public void ExpandAll()
        {
            TreeListViewUtilities.ExpandAll();
        }

        public bool CanExpandAll()
        {
            return !IsBusy;
        }

        public void CollapseAll()
        {
            TreeListViewUtilities.CollapseAll();
        }

        public bool CanCollapseAll()
        {
            return !IsBusy;
        }

        #endregion

        #region Event handlers

        public virtual void OnLoaded()
        {
            try
            {
                DispatcherService.BeginInvoke(() =>
                {
                    IsBusy = true;
                    this.RaisePropertyChanged(x => x.IsBusy);
                    if (Parameter is object[] arrays)
                    {
                        if (arrays[0] is OpcDaServer)
                        {
                            OpcDaServer = arrays[0] as OpcDaServer;
                            IsCreateStationMode = true;
                        }
                        Refresh();
                    }
                });
            }
            catch
            {
                IsBusy = false;
            }
        }

        public virtual void OpenOnDoubleClick(object selectedItem)
        {
            if (selectedItem != null && selectedItem == SelectedItem)
                TreeListViewUtilities.ToggleCurrentNode();
        }

        #endregion

        #region Methods
        private void RemoveUncheckedItem(IGroupItem groupItem)
        {
            groupItem.Childs.ToList().ForEach(x =>
            {
                if (x is ICoreItem coreItem)
                {
                    if (coreItem.IsChecked.HasValue && !coreItem.IsChecked.Value)
                    {
                        groupItem.Childs.Remove(coreItem);
                    }
                    else
                    {
                        if (x is IGroupItem childGroupItem)
                            RemoveUncheckedItem(childGroupItem);
                    }
                }
            });
        }
        #endregion
    }
}
