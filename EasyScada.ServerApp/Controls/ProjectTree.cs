using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EasyScada.Core;
using EasyDriverPlugin;

namespace EasyScada.ServerApp
{
    public partial class ProjectTree : UserControl
    {
        public List<IChannel> Channels { get; set; }
        public ProjectTree()
        {
            InitializeComponent();
            Load += ProjectTree_Load;
            
        }

        private void ProjectTree_Load(object sender, EventArgs e)
        {
            Channels = new List<IChannel>();
            var channel = new Channel(null);

            var device = new Device(channel);

            channel.Add(device);
            channel.Name = "Channel";
            device.Name = "Device";

            Channels.Add(channel);
            var root = treeView1.Nodes.Add("Channel", "Channel", 0, 0);

            var deviceNode = root.Nodes.Add("Device", "Device1", 1, 1);
        }
    }
}
