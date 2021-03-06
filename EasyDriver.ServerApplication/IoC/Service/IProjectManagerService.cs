﻿using EasyDriver.Core;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace EasyScada.ServerApplication
{
    public interface IProjectManagerService
    {
        IEasyScadaProject CurrentProject { get; set; }
        void Save(IEasyScadaProject project);
        Task SaveAsync(IEasyScadaProject project);
        IEasyScadaProject CreateProject(string path);
        Task<IEasyScadaProject> CreateProjectAsync(string path);
        IEasyScadaProject OpenProject(string path);
        Task<IEasyScadaProject> OpenProjectAsync(string path);

        event EventHandler<ProjectChangedEventArgs> ProjectChanged;
        event EventHandler<ProjectSavedEventArgs> ProjectSaved;
        event EventHandler<ProjectSavingEventArgs> ProjectSaving;
    }

    public class ProjectManagerService : IProjectManagerService
    {
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
                    ProjectChanged?.Invoke(this, new ProjectChangedEventArgs(oldProject, value));
                    if (File.Exists(currentProject.ProjectPath))
                        SaveStartupPath(currentProject.ProjectPath);
                }
            }
        }

        public IEasyScadaProject CreateProject(string path)
        {
            IEasyScadaProject project = new EasyScadaProject
            {
                ProjectPath = path,
                Name = Path.GetFileNameWithoutExtension(path)
            };
            project.AcceptChanges();
            Save(project);
            return project;
        }

        public Task<IEasyScadaProject> CreateProjectAsync(string path)
        {
            return Task.Run(() =>
            {
                return CreateProject(path);
            });
        }

        public void Save(IEasyScadaProject project)
        {
            try
            {
                if (project != null)
                {
                    project.ModifiedDate = DateTime.Now;
                    project.AcceptChanges();
                    ProjectSaving?.Invoke(this, new ProjectSavingEventArgs(CurrentProject, project));
                    var converter = new EasyScadaProjectJsonConverter();
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

        public Task SaveAsync(IEasyScadaProject project)
        {
            return Task.Run(() => Save(project));
        }

        public IEasyScadaProject OpenProject(string path)
        {
            string resJson = File.ReadAllText(path);
            IEasyScadaProject project = JsonConvert.DeserializeObject<IEasyScadaProject>(resJson, new EasyScadaProjectJsonConverter());
            project.ProjectPath = path;
            project.Name = Path.GetFileNameWithoutExtension(path);
            project.CreatedDate = File.GetCreationTime(path);
            project.ModifiedDate = File.GetLastWriteTime(path);
            project.AcceptChanges();
            return project;
        }

        public Task<IEasyScadaProject> OpenProjectAsync(string path)
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
    }

    //public class ProjectManagerService : IProjectManagerService
    //{
    //    readonly BinaryFormatter bf = new BinaryFormatter();
    //    public FileStream CurrentProjectStream { get; private set; }

    //    private IEasyScadaProject currentProject;
    //    public IEasyScadaProject CurrentProject
    //    {
    //        get { return currentProject; }
    //        set
    //        {
    //            if (currentProject != value)
    //            {
    //                var oldProject = currentProject;
    //                currentProject = value;
    //                UnLockProjectStream();
    //                DisposeProjectStream();
    //                if (File.Exists(value.ProjectPath))
    //                {
    //                    CurrentProjectStream = File.Open(value.ProjectPath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
    //                    LockProjectStream();
    //                }
    //                ProjectChanged?.Invoke(this, new ProjectChangedEventArgs(oldProject, value));
    //            }
    //        }
    //    }

    //    public IEasyScadaProject CreateProject(string path)
    //    {
    //        IEasyScadaProject project = new EasyScadaProject
    //        {
    //            ProjectPath = path,
    //            Name = Path.GetFileNameWithoutExtension(path)
    //        };
    //        project.AcceptChanges();
    //        Save(project);
    //        return project;
    //    }

    //    public Task<IEasyScadaProject> CreateProjectAsync(string path)
    //    {
    //        return Task.Run(() =>
    //        {
    //            return CreateProject(path);
    //        });
    //    }

    //    public void Save(IEasyScadaProject project)
    //    {
    //        try
    //        {
    //            if (project == CurrentProject)
    //            {
    //                UnLockProjectStream();
    //                DisposeProjectStream();
    //                project.ModifiedDate = DateTime.Now;
    //                project.AcceptChanges();
    //                ProjectSaving?.Invoke(this, new ProjectSavingEventArgs(CurrentProject, project));
    //                CurrentProjectStream = File.Open(project.ProjectPath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
    //                bf.Serialize(CurrentProjectStream, project);
    //                ProjectSaved?.Invoke(this, new ProjectSavedEventArgs(CurrentProject, project));
    //                LockProjectStream();
    //            }
    //            else
    //            {
    //                project.ModifiedDate = DateTime.Now;
    //                project.AcceptChanges();
    //                ProjectSaving?.Invoke(this, new ProjectSavingEventArgs(CurrentProject, project));
    //                using (FileStream fs = new FileStream(project.ProjectPath, FileMode.OpenOrCreate))
    //                {
    //                    bf.Serialize(fs, project);
    //                }
    //                ProjectSaved?.Invoke(this, new ProjectSavedEventArgs(CurrentProject, project));
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            Debug.WriteLine(ex.ToString());
    //        }
    //    }

    //    public Task SaveAsync(IEasyScadaProject project)
    //    {
    //        return Task.Run(() => Save(project));
    //    }

    //    public IEasyScadaProject OpenProject(string path)
    //    {
    //        using (FileStream fs = File.Open(path, FileMode.OpenOrCreate, FileAccess.ReadWrite))
    //        {
    //            IEasyScadaProject project = (IEasyScadaProject)bf.Deserialize(fs);
    //            project.ProjectPath = path;
    //            project.Name = Path.GetFileNameWithoutExtension(path);
    //            project.CreatedDate = File.GetCreationTime(path);
    //            project.ModifiedDate = File.GetLastWriteTime(path);
    //            project.AcceptChanges();
    //            return project;
    //        }
    //    }

    //    public Task<IEasyScadaProject> OpenProjectAsync(string path)
    //    {
    //        return Task.Run(() => OpenProject(path));
    //    }

    //    public event EventHandler<ProjectChangedEventArgs> ProjectChanged;
    //    public event EventHandler<ProjectSavedEventArgs> ProjectSaved;
    //    public event EventHandler<ProjectSavingEventArgs> ProjectSaving;

    //    private void LockProjectStream() => CurrentProjectStream?.Lock(CurrentProjectStream.Position, CurrentProjectStream.Length);
    //    private void UnLockProjectStream() => CurrentProjectStream?.Unlock(CurrentProjectStream.Position, CurrentProjectStream.Length);
    //    private void DisposeProjectStream() => CurrentProjectStream?.Dispose();
    //}

    public class ProjectChangedEventArgs : EventArgs
    {
        public IEasyScadaProject OldProject { get; private set; }
        public IEasyScadaProject NewProject { get; private set; }

        public ProjectChangedEventArgs(IEasyScadaProject oldProject, IEasyScadaProject newProject)
        {
            OldProject = oldProject;
            NewProject = newProject;
        }
    }

    public class ProjectSavingEventArgs : EventArgs
    {
        public IEasyScadaProject OldProject { get; private set; }
        public IEasyScadaProject NewProject { get; private set; }

        public ProjectSavingEventArgs(IEasyScadaProject oldPorject, IEasyScadaProject newProject)
        {
            OldProject = oldPorject;
            NewProject = newProject;
        }
    }

    public class ProjectSavedEventArgs : EventArgs
    {
        public IEasyScadaProject OldProject { get; private set; }
        public IEasyScadaProject NewProject { get; private set; }

        public ProjectSavedEventArgs(IEasyScadaProject oldPorject, IEasyScadaProject newProject)
        {
            OldProject = oldPorject;
            NewProject = newProject;
        }
    }
}
