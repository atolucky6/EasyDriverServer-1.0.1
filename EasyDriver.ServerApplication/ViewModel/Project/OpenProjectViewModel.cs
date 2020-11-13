using DevExpress.Mvvm;
using DevExpress.Mvvm.POCO;
using EasyDriver.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace EasyScada.ServerApplication
{
    public class OpenProjectViewModel
    {
        #region Inner classes

        public class RecentOpenProjectModel
        {
            public string Name { get; set; }
            public string Path { get; set; }
            public DateTime ModifiedDate { get; set; }
            public DateTime CreatedDate { get; set; }
        }

        #endregion

        #region Constructors

        public OpenProjectViewModel(
            IProjectManagerService projectManagerService,
            IWorkspaceManagerService workspaceManagerService,
            IReverseService reverseService)
        {
            ProjectManagerService = projectManagerService;
            WorkspaceManagerService = workspaceManagerService;
            ReverseService = reverseService;

            Title = "Open project";
            SizeToContent = SizeToContent.Manual;
            Width = 800;
            Height = 400;
        }

        #endregion

        #region Public members

        public string Title { get; set; }
        public SizeToContent SizeToContent { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public virtual ObservableCollection<RecentOpenProjectModel> RecentOpenProjects { get; set; }
        public virtual RecentOpenProjectModel SelectedItem { get; set; }
        public virtual bool IsBusy { get; set; }

        #endregion

        #region Injected services

        protected IProjectManagerService ProjectManagerService { get; set; }
        protected IWorkspaceManagerService WorkspaceManagerService { get; set; }
        protected IReverseService ReverseService { get; set; }

        #endregion

        #region UI services

        protected ICurrentWindowService CurrentWindowService { get => this.GetService<ICurrentWindowService>(); }
        protected IMessageBoxService MessageBoxService { get => this.GetService<IMessageBoxService>(); }
        protected IOpenFileDialogService OpenFileDialogService { get => this.GetService<IOpenFileDialogService>(); }

        #endregion

        #region Event handlers

        /// <summary>
        /// The event callback when the view was loaded
        /// </summary>
        public virtual void OnLoaded()
        {
            //Create new recent project collection
            RecentOpenProjects = new ObservableCollection<RecentOpenProjectModel>();
            //Restore the recent project collection from settings
            JsonConvert.DeserializeObject<List<RecentOpenProjectModel>>(Properties.Settings.Default["RecentOpenProjects"].ToString())?.ForEach(x => RecentOpenProjects.Add(x));
        }

        /// <summary>
        /// The event callbacck when the view was unloaded
        /// </summary>
        public virtual void OnUnloaded()
        {
            //Save the recent project collection to settings when the view is closing
            Properties.Settings.Default["RecentOpenProjects"] = JsonConvert.SerializeObject(RecentOpenProjects.ToList());
            Properties.Settings.Default.Save();
        }

        public async virtual Task OpenOnDoubleClick(RecentOpenProjectModel projectInfo)
        {
            try
            {
                if (projectInfo != null && !IsBusy)
                {
                    if (!File.Exists(SelectedItem.Path))
                    {
                        MessageBoxService.ShowMessage($"The selected project '{SelectedItem.Name} can't be found under the specified path. " +
                            $"After this message the project will remove from the recent open projects."
                            , "Easy Scada", MessageButton.OK, MessageIcon.Information);
                        RecentOpenProjects.Remove(SelectedItem);
                    }
                    else
                    {
                        //Check the current working project is not null and it has changes
                        if (ProjectManagerService.CurrentProject != null && ProjectManagerService.CurrentProject.HasChanges())
                        {
                            //Ask the user want to save the current working project or not
                            var mbr = MessageBoxService.ShowMessage("The current working project has changes. " +
                                "Do you want to save now ?", "Easy Scada", MessageButton.YesNoCancel, MessageIcon.Question);
                            //If choose cancel just return
                            if (mbr == MessageResult.Cancel)
                                return;
                            //If choose yes then save the current project before open another project
                            if (mbr == MessageResult.Yes)
                            {
                                //Block the view
                                IsBusy = true;
                                //Save the current working project
                                await ProjectManagerService.SaveAsync(ProjectManagerService.CurrentProject);
                            }
                        }
                        IsBusy = true;
                        IEasyScadaProject openedProject = await ProjectManagerService.OpenProjectAsync(SelectedItem.Path);
                        if (openedProject != null)
                        {
                            ProjectManagerService.CurrentProject = openedProject;
                            WorkspaceManagerService.RemoveAllDocumentPanel();
                            Messenger.Default.Send(new HidePropertiesMessage(this));
                            ReverseService.ClearHistory();
                            CurrentWindowService.Close();
                        }
                        else
                        {

                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            finally
            {
                //Release the view
                IsBusy = false;
            }
        }

        #endregion

        #region Commands

        /// <summary>
        /// The command to open the project and load it into the editor asynchronously
        /// </summary>
        /// <returns></returns>
        public async Task Open()
        {
            try
            {
                if (!File.Exists(SelectedItem.Path))
                {
                    MessageBoxService.ShowMessage($"The selected project '{SelectedItem.Name} can't be found under the specified path. " +
                        $"After this message the project will remove from the recent open projects."
                        , "Easy Scada", MessageButton.OK, MessageIcon.Information);
                    RecentOpenProjects.Remove(SelectedItem);
                }
                else
                {
                    //Check the current working project is not null and it has changes
                    if (ProjectManagerService.CurrentProject != null && ProjectManagerService.CurrentProject.HasChanges())
                    {
                        //Ask the user want to save the current working project or not
                        var mbr = MessageBoxService.ShowMessage("The current working project has changes. " +
                            "Do you want to save now ?", "Easy Scada", MessageButton.YesNoCancel, MessageIcon.Question);
                        //If choose cancel just return
                        if (mbr == MessageResult.Cancel)
                            return;
                        //If choose yes then save the current project before open another project
                        if (mbr == MessageResult.Yes)
                        {
                            //Block the view
                            IsBusy = true;
                            //Save the current working project
                            await ProjectManagerService.SaveAsync(ProjectManagerService.CurrentProject);
                        }
                    }
                    
                    //Block the view
                    IsBusy = true;
                    IEasyScadaProject openedProject = await ProjectManagerService.OpenProjectAsync(SelectedItem.Path);
                    if (openedProject != null)
                    {
                        ProjectManagerService.CurrentProject = openedProject;
                        WorkspaceManagerService.RemoveAllDocumentPanel();
                        Messenger.Default.Send(new HidePropertiesMessage(this));
                        ReverseService.ClearHistory();
                        CurrentWindowService.Close();
                    }
                    else
                    {

                    }
                }
            }
            catch (Exception)
            {
            }
            finally
            {
                //Release the view
                IsBusy = false;
            }
        }

        /// <summary>
        /// Condition to perform Open command
        /// </summary>
        /// <returns></returns>
        public bool CanOpen() => SelectedItem != null && !IsBusy;

        /// <summary>
        /// The command to open the browse to choose the path to the project
        /// </summary>
        public async Task Browse()
        {
            try
            {
                //Set title and filter for browse dialog
                OpenFileDialogService.Title = "Open project";
                OpenFileDialogService.Filter = "Easy Scada Project (*.json)|*.json";

                //Show browse dialog 
                if (OpenFileDialogService.ShowDialog())
                {

                    //Get file path
                    string filePath = OpenFileDialogService.File.GetFullName();
                    //Create project info
                    var projectInfo = new RecentOpenProjectModel
                    {
                        Name = Path.GetFileNameWithoutExtension(filePath),
                        CreatedDate = File.GetCreationTime(filePath),
                        ModifiedDate = Directory.GetLastWriteTime(filePath),
                        Path = filePath
                    };

                    //Check the current working project is not null and it has changes
                    if (ProjectManagerService.CurrentProject != null && ProjectManagerService.CurrentProject.HasChanges())
                    {
                        //Ask the user want to save the current working project or not
                        var mbr = MessageBoxService.ShowMessage("The current working project has changes. " +
                            "Do you want to save now ?", "Easy Scada", MessageButton.YesNoCancel, MessageIcon.Question);
                        //If choose cancel just return
                        if (mbr == MessageResult.Cancel)
                            return;
                        //If choose yes then save the current project before open another project
                        if (mbr == MessageResult.Yes)
                        {
                            //Block the view
                            IsBusy = true;
                            //Save the current working project
                            await ProjectManagerService.SaveAsync(ProjectManagerService.CurrentProject);
                        }
                    }

                    //Block the view
                    IsBusy = true;
                    IEasyScadaProject openedProject = await ProjectManagerService.OpenProjectAsync(filePath);
                    if (openedProject != null)
                    {
                        //Add the project info if it not exists
                        if (RecentOpenProjects.FirstOrDefault(x => x.Name == projectInfo.Name && x.Path == projectInfo.Path) == null)
                            RecentOpenProjects.Add(projectInfo);
                        ProjectManagerService.CurrentProject = openedProject;
                        WorkspaceManagerService.RemoveAllDocumentPanel();
                        Messenger.Default.Send(new HidePropertiesMessage(this));
                        ReverseService.ClearHistory();
                        CurrentWindowService.Close();
                    }
                    else
                    {

                    }
                }
            }
            catch (Exception ex)
            {
            }
            finally
            {
                //Release the view
                IsBusy = false;
            }
        }

        /// <summary>
        /// Condition to perform Browse command
        /// </summary>
        /// <returns></returns>
        public bool CanBrowse() => !IsBusy;

        /// <summary>
        /// The command to remove the current selected <see cref="ProjectInfo"/> on the collection view
        /// </summary>
        public void Remove() => RecentOpenProjects.Remove(SelectedItem);

        /// <summary>
        /// Condition to perform Remove command
        /// </summary>
        /// <returns></returns>
        public bool CanRemove() => SelectedItem != null && !IsBusy;

        /// <summary>
        /// The command to close the view
        /// </summary>
        public void Close() => CurrentWindowService.Close();

        /// <summary>
        /// Condition to perform Close command
        /// </summary>
        /// <returns></returns>
        public bool CanClose() => !IsBusy;

        #endregion
    }
}
