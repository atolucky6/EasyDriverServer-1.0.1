using DevExpress.Xpf.Grid;
using EasyDriverPlugin;
using System.Collections;
using EasyDriver.Core;
using System.Linq;
using System.Collections.Generic;

namespace EasyScada.ServerApplication
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
                else if (item is HubModel hubModel)
                    return hubModel.Childs;
            }
            return null;
        }
    }
}
