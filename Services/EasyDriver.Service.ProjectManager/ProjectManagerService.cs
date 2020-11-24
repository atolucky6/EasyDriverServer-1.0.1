using EasyDriver.Service.DriverManager;
using EasyDriver.Service.RemoteConnectionManager;
using EasyDriver.ServicePlugin;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace EasyDriver.Service.ProjectManager
{
    [Service(ushort.MaxValue, true)]
    public class ProjectManagerService : EasyServicePluginBase, IProjectManagerService
    {
        public IDriverManagerService DriverManagerService { get; set; }
        public IRemoteConnectionManagerService RemoteConnectionManagerService { get; set; }

        private IProjectItem currentProject;
        public IProjectItem CurrentProject
        {
            get { return currentProject; }
            set
            {
                if (currentProject != value)
                {
                    var oldProject = currentProject;
                    currentProject = value;
                    ProjectChanged?.Invoke(this, new ProjectChangedEventArgs(oldProject, value));
                    if (File.Exists(currentProject.ProjectPath))
                        SaveStartupPath(currentProject.ProjectPath);
                }
            }
        }

        public IProjectItem CreateProject(string path)
        {
            IProjectItem project = new ProjectItem
            {
                ProjectPath = path,
                Name = Path.GetFileNameWithoutExtension(path)
            };
            project.AcceptChanges();
            Save(project);
            return project;
        }

        public Task<IProjectItem> CreateProjectAsync(string path)
        {
            return Task.Run(() =>
            {
                return CreateProject(path);
            });
        }

        public void Save(IProjectItem project)
        {
            try
            {
                if (project != null)
                {
                    project.ModifiedDate = DateTime.Now;
                    project.AcceptChanges();
                    ProjectSaving?.Invoke(this, new ProjectSavingEventArgs(CurrentProject, project));
                    var converter = new CoreItemJsonConverter(DriverManagerService, RemoteConnectionManagerService);
                    string resJson = JsonConvert.SerializeObject(project, Formatting.Indented, converter);
                    File.WriteAllText(project.ProjectPath, resJson);
                    ProjectSaved?.Invoke(this, new ProjectSavedEventArgs(CurrentProject, project));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        public Task SaveAsync(IProjectItem project)
        {
            return Task.Run(() => Save(project));
        }

        public IProjectItem OpenProject(string path)
        {
            string resJson = File.ReadAllText(path);
            IProjectItem project = JsonConvert.DeserializeObject<IProjectItem>(resJson, new CoreItemJsonConverter(DriverManagerService, RemoteConnectionManagerService));
            project.ProjectPath = path;
            project.Name = Path.GetFileNameWithoutExtension(path);
            project.CreatedDate = File.GetCreationTime(path);
            project.ModifiedDate = File.GetLastWriteTime(path);
            project.AcceptChanges();
            return project;
        }

        public Task<IProjectItem> OpenProjectAsync(string path)
        {
            return Task.Run(() => OpenProject(path));
        }

        private void SaveStartupPath(string startUpPath)
        {
            string filePath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\startup.ini";
            File.WriteAllText(filePath, startUpPath);
        }

        public event EventHandler<ProjectChangedEventArgs> ProjectChanged;
        public event EventHandler<ProjectSavedEventArgs> ProjectSaved;
        public event EventHandler<ProjectSavingEventArgs> ProjectSaving;

        public override void EndInit()
        {
            DriverManagerService = ServiceContainer.GetService<IDriverManagerService>();
            RemoteConnectionManagerService = ServiceContainer.GetService<IRemoteConnectionManagerService>();
        }
    }
}
