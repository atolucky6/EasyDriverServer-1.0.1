using DevExpress.Xpf.Grid;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using EasyDriverPlugin;

namespace EasyDriver.Workspace.Main
{
    public class ProjectTreeChildNodeSelector : IChildNodesSelector
    {
        public bool IncludeTags { get; set; } = false;

        public IEnumerable SelectChildren(object item)
        {
            if (item != null)
            {
                if (item is IGroupItem groupItem)
                    return groupItem.Childs;
                else if (item is IClientObject clientObject)
                    return new List<IClientObject>(clientObject.Childs.Where(x => x.ItemType != ItemType.Tag));
            }
            return null;
        }
    }
}
