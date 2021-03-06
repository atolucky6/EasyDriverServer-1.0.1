﻿using DevExpress.Mvvm;
using DevExpress.Xpf.Core;
using EasyDriver.Opc.Client;
using Microsoft.Owin.Hosting;
using Microsoft.Shell;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Forms;

namespace EasyScada.ServerApplication
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application, ISingleInstanceApp
    {
        private static SplashScreenManager manager;
        private const string Unique = "EasyDriverServer";
        NotifyIcon MainNotifyIcon;

        [DllImport("user32.dll")]
        private static extern
        bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern
            bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        private static extern
            bool IsIconic(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern 
            bool MoveWindow(IntPtr handle, int x, int y, int width, int height, bool redraw);

        [DllImport("User32")]
        private static extern int ShowWindow(int hwnd, int nCmdShow);

        private const int SW_HIDE = 0;
        private const int SW_SHOWNORMAL = 1;
        private const int SW_SHOWMINIMIZED = 2;
        private const int SW_SHOWMAXIMIZED = 3;
        private const int SW_SHOWNOACTIVATE = 4;
        private const int SW_RESTORE = 9;
        private const int SW_SHOWDEFAULT = 10;
        private const int SW_SHOW = 5;

        public bool SignalExternalCommandLineArgs(IList<string> args)
        {
            return true;
        }

        public static void Main()
        {
            if (SingleInstance<App>.InitializeAsFirstInstance(Unique))
            {
                OpcDaBootstrap.Initialize();
                var staThread = new Thread(() =>
                {
                    var application = new App();
                    application.InitializeComponent();
                    application.Run();
                    SingleInstance<App>.Cleanup();
                });
                staThread.SetApartmentState(ApartmentState.STA);
                staThread.Start();
            }
            else
            {
                // get the name of our process
                string proc = Process.GetCurrentProcess().ProcessName;
                // get the list of all processes by that name
                Process[] processes = Process.GetProcessesByName(proc);
                // if there is more than one process...
                if (processes.Length > 1)
                {
                    // Assume there is our process, which we will terminate, 
                    // and the other process, which we want to bring to the 
                    // foreground. This assumes there are only two processes 
                    // in the processes array, and we need to find out which 
                    // one is NOT us.

                    // get our process
                    Process p = Process.GetCurrentProcess();
                    int n = 0;        // assume the other process is at index 0
                                      // if this process id is OUR process ID...
                    if (processes[0].Id == p.Id)
                    {
                        // then the other process is at index 1
                        n = 1;
                    }
                    // get the window handleset v
                    IntPtr hWnd = processes[n].MainWindowHandle;

                    ShowWindowAsync(hWnd, SW_RESTORE);

                    // bring it to the foreground
                    SetForegroundWindow(hWnd);
                    // exit our process
                    return;
                }
            }
        }

        static App()
        {
            var splashVM = new DXSplashScreenViewModel() { Status = "Starting..." };
            manager = SplashScreenManager.CreateWaitIndicator(splashVM);
            manager.ShowOnStartup(true);
        }

        [STAThread]
        protected override void OnStartup(StartupEventArgs e)
        {
            // Load custom theme
            var theme = new Theme("BlueTheme");
            theme.AssemblyName = "DevExpress.Xpf.Themes.BlueTheme.v20.1";
            Theme.RegisterTheme(theme);
            ApplicationThemeHelper.ApplicationThemeName = "BlueTheme";

            IoC.Instance.Setup();
            // Show the main window
            Current.MainWindow = new MainWindow();

            MainWindow.Closing += (s, args) =>
            {
                if (!IoC.Instance.Get<ApplicationViewModel>().IsMainWindowExit)
                {
                    args.Cancel = true;
                    MainWindow.Hide();
                }
            };

            //Initialize notify icon
            MainNotifyIcon = new NotifyIcon()
            {
                Icon = EasyScada.ServerApplication.Properties.Resources.EasyScadaLogo,
                Visible = true
            };
            MainNotifyIcon.DoubleClick += (s, args) => ShowMainWindow();
            MainNotifyIcon.ContextMenuStrip = new ContextMenuStrip();
            MainNotifyIcon.ContextMenuStrip.Items.Add("Show").Click += (s, args) => ShowMainWindow();
            MainNotifyIcon.ContextMenuStrip.Items.Add("Exit").Click += (s, args) => ExitMainWindow();

            string url = $"http://*:{IoC.Instance.Get<ApplicationViewModel>().ServerConfiguration.Port}";
            WebApp.Start(url);
            Current.MainWindow.Show();

            string startUpFilePath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\startup.ini";
            if (File.Exists(startUpFilePath))
            {
                string startUpProjectPath = File.ReadAllText(startUpFilePath);
                if (File.Exists(startUpProjectPath))
                {
                    var projectManager = IoC.Instance.Get<IProjectManagerService>();
                    var loadedProject = projectManager.OpenProject(startUpProjectPath);
                    if (loadedProject != null)
                    {
                        projectManager.CurrentProject = loadedProject;
                    }
                }
            }
            manager.Close();
        }

        void ShowMainWindow()
        {
            if (MainWindow.IsVisible)
            {
                if (MainWindow.WindowState == WindowState.Minimized)
                    MainWindow.WindowState = WindowState.Normal;
                MainWindow.Activate();
            }
            else
                MainWindow.Show();
        }

        void ExitMainWindow()
        {
            IoC.Instance.Get<ApplicationViewModel>().IsMainWindowExit = true;
            MainWindow.Close();
            MainNotifyIcon.Dispose();
            MainNotifyIcon = null;
        }
    }
}
