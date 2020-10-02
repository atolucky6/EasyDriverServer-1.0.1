using EasyScada.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace EasyScada.Winforms.Controls
{
    public partial class SelectTagPathDesignerForm : EasyForm
    {
        public SelectTagPathDesignerForm(IServiceProvider serviceProvider)
        {
            InitializeComponent();
            this.serviceProvider = serviceProvider;
            projectTree.LabelEdit = false;
            Load += SelectTagDesignerForm_Load;
            btnOk.Click += BtnOk_Click;
            btnCancel.Click += BtnCancel_Click;
            projectTree.AfterSelect += ProjectTree_AfterSelect;
            searchTagControl1.CoreItemSource = new List<ICoreItem>();
            projectTree.AfterSelect += ProjectTree_AfterSelect;
            searchTagControl1.SelectedItemDoubleClick += SearchTagControl1_SelectedItemDoubleClick;
        }

        private IServiceProvider serviceProvider;
        private ConnectionSchema connectionSchema;
        private ICoreItem selectedItem;
        public string SelectedTagPath { get; private set; }

        #region Event handlers
        private void SelectTagDesignerForm_Load(object sender, EventArgs e)
        {
            if (serviceProvider == null)
                Close();
            connectionSchema = DesignerHelper.GetDesignConnectionSchema(serviceProvider);
            if (connectionSchema == null)
            {
                var mbr = MessageBox.Show(
                    $"Could not found or load ConnectionSchema.json file at '{DesignerHelper.GetApplicationOutputPath(serviceProvider)}'.\nClick 'Yes' to load ConnectionSchema file or click 'No' to exit.",
                    "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                if (mbr == DialogResult.Yes)
                {

                }
                else
                {
                    Close();
                }
            }

            if (connectionSchema != null)
            {
                ReloadTreeNode(connectionSchema);
                Show();
            }
        }

        private void SearchTagControl1_SelectedItemDoubleClick(object obj)
        {
            BtnOk_Click(null, null);
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close(); 
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            SelectedTagPath = $"{selectedItem?.Path}/{searchTagControl1.SelectedItem}";
            Close();
        }

        private void ProjectTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            try
            {
                if (e.Action == TreeViewAction.ByMouse ||
                    e.Action == TreeViewAction.ByKeyboard)
                {
                    if (e.Node.Tag is ICoreItem coreItem)
                    {
                        selectedItem = coreItem;
                        searchTagControl1.CoreItemSource = coreItem.Childs.Where(x => x is ITag).Select(x => x as ICoreItem).ToList();

                        groupTagCollection.ValuesPrimary.Heading = $"{coreItem.Name} - Tag Collection";
                        groupTagCollection.ValuesSecondary.Heading = $"Total: {coreItem.Childs.Count}";
                    }
                }
            }
            catch { }
        }

        private void ReloadTreeNode(ConnectionSchema connectionSchema)
        {
            projectTree.Nodes.Clear();
            projectTree.Nodes.Add(connectionSchema.ToTreeNode(true, false));
            int tagCount = 0;
            var tags = connectionSchema.GetAllTags();
            if (tags != null)
                tagCount = tags.Count();
            groupProjectTree.ValuesSecondary.Heading = $"Total tags: {tagCount}";
            ExpandAllNode();
        }

        private void ExpandAllNode()
        {
            if (projectTree.Nodes.Count > 0)
                projectTree.Nodes[0].ExpandAll();
        }

        private void CollapseAllNode()
        {
            if (projectTree.Nodes.Count > 0)
                projectTree.Nodes[0].Collapse();
        }
        #endregion
    }
}
