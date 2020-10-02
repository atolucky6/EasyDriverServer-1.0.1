using DevExpress.Mvvm.POCO;
using EasyDriver.MenuPlugin;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace EasyDriverServer.ViewModel
{
    public class MenuModule
    {
        #region Constructors

        public MenuModule()
        {
            MenuPlugins = new List<IEasyMenuPlugin>();
            BarItemsSource = new ObservableCollection<IBarComponent>();
            InitializeBarItems();
            LoadMenuPlugins();
        }

        #endregion

        #region Public properties

        public List<IEasyMenuPlugin> MenuPlugins { get; private set; }
        public ObservableCollection<IBarComponent> BarItemsSource { get; set; }

        public IBarComponent MainMenu { get; private set; }
        public IBarComponent FileToolBar { get; private set; }
        public IBarComponent EditToolBar { get; private set; }
        public IBarComponent StatusBar { get; private set; }

        public IBarComponent FileSubMenu { get; private set; }
        public IBarComponent EditSubMenu { get; private set; }
        public IBarComponent ViewSubMenu { get; private set; }
        public IBarComponent ToolsSubMenu { get; private set; }
        public IBarComponent WindowSubMenu { get; private set; }
        public IBarComponent HelpSubMenu { get; private set; }

        #endregion

        #region Public methods

        public void AddChildComponentToBarComponent(IBarComponent barSubItem, IEnumerable<IBarComponent> components)
        {
            if (components != null)
            {
                foreach (var component in components)
                {
                    if (barSubItem.BarItems.FirstOrDefault(x => x.DisplayName == component.DisplayName) is IBarComponent existComponent)
                    {
                        AddChildComponentToBarComponent(existComponent, component.BarItems);
                    }
                    else
                    {
                        barSubItem.Add(component);
                    }
                }
            }
        }

        #endregion

        #region Private methods

        private void InitializeBarItems()
        {
            MainMenu = BarFactory.Default.CreateMainMenu("MainMenu");
            FileToolBar = BarFactory.Default.CreateToolBar("File");
            EditToolBar = BarFactory.Default.CreateToolBar("Edit");
            StatusBar = BarFactory.Default.CreateStatusBar("StatusBar");

            BarItemsSource.Add(MainMenu);
            BarItemsSource.Add(FileToolBar);
            BarItemsSource.Add(EditToolBar);
            BarItemsSource.Add(StatusBar);

            AddBarItemsForMainMenu();

            this.RaisePropertyChanged(x => x.BarItemsSource);
        }

        private void AddBarItemsForMainMenu()
        {
            FileSubMenu = BarFactory.Default.CreateSubItem("File");
            EditSubMenu = BarFactory.Default.CreateSubItem("Edit");
            ViewSubMenu = BarFactory.Default.CreateSubItem("View");
            ToolsSubMenu = BarFactory.Default.CreateSubItem("Tools");
            WindowSubMenu = BarFactory.Default.CreateSubItem("Window");

            MainMenu.Add(FileSubMenu).Add(EditSubMenu).Add(ViewSubMenu).Add(ToolsSubMenu).Add(WindowSubMenu);
        }

        private void LoadMenuPlugins()
        {
            string menuDir = ApplicationHelper.GetApplicationPath() + "\\Menus\\";
            if (Directory.Exists(menuDir))
            {
                var menuPluginPaths = Directory.GetFiles(menuDir, "*.dll").Select(x => Path.GetFileName(x)).ToList();

                string[] localMenuPluginPaths = Directory.GetFiles($"{ApplicationHelper.GetApplicationPath()}\\", "*.dll")
                    .Where(x => menuPluginPaths.Contains(Path.GetFileName(x))).ToArray();

                List<string> assemblyFullNames = new List<string>();
                foreach (var menuPluginPath in localMenuPluginPaths)
                {   
                    string fullPath = Path.GetFullPath(menuPluginPath);
                    Assembly loadedAssembly = Assembly.LoadFile(fullPath);
                    loadedAssembly = AppDomain.CurrentDomain.Load(loadedAssembly.GetName());
                    assemblyFullNames.Add(loadedAssembly.FullName);
                }

                Type menuPluginType = typeof(IEasyMenuPlugin);
                foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    try
                    {
                        if (assemblyFullNames.Contains(assembly.FullName))
                        {
                            foreach (Type type in assembly.GetTypes())
                            {
                                Type instanceType = null;
                                if (menuPluginType.IsAssignableFrom(type) && type.IsClass)
                                {
                                    instanceType = type;
                                    InitializeMenu(instanceType);
                                }
                            }
                        }
                    }
                    catch (Exception) { }
                }
            }
        }

        private void InitializeMenu(Type instanceType)
        {
            if (instanceType != null)
            {
                IoC.Instance.Kernel.Bind(instanceType).To(instanceType);
                if (IoC.Instance.Get(instanceType) is IEasyMenuPlugin menuPlugin)
                {
                    var extendMainMenuBarItems = menuPlugin.GetExtendApplicationMenuItems(MainMenu, null);
                    AddChildComponentToBarComponent(MainMenu, extendMainMenuBarItems);

                    var extendStatusBarItems = menuPlugin.GetExtendApplicationStatusBarItems(StatusBar, null);
                    AddChildComponentToBarComponent(StatusBar, extendStatusBarItems);

                    var extendToolBarItems = menuPlugin.GetExtendApplicationToolBarItems(null, null);
                    if (extendToolBarItems != null)
                    {
                        foreach (var item in extendToolBarItems)
                        {
                            if (BarItemsSource.FirstOrDefault(x => x.DisplayName == item.DisplayName) is IBarComponent existBarItem)
                                AddChildComponentToBarComponent(existBarItem, item.BarItems);
                            else
                                BarItemsSource.Add(item);
                        }
                    }

                    MenuPlugins.Add(menuPlugin);
                }
            }
        }

        #endregion
    }
}
