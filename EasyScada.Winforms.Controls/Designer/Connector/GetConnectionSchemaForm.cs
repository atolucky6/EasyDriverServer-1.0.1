﻿using EasyScada.Core;
using Microsoft.AspNet.SignalR.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using ConnectionState = Microsoft.AspNet.SignalR.Client.ConnectionState;

namespace EasyScada.Winforms.Controls
{
    public partial class GetConnectionSchemaForm : EasyForm
    {
        #region Constructors
        public GetConnectionSchemaForm(string serverAddress, ushort port)
        {
            InitializeComponent();
            ServerAddress = serverAddress;
            Port = port;

            btnOk.Click += BtnOk_Click;
            btnCancel.Click += BtnCancel_Click;
            Load += GetConnectionSchemaForm_Load;
            projectTree.AfterCheck += ProjectTree_AfterCheck;
        }
        #endregion

        #region Properties
        public string ServerAddress { get; set; }
        public ushort Port { get; set; }
        public ConnectionSchema ConnectionSchema { get; set; }
        private ICoreItem selectedItem;
        #endregion

        #region Event handlers
        private void ProjectTree_AfterCheck(object sender, TreeViewEventArgs e)
        {
            try
            {
                if (e.Action == TreeViewAction.ByMouse ||
                    e.Action == TreeViewAction.ByKeyboard)
                {
                    if (e.Node.Tag is ICoreItem coreItem)
                        DisplayTagCollection(coreItem);
                }
            }
            catch { }
        }

        private async void GetConnectionSchemaForm_Load(object sender, EventArgs e)
        {
            await GetConnectionSchemaFromServer(ServerAddress, Port);
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
        }
        #endregion

        #region Methods
        private async Task GetConnectionSchemaFromServer(string serverAddress, ushort port)
        {
            try
            {
                ConnectionSchema = null;
                using (HubConnection hubConnection = new HubConnection($"http://{serverAddress}:{port}/easyScada"))
                {
                    IHubProxy hubProxy = hubConnection.CreateHubProxy("EasyDriverServerHub");
                    await hubConnection.Start();
                    if (hubConnection.State == ConnectionState.Connected)
                    {
                        ConnectionSchema = new ConnectionSchema()
                        {
                            CreatedDate = DateTime.Now,
                            Port = port,
                            ServerAddress = serverAddress,
                        };

                        string resJson = await hubProxy.Invoke<string>("getAllElementsAsync");

                        List<ICoreItem> coreItems = new List<ICoreItem>();
                        if (JsonConvert.DeserializeObject(resJson) is JArray jArray)
                        {
                            foreach (var item in jArray)
                            {
                                ICoreItem coreItem = JsonConvert.DeserializeObject<ICoreItem>(item.ToString(), new ConnectionSchemaJsonConverter());
                                if (coreItem != null)
                                    coreItems.Add(coreItem);
                            }
                        }
                        if (coreItems != null)
                            coreItems.ForEach(x => ConnectionSchema.Childs.Add(x));
                    }
                    else
                    {
                        if (!Disposing && Visible)
                            MessageBox.Show($"Can't connect to server {serverAddress}:{port}. Please make sure Easy Driver Server application is already opened before doing this action!", "Easy Driver Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception)
            {
                if (!Disposing && Visible)
                    MessageBox.Show($"Can't connect to server {serverAddress}:{port}. Please make sure Easy Driver Server application is already opened before doing this action!", "Easy Driver Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Invoke(new Action(() =>
                {
                    ReloadTreeNode(ConnectionSchema);
                }));
            }
        }

        private void ReloadTreeNode(ConnectionSchema connectionSchema)
        {
            projectTree.Nodes.Clear();
            if (connectionSchema != null)
                projectTree.Nodes.Add(connectionSchema.ToTreeNode(true, false));
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
                projectTree.Nodes[0].ExpandAll();
        }

        private void CollapseAllNode()
        {
            if (projectTree.Nodes.Count > 0)
                projectTree.Nodes[0].Collapse();
        }

        private void DisplayTagCollection(ICoreItem coreItem)
        {
            selectedItem = coreItem;
            if (coreItem != null)
            {
                searchTagControl1.CoreItemSource = coreItem.Childs.Where(x => x is ITag).Select(x => x as ICoreItem).ToList();
                groupTagCollection.ValuesPrimary.Heading = $"{coreItem.Name} - Tag Collection";
                groupTagCollection.ValuesSecondary.Heading = $"Total: {coreItem.Childs?.Count}";
            }
            else
            {
                searchTagControl1.CoreItemSource = new List<ICoreItem>();
                groupTagCollection.ValuesPrimary.Heading = $"Tag Collection";
                groupTagCollection.ValuesSecondary.Heading = $"Total: 0";
            }
        }

        private ConnectionSchema GetCheckedItems()
        {
            if (ConnectionSchema == null)
                return null;

            //ConnectionSchema cloneConnectionSchema = JsonConvert.DeserializeObject<ConnectionSchema>(JsonConvert.SerializeObject(ConnectionSchema));
            //if (cloneConnectionSchema != null)
            //{
            //    if (cloneConnectionSchema.Childs != null)
            //    {
            //        cloneConnectionSchema.Childs.ForEach(x =>
            //        {
            //            if (!x.Checked)
            //                cloneConnectionSchema.Stations.Remove(x);
            //            else
            //                FilterCheckedItems(x);
            //        });
            //    }
            //}
            return null;
        }

        private void FilterCheckedItems(object item)
        {

        }


        #endregion
    }
}
