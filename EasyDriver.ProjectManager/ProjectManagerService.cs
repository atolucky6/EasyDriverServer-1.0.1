using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EasyDriver.Core;
using EasyDriver.ServiceContainer;
using EasyDriver.ServicePlugin;

namespace EasyDriver.ProjectManager
{
    public class ProjectManagerService : EasyServicePlugin, IProjectManagerService
    {
        public ProjectManagerService(IServiceContainer serviceContainer) : base(serviceContainer)
        {
        }

        private IEasyScadaProject currentProject;
        public IEasyScadaProject CurrentProject
        {
            get { return currentProject; }
            set
            {
                if (currentProject != value)
                {
                    var oldProject = currentProject;
                    currentProject = value;
                    UnLockProjectStream();
                    DisposeProjectStream();
                    if (File.Exists(value?.ProjectPath))
                    {
                        CurrentProjectStream = File.Open(value.ProjectPath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                        LockProjectStream();
                    }
                    ProjectChanged?.Invoke(this, new ProjectChangedEventArgs(oldProject, value));
                }
            }
        }
        public FileStream CurrentProjectStream { get; private set; }

        public IEasyScadaProject CreateProject()
        {
            IEasyScadaProject project = new EasyScadaProject();
            project.AcceptChanges();
            return project;
        }

        public async Task<IEasyScadaProject> CreateProjectAsync()
        {
            return await Task.Run(() => CreateProject());
        }

        public IEasyScadaProject OpenProject(string path)
        {
            using (FileStream fs = File.Open(path, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                using (StreamReader sr = new StreamReader(fs))
                {
                    IEasyScadaProject project = CreateProject();
                    //project.Childs.AddRange(ProjectConverter.Deserialize(sr.ReadToEnd(), project));
                    project.ProjectPath = path;
                    project.Name = Path.GetFileNameWithoutExtension(path);
                    project.CreatedDate = File.GetCreationTime(path);
                    project.ModifiedDate = File.GetLastWriteTime(path);
                    project.AcceptChanges();
                    return project;
                }
            }
        }

        public async Task<IEasyScadaProject> OpenProjectAsync(string path)
        {
            return await Task.Run(() => OpenProject(path));
        }

        public void Save(IEasyScadaProject project)
        {
            try
            {
                if (project == null)
                    throw new ArgumentNullException("project");

                if (project == CurrentProject)
                {
                    UnLockProjectStream();
                    DisposeProjectStream();
                    project.ModifiedDate = DateTime.Now;
                    project.AcceptChanges();
                    ProjectSaving?.Invoke(this, new ProjectSavingEventArgs(CurrentProject, project));
                    CurrentProjectStream = File.Open(project.ProjectPath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                    using (StreamWriter sw = new StreamWriter(CurrentProjectStream))
                    {
                        //sw.Write(ProjectConverter.Serialize(project.Childs?.Select(x => x as IStationClient).ToList()));
                    }
                    ProjectSaved?.Invoke(this, new ProjectSavedEventArgs(CurrentProject, project));
                    LockProjectStream();
                }
                else
                {
                    project.ModifiedDate = DateTime.Now;
                    project.AcceptChanges();
                    ProjectSaving?.Invoke(this, new ProjectSavingEventArgs(CurrentProject, project));
                    using (FileStream fs = new FileStream(project.ProjectPath, FileMode.OpenOrCreate))
                    {
                        using (StreamWriter sw = new StreamWriter(fs))
                        {
                            //sw.Write(ProjectConverter.Serialize(project.Childs?.Select(x => x as IStationClient).ToList()));
                        }
                    }
                    ProjectSaved?.Invoke(this, new ProjectSavedEventArgs(CurrentProject, project));
                }
            }
            catch (Exception) { }
        }

        public Task SaveAsync(IEasyScadaProject project)
        {
            throw new NotImplementedException();
        }

        public event EventHandler<ProjectChangedEventArgs> ProjectChanged;
        public event EventHandler<ProjectSavedEventArgs> ProjectSaved;
        public event EventHandler<ProjectSavingEventArgs> ProjectSaving;

        private void LockProjectStream() => CurrentProjectStream?.Lock(CurrentProjectStream.Position, CurrentProjectStream.Length);
        private void UnLockProjectStream() => CurrentProjectStream?.Unlock(CurrentProjectStream.Position, CurrentProjectStream.Length);
        private void DisposeProjectStream() => CurrentProjectStream?.Dispose();

        public override void BeginInit()
        {
        }

        public override void EndInit()
        {
        }
    }
}
