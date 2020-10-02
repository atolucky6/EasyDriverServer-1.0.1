using System.Collections.Generic;
using System.Linq;

namespace EasyScada.Core
{
    public static class ConnectorHelper
    {
        public static IEnumerable<string> GetAllTagPath(this ICoreItem coreItem)
        {
            var allTags = coreItem.GetAllTags();
            return allTags.Select(X => X.Path);
        }

        public static IEnumerable<ITag> GetAllTags(this ICoreItem coreItem)
        {
            if (coreItem != null && coreItem.Childs != null)
            {
                foreach (var item in coreItem.Childs)
                {
                    if (item is ITag tag)
                        yield return tag;

                    foreach (var childTag in item.GetAllTags())
                        yield return childTag as ITag;
                }
            }
        }
    }
}
