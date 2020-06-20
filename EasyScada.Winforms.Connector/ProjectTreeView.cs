using System.Windows.Forms;

namespace EasyScada.Winforms.Connector
{
    class ProjectTreeView : TreeView
    {
        public ProjectTreeView() : base()
        {

        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg != 0x203) base.WndProc(ref m);
        }
    }
}
