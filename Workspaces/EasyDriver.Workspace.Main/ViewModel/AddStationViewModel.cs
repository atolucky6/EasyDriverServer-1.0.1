using DevExpress.Mvvm;
using DevExpress.Mvvm.POCO;
using EasyDriverPlugin;
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using EasyDriver.Service.ContextWindow;
using EasyDriver.Service.ProjectManager;
using EasyDriver.Service.DriverManager;
using EasyDriver.Service.RemoteConnectionManager;
using EasyDriver.RemoteConnectionPlugin;

namespace EasyDriver.Workspace.Main
{
    public class AddStationViewModel : IDataErrorInfo, ISupportParameter, ISupportParentViewModel
    {
        #region UI services

        protected ICurrentWindowService CurrentWindowService { get => this.GetService<ICurrentWindowService>(); }
        protected IOpenFileDialogService OpenFileDialogService { get => this.GetService<IOpenFileDialogService>(); }
        protected IMessageBoxService MessageBoxService { get => this.GetService<IMessageBoxService>(); }

        #endregion

        #region Inject services

        protected IProjectManagerService ProjectManagerService { get; set; }
        protected IRemoteConnectionManagerService RemoteConnectionManagerService { get; set; }
        protected ITreeListViewUtilities TreeListViewUtilities { get; set; }
        protected IContextWindowService ContextWindowService { get; set; }

        #endregion

        #region Constructors

        public AddStationViewModel(
            ITreeListViewUtilities treeListViewUtilities, 
            IContextWindowService contextWindowService)
        {
            Title = "Add Station";
            SizeToContent = SizeToContent.WidthAndHeight;
            Width = 600;
            Height = 120;
            ProjectManagerService = ServiceLocator.ProjectManagerService;
            RemoteConnectionManagerService = ServiceLocator.RemoteConnectionManagerService;
            TreeListViewUtilities = treeListViewUtilities;
            ContextWindowService = contextWindowService;
        }

        #endregion

        #region Public members

        public string Title { get; set; }
        public SizeToContent SizeToContent { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public virtual string Name { get; set; }
        public virtual string SelectedStationType { get; set; }
        public List<string> StationTypeSource { get; set; }

        public object Parameter { get; set; }
        public object ParentViewModel { get; set; }
        public IGroupItem Parent { get => Parameter as IGroupItem; }
        public virtual bool IsBusy { get; set; }
        #endregion

        #region Commands

        public void Save()
        {
            try
            {
                if (Parent.Childs.FirstOrDefault(x => (x as ICoreItem).Name == Name?.Trim()) != null)
                {
                    MessageBoxService.ShowMessage($"The station name '{Name?.Trim()}' is already in use.", "Message", MessageButton.OK, MessageIcon.Warning);
                }
                else
                {
                    IsBusy = true;

                    IEasyRemoteConnectionPlugin connPlugin = RemoteConnectionManagerService.CreateRemoteConnection(SelectedStationType);
                    if (connPlugin != null)
                    {
                        CurrentWindowService.Hide();
                        if (ContextWindowService.ShowDialog(connPlugin.GetCreateRemoteConnectionView(Parent), "Add Station") is IStationCore station)
                        {
                            station.Name = Name;
                            Parent.Childs.Add(station);
                            RemoteConnectionManagerService.AddRemoteConnection(station, connPlugin);
                            connPlugin.Start(station);
                            TreeListViewUtilities.ExpandNodeByContent(station);
                        }
                        CurrentWindowService.Close();
                    }
                    else
                    {
                        MessageBoxService.ShowMessage($"Can't load the remote connection plugin {SelectedStationType}.", "Message", MessageButton.OK, MessageIcon.Error);
                    }
                }
            }
            catch (Exception)
            {
                MessageBoxService.ShowMessage($"Can't load the driver {SelectedStationType}.", "Message", MessageButton.OK, MessageIcon.Error);
            }
            finally { IsBusy = false; }
        }

        public bool CanSave() => string.IsNullOrEmpty(Error) && !string.IsNullOrEmpty(SelectedStationType) && !IsBusy;

        public void Close() => CurrentWindowService.Close();

        public bool CanClose() => !IsBusy;

        #endregion

        #region Event handlers

        public void OnLoaded()
        {
            Name = Parent.GetUniqueNameInGroup("RemoteStation1");
            StationTypeSource = RemoteConnectionManagerService.AvailableRemoteConnections.ToList();
            SelectedStationType = StationTypeSource.FirstOrDefault();
            this.RaisePropertyChanged(x => x.StationTypeSource);
        }

        #endregion

        #region IDataErrorInfo

        public string Error { get; private set; }

        public string this[string columnName]
        {
            get
            {
                switch (columnName)
                {
                    case nameof(Name):
                        Error = Name.ValidateFileName();
                        break;
                    default:
                        Error = string.Empty;
                        break;
                }
                return Error;
            }
        }

        #endregion
    }
}
