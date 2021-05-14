using DevExpress.Mvvm;
using DevExpress.Mvvm.POCO;
using EasyDriver.Core;
using System;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace EasyScada.ServerApplication
{
    /// <summary>
    /// The view model to handle create new project
    /// </summary>
    public class NewProjectViewModel : IDataErrorInfo
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

        /// <summary>
        /// The key to get default path was stored in settings
        /// </summary>
        const string DefaultSaveProjectPathKey = "DefaultSaveProjectPath";

        /// <summary>
        /// The key to get allow save project value was stored in settings
        /// </summary>
        const string AllowSaveProjectPathKey = "AllowSaveProjectPath";

        #endregion

        #region Inject services

        protected IProjectManagerService ProjectManagerService { get; set; }
        protected IWorkspaceManagerService WorkspaceManagerService { get; set; }
        protected IReverseService ReverseService { get; set; }

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
        public NewProjectViewModel(
            IProjectManagerService projectManagerService,
            IWorkspaceManagerService workspaceManagerService,
            IReverseService reverseService)
        {
            Title = "New project";
            SizeToContent = SizeToContent.Height;
            ProjectManagerService = projectManagerService;
            WorkspaceManagerService = workspaceManagerService;
            ReverseService = reverseService;
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
                        $"The project with this name '{Name}' is already existed. Please try another name !", "AHD Scada", MessageButton.OK, MessageIcon.Warning);
                    return;
                }

                //Check the current working project is not null and it has changes
                if (ProjectManagerService.CurrentProject != null && ProjectManagerService.CurrentProject.HasChanges())
                {
                    //Ask the user want to save the current working project or not
                    var mbr = MessageBoxService.ShowMessage("The current working project has changes. " +
                        "Do you want to save now ?", "AHD Scada", MessageButton.YesNoCancel, MessageIcon.Question);
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
                IEasyScadaProject project = await ProjectManagerService.CreateProjectAsync(Path);
                ProjectManagerService.CurrentProject = project;

                //Save the path if require
                if (AllowSaveProjectPath)
                {
                    EasyDriver.ServerApplication.Properties.Settings.Default["DefaultSaveProjectPath"] = CurrentSelectedPath;
                    EasyDriver.ServerApplication.Properties.Settings.Default.Save();
                }
                //Clear all document of previous project
                WorkspaceManagerService.RemoveAllDocumentPanel();
                //Clear Undo & Redo history
                ReverseService.ClearHistory();
                //Close the view
                CurrentWindowService.Close();
            }
            catch (Exception ex)
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
                CurrentSelectedPath = FolderBrowserDialogService.ResultPath;
            this.RaisePropertyChanged(x => Path);
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
        /// The event callback when AllowSaveProjectPath changed
        /// </summary>
        public virtual void OnAllowSaveProjectPathChanged()
        {
            EasyDriver.ServerApplication.Properties.Settings.Default["AllowSaveProjectPath"] = AllowSaveProjectPath;
            EasyDriver.ServerApplication.Properties.Settings.Default.Save();
        }

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
            AllowSaveProjectPath = Convert.ToBoolean(EasyDriver.ServerApplication.Properties.Settings.Default["AllowSaveProjectPath"]);
            //Retrive the previous path from settingss
            CurrentSelectedPath = EasyDriver.ServerApplication.Properties.Settings.Default["DefaultSaveProjectPath"].ToString();
            this.RaisePropertyChanged(x => Path);
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
