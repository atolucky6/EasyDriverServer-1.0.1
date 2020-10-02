using EasyDriver.Core;
using EasyDriver.ServicePlugin;
using System;
using System.Threading.Tasks;

namespace EasyDriver.ProjectManager
{
    public interface IProjectManagerService : IEasyServicePlugin
    {
        IEasyScadaProject CurrentProject { get; set; }

        void Save(IEasyScadaProject project);
        Task SaveAsync(IEasyScadaProject project);
        IEasyScadaProject OpenProject(string path);
        Task<IEasyScadaProject> OpenProjectAsync(string path);
        IEasyScadaProject CreateProject();
        Task<IEasyScadaProject> CreateProjectAsync();

        event EventHandler<ProjectChangedEventArgs> ProjectChanged;
        event EventHandler<ProjectSavedEventArgs> ProjectSaved;
        event EventHandler<ProjectSavingEventArgs> ProjectSaving;
    }
}
