using EasyDriver.Service.ApplicationProperties;
using EasyDriver.Service.Clipboard;
using EasyDriver.Service.DriverManager;
using EasyDriver.Service.InternalStorage;
using EasyDriver.Service.ProjectManager;
using EasyDriver.Service.RemoteConnectionManager;
using EasyDriver.Service.Reversible;
using EasyScada.WorkspaceManager;
using System;
using System.Collections.Generic;
using System.Text;

namespace EasyDriver.Workspace.Main
{
    public static class ServiceLocator
    {
        public static IWorkspaceManagerService WorkspaceManagerService { get; set; }
        public static IReversibleService ReversibleService { get; set; }
        public static IDriverManagerService DriverManagerService { get; set; }
        public static IRemoteConnectionManagerService RemoteConnectionManagerService { get; set; }
        public static IClipboardService ClipboardService { get; set; }
        public static IProjectManagerService ProjectManagerService { get; set; }
        public static IInternalStorageService InternalStorageService { get; set; }
        public static IApplicationPropertiesService ApplicationPropertiesService { get; set; }
    }
}
