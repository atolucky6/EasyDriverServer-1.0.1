using DevExpress.Xpf.Core;
using EasyDriverServer.ModuleInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
        /* Các bước để host SignalR Core vào WPF
         * 1. Cài đặt nuget Microsoft.AspNetCore.App vào wpf project
         * 2. Tạo một web app .net core để host SignalR
         * 3. Reference tới web app
         * 4. Override OnStartup (nằm ở App.xaml.cs) và thêm đoạn code sau đây để chạy web app:
         * 
         *      host = Host.CreateDefaultBuilder(e.Args)
         *          .ConfigureWebHostDefaults(webHostBuilder => webHostBuilder.UseStartup<{WebAppProject}.Startup>())
         *          .ConfigureServices(services =>
         *          {
         *              services.AddTransient<MainWindow>();
         *          }).Build();
         *      host.Start();
         *      host.Services.GetRequiredService<MainWindow>().Show();
         *      
         * 5. Cấu hình Hub ở web app
         */
        private IHost host;

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

            // Get all assemblies in current directory
            AppDomain currentDomain = AppDomain.CurrentDomain;
            if (Directory.Exists(ApplicationPath))
            {
                foreach (var assemblyPath in Directory.GetFiles(ApplicationPath, "*.dll", SearchOption.AllDirectories))
                {
                    AssemblyPathDictionary[Path.GetFileName(assemblyPath)] = assemblyPath;
                }
            }
            // Register assembly resolve
            currentDomain.AssemblyResolve += OnAssemblyResolve;

            // Set up dependency
            IoC.Instance.Setup();

            host = Host.CreateDefaultBuilder(e.Args)
                .ConfigureWebHostDefaults(webHostBuilder => webHostBuilder.UseStartup<WebHosting.Startup>())
                .ConfigureServices(services =>
                {
                    services.AddTransient<MainWindow>();
                }).Build();

            host.Start();
            host.Services.GetRequiredService<MainWindow>().Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            host?.Dispose();
            base.OnExit(e);
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
