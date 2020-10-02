using DevExpress.Mvvm;
using DevExpress.Mvvm.POCO;
using DevExpress.Xpf.Core;
using EasyDriver.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace EasyDriver.ProjectMenu
{
    public class RecentProjectsViewModel
    {
        #region Constructors

        public RecentProjectsViewModel(EasyProjectMenuPlugin menuPlugin)
        {
            MenuPlugin = menuPlugin;

            var fixedItems = menuPlugin.RecentOpenProjects.Where(x => x.Pined);
            FixedItems.AddRange(fixedItems);
        }

        #endregion

        #region Public members

        public virtual List<RecentOpenProjectModel> FixedItems { get; set; } = new List<RecentOpenProjectModel>();
        public virtual RecentOpenProjectModel SelectedItem { get; set; }
        public virtual bool IsBusy { get; set; }

        #endregion

        #region Injected services

        public EasyProjectMenuPlugin MenuPlugin { get; set; }

        #endregion

        #region UI services

        protected ICurrentWindowService CurrentWindowService { get => this.GetService<ICurrentWindowService>(); }
        protected IMessageBoxService MessageBoxService { get => this.GetService<IMessageBoxService>(); }
        protected IOpenFileDialogService OpenFileDialogService { get => MenuPlugin.openFileDialogService; }

        #endregion

        #region Event handlers

        /// <summary>
        /// The event callback when the view was loaded
        /// </summary>
        public virtual void OnLoaded()
        {
        }

        /// <summary>
        /// The event callbacck when the view was unloaded
        /// </summary>
        public virtual void OnUnloaded()
        {
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
                        MenuPlugin.RecentOpenProjects.Remove(SelectedItem);
                    }
                    else
                    {
                        //Check the current working project is not null and it has changes
                        if (MenuPlugin.projectManagerService.CurrentProject != null && MenuPlugin.projectManagerService.CurrentProject.HasChanges())
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
                                await MenuPlugin.projectManagerService.SaveAsync(MenuPlugin.projectManagerService.CurrentProject);
                            }
                        }
                        IsBusy = true;
                        IEasyScadaProject openedProject = await MenuPlugin.projectManagerService.OpenProjectAsync(SelectedItem.Path);
                        if (openedProject != null)
                        {
                            MenuPlugin.projectManagerService.CurrentProject = openedProject;
                            MenuPlugin.workspaceManagerService.RemoveAllDocumentPanel();
                            MenuPlugin.reverseService.ClearHistory();
                            CurrentWindowService.Close();
                        }
                        else
                        {
                            DXMessageBox.Show("Can't open project file.", MenuPlugin.applicationSyncService.MessageTitle,
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
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
                    MenuPlugin.RecentOpenProjects.Remove(SelectedItem);
                }
                else
                {
                    //Check the current working project is not null and it has changes
                    if (MenuPlugin.projectManagerService.CurrentProject != null && MenuPlugin.projectManagerService.CurrentProject.HasChanges())
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
                            await MenuPlugin.projectManagerService.SaveAsync(MenuPlugin.projectManagerService.CurrentProject);
                        }
                    }

                    //Block the view
                    IsBusy = true;
                    IEasyScadaProject openedProject = await MenuPlugin.projectManagerService.OpenProjectAsync(SelectedItem.Path);
                    if (openedProject != null)
                    {
                        MenuPlugin.projectManagerService.CurrentProject = openedProject;
                        MenuPlugin.workspaceManagerService.RemoveAllDocumentPanel();
                        MenuPlugin.reverseService.ClearHistory();
                        CurrentWindowService.Close();
                    }
                    else
                    {
                        DXMessageBox.Show("Can't open project file.", MenuPlugin.applicationSyncService.MessageTitle,
                             MessageBoxButton.OK, MessageBoxImage.Warning);
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
                OpenFileDialogService.Filter = "Easy Scada Project (*.esprj)|*.esprj";

                //Show browse dialog 
                if (OpenFileDialogService.ShowDialog())
                {
                    //Get file path
                    string projectPath = OpenFileDialogService.File.GetFullName();

                    //Check the current working project is not null and it has changes
                    if (MenuPlugin.projectManagerService.CurrentProject != null && MenuPlugin.projectManagerService.CurrentProject.HasChanges())
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
                            await MenuPlugin.projectManagerService.SaveAsync(MenuPlugin.projectManagerService.CurrentProject);
                        }
                    }

                    //Block the view
                    IsBusy = true;
                    IEasyScadaProject openedProject = await MenuPlugin.projectManagerService.OpenProjectAsync(projectPath);
                    if (openedProject != null)
                    {
                        MenuPlugin.projectManagerService.CurrentProject = openedProject;
                        MenuPlugin.workspaceManagerService.RemoveAllDocumentPanel();
                        MenuPlugin.reverseService.ClearHistory();
                        CurrentWindowService.Close();
                    }
                    else
                    {
                        DXMessageBox.Show("Can't open project file.", MenuPlugin.applicationSyncService.MessageTitle,
                             MessageBoxButton.OK, MessageBoxImage.Warning);
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
        /// Condition to perform Browse command
        /// </summary>
        /// <returns></returns>
        public bool CanBrowse() => !IsBusy;

        /// <summary>
        /// The command to remove the current selected <see cref="ProjectInfo"/> on the collection view
        /// </summary>
        public void Remove() => MenuPlugin.RecentOpenProjects.Remove(SelectedItem);

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
