using DevExpress.Mvvm;
using DevExpress.Mvvm.POCO;
using Microsoft.AspNet.SignalR.Client;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.IO;
using EasyDriver.Core;
using EasyDriverPlugin;

namespace EasyScada.ServerApplication
{
    public class RemoteProjectTreeViewModel : ISupportParameter, ISupportParentViewModel
    {
        #region Constructors

        public RemoteProjectTreeViewModel(
            IRemoteConnectionManagerService hubConnectionManagerService,
            IHubFactory hubFactory)
        {
            Title = "Easy Driver Server";
            SizeToContent = SizeToContent.Manual;
            Height = 600;
            Width = 800;
            HubConnectionManagerService = hubConnectionManagerService;
            HubFactory = hubFactory;
        }

        #endregion

        #region Injected services

        protected IHubFactory HubFactory { get; set; }
        protected IRemoteConnectionManagerService HubConnectionManagerService { get; set; }

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
        public HubModel HubModel { get; private set; }
        public ConnectionSchema ConnectionSchema { get; private set; }
        public bool IsCreateStationMode { get; private set; }
        public List<HubModel> Source { get; set; }
        public List<IClientObject> TagsSource { get; set; }

        HubConnection hubConnection;
        IHubProxy hubProxy;

        #endregion

        #region Commands

        public void Refresh()
        {
            IsBusy = true;
            try
            {
                DispatcherService.BeginInvoke(async () =>
                {
                    if (hubConnection.State == ConnectionState.Disconnected)
                    {
                        try
                        {
                            await hubConnection.Start();
                        }
                        catch { }
                    }
                    await Task.Delay(50);

                    if (hubConnection.State == ConnectionState.Connected)
                    {
                        string resJson = await hubProxy.Invoke<string>("getAllElementsAsync");
                        List<ClientObject> clientObjects = JsonConvert.DeserializeObject<List<ClientObject>>(resJson);
                        HubModel.Childs = new List<IClientObject>(clientObjects);
                        HubModel.IsChecked = true;
                        HubModel.Childs.ForEach(x => CheckAllChildItems(x));
                        Source = new List<HubModel>() { HubModel };
                        this.RaisePropertyChanged(x => x.Source);
                        IsBusy = false;
                    }
                    else
                    {
                        MessageBoxService.ShowMessage($"Can't connect to server {HubModel.RemoteAddress}:{HubModel.Port}", "Easy Driver Server", MessageButton.OK, MessageIcon.Warning);
                    }
                });
            }
            catch
            {
                IsBusy = false;
                MessageBoxService.ShowMessage($"Can't connect to server {HubModel.RemoteAddress}:{HubModel.Port}", "Easy Driver Server", MessageButton.OK, MessageIcon.Warning);
            }
        }

        public bool CanRefresh()
        {
            return !IsBusy && HubModel != null;
        }

        public void Comfirm()
        {
            try
            {
                if (IsCreateStationMode)
                {
                    IsBusy = true;
                    RemoveUncheckedItem(HubModel);
                    Messenger.Default.Send(new CreateRemoteStationSuccessMessage(HubModel, hubConnection, hubProxy));
                    CurrentWindowService.Close();
                    IsBusy = false;
                }
                else
                {
                    SaveFileDialogService.Title = "Save Connection Schema";
                    SaveFileDialogService.Filter = "Connection Schema File (*.json)|*.json";

                    if (SaveFileDialogService.ShowDialog())
                    {
                        IsBusy = true;
                        RemoveUncheckedItem(HubModel);
                        ConnectionSchema.Childs = HubModel.Childs;
                        string savePath = SaveFileDialogService.File.GetFullName();
                        string connectionSchemaJson = JsonConvert.SerializeObject(ConnectionSchema, Formatting.Indented);
                        try
                        {
                            File.WriteAllText(savePath, connectionSchemaJson);
                            MessageBoxService.ShowMessage("Create connection schema successfully!", "Easy Driver Server", MessageButton.OK, MessageIcon.Information);
                            Messenger.Default.Send(new CreateConnectionSchemaSuccessMessage(ConnectionSchema, savePath));
                            CurrentWindowService.Close();
                            IsBusy = false;
                        }
                        catch
                        {
                            MessageBoxService.ShowMessage($"Can' create connection schema file at '{savePath}'", "Easy Driver Server", MessageButton.OK, MessageIcon.Error);
                        }
                    }
                }
            }
            catch
            {
            }
            finally { IsBusy = false; }
        }

        public bool CanComfirm()
        {
            return !IsBusy && HubModel != null;
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
                        if (arrays[0] is HubModel)
                        {
                            HubModel = arrays[0] as HubModel;
                            IsCreateStationMode = true;
                        }
                        else if (arrays[0] is ConnectionSchema)
                        {
                            ConnectionSchema = arrays[0] as ConnectionSchema;
                            HubModel = new HubModel()
                            {
                                Port = ConnectionSchema.Port.ToString(),
                                RemoteAddress = ConnectionSchema.ServerAddress,
                                StationName = "Server Station"
                            };
                        }

                        hubConnection = arrays[1] as HubConnection;
                        hubProxy = arrays[2] as IHubProxy;
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

        public virtual void OnSelectedItemChanged()
        {
            TagsSource = new List<IClientObject>();
            if (SelectedItem is IClientObject clientObject)
            {
                if (clientObject.Childs != null)
                {
                    TagsSource.AddRange(clientObject.Childs.Where(x => x.ItemType == ItemType.Tag));
                }
            }
            this.RaisePropertyChanged(x => x.TagsSource);
        }

        #endregion

        #region Methods
        private void RemoveUncheckedItem(HubModel hubModel)
        {
            if (hubModel != null && hubModel.Childs != null)
            {
                foreach (var item in hubModel.Childs.ToList())
                {
                    if (item.ItemType != ItemType.Tag)
                    {
                        if (item is ICheckable checkObj)
                        {
                            if (checkObj.IsChecked.HasValue && !checkObj.IsChecked.Value)
                            {
                                hubModel.Childs.Remove(item);
                            }
                            else
                            {
                                RemoveUncheckedItem(item);
                            }
                        }
                        else
                        {
                            hubModel.Childs.Remove(item);
                        }
                    }
                }
            }
        }

        private void RemoveUncheckedItem(IClientObject clientObject)
        {
            if (clientObject != null && clientObject.Childs != null)
            {
                foreach (var item in clientObject.Childs.ToList())
                {
                    if (item.ItemType != ItemType.Tag)
                    {
                        if (item is ICheckable checkObj && item.ItemType != ItemType.Tag)
                        {
                            if (checkObj.IsChecked.HasValue && !checkObj.IsChecked.Value)
                            {
                                clientObject.Childs.Remove(item);
                            }
                            else
                            {
                                RemoveUncheckedItem(item);
                            }
                        }
                        else
                        {
                            clientObject.Childs.Remove(item);
                        }
                    }
                }
            }
        }

        private void CheckAllChildItems(IClientObject clientObject)
        {
            if (clientObject != null)
            {
                if (clientObject is ICheckable checkObj)
                {
                    checkObj.IsChecked = true;
                    if (clientObject.Childs != null)
                    {
                        foreach (var item in clientObject.Childs)
                            CheckAllChildItems(item);
                    }
                }
            }
        }

        private void CountTags()
        {
            tagCount = 0;
            if (Source.Count > 0)
            {
                foreach (var item in Source[0].Childs)
                {
                    CountTags(item);
                }
            }
            this.RaisePropertyChanged(x => x.TotalTags);
        }

        private void CountTags(IClientObject clientObject)
        {
            if (clientObject != null)
            {
                if (clientObject.Childs != null)
                {
                    tagCount += clientObject.Childs.Count(x => x.ItemType == ItemType.Tag);
                    foreach (var item in clientObject.Childs)
                    {
                        CountTags(item);
                    }
                }
            }
        }
        #endregion
    }
}
