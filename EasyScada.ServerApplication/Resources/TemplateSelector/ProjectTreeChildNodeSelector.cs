using DevExpress.Xpf.Grid;
using EasyDriverPlugin;
using EasyScada.Core;
using System.Collections;

namespace EasyScada.ServerApplication
{
    public class ProjectTreeChildNodeSelector : IChildNodesSelector
    {
        public IEnumerable SelectChildren(object item)
        {
            if (item != null)
            {
                if (item is IDevice)
                    return null;
                if (item is IGroupItem groupItem)
                    return groupItem.Childs;
            }
            return null;
        }
    }
}
