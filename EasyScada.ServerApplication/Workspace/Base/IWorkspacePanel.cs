using DevExpress.Mvvm;
using DevExpress.Xpf.Docking;

namespace EasyScada.ServerApplication.Workspace
{
    public interface IWorkspacePanel : IMVVMDockingProperties, ISupportParentViewModel
    {
        string Caption { get; set; }
        string WorkspaceName { get; }
        bool IsDocument { get; }
        bool AutoHidden { get; set; }
        bool IsActive { get; set; }
        bool IsClosed { get; set; }
        bool IsOpened { get; }
        void Hide();
        void Show();
        object Token { get; set; }
    }
}
