using EasyDriver.WorkspaceManager;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;

namespace EasyDriverServer.ViewModel
{
    public class WorkspaceModule
    {
        #region Constructors

        public WorkspaceModule(IWorkspaceManagerService workspaceManagerService)
        {
            WorkspaceManagerService = workspaceManagerService;
            LoadWorkspaces();
        }

        #endregion

        #region Inject services

        protected IWorkspaceManagerService WorkspaceManagerService { get; set; }

        #endregion

        #region Public properties

        public ObservableCollection<IWorkspacePanel> Workspaces { get => WorkspaceManagerService.Workspaces; }

        #endregion

        #region Private methods

        private void LoadWorkspaces()
        {
            string workspacesDir = ApplicationHelper.GetApplicationPath() + "\\Workspaces\\";
            if (Directory.Exists(workspacesDir))
            {
                var workspacePaths = Directory.GetFiles(workspacesDir, "*.dll").Select(x => Path.GetFileName(x)).ToList();
                string[] localWorkspacePaths = Directory.GetFiles($"{ApplicationHelper.GetApplicationPath()}\\", "*.dll")
                    .Where(x => workspacePaths.Contains(Path.GetFileName(x))).ToArray();

                List<string> assemblyFullNames = new List<string>();
                foreach (var workspacePath in localWorkspacePaths)
                {
                    string fullPath = Path.GetFullPath(workspacePath);
                    Assembly loadedAssembly = Assembly.LoadFile(fullPath);
                    loadedAssembly = AppDomain.CurrentDomain.Load(loadedAssembly.GetName());
                    assemblyFullNames.Add(loadedAssembly.FullName);
                }

                Type workspacePanelType = typeof(IWorkspacePanel);
                foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    try
                    {
                        if (assemblyFullNames.Contains(assembly.FullName))
                        {
                            foreach (Type type in assembly.GetTypes())
                            {
                                Type instanceType = null;
                                if (workspacePanelType.IsAssignableFrom(type) && type.IsClass)
                                {
                                    instanceType = type;
                                    InitializeWorkspace(instanceType);
                                }
                            }
                        }
                    }
                    catch (Exception ex) { Debug.WriteLine(ex.ToString()); }
                }
            }
        }

        private void InitializeWorkspace(Type instanceType)
        {
            if (instanceType != null)
            {
                if (IoC.Instance.GetPOCOViewModel(instanceType) is IWorkspacePanel workspacePanel)
                {
                    if (workspacePanel.UriTemplate != null)
                        Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = workspacePanel.UriTemplate });
                    if (!workspacePanel.IsDocument)
                        IoC.Instance.Kernel.Rebind(instanceType).ToConstant(workspacePanel);
                    WorkspaceManagerService.AddPanel(workspacePanel);
                }
            }
        }

        #endregion
    }
}
