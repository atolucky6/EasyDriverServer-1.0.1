using EasyDriver.ServicePlugin;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EasyDriver.Service.ProjectManager
{
    public interface IProjectManagerService : IEasyServicePlugin
    {
        IProjectItem CurrentProject { get; set; }
        void Save(IProjectItem project);
        Task SaveAsync(IProjectItem project);
        IProjectItem CreateProject(string path);
        Task<IProjectItem> CreateProjectAsync(string path);
        IProjectItem OpenProject(string path);
        Task<IProjectItem> OpenProjectAsync(string path);

        event EventHandler<ProjectChangedEventArgs> ProjectChanged;
        event EventHandler<ProjectSavedEventArgs> ProjectSaved;
        event EventHandler<ProjectSavingEventArgs> ProjectSaving;
    }

    public class ProjectChangedEventArgs : EventArgs
    {
        public IProjectItem OldProject { get; private set; }
        public IProjectItem NewProject { get; private set; }

        public ProjectChangedEventArgs(IProjectItem oldProject, IProjectItem newProject)
        {
            OldProject = oldProject;
            NewProject = newProject;
        }
    }

    public class ProjectSavingEventArgs : EventArgs
    {
        public IProjectItem OldProject { get; private set; }
        public IProjectItem NewProject { get; private set; }

        public ProjectSavingEventArgs(IProjectItem oldPorject, IProjectItem newProject)
        {
            OldProject = oldPorject;
            NewProject = newProject;
        }
    }

    public class ProjectSavedEventArgs : EventArgs
    {
        public IProjectItem OldProject { get; private set; }
        public IProjectItem NewProject { get; private set; }

        public ProjectSavedEventArgs(IProjectItem oldPorject, IProjectItem newProject)
        {
            OldProject = oldPorject;
            NewProject = newProject;
        }
    }
}
