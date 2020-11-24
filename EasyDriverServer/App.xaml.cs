using DevExpress.Xpf.Core;
using EasyDriverServer.ModuleInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows;

namespace EasyDriverServer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static string ApplicationPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        private static Dictionary<string, string> AssemblyPathDictionary = new Dictionary<string, string>();

        [STAThread]
        protected override void OnStartup(StartupEventArgs e)
        {
            // Load custom theme
            var theme = new Theme("BlueTheme");
            theme.AssemblyName = "DevExpress.Xpf.Themes.BlueTheme.v20.2";
            Theme.RegisterTheme(theme);
            ApplicationThemeHelper.ApplicationThemeName = "BlueTheme";

            AppDomain currentDomain = AppDomain.CurrentDomain;
            if (Directory.Exists(ApplicationPath))
            {
                foreach (var assemblyPath in Directory.GetFiles(ApplicationPath, "*.dll", SearchOption.AllDirectories))
                {
                    AssemblyPathDictionary[Path.GetFileName(assemblyPath)] = assemblyPath;
                }
            }
            currentDomain.AssemblyResolve += OnAssemblyResolve;

            IoC.Instance.Setup();

            Current.MainWindow = new MainWindow();
            Current.MainWindow.Show();

        }

        private static Assembly OnAssemblyResolve(object sender, ResolveEventArgs args)
        {
            string assemblyName = new AssemblyName(args.Name).Name + ".dll";

            if (!AssemblyPathDictionary.ContainsKey(assemblyName))
                return null;
            string assemblyPath = AssemblyPathDictionary[assemblyName];
            Assembly assembly = Assembly.LoadFrom(assemblyPath);
            return assembly;
        }
    }
}
