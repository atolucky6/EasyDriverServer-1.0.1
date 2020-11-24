using DevExpress.Mvvm.UI;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Grid;
using System.Collections.Generic;

namespace EasyDriver.Workspace.Main
{
    /// <summary>
    /// Cung cấp chức năng thao tác với tree view
    /// </summary>
    public interface ITreeListViewUtilities
    {
        void ExpandAll();
        void CollapseAll();
        void ExpandCurrentNode();
        void CollapseCurrentNode();
        void ToggleCurrentNode();
        void ExpandNodeByContent(object content);
        void CollapseNodeByContent(object content);
        void Select(IEnumerable<object> selectedContents);
        void ShowSearchPanel();
    }

    public class TreeListViewUtilities : ServiceBase, ITreeListViewUtilities
    {
        TreeListView TreeListView { get => AssociatedObject as TreeListView; }

        public void CollapseAll()
        {
            TreeListView.CollapseAllNodes();
        }

        public void CollapseCurrentNode()
        {
            TreeListView.GetNodeByRowHandle(TreeListView.FocusedRowHandle).IsExpanded = false;
        }

        public void CollapseNodeByContent(object content)
        {
            TreeListNode findedNode = TreeListView.GetNodeByContent(content);
            if (findedNode != null)
            {
                findedNode.IsExpanded = false;
                foreach (var parentNode in GetParentNodes(findedNode))
                    parentNode.IsExpanded = false;
            }
        }

        public void ExpandAll()
        {
            TreeListView.ExpandAllNodes();
        }

        public void ExpandCurrentNode()
        {
            TreeListView.GetNodeByRowHandle(TreeListView.FocusedRowHandle).IsExpanded = true;
        }

        public void ExpandNodeByContent(object content)
        {
            if (TreeListView.Nodes.Count > 0)
            {
                TreeListNode foundedNode = TreeListView.GetNodeByContent(content);
                if (foundedNode != null)
                {
                    foundedNode.IsExpanded = true;
                    foreach (var parentNode in GetParentNodes(foundedNode))
                        parentNode.IsExpanded = true;
                }
            }
        }

        public void Select(IEnumerable<object> selectedContents)
        {
            TreeListView.DataControl.BeginSelection();
            TreeListView.DataControl.SelectedItems.Clear();
            foreach (var item in selectedContents)
                TreeListView.DataControl.SelectedItems.Add(item);
            TreeListView.DataControl.EndSelection();
        }

        public void ToggleCurrentNode()
        {
            var node = TreeListView.GetNodeByRowHandle(TreeListView.FocusedRowHandle);
            if (node != null)
                node.IsExpanded ^= true;
        }

        public void ShowSearchPanel()
        {
            TreeListView.ShowSearchPanel(true);
        }

        private IEnumerable<TreeListNode> GetParentNodes(TreeListNode node)
        {
            if (node != null)
            {
                if (node.ParentNode != null)
                {
                    yield return node.ParentNode;
                    foreach (var item in GetParentNodes(node.ParentNode))
                        yield return item;
                }
            }
        }
    }
}
