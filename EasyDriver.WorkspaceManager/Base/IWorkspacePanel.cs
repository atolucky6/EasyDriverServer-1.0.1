using DevExpress.Mvvm;
using DevExpress.Xpf.Docking;
using System;

namespace EasyDriver.WorkspaceManager
{
    public interface IWorkspacePanel : IMVVMDockingProperties, ISupportParentViewModel
    {
        Uri UriTemplate { get; }
        string Caption { get; set; }
        string WorkspaceRegion { get; }
        bool IsDocument { get; }
        bool AutoHidden { get; set; }
        bool IsActive { get; set; }
        bool IsClosed { get; set; }
        bool IsOpened { get; }
        void Hide();
        void Show();
        object WorkspaceContext { get; set; }

        event EventHandler<WorkspacePanelStateChangedEventArgs> StateChanged;
    }
}
