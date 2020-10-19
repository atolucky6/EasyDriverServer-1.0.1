using EasyScada.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace EasyScada.Winforms.Controls
{
    public partial class ConnectionSchemeDesignerForm : EasyForm
    {
        public ConnectionSchemeDesignerForm(EasyDriverConnector driverConnector, IServiceProvider serviceProvider)
        {
            InitializeComponent();
            this.driverConnector = driverConnector;
            this.serviceProvider = serviceProvider;

            foreach (var item in Enum.GetValues(typeof(CommunicationMode)))
                cobCommunicationMode.Items.Add(item.ToString());

            Load += ConnectionSchemeDesignerForm_Load;
            projectTree.AfterSelect += ProjectTree_AfterSelect;

            openToolStripMenuItem.Click += OpenToolStripMenuItem_Click;
            btnOpen.Click += OpenToolStripMenuItem_Click;
            saveToolStripMenuItem.Click += SaveToolStripMenuItem_Click;
            btnSave.Click += SaveToolStripMenuItem_Click;
            saveAsToolStripMenuItem.Click += SaveAsToolStripMenuItem_Click;
            closeToolStripMenuItem.Click += CloseToolStripMenuItem_Click;
            getConnectionSchemaToolStripMenuItem.Click += GetConnectionSchemaToolStripMenuItem_Click;
            btnGetConnectionSchema.Click += GetConnectionSchemaToolStripMenuItem_Click;

            projectTree.EndInit();
        }

        #region Fields
        private ICoreItem selectedItem;
        private EasyDriverConnector driverConnector;
        private IServiceProvider serviceProvider;
        private ConnectionSchema connectionSchema;
        #endregion

        #region Event handlers
        private void ProjectTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Tag is ICoreItem coreItem)
                DisplayTagCollection(coreItem);
        }

        private void GetConnectionSchemaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!txbServerAddress.Text.IsIpAddress())
            {
                MessageBox.Show($"The server address '{txbServerAddress.Text}' wasn't in correct IPv4 format.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            GetConnectionSchemaForm form = new GetConnectionSchemaForm(txbServerAddress.Text?.Trim(), decimal.ToUInt16(txbPort.Value));
            if (form.ShowDialog() == DialogResult.OK)
            {
                connectionSchema = form.ConnectionSchema;
                connectionSchema.ServerAddress = txbServerAddress.Text?.Trim();
                connectionSchema.Port = decimal.ToUInt16(txbPort.Value);
                connectionSchema.RefreshRate = decimal.ToInt32(txbRefreshRate.Value);
                ReloadTreeNode(connectionSchema);
            }
        }

        private void CloseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void SaveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (connectionSchema != null)
            {
                saveFileDialog1.Title = "Save as...";
                saveFileDialog1.Filter = "Connection Schema File (*.json)|*.json";
                saveFileDialog1.FileName = "ConnectionSchema.json";
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    File.WriteAllText(saveFileDialog1.FileName, JsonConvert.SerializeObject(connectionSchema, Formatting.Indented, new ConnectionSchemaJsonConverter()));
                }
            }
        }

        private void SaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string saveFilePath = DesignerHelper.GetApplicationOutputPath(serviceProvider) + "ConnectionSchema.json";
            try
            {
                if (connectionSchema != null)
                {
                    if (!Enum.TryParse(cobCommunicationMode.Text, out CommunicationMode mode))
                    {
                        MessageBox.Show($"Couldn't convert '{cobCommunicationMode.Text}' to CommunicationMode.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    if (!txbServerAddress.Text.IsIpAddress())
                    {
                        MessageBox.Show($"The server address '{txbServerAddress.Text}' wasn't in correct IPv4 format.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    
                    connectionSchema.ServerAddress = txbServerAddress.Text?.Trim();
                    connectionSchema.Port = decimal.ToUInt16(txbPort.Value);
                    connectionSchema.RefreshRate = decimal.ToInt32(txbRefreshRate.Value);
                    connectionSchema.CommunicationMode = mode;
                    string resJson = JsonConvert.SerializeObject(connectionSchema, Formatting.Indented, new ConnectionSchemaJsonConverter());
                    File.WriteAllText(saveFilePath, resJson);
                    DisplayConnectionParameters(connectionSchema);
                    MessageBox.Show("Save successfully!", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch
            {
                MessageBox.Show($"Some error occurs when save file at 'saveFilePath'.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                openFileDialog1.Title = "Open";
                openFileDialog1.Filter = "Connection Schema File (*.json)|*.json";
                openFileDialog1.Multiselect = false;
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    string filePath = openFileDialog1.FileName;
                    if (File.Exists(filePath))
                    {
                        try
                        {
                            string resJson = File.ReadAllText(filePath);
                            ConnectionSchema connectionSchema = JsonConvert.DeserializeObject<ConnectionSchema>(resJson, new ConnectionSchemaJsonConverter());
                            DisplayConnectionParameters(connectionSchema);
                            DisplayTagCollection(null);
                            this.connectionSchema = connectionSchema;
                            ReloadTreeNode(connectionSchema);
                        }
                        catch
                        {
                            MessageBox.Show("Can't deserialize file to Connection Schema.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch
            {
                MessageBox.Show($"Couldn't read file '{openFileDialog1.FileName}'.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ConnectionSchemeDesignerForm_Load(object sender, EventArgs e)
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
                    if (!txbServerAddress.Text.IsIpAddress())
                        txbServerAddress.Text = "127.0.0.1";
                    GetConnectionSchemaToolStripMenuItem_Click(null, null);
                }
                else
                {
                    Close();
                }
            }
            else
            {
                ReloadTreeNode(connectionSchema);
            }

            DisplayConnectionParameters(connectionSchema);
        }
        #endregion

        #region Methods
        private void DisplayTagCollection(ICoreItem coreItem)
        {
            selectedItem = coreItem;
            if (coreItem != null)
            {
                searchTagControl.CoreItemSource = coreItem.Childs.Where(x => x is ITag).Select(x => x as ICoreItem).ToList();
                groupTagCollection.ValuesPrimary.Heading = $"{coreItem.Name} - Tag Collection";
                groupTagCollection.ValuesSecondary.Heading = $"Total: {coreItem.Childs?.Count}";
            }
            else
            {
                searchTagControl.CoreItemSource = new List<ICoreItem>();
                groupTagCollection.ValuesPrimary.Heading = $"Tag Collection";
                groupTagCollection.ValuesSecondary.Heading = $"Total: 0";
            }
        }

        private void DisplayConnectionParameters(ConnectionSchema connectionSchema)
        {
            if (connectionSchema != null)
            {
                txbServerAddress.Text = connectionSchema.ServerAddress;
                cobCommunicationMode.Text = connectionSchema.CommunicationMode.ToString();
                txbPort.Value = connectionSchema.Port;
                txbRefreshRate.Value = connectionSchema.RefreshRate;
            }
            else
            {
                txbServerAddress.Text = "127.0.0.1";
                txbPort.Value = 8800;
                txbRefreshRate.Value = 500;
                cobCommunicationMode.SelectedIndex = 0;
            }
        }

        private void ReloadTreeNode(ConnectionSchema connectionSchema)
        {
            projectTree.Nodes.Clear();
            if (connectionSchema != null)
            {
                if (connectionSchema.Childs != null)
                {
                    foreach (var item in connectionSchema.Childs)
                    {
                        projectTree.Nodes.Add(item.ToTreeNode(true, false));
                    }
                }
            }
            int tagCount = 0;
            var tags = connectionSchema?.GetAllTags();
            if (tags != null)
                tagCount = tags.Count();
            groupProjectTree.ValuesSecondary.Heading = $"Total tags: {tagCount}";
            ExpandAllNode();
        }

        private void ExpandAllNode()
        {
            if (projectTree.Nodes.Count > 0)
            {
                foreach (TreeNode node in projectTree.Nodes)
                {
                    node.ExpandAll();
                }
            }
        }

        private void CollapseAllNode()
        {
            if (projectTree.Nodes.Count > 0)
            {
                foreach (TreeNode node in projectTree.Nodes)
                {
                    node.Collapse();
                }
            }
        }
        #endregion
    }
}
