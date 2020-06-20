﻿using DevExpress.Mvvm;
using DevExpress.Mvvm.POCO;
using EasyDriverPlugin;
using EasyDriver.Server.Models;
using EasyScada.ServerApplication.Workspace;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace EasyScada.ServerApplication
{
    public class ProjectTreeWorkspaceViewModel : WorkspacePanelViewModelBase, ISupportEdit
    {
        #region Public members

        public override bool IsBusy
        {
            get => MainViewModel.IsBusy;
            set => MainViewModel.IsBusy = value;
        }

        public MainViewModel MainViewModel { get => ParentViewModel as MainViewModel; }

        #endregion

        #region Injected members

        protected IWorkspaceManagerService WorkspaceManagerService { get; set; }
        protected IReverseService ReverseService { get; set; }
        protected IProjectManagerService ProjectManagerService { get; set; }
        protected IDriverManagerService DriverManagerService { get; set; }
        protected IHubFactory HubFactory { get; set; }
        protected IHubConnectionManagerService HubConnectionManagerService { get; set; }

        #endregion

        #region UI services

        protected ITreeListViewUtilities TreeListViewUtilities { get => this.GetService<ITreeListViewUtilities>(); }
        protected IMessageBoxService MessageBoxService { get => this.GetService<IMessageBoxService>(); }
        public IWindowService WindowService { get => this.GetService<IWindowService>(); }
        protected IContextWindowService ContextWindowService { get => this.GetService<IContextWindowService>(); }

        #endregion

        #region Constructors

        public ProjectTreeWorkspaceViewModel(
            IWorkspaceManagerService workspaceManagerService, 
            IReverseService reverseService,
            IProjectManagerService projectManagerService,
            IDriverManagerService driverManagerService,
            IHubFactory hubFactory,
            IHubConnectionManagerService hubConnectionManagerService) : base(null)
        {
            WorkspaceName = WorkspaceRegion.ProjectTree;
            Caption = "Project Explorer";
            WorkspaceManagerService = workspaceManagerService;
            ReverseService = reverseService;
            ProjectManagerService = projectManagerService;
            DriverManagerService = driverManagerService;
            HubFactory = hubFactory;
            HubConnectionManagerService = hubConnectionManagerService;

            ProjectManagerService.ProjectChanged += ProjectManagerService_ProjectChanged;

        }

        #endregion

        #region Public members

        public override string WorkspaceName { get; protected set; }

        public IEasyScadaProject CurrentProject => ProjectManagerService.CurrentProject;

        public virtual object SelectedItem { get; set; }

        public virtual object CurrentItem { get; set; }

        public virtual ObservableCollection<object> SelectedItems { get; set; }

        #endregion

        #region Property changed handlers

        public virtual void ShowPropertyOnClick(object item)
        {
            if (item != null)
                Messenger.Default.Send(new ShowPropertiesMessage(this, item));
        }

        #endregion

        #region Event handlers

        private void ProjectManagerService_ProjectChanged(object sender, ProjectChangedEventArgs e)
        {
            this.RaisePropertyChanged(x => x.CurrentProject);
            WorkspaceManagerService.RemoveAllDocumentPanel();

            // TODO: Need to check driver path is valid or not

            if (e.OldProject != null)
            {
                foreach (var item in e.OldProject.Childs)
                {
                    IStationCore station = item as IStationCore;
                    if (station is LocalStation)
                    {
                        foreach (var x in station.Childs)
                        {
                            if (x is IChannelCore channel)
                                DriverManagerService.RemoveDriver(channel);
                        }
                    }
                }
            }
            
            if (e.NewProject != null)
            {
                foreach (var item in e.NewProject.Childs)
                {
                    IStationCore station = item as IStationCore;
                    if (station is LocalStation)
                    {
                        foreach (var x in station.Childs)
                        {
                            if (x is IChannelCore channel)
                            {
                                IEasyDriverPlugin driver = DriverManagerService.AddDriver(channel, channel.DriverPath); 
                                driver?.Connect();
                            }
                        }
                    }
                }
            }
        }

        public virtual void OpenOnDoubleClick(object item)
        {
            if (IsBusy)
                return;

            IsBusy = true;
            if (item != null)
            {
                TreeListViewUtilities.ToggleCurrentNode();
                WorkspaceManagerService.OpenPanel(item, true, true, ParentViewModel);
            }
            IsBusy = false;
        }

        #endregion

        #region Commands

        public void AddStation()
        {
            try
            {
                IsBusy = true;
                WindowService.Show("RemoteStationView", ProjectManagerService.CurrentProject, this);
                IsBusy = false;
            }
            catch (Exception ex)
            {

            }
            finally { IsBusy = false; }
        }

        public bool CanAddStation()
        {
            if (ProjectManagerService.CurrentProject != null)
                return !IsBusy;
            return false;
        }

        public void AddChannel()
        {
            try
            {
                IsBusy = true;
                WindowService.Show("AddChannelView", SelectedItem, this);
            }
            catch (Exception ex)
            {

            }
            finally { IsBusy = false; }
        }

        public bool CanAddChannel()
        {
            if (SelectedItem is LocalStation)
                return !IsBusy;
            return false;
        }

        public void AddDevice()
        {
            try
            {
                IsBusy = true;
                IEasyDriverPlugin driver = DriverManagerService.GetDriver(SelectedItem as IChannelCore);
                if (driver != null)
                {
                    ContextWindowService.Show(driver.GetCreateDeviceControl(), "Add Device");
                    TreeListViewUtilities.ExpandCurrentNode();
                }
            }
            catch (Exception ex)
            {

            }
            finally { IsBusy = false; }
        }

        public bool CanAddDevice()
        {
            if (SelectedItem is IChannelCore)
                return !IsBusy;
            return false;
        }

        public void Open()
        {
            IsBusy = true;
            WorkspaceManagerService.OpenPanel(SelectedItem, true, true, ParentViewModel);
            IsBusy = false;
        }

        public bool CanOpen()
        {
            if (SelectedItem is IDeviceCore)
                return !IsBusy;
            return false;
        }

        public void Edit()
        {
            try
            {
                IsBusy = true;
                if (SelectedItem is IChannelCore channel)
                {
                    IEasyDriverPlugin driver = DriverManagerService.GetDriver(channel);
                    ContextWindowService.Show(driver.GetEditChannelControl(channel), $"Edit Channel - {channel.Name}");
                }
                else if (SelectedItem is IDeviceCore device)
                {
                    IEasyDriverPlugin driver = DriverManagerService.GetDriver(device);
                    ContextWindowService.Show(driver.GetEditDeviceControl(device), $"Edit Device - {device.Name}");
                }
                else if (SelectedItem is RemoteStation remoteStation)
                {
                    WindowService.Show("RemoteStationView", SelectedItem, this);
                }
            }
            catch (Exception ex)
            {

            }
            finally { IsBusy = false; }
        }

        public bool CanEdit()
        {
            if (SelectedItem is ICoreItem coreItem && !IsBusy)
            {
                if (!coreItem.IsReadOnly)
                    return !(SelectedItem is LocalStation);
            }
            return false;
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

        #region ISupportEdit

        public void Copy()
        {
        }

        public bool CanCopy()
        {
            return false;
        }

        public void Cut()
        {
        }

        public bool CanCut()
        {
            return false;
        }

        public void Paste()
        {
            
        }

        public bool CanPaste()
        {
            return false;
        }

        public void Delete()
        {
        }

        public bool CanDelete()
        {
            if (SelectedItems == null)
                return false;
            if (SelectedItems.Count == 0)
                return false;
            if (SelectedItems.FirstOrDefault(x => !(x as ICoreItem).IsReadOnly) != null)
                return true;
            return false;
        }

        #endregion
    }
}