using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace EasyScada.ServerApplication.Workspace
{
    public interface IWorkspaceManagerService<TPanel>
        where TPanel : IWorkspacePanel
    {
        ObservableCollection<TPanel> Workspaces { get; }
        TPanel CurrentActivePanel { get; set; }

        void AddPanel(TPanel panelViewModel, bool addAndOpen = true);
        void RemovePanel(Func<TPanel, bool> predicate);
        void RemoveAllDocumentPanel();
        void Clear();

        bool RemovePanel(TPanel panelViewModel);
        bool ContainPanel(TPanel panelViewModel);

        TPanel FindPanelByToken(object token);
        TPanel GetCurrentActivePanel();

        IEnumerable<TPanel> GetAllWorkspacePanel();
        IEnumerable<TPanel> GetAllDocumentPanel();

        void AutoHideAll();
        void CloseAllDocumentPanel();
        void ClosePanel(object token);
        void ClosePanel(TPanel panelViewModel);
        void OpenPanel(object token, bool activeOnOpen = true, bool createIfNotExists = false, object parentViewModel = null);
        void OpenPanel(TPanel panelViewModel, bool activeOnOpen = true);
        void TogglePanel(object token, bool createIfNotExists = false);
        void TogglePanel(TPanel panelViewModel);

        TPanel CreatePanelByToken(object token);
    }
}
