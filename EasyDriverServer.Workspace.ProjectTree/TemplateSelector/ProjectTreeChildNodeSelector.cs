using DevExpress.Xpf.Grid;
using EasyDriverPlugin;
using System.Collections;
using EasyDriver.Core;
using System.Linq;

namespace EasyDriverServer.Workspace.ProjectTree
{
    public class ProjectTreeChildNodeSelector : IChildNodesSelector
    {
        public IEnumerable SelectChildren(object item)
        {
            if (item != null)
            {
                if (item is IGroupItem groupItem)
                {
                    if (item is IHaveTag)
                        return null;
                    return groupItem.Childs;
                }
            }
            return null;
        }
    }
}
