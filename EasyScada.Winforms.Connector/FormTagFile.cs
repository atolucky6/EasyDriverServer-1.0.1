using EasyDriver.Client.Models;
using Microsoft.AspNet.SignalR.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EasyScada.Winforms.Connector
{
    partial class FormTagFile : Form
    {
        readonly EasyDriverConnectorDesignerActionList actionList;
        readonly string serverAddress;
        readonly ushort port;
        DriverConnector serverConnector;
        DriverConnector projectConnector;
        DriverConnector checkedConnector;
        string projectPath;
        string debugPath;
        string releasePath;

        public FormTagFile()
        {
            InitializeComponent();
        } 

        public FormTagFile(EasyDriverConnectorDesignerActionList actionList)
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
            this.actionList = actionList;
            serverAddress = actionList.ServerAddress;
            port = actionList.Port;
            projectPath = GetCurrentDesignPath();
            debugPath = projectPath + "\\Debug";
            releasePath = projectPath + "\\Release";
            serverProjectTree.AfterCheck += ServerProjectTree_AfterCheck;
            btnSave.Enabled = false;
            btnTransfer.Enabled = false;
            Load += FormTagFile_Load;
        }

        private void ServerProjectTree_AfterCheck(object sender, TreeViewEventArgs e)
        {
            serverProjectTree.BeginUpdate();
            if (e.Action != TreeViewAction.Unknown)
            {
                
                UpdateCheckStateParent(e.Node);
                UpdateCheckStateChild(e.Node, e.Node.Checked);

                if (serverProjectTree.Nodes.Count > 0)
                    btnTransfer.Enabled = serverProjectTree.Nodes[0].Checked;
                else
                    btnTransfer.Enabled = false;

            }
            serverProjectTree.EndUpdate();
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
        }

        private void UpdateCheckStateChild(TreeNode treeNode, bool isChecked)
        {
            foreach (TreeNode item in treeNode.Nodes)
            {
                item.Checked = isChecked;

                if (item.Nodes.Count > 0)
                {
                    UpdateCheckStateChild(item, isChecked);
                }
            }
        }

        private async void FormTagFile_Load(object sender, EventArgs e)
        {
            try
            {
                ReloadLocalProjectTree();
                await ReloadServerProjectTree();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                MessageBox.Show($"Can't connect to server {serverAddress}:{port}. Please make sure Easy Driver Server application is already opened before doing this action!", "Easy Driver Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ReloadLocalProjectTree()
        {
            projectConnector = null;

            if (!Directory.Exists(debugPath))
                Directory.CreateDirectory(debugPath);
            if (!Directory.Exists(releasePath))
                Directory.CreateDirectory(releasePath);

            if (File.Exists(debugPath + "\\TagFile.json"))
            {
                string tagFileJson = File.ReadAllText(debugPath + "\\TagFile.json");
                try
                {
                    projectConnector = JsonConvert.DeserializeObject<DriverConnector>(tagFileJson);
                }
                catch { }
            }
            else if (File.Exists(releasePath + "\\TagFile.json"))
            {
                string tagFileJson = File.ReadAllText(releasePath + "\\TagFile.json");
                try
                {
                    projectConnector = JsonConvert.DeserializeObject<DriverConnector>(tagFileJson);
                }
                catch { }
            }


            RefreshProjectTreeView(localProjectTree, projectConnector);
        }

        private async Task ReloadServerProjectTree()
        {
            try
            {
                serverConnector = null;
                using (HubConnection hubConnection = new HubConnection($"http://{serverAddress}:{port}/easyScada"))
                {
                    IHubProxy hubProxy = hubConnection.CreateHubProxy("EasyDriverServerHub");
                    await hubConnection.Start();
                    if (hubConnection.State == ConnectionState.Connected)
                    {
                        serverConnector = new DriverConnector()
                        {
                            CommunicationMode = actionList.CommunicationMode,
                            CreatedDate = DateTime.Now,
                            Port = port,
                            ServerAddress = serverAddress,
                            RefreshRate = actionList.RefreshRate,
                            Stations = new List<Station>()
                        };

                        string resJson = await hubProxy.Invoke<string>("getAllStations");
                        List<Station> stations = JsonConvert.DeserializeObject<List<Station>>(resJson);
                        serverConnector.Stations = stations;
                        try
                        {

                        }
                        catch { }
                        
                    }
                    else
                    {
                        MessageBox.Show($"Can't connect to server {serverAddress}:{port}. Please make sure Easy Driver Server application is already opened before doing this action!", "Easy Driver Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show($"Can't connect to server {serverAddress}:{port}. Please make sure Easy Driver Server application is already opened before doing this action!", "Easy Driver Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                RefreshProjectTreeView(serverProjectTree, serverConnector, true);
            }
        }

        private void RefreshProjectTreeView(TreeView treeView, DriverConnector connector, bool allowCheck = false)
        {
            treeView.Nodes.Clear();
            treeView.FullRowSelect = true;
            treeView.ShowLines = true;
            treeView.ShowRootLines = true;
            treeView.ShowPlusMinus = true;
            treeView.CheckBoxes = allowCheck;
            if (connector != null)
            {
                TreeNode root = new TreeNode($"Easy Driver Server - {serverAddress}:{port}");
                if (connector.Stations != null && connector.Stations.Count > 0)
                {
                    foreach (var station in connector.Stations)
                    {
                        TreeNode childNode = GetStationTreeNode(station);
                        if (childNode != null)
                            root.Nodes.Add(childNode);
                    }
                    treeView.Nodes.Add(root);
                }
            }
        }

        private TreeNode GetStationTreeNode(Station station)
        {
            if (station == null)
                return null;
            TreeNode treeNode = new TreeNode(station.Name, 1, 1);
            if (station.Channels != null && station.Channels.Count > 0)
            {
                foreach (var channel in station.Channels)
                {
                    TreeNode childNode = GetChannelTreeNode(channel);
                    if (childNode != null)
                        treeNode.Nodes.Add(childNode);
                }
            }
            if (station.RemoteStations != null && station.RemoteStations.Count > 0)
            {
                foreach (var innerStation in station.RemoteStations)
                {
                    TreeNode childNode = GetStationTreeNode(innerStation);
                    if (childNode != null)
                        treeNode.Nodes.Add(childNode);
                }
            }
            return treeNode;
        }

        private TreeNode GetChannelTreeNode(Channel channel)
        {
            if (channel == null)
                return null;
            TreeNode treeNode = new TreeNode(channel.Name, 2, 2);
            if (channel.Devices != null && channel.Devices.Count > 0)
            {
                foreach (var device in channel.Devices)
                {
                    TreeNode childNode = GetDeviceTreeNode(device);
                    if (childNode != null)
                        treeNode.Nodes.Add(childNode);
                }
            }
            return treeNode;
        }

        private TreeNode GetDeviceTreeNode(Device device)
        {
            if (device == null)
                return null;
            TreeNode treeNode = new TreeNode(device.Name, 3, 3);
            if (device.Tags != null && device.Tags.Count > 0)
            {
                foreach (var tag in device.Tags)
                {
                    TreeNode childNode = GetTagTreeNode(tag);
                    if (childNode != null)
                        treeNode.Nodes.Add(childNode);
                }
            }
            return treeNode;
        }

        private TreeNode GetTagTreeNode(Tag tag)
        {
            if (tag == null)
                return null;
            TreeNode treeNode = new TreeNode(tag.Name, 4, 4);
            return treeNode;
        }

        private string GetCurrentDesignPath()
        {
            try
            {
                EnvDTE.DTE dte = actionList.GetService(typeof(EnvDTE.DTE)) as EnvDTE.DTE;
                return Path.GetDirectoryName(dte.ActiveDocument.FullName) + "\\bin";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return string.Empty;
            }
        }

        private async void btnReloadAll_Click(object sender, EventArgs e)
        {
            btnTransfer.Enabled = false;
            btnSave.Enabled = false;
            ReloadLocalProjectTree();
            await ReloadServerProjectTree();
        }

        private async void btnContextRefreshServer_Click(object sender, EventArgs e)
        {
            btnTransfer.Enabled = false;
            btnSave.Enabled = false;
            await ReloadServerProjectTree();
            RefreshProjectTreeView(serverProjectTree, serverConnector, true);
        }

        private void btnContextRefreshLocal_Click(object sender, EventArgs e)
        {
            btnSave.Enabled = false;
            ReloadLocalProjectTree();
            RefreshProjectTreeView(localProjectTree, projectConnector);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (checkedConnector != null)
            {
                if (Directory.Exists(debugPath))
                {
                    File.WriteAllText($"{debugPath}\\TagFile.json", JsonConvert.SerializeObject(checkedConnector));
                }
                else
                {
                    MessageBox.Show($"The directory '{debugPath}' doesn't exist. Please build the project first!");
                    return;
                }

                if (Directory.Exists(releasePath))
                {
                    File.WriteAllText($"{releasePath}\\TagFile.json", JsonConvert.SerializeObject(checkedConnector));
                }
                else
                {
                    MessageBox.Show($"The directory '{debugPath}' doesn't exist. Please build the project first!");
                    return;
                }
                btnSave.Enabled = false;
            }
        }

        private void btnTransfer_Click(object sender, EventArgs e)
        {
            btnSave.Enabled = true;
            checkedConnector = new DriverConnector()
            {
                RefreshRate = serverConnector.RefreshRate,
                CommunicationMode = serverConnector.CommunicationMode,
                CreatedDate = DateTime.Now,
                Port = serverConnector.Port,
                ServerAddress = serverConnector.ServerAddress,
                Stations = serverConnector.Stations,
            };
            projectConnector = checkedConnector;
            RefreshProjectTreeView(localProjectTree, projectConnector);
        }
    }
}
