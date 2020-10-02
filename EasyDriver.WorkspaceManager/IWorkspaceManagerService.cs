using EasyDriver.ServicePlugin;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace EasyDriver.WorkspaceManager
{
    public interface IWorkspaceManagerService : IEasyServicePlugin, INotifyPropertyChanged
    {
        ObservableCollection<IWorkspacePanel> Workspaces { get; }
        IWorkspacePanel CurrentActivePanel { get; set; }
        List<Func<object, IWorkspacePanel>> CreatePanelByContextFuncs { get; set; }

        void AddPanel(IWorkspacePanel panelViewModel, bool addAndOpen = true);
        void RemovePanel(Func<IWorkspacePanel, bool> predicate);
        void RemoveAllDocumentPanel();
        void Clear();

        bool RemovePanel(IWorkspacePanel panelViewModel);
        bool ContainPanel(IWorkspacePanel panelViewModel);

        IWorkspacePanel FindPanelByContext(object context);
        IWorkspacePanel GetCurrentActivePanel();

        IEnumerable<IWorkspacePanel> GetAllWorkspacePanel();
        IEnumerable<IWorkspacePanel> GetAllDocumentPanel();

        void AutoHideAll();
        void CloseAllDocumentPanel();
        void ClosePanel(object context);
        void ClosePanel(IWorkspacePanel panelViewModel);
        void OpenPanel(object context, bool activeOnOpen = true, bool createIfNotExists = false, object parentViewModel = null);
        void OpenPanel(IWorkspacePanel panelViewModel, bool activeOnOpen = true);
        void TogglePanel(object context, bool createIfNotExists = false);
        void TogglePanel(IWorkspacePanel panelViewModel);

        IWorkspacePanel CreatePanelByWorkspaceContext(object context);
    }
}
