using DevExpress.Mvvm.POCO;
using EasyDriver.Service.BarManager;
using EasyScada.WorkspaceManager;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace EasyDriverServer
{
    public class MainViewModel
    {
        #region Public properties
        public virtual ObservableCollection<IBarComponent> BarSource
        {
            get => BarManagerService.BarSource;
        }

        public virtual ObservableCollection<IWorkspacePanel> WorkspaceSource
        {
            get => WorkspaceManagerService.Workspaces;
        }
        #endregion

        #region Injected services
        protected IBarManagerService BarManagerService { get; set; }
        protected IWorkspaceManagerService WorkspaceManagerService { get; set; }
        #endregion

        #region Constructors
        public MainViewModel(
            IBarManagerService barManagerService,
            IWorkspaceManagerService workspaceManagerService)
        {
            BarManagerService = barManagerService;
            WorkspaceManagerService = workspaceManagerService;
        }
        #endregion

        #region Event handlers
        public virtual void OnLoaded()
        {
            this.RaisePropertyChanged(x => x.BarSource);
            this.RaisePropertyChanged(x => x.WorkspaceSource);
        }
        #endregion
    }
}
