using DevExpress.Xpf.Grid;
using EasyDriverPlugin;
using System.Collections;

namespace EasyDriverServer.Workspace.ProjectTree
{
    public class TagChildNodeSelector : IChildNodesSelector
    {
        public IEnumerable SelectChildren(object item)
        {
            if (item != null)
            {
                return null;
                //if (item is ITag tag)
                //{
                //    if (tag.DataType != null)
                //        return tag.DataType.Childs;
                //}
                //else if (item is IDataTypeIndex dtIndex)
                //{
                //    if (dtIndex.DataType != null)
                //        return dtIndex.DataType.Childs;
                //}
            }
            return null;
        }
    }
}
