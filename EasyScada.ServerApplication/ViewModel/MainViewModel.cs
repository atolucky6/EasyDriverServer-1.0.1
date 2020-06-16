using DevExpress.Mvvm;
using DevExpress.Mvvm.POCO;
using EasyScada.Api.Interfaces;
using EasyScada.Core;
using EasyScada.ServerApplication.Workspace;
using Microsoft.AspNet.SignalR.Client;
using Microsoft.AspNet.SignalR.Client.Transports;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace EasyScada.ServerApplication
{
    public class MainViewModel
    {
        #region Public members

        public ObservableCollection<IWorkspacePanel> Workspaces => WorkspaceManagerService.Workspaces;
        public virtual bool IsBusy { get; set; }
        public IEasyScadaProject CurrentProject => ProjectManagerService.CurrentProject;

        #endregion

        #region Private members

        protected ProjectTreeWorkspaceViewModel ProjectTreeWorkspace { get; set; }
        protected ItemPropertiesWorkspaceViewModel ItemPropertiesWorkspace { get; set; }

        #endregion

        #region Injected services

        protected IWorkspaceManagerService WorkspaceManagerService { get; set; }
        protected IProjectManagerService ProjectManagerService { get; set; }
        protected IReverseService ReverseService { get; set; }

        #endregion

        #region UI services

        public IMessageBoxService MessageBoxService { get => this.GetService<IMessageBoxService>(); }
        public IWindowService WindowService { get => this.GetService<IWindowService>(); }
        public ISaveFileDialogService SaveFileDialogService { get => this.GetService<ISaveFileDialogService>(); }
        public IOpenFileDialogService OpenFileDialogService { get => this.GetService<IOpenFileDialogService>(); }
        public IDispatcherService DispatcherService { get => this.GetService<IDispatcherService>(); }
        public ICurrentWindowService CurrentWindowService { get => this.GetService<ICurrentWindowService>(); }

        #endregion

        #region Constructors

        public MainViewModel(
            IWorkspaceManagerService workspaceManagerService,
            IReverseService reverseService,
            IProjectManagerService projectManagerService)
        {
            WorkspaceManagerService = workspaceManagerService;
            ReverseService = reverseService;
            ProjectManagerService = projectManagerService;
        }

        #endregion

        #region Event handlers

        public virtual void OnLoaded()
        {
            InitializeWorkspaces();
        }

        #endregion

        #region Private methods

        private void InitializeWorkspaces()
        {
            ProjectTreeWorkspace = IoC.Instance.GetPOCOViewModel<ProjectTreeWorkspaceViewModel>(this);
            ItemPropertiesWorkspace = IoC.Instance.GetPOCOViewModel<ItemPropertiesWorkspaceViewModel>(this);
            WorkspaceManagerService.AddPanel(ProjectTreeWorkspace, true);
            WorkspaceManagerService.AddPanel(ItemPropertiesWorkspace, false);
        }

        #endregion

        #region Commands

        #region File category

        /// <summary>
        /// Lệnh tạo mới project
        /// </summary>
        public void New()
        {
            try
            {
                IsBusy = true;
                WindowService.Show("NewProjectView", null, this);
            }
            catch (Exception ex)
            {

            }
            finally { IsBusy = false; }
        }

        /// <summary>
        /// Điều kiện để thực thi lệnh <see cref="New"/>
        /// </summary>
        /// <returns></returns>
        public bool CanNew()
        {
            return !IsBusy;
        }

        /// <summary>
        /// Lệnh mở project
        /// </summary>
        public void Open()
        {
            try
            {
                IsBusy = true;
                WindowService.Show("OpenProjectView", null, this);
            }
            catch (Exception ex)
            {

            }
            finally { IsBusy = false; }
        }

        /// <summary>
        /// Điều kiện để thực thi lệnh <see cref="Open"/>
        /// </summary>
        /// <returns></returns>
        public bool CanOpen()
        {
            return !IsBusy;
        }

        /// <summary>
        /// Lệnh lưu project
        /// </summary>
        /// <returns></returns>
        public async Task Save()
        {
            try
            {
                IsBusy = true;
                await ProjectManagerService.SaveAsync(CurrentProject);
                ReverseService.ClearHistory();
            }
            catch (Exception ex)
            {

            }
            finally { IsBusy = false; }
        }

        /// <summary>
        /// Điều kiện để thực thi lệnh <see cref="Save"/>
        /// </summary>
        /// <returns></returns>
        public bool CanSave()
        {
            return !IsBusy && ProjectManagerService.CurrentProject != null;
        }

        /// <summary>
        /// Lệnh lưu project ở một nơi khác
        /// </summary>
        /// <returns></returns>
        public async Task SaveAs()
        {
            try
            {
                IsBusy = true;
                if (CurrentProject.HasChanges())
                {
                    var mbr = MessageBoxService.ShowMessage(
                        "The current working project has changes. Do you want to save now ?", "Easy Scada", MessageButton.YesNoCancel, MessageIcon.Question);
                    if (mbr == MessageResult.Cancel)
                        return;
                    if (mbr == MessageResult.Yes)
                    {
                        await ProjectManagerService.SaveAsync(CurrentProject);
                        ReverseService.ClearHistory();
                    }

                    IsBusy = false;
                }
                else
                {
                    SaveFileDialogService.Title = "Save as...";
                    SaveFileDialogService.Filter = "Easy Scada Project (*.esprj)|*.esprj";
                    if (SaveFileDialogService.ShowDialog())
                    {
                        IsBusy = true;
                        string projectPath = SaveFileDialogService.File.GetFullName();
                        CurrentProject.Name = Path.GetFileNameWithoutExtension(projectPath);
                        CurrentProject.ProjectPath = projectPath;
                        ReverseService.ClearHistory();
                        await ProjectManagerService.SaveAsync(CurrentProject);
                    }
                }
            }
            catch (Exception ex)
            {

            }
            finally { IsBusy = false; }
        }

        /// <summary>
        /// Điều kiện để thực thi lệnh <see cref="SaveAs"/>
        /// </summary>
        /// <returns></returns>
        public bool CanSaveAs()
        {
            return !IsBusy && ProjectManagerService.CurrentProject != null;
        }

        /// <summary>
        /// Lệnh thoát chương trình
        /// </summary>
        /// <returns></returns>
        public async Task Close()
        {
            if (CurrentProject != null)
            {
                if (CurrentProject.HasChanges())
                {
                    var mbr = MessageBoxService.ShowMessage(
                        "The current working project has changes. Do you want to save now ?", "Easy Scada", MessageButton.YesNoCancel, MessageIcon.Question);
                    if (mbr == MessageResult.Yes)
                        return;
                    if (mbr == MessageResult.Yes)
                    {
                        IsBusy = true;
                        await ProjectManagerService.SaveAsync(CurrentProject);
                        IsBusy = false;
                    }
                }
            }
            DispatcherService.BeginInvoke(() => Application.Current.MainWindow.Close());
        }

        /// <summary>
        /// Điều kiện để thực thi lệnh <see cref="Close"/>
        /// </summary>
        /// <returns></returns>
        public bool CanClose()
        {
            return !IsBusy;
        }

        #endregion

        #region Edit category

        /// <summary>
        /// Lệnh thêm <see cref="RemoteStation"/> vào <see cref="IEasyScadaProject"/>
        /// </summary>
        public void AddStation()
        {

        }

        /// <summary>
        /// Điều kiện để thực thi lệnh <see cref="AddStation"/>
        /// </summary>
        /// <returns></returns>
        public bool CanAddStation()
        {
            return !IsBusy;
        }

        /// <summary>
        /// Lệnh thêm <see cref="IChannel"/> vào <see cref="IStation"/>
        /// </summary>
        public async void AddChannel()
        {
            try
            {
                var hubConnection = new HubConnection("http://localhost:999/easyScada/");

                IHubProxy hubProxy = hubConnection.CreateHubProxy("EasyDriverServerHub");
                await hubConnection.Start(new LongPollingTransport());
                Thread.Sleep(1000);
                var result = await hubProxy.Invoke<string>("getAllStations");
                var stations = JsonConvert.DeserializeObject<List<Station>>(result);
                Thread.Sleep(1000);

                //hubConnection.Dispose();
            }
            catch (Exception ex)
            {

            }
        }
        /// <summary>
        /// Điều kiện để thực thi lệnh <see cref="AddChannel"/>
        /// </summary>
        /// <returns></returns>
        public bool CanAddChannel()
        {
            return !IsBusy;
        }

        /// <summary>
        /// Lệnh thêm <see cref="IDevice"/> vào <see cref="IChannel"/>
        /// </summary>
        public void AddDevice()
        {

        }

        /// <summary>
        /// Điều kiện để thực thi lệnh <see cref="AddDevice"/>
        /// </summary>
        /// <returns></returns>
        public bool CanAddDevice()
        {
            return !IsBusy;
        }

        /// <summary>
        /// Lệnh thêm <see cref="ITag"/> vào <see cref="IDevice"/>
        /// </summary>
        public void AddTag()
        {
            
        }

        /// <summary>
        /// Điều kiện để thực thi lệnh <see cref="AddTag"/>
        /// </summary>
        /// <returns></returns>
        public bool CanAddTag()
        {
            return !IsBusy;
        }

        /// <summary>
        /// Lệnh hoàn tác việc đã làm
        /// </summary>
        public void Undo()
        {

        }

        /// <summary>
        /// Điều kiện để thực thi lệnh <see cref="Undo"/>
        /// </summary>
        /// <returns></returns>
        public bool CanUndo()
        {
            return !IsBusy;
        }

        /// <summary>
        /// Lệnh làm lại việc đã hoàn tác
        /// </summary>
        public void Redo()
        {

        }

        /// <summary>
        /// Điều kiện để thực thi lệnh <see cref="Redo"/>
        /// </summary>
        /// <returns></returns>
        public bool CanRedo()
        {
            return !IsBusy;
        }

        /// <summary>
        /// Lệnh copy một hoặc nhiệu đối tượng
        /// </summary>
        public void Copy()
        {

        }

        /// <summary>
        /// Điều kiện để thực thi lệnh <see cref="Copy"/>
        /// </summary>
        /// <returns></returns>
        public bool CanCopy()
        {
            return !IsBusy;
        }

        /// <summary>
        /// Lệnh cut một hoặc nhiều đối tượng
        /// </summary>
        public void Cut()
        {

        }

        /// <summary>
        /// Điều kiện để thực thi lệnh <see cref="Cut"/>
        /// </summary>
        /// <returns></returns>
        public bool CanCut()
        {
            return !IsBusy;
        }

        /// <summary>
        /// Lệnh paste một hoặc nhiều đối tượng
        /// </summary>
        public void Paste()
        {

        }

        /// <summary>
        /// Điều kiện để thực thi lệnh <see cref="Paste"/>
        /// </summary>
        /// <returns></returns>
        public bool CanPaste()
        {
            return !IsBusy;
        }

        /// <summary>
        /// Lệnh xóa một hoặc nhiều đối tượng
        /// </summary>
        public void Delete()
        {

        }

        /// <summary>
        /// Điều kiện để thực thi lệnh <see cref="Delete"/>
        /// </summary>
        /// <returns></returns>
        public bool CanDelete()
        {
            return !IsBusy;
        }

        #endregion

        #endregion
    }
}
