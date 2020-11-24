using DevExpress.Mvvm;
using DevExpress.Mvvm.POCO;
using EasyDriver.Service.ApplicationProperties;
using EasyDriver.Service.InternalStorage;
using EasyDriver.Service.ProjectManager;
using EasyDriver.Service.Reversible;
using EasyDriverPlugin;
using EasyScada.WorkspaceManager;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace EasyDriver.Workspace.Main
{
    /// <summary>
    /// The view model to handle create new project
    /// </summary>
    public class NewProjectViewModel : IDataErrorInfo, ISupportParentViewModel
    {
        #region Members

        /// <summary>
        /// The name of the new project
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// The full path of the new project
        /// </summary>
        public virtual string Path
        {
            get
            {
                if (string.IsNullOrEmpty(CurrentSelectedPath))
                    return string.Empty;
                else
                    return $"{CurrentSelectedPath}\\{Name?.Trim()}.json";
            }
        }

        /// <summary>
        /// The title of the view
        /// </summary>
        public string Title { get; set; }

        public SizeToContent SizeToContent { get; set; }

        public double Width { get; set; }

        public double Height { get; set; }

        /// <summary>
        /// Whether the view model is busy
        /// </summary>
        public virtual bool IsBusy { get; set; }

        /// <summary>
        /// Allow save the project path to settings
        /// </summary>
        public virtual bool AllowSaveProjectPath { get; set; }

        /// <summary>
        /// The path was selected from browse
        /// </summary>
        protected string CurrentSelectedPath { get; set; }

        public List<string> FilesInFolder { get; set; }
        public object ParentViewModel { get; set; }
        public ProjectTreeViewModel ProjectTreeViewModel { get => ParentViewModel as ProjectTreeViewModel; }

        #endregion

        #region Injected services
        protected IProjectManagerService ProjectManagerService { get => ProjectTreeViewModel.ProjectManagerService; }
        protected IWorkspaceManagerService WorkspaceManagerService { get => ProjectTreeViewModel.WorkspaceManagerService; }
        protected IReversibleService ReversibleService { get => ProjectTreeViewModel.ReversibleService; }
        protected IInternalStorageService InternalStorageService { get => ProjectTreeViewModel.InternalStorageService; }
        protected IApplicationPropertiesService ApplicationPropertiesService { get => ProjectTreeViewModel.ApplicationPropertiesService; }
        #endregion

        #region UI services
        protected ICurrentWindowService CurrentWindowService { get => this.GetService<ICurrentWindowService>(); }
        protected IFolderBrowserDialogService FolderBrowserDialogService { get => this.GetService<IFolderBrowserDialogService>(); }
        protected IMessageBoxService MessageBoxService { get => this.GetService<IMessageBoxService>(); }
        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public NewProjectViewModel()
        {
            FilesInFolder = new List<string>();
            Title = "New project";
            SizeToContent = SizeToContent.Height;
            Width = 800;
            Height = double.NaN;
        }

        #endregion

        #region Commands

        /// <summary>
        /// The command to accept create new project
        /// </summary>
        /// <returns></returns>
        public async Task Accept()
        {
            try
            {
                //Check the path of the new project is not exits before
                if (File.Exists(Path))
                {
                    //Display the error to user
                    MessageBoxService.ShowMessage(
                        $"The project with this name '{Name}' is already existed. Please try another name !", "Warning", MessageButton.OK, MessageIcon.Warning);
                    return;
                }

                //Check the current working project is not null and it has changes
                if (ProjectManagerService.CurrentProject != null && ProjectManagerService.CurrentProject.HasChanges())
                {
                    //Ask the user want to save the current working project or not
                    var mbr = MessageBoxService.ShowMessage("The current working project has changes. " +
                        "Do you want to save now ?", "Message", MessageButton.YesNoCancel, MessageIcon.Question);
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
                //Create new project
                IProjectItem project = await ProjectManagerService.CreateProjectAsync(Path);
                ProjectManagerService.CurrentProject = project;

                //Save the path if require
                if (AllowSaveProjectPath)
                    ApplicationPropertiesService.SetValue("OpenProjectDirectory", CurrentSelectedPath);
                ApplicationPropertiesService.SetValue("AllowSaveProjectPath", AllowSaveProjectPath.ToString());
                ApplicationPropertiesService.Save();

                //Clear all document of previous project
                WorkspaceManagerService.RemoveAllDocumentPanel();
                //Clear Undo & Redo history
                ReversibleService.ClearHistory();
                //Close the view
                CurrentWindowService.Close();
            }
            catch (Exception)
            {
                CurrentWindowService.Close();
            }
            finally
            {
                //Release the view
                IsBusy = false;
            }
        }

        /// <summary>
        /// Condition to perform Accept command
        /// </summary>
        /// <returns></returns>
        public bool CanAccept() => string.IsNullOrEmpty(Error) && !string.IsNullOrEmpty(CurrentSelectedPath) && !IsBusy;

        /// <summary>
        /// The command to browse the path of the new project
        /// </summary>
        public void Browse()
        {
            //Set start path if it exists
            if (!string.IsNullOrEmpty(CurrentSelectedPath))
                FolderBrowserDialogService.StartPath = CurrentSelectedPath;
            //Show browse dialog
            if (FolderBrowserDialogService.ShowDialog())
            {
                CurrentSelectedPath = FolderBrowserDialogService.ResultPath;
                FilesInFolder = Directory.GetFiles(CurrentSelectedPath).Select(x => System.IO.Path.GetFileNameWithoutExtension(x)).ToList();
            }
            this.RaisePropertyChanged(x => Path);
            this.RaisePropertyChanged(x => Name);
        }
        //public bool CanBrowse() => !IsBusy;

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

        #region Events

        /// <summary>
        /// The event callback when Name property changed
        /// </summary>
        public virtual void OnNameChanged() => this.RaisePropertyChanged(x => x.Path);

        /// <summary>
        /// The event callback when the view has loaded
        /// </summary>    
        public virtual void OnLoaded()
        {
            //Clear the name
            Name = string.Empty;
            //Get the allow save project property value from settings
            if (ApplicationPropertiesService.TryGetValue("AllowSaveProjectPath", out bool allowSave))
                AllowSaveProjectPath = allowSave;

            //Retrive the previous path from settingss
            if (ApplicationPropertiesService.TryGetValue("OpenProjectDirectory", out string directory))
            {
                if (!string.IsNullOrEmpty(directory) && Directory.Exists(directory))
                    CurrentSelectedPath = directory;
            }

            if (Directory.Exists(CurrentSelectedPath))
                FilesInFolder = Directory.GetFiles(CurrentSelectedPath).Select(x => System.IO.Path.GetFileNameWithoutExtension(x)).ToList();

            this.RaisePropertyChanged(x => Path);
        }

        #endregion

        #region IDataErrorInfo

        public string Error { get; private set; }
        public string this[string columnName]
        {
            get
            {
                Error = string.Empty;
                switch (columnName)
                {
                    case nameof(Name):
                        Error = Name.ValidateFileName();
                        if (FilesInFolder.Any(x => x == Name))
                            Error = $"The project name '{Name}' is already existed";
                        break;
                    case nameof(Path):
                        if (!Directory.Exists(CurrentSelectedPath))
                            Error = "Current directory doesn't exists";
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
