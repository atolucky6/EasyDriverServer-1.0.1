using EasyScada.Core;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace EasyScada.Winforms.Controls
{
    public class HieraticalTreeView : TreeView, ISupportInitialize
    {
        public HieraticalTreeView() : base()
        {
            
        }

        protected override void OnAfterCheck(TreeViewEventArgs e)
        {
            base.OnAfterCheck(e);
            BeginUpdate();
            if (e.Action != TreeViewAction.Unknown)
            {
                UpdateCheckStateParent(e.Node);
                UpdateCheckStateChild(e.Node, e.Node.Checked);
            }

            if (e.Node.Tag is ICheckable checkable)
                checkable.Checked = e.Node.Checked;
            EndUpdate();
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg != 0x203 || !CheckBoxes)
                base.WndProc(ref m);
        }

        private void UpdateCheckStateParent(TreeNode treeNode)
        {
            if (treeNode.Checked)
            {
                if (treeNode.Parent != null)
                {
                    treeNode.Parent.Checked = true;
                    UpdateCheckStateParent(treeNode.Parent);
                }
            }
            else
            {
                if (treeNode.Parent != null)
                {
                    bool containCheckedChild = false;
                    foreach (TreeNode item in treeNode.Parent.Nodes)
                    {
                        if (item.Checked)
                        {
                            containCheckedChild = true;
                            break;
                        }
                    }
                    if (!containCheckedChild)
                    {
                        treeNode.Parent.Checked = false;
                        UpdateCheckStateParent(treeNode.Parent);
                    }
                }
            }

            if (treeNode.Tag is ICheckable checkable)
                checkable.Checked = treeNode.Checked;
        }

        private void UpdateCheckStateChild(TreeNode treeNode, bool isChecked)
        {
            foreach (TreeNode item in treeNode.Nodes)
            {
                item.Checked = isChecked;
                if (item.Nodes.Count > 0)
                    UpdateCheckStateChild(item, isChecked);

                if (item.Tag is ICheckable checkable)
                    checkable.Checked = item.Checked;
            }
        }

        public void BeginInit()
        {
        }

        public void EndInit()
        {
            // Pad an image int image list to 6px
            Point destPt = new Point(2, 0);
            Size size = new Size(20, 18);
            var imageListSource = ImageList;
            if (imageListSource != null && imageListSource.Images.Count > 0)
            {
                ImageList = new ImageList();
                ImageList.ImageSize = size;
                ImageList.ColorDepth = ColorDepth.Depth32Bit;
                foreach (string imgKey in imageListSource.Images.Keys)
                {
                    Bitmap bmp = new Bitmap(size.Width, size.Height);
                    Graphics g = Graphics.FromImage(bmp);
                    g.DrawImage(imageListSource.Images[imgKey], destPt);
                    g.Dispose();
                    ImageList.Images.Add(imgKey, bmp);
                }
            }
        }
    }
}
