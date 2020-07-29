using Microsoft.AspNet.SignalR.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EasyScada.Winforms.Connector
{
    partial class FormConnectionSchema : Form
    {
        #region Members

        readonly EasyDriverConnectorDesignerActionList actionList;
        readonly string serverAddress;
        readonly ushort port;
        ConnectionSchema serverConnector;
        ConnectionSchema projectConnector;
        ConnectionSchema checkedConnector;
        string projectPath;
        string debugPath;
        string releasePath;

        #endregion

        #region Constructors

        public FormConnectionSchema()
        {
            InitializeComponent();
        } 

        public FormConnectionSchema(EasyDriverConnectorDesignerActionList actionList)
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
            localProjectTree.AfterCheck += LocalProjectTree_AfterCheck;
            btnSave.Enabled = false;
            btnTransfer.Enabled = false;
            Load += FormTagFile_Load;
        }

        #endregion

        #region Event handlers

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

        private void LocalProjectTree_AfterCheck(object sender, TreeViewEventArgs e)
        {
            localProjectTree.BeginUpdate();
            if (e.Action != TreeViewAction.Unknown)
            {
                UpdateCheckStateParent(e.Node);
                UpdateCheckStateChild(e.Node, e.Node.Checked);

                if (serverProjectTree.Nodes.Count > 0)
                    btnTransfer.Enabled = serverProjectTree.Nodes[0].Checked;
                else
                    btnTransfer.Enabled = false;
                btnSave.Enabled = true;
            }
            localProjectTree.EndUpdate();
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
            await ReloadServerProjectTree();
        }

        private void btnContextRefreshLocal_Click(object sender, EventArgs e)
        {
            btnSave.Enabled = false;
            ReloadLocalProjectTree();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            checkedConnector = new ConnectionSchema()
            {
                RefreshRate = serverConnector.RefreshRate,
                CommunicationMode = serverConnector.CommunicationMode,
                CreatedDate = DateTime.Now,
                Port = serverConnector.Port,
                ServerAddress = serverConnector.ServerAddress,
                Stations = new List<Station>()
            };
            foreach (TreeNode serverRoot in serverProjectTree.Nodes)
            {
                foreach (TreeNode serverNode in serverRoot.Nodes)
                {
                    Station station = GetCheckedStation(serverNode);
                    if (station != null)
                        checkedConnector.Stations.Add(station);
                }
            }

            if (checkedConnector != null)
            {
                if (Directory.Exists(debugPath))
                {
                    File.WriteAllText($"{debugPath}\\ConnectionSchema.json", JsonConvert.SerializeObject(checkedConnector, Formatting.Indented));
                }
                else
                {
                    MessageBox.Show($"The directory '{debugPath}' doesn't exist. Please build the project first!");
                    return;
                }

                if (Directory.Exists(releasePath))
                {
                    File.WriteAllText($"{releasePath}\\ConnectionSchema.json", JsonConvert.SerializeObject(checkedConnector, Formatting.Indented));
                }
                else
                {
                    MessageBox.Show($"The directory '{debugPath}' doesn't exist. Please build the project first!");
                    return;
                }
                btnSave.Enabled = false;
                Thread.Sleep(300);
                ReloadLocalProjectTree();
            }
        }

        private void btnTransfer_Click(object sender, EventArgs e)
        {
            try
            {
                btnSave.Enabled = true;
                checkedConnector = new ConnectionSchema()
                {
                    RefreshRate = serverConnector.RefreshRate,
                    CommunicationMode = serverConnector.CommunicationMode,
                    CreatedDate = DateTime.Now,
                    Port = serverConnector.Port,
                    ServerAddress = serverConnector.ServerAddress,
                    Stations = new List<Station>()
                };
                foreach (TreeNode serverRoot in serverProjectTree.Nodes)
                {
                    foreach (TreeNode serverNode in serverRoot.Nodes)
                    {
                        Station station = GetCheckedStation(serverNode);
                        if (station != null)
                            checkedConnector.Stations.Add(station);
                    }
                }
                projectConnector = checkedConnector;
                RefreshProjectTreeView(localProjectTree, projectConnector, true);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Easy Driver Server");
            }
        }

        #endregion

        #region Methods

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

        private void ReloadLocalProjectTree()
        {
            projectConnector = null;

            if (!Directory.Exists(debugPath))
                Directory.CreateDirectory(debugPath);
            if (!Directory.Exists(releasePath))
                Directory.CreateDirectory(releasePath);

            if (File.Exists(debugPath + "\\ConnectionSchema.json"))
            {
                string tagFileJson = File.ReadAllText(debugPath + "\\ConnectionSchema.json");
                try
                {
                    projectConnector = JsonConvert.DeserializeObject<ConnectionSchema>(tagFileJson);
                }
                catch { }
            }
            else if (File.Exists(releasePath + "\\ConnectionSchema.json"))
            {
                string tagFileJson = File.ReadAllText(releasePath + "\\ConnectionSchema.json");
                try
                {
                    projectConnector = JsonConvert.DeserializeObject<ConnectionSchema>(tagFileJson);
                }
                catch { }
            }

            if (projectConnector != null)
                btnSave.Enabled = true;

            RefreshProjectTreeView(localProjectTree, projectConnector, true);
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
                        serverConnector = new ConnectionSchema()
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

        private void RefreshProjectTreeView(TreeView treeView, ConnectionSchema connector, bool allowCheck = false)
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
                root.Tag = connector;
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
            int imgIndex = 0;
            if (station.StationType == StationType.Local)
                imgIndex = 1;
            string stationName = station.Name;
            if (station.StationType == StationType.Remote)
                stationName += $" - {station.RemoteAddress}:{station.Port}";
            TreeNode treeNode = new TreeNode(stationName, imgIndex, imgIndex);
            treeNode.Tag = station;
            treeNode.Checked = station.Checked;
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
            TreeNode treeNode = new TreeNode($"{channel.Name} - {channel.DriverName}", 2, 2);
            treeNode.Tag = channel;
            treeNode.Checked = channel.Checked;
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
            treeNode.Tag = device;
            treeNode.Checked = device.Checked;
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
            treeNode.Tag = tag;
            treeNode.Checked = tag.Checked;
            return treeNode;
        }

        private Station GetCheckedStation(TreeNode treeNode)
        {
            if (treeNode.Checked && treeNode.Tag is Station station)
            {
                Station result = DeepCopy(station);
                result.Checked = true;
                List<Channel> channels = new List<Channel>();
                List<Station> stations = new List<Station>();
                foreach (TreeNode childNode in treeNode.Nodes)
                {
                    if (childNode.Tag is Channel)
                    {
                        Channel channel = GetCheckedChannel(childNode);
                        if (channel != null)
                            channels.Add(channel);
                    }
                    else if (childNode.Tag is Station)
                    {
                        Station innerStation = GetCheckedStation(childNode);
                        if (innerStation != null)
                            stations.Add(innerStation);
                    }
                }
                result.Channels = channels;
                result.RemoteStations = stations;
                return result;
            }
            return null;
        }

        private Channel GetCheckedChannel(TreeNode treeNode)
        {
            if (treeNode.Checked && treeNode.Tag is Channel channel)
            {
                Channel result = DeepCopy(channel);
                result.Checked = true;
                List<Device> devices = new List<Device>();
                foreach (TreeNode childNode in treeNode.Nodes)
                {
                    Device device = GetCheckedDevice(childNode);
                    if (device != null)
                        devices.Add(device);
                }
                result.Devices = devices;
                return result;
            }
            return null;
        }

        private Device GetCheckedDevice(TreeNode treeNode)
        {
            if (treeNode.Checked && treeNode.Tag is Device device)
            {
                Device result = DeepCopy(device);
                result.Checked = true;
                List<Tag> tags = new List<Tag>();
                foreach (TreeNode childNode in treeNode.Nodes)
                {
                    Tag tag = GetCheckedTag(childNode);
                    if (tag != null)
                        tags.Add(tag);
                }
                result.Tags = tags;
                return result;
            }
            return null;
        }

        private Tag GetCheckedTag(TreeNode treeNode)
        {
            if (treeNode.Checked && treeNode.Tag is Tag tag)
            {
                Tag result = DeepCopy(tag);
                result.Checked = true;
                return result;
            }
            return null;
        }

        private T DeepCopy<T>(T item)
            where T : class
        {
            using (var stream = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, item);
                stream.Seek(0, SeekOrigin.Begin);

                return formatter.Deserialize(stream) as T;
            }
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

        private void btnContextCollapseAllServer_Click(object sender, EventArgs e)
        {
            serverProjectTree.ExpandAll();
        }

        private void btnContextExpandAllServer_Click(object sender, EventArgs e)
        {
            serverProjectTree.CollapseAll();
        }

        private void btnContextExpandAllProject_Click(object sender, EventArgs e)
        {
            localProjectTree.ExpandAll();
        }

        private void btnContextCollapseAllProject_Click(object sender, EventArgs e)
        {
            localProjectTree.CollapseAll();
        }

        #endregion
    }
}
