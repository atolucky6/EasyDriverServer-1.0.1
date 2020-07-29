using DevExpress.Mvvm.POCO;
using EasyDriver.Core;
using EasyDriverPlugin;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace EasyScada.ServerApplication
{
    public class ApplicationViewModel : IDisposable
    {
        #region Constructors

        public ApplicationViewModel(IProjectManagerService projectManagerService,
            IWorkspaceManagerService workspaceManagerService,
            IDispatcherFacade dispatcherFacade)
        {
            ProjectManagerService = projectManagerService;
            ProjectManagerService.ProjectChanged += OnProjectChanged;
            WorkspaceManagerService = workspaceManagerService;
            DispatcherFacade = dispatcherFacade;

            ApplicationPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            DriverFolderPath = $"{ApplicationPath}\\Driver";
            Initialize();
        }

        ~ApplicationViewModel()
        {
            Dispose();
        }

        #endregion

        #region Injected services

        protected IWorkspaceManagerService WorkspaceManagerService { get; set; }
        protected IProjectManagerService ProjectManagerService { get; set; }
        protected IDispatcherFacade DispatcherFacade { get; set; }

        #endregion

        #region Public members

        public bool IsMainWindowExit { get; set; }
        public List<string> ConnectedClients { get; set; } = new List<string>();
        public virtual List<string> DriverNameSource { get; set; }
        public virtual string DriverFolderPath { get; set; }
        public virtual string ApplicationPath { get; set; }
        public virtual string ServerIniPath { get; set; }

        public ServerConfiguration ServerConfiguration { get; set; }

        public int TotalConnectedClients
        {
            get { return ConnectedClients.Count; }
        }
        public virtual string CurrentOpenedProjectPath { get; protected set; }
        public bool IsDisposed { get; protected set; }

        #endregion

        #region Private members

        private Task syncUIUpdateTask;
        private SynchronizationContext uiSyncContext;

        #endregion

        #region Event handlers

        private void OnProjectChanged(object sender, ProjectChangedEventArgs e)
        {
            CurrentOpenedProjectPath = ProjectManagerService.CurrentProject?.ProjectPath;
            this.RaisePropertyChanged(x => x.CurrentOpenedProjectPath);
        }

        #endregion

        #region Methods

        private void Initialize()
        {
            if (!Directory.Exists(DriverFolderPath))
                Directory.CreateDirectory(DriverFolderPath);
            DriverNameSource = new List<string>(
                Directory.GetFiles(DriverFolderPath, "*.dll", SearchOption.AllDirectories).ToList().Select(x => Path.GetFileNameWithoutExtension(x)));
            uiSyncContext = SynchronizationContext.Current;
            syncUIUpdateTask = Task.Factory.StartNew(UpdateUI, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default);

            ServerIniPath = ApplicationPath + "\\server.ini";
            ServerConfiguration = new ServerConfiguration(ServerIniPath);
        }

        private void UpdateUI()
        {
            while (!IsDisposed)
            {
                try
                {
                    Stopwatch sw = new Stopwatch();
                    sw.Restart();
                    foreach (var panel in WorkspaceManagerService.GetAllDocumentPanel().ToArray())
                    {
                        if (panel is TagCollectionViewModel tagCollectionVM)
                        {
                            if (tagCollectionVM.IsOpened && tagCollectionVM.Parent is IDeviceCore deviceCore)
                            {
                                foreach (var item in deviceCore.Childs.ToArray())
                                {
                                    if (item is TagCore tag)
                                    {
                                        if (tag.NeedToUpdateValue)
                                        {
                                            tag.NeedToUpdateValue = false;
                                            DispatcherFacade.AddToDispatcherQueue(new Action(() =>
                                            {
                                                tag.RaisePropertyChanged("Value");
                                            }));
                                        }

                                        if (tag.NeedToUpdateTimeStamp)
                                        {
                                            tag.NeedToUpdateTimeStamp = false;
                                            DispatcherFacade.AddToDispatcherQueue(new Action(() =>
                                            {
                                                tag.RaisePropertyChanged("TimeStamp");
                                            }));
                                        }

                                        if (tag.NeedToUpdateQuality)
                                        {
                                            tag.NeedToUpdateQuality = false;
                                            DispatcherFacade.AddToDispatcherQueue(new Action(() =>
                                            {
                                                tag.RaisePropertyChanged("Quality");
                                            }));
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (ProjectManagerService.CurrentProject != null)
                    {
                        foreach (var item in ProjectManagerService.CurrentProject.Childs.ToArray())
                        {
                            if (item is RemoteStation remoteStation)
                            {
                                UpdateRemoteStation(remoteStation);
                            }
                        }
                    }
                    sw.Stop();
                }
                catch { }
                Thread.Sleep(100);
            }
            syncUIUpdateTask.Dispose();
        }

        public void UpdateRemoteStation(RemoteStation remoteStation)
        {
            if (remoteStation != null)
            {
                DispatcherFacade.AddToDispatcherQueue(new Action(() =>
                {
                    remoteStation.RaisePropertyChanged("ConnectionStatus");
                }));
                foreach (var item in remoteStation.Childs)
                {
                    if (item is RemoteStation childRemoteStation)
                    {
                        UpdateRemoteStation(childRemoteStation);
                    }
                }
            }
        }

        public void Dispose()
        {
            IsDisposed = true;
        }

        #endregion
    }
}