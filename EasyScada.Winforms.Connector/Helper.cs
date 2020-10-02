using EasyScada.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EasyScada.Winforms.Connector
{
    public static class Helper
    {
        public static TreeNode ToTreeNode(this ConnectionSchema item, bool includeChilds = true, bool includeTags = true)
        {
            TreeNode node = new TreeNode($"Easy Driver Server - {item.ServerAddress}:{item.Port}", 1, 1);
            node.Tag = item;
            if (includeChilds && item.Stations != null && item.Stations.Count > 0)
            {
                for (int i = 0; i < item.Stations.Count; i++)
                {
                    TreeNode childNode = item.Stations[i].ToTreeNode(includeChilds, includeTags);
                    if (childNode != null)
                        node.Nodes.Add(childNode);
                }
            }
            return node;
        }

        public static TreeNode ToTreeNode(this Station item, bool includeChilds = true, bool includeTags = true)
        {
            if (item == null)
                return null;
            int imageIndex = 0;
            if (item.Name != "Local Station")
                imageIndex = 1;

            TreeNode node = new TreeNode(item.Name, imageIndex, imageIndex);
            node.Tag = item;
            node.Checked = item.Checked;



            if (includeChilds && item.RemoteStations != null && item.RemoteStations.Count > 0)
            {
                for (int i = 0; i < item.RemoteStations.Count; i++)
                {
                    TreeNode childNode = item.RemoteStations[i].ToTreeNode(includeChilds, includeTags);
                    if (childNode != null)
                        node.Nodes.Add(childNode);
                }
            }
            if (includeChilds && item.Channels != null && item.Channels.Count > 0)
            {
                for (int i = 0; i < item.Channels.Count; i++)
                {
                    TreeNode childNode = item.Channels[i].ToTreeNode(includeChilds, includeTags);
                    if (childNode != null)
                        node.Nodes.Add(childNode);
                }
            }

            return node;
        }

        public static TreeNode ToTreeNode(this Channel item, bool includeChilds = true, bool includeTags = true)
        {
            if (item == null)
                return null;
            TreeNode node = new TreeNode(item.Name, 2, 2);
            node.Tag = item;
            node.Checked = item.Checked;
            if (includeChilds && item.Devices != null && item.Devices.Count > 0)
            {
                for (int i = 0; i < item.Devices.Count; i++)
                {
                    TreeNode childNode = item.Devices[i].ToTreeNode(includeChilds, includeTags);
                    if (childNode != null)
                        node.Nodes.Add(childNode);
                }
            }
            return node;
        }

        public static TreeNode ToTreeNode(this Device item, bool includeChilds = true, bool includeTags = true)
        {
            if (item == null)
                return null;
            TreeNode node = new TreeNode(item.Name, 3, 3);
            node.Tag = item;
            node.Checked = item.Checked;
            if (includeChilds && includeTags && item.Tags != null && item.Tags.Count > 0)
            {
                for (int i = 0; i < item.Tags.Count; i++)
                {
                    TreeNode childNode = item.Tags[i].ToTreeNode();
                    if (childNode != null)
                        node.Nodes.Add(childNode);
                }
            }
            return node;
        }

        public static TreeNode ToTreeNode(this Tag item, bool includeChilds = true)
        {
            if (item == null)
                return null;
            TreeNode node = new TreeNode(item.Name, 4, 4);
            node.Tag = item;
            node.Checked = item.Checked;
            return node;
        }
    }
}
