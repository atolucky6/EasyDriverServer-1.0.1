using System;
using System.Collections.Generic;
using System.Linq;

namespace EasyDriverPlugin
{
    public static class Extensions
    {
        /// <summary>
        /// Tạo 1 tên mới không trùng với bất kì item nào trong group
        /// </summary>
        /// <param name="group"></param>
        /// <param name="patternName"></param>
        /// <param name="insertBrackets"></param>
        /// <returns></returns>
        public static string GetUniqueNameInGroup(this IGroupItem group, string patternName, bool insertBrackets = false)
        {
            uint index = patternName.ExtractLastNumberFromString(out bool hasIndex, out bool hasBracketsSurround);
            index = insertBrackets ? 1 : index;
            if (index == 0)
                index++;
            if (patternName.IsUniqueNameInGroup(group, null))
                return patternName;
            patternName = insertBrackets ? hasBracketsSurround ? patternName.RemoveLastNumberFromString() : patternName?.Trim() : patternName.RemoveLastNumberFromString();
            string name = string.Format("{0}{1}", patternName, hasBracketsSurround || insertBrackets ? $"({index})" : $"{index}");
            while (!name.IsUniqueNameInGroup(group, null))
            {
                index++;
                name = string.Format("{0}{1}", patternName, hasBracketsSurround || insertBrackets ? $"({index})" : $"{index}");
            }
            return name;
        }

        public static string GetUniqueNameInGroupTags(this IHaveTag group, string patternName, bool insertBrackets = false)
        {
            uint index = patternName.ExtractLastNumberFromString(out bool hasIndex, out bool hasBracketsSurround);
            index = insertBrackets ? 1 : index;
            if (index == 0)
                index++;
            if (patternName.IsUniqueNameInGroupTags(group, null))
                return patternName;
            patternName = insertBrackets ? hasBracketsSurround ? patternName.RemoveLastNumberFromString() : patternName?.Trim() : patternName.RemoveLastNumberFromString();
            string name = string.Format("{0}{1}", patternName, hasBracketsSurround || insertBrackets ? $"({index})" : $"{index}");
            while (!name.IsUniqueNameInGroupTags(group, null))
            {
                index++;
                name = string.Format("{0}{1}", patternName, hasBracketsSurround || insertBrackets ? $"({index})" : $"{index}");
            }
            return name;
        }

        /// <summary>
        /// Kiểm tra tên có phải là duy nhất trong group không
        /// </summary>
        /// <param name="name"></param>
        /// <param name="item"></param>
        /// <param name="findAllLevel"></param>
        /// <returns></returns>
        public static bool IsUniqueNameInGroup(this string name, IGroupItem item, ICoreItem ignoreItem, bool findAllLevel = false)
        {
            if (item.Childs.FirstOrDefault(x => {
                if (x is ICoreItem coreItem)
                    return coreItem.Name == name && x != ignoreItem;
                return false;
            }) == null)
                return true;
            return false;
        }

        public static bool IsUniqueNameInGroupTags(this string name, IHaveTag item, ICoreItem ignoreItem, bool findAllLevel = false)
        {
            if (item.Tags.FirstOrDefault(x =>
            {
                if (x is ICoreItem coreItem)
                    return coreItem.Name == name && x != ignoreItem;
                return false;
            }) == null)
                return true;
            return false;
        }

        /// <summary>
        /// Hàm kiểm tra xem giá trị của thuộc tính trong đối tượng có trùng với giá trị của các đối tượng khác trong danh sách
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="objCollection"></param>
        /// <param name="getValueFunc"></param>
        /// <returns></returns>
        public static bool IsUniquePropertyValue<T>(this T obj, IEnumerable<T> objCollection, Func<T, object> getValueFunc)
            where T : class
        {
            object value = getValueFunc(obj);
            if (objCollection.FirstOrDefault(x => x != obj && Equals(value, getValueFunc(x))) != null)
                return true;
            return false;
        }

        /// <summary>
        /// Tìm kiếm item có phù hợp với điều kiện hay không. Nếu không thì kiểm tra parent của item
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static T FindParent<T>(this ICoreItem item, Func<ICoreItem, bool> predicate)
            where T : ICoreItem
        {
            if (predicate(item))
                return (T)item;
            else
            {
                if (item.Parent == null)
                    return default;
                return FindParent<T>(item.Parent, predicate);
            }
        }

        public static IEnumerable<ITagCore> GetAllTags(this IGroupItem groupItem, bool findDeep = true)
        {
            if (groupItem is IHaveTag haveTagObj)
            {
                if (haveTagObj.HaveTags)
                {
                    foreach (var item in haveTagObj.Tags)
                    {
                        if (item is ITagCore tag)
                            yield return tag;
                    }
                }
            }

            if (findDeep)
            {
                foreach (var childItem in groupItem.Find(x => x is IHaveTag, true))
                {
                    if (childItem is IHaveTag haveTag)
                    {
                        if (haveTag.HaveTags)
                        {
                            foreach (var item in haveTag.Tags)
                            {
                                if (item is ITagCore tag)
                                    yield return tag;

                            }
                        }
                    }
                }
            }
        }

        public static IEnumerable<IChannelCore> GetAllChannels(this IGroupItem groupItem, bool findDeep = true)
        {
            if (!(groupItem is IDeviceCore) || !(groupItem is ITagCore))
            {
                foreach (var item in groupItem.Childs)
                {
                    if (item is IChannelCore)
                        yield return item as IChannelCore;
                    if (findDeep && !(item is ITagCore) && !(item is IDeviceCore) && !(item is IChannelCore) && item is IGroupItem childGroup)
                    {
                        foreach (var childChannel in childGroup.GetAllChannels())
                        {
                            if (childChannel is IChannelCore)
                                yield return childChannel as IChannelCore;
                        }
                    }
                }
            }
        }

        public static IEnumerable<IDeviceCore> GetAllDevices(this IGroupItem groupItem, bool findDeep = true)
        {
            if (!(groupItem is ITagCore))
            {
                foreach (var item in groupItem.Childs)
                {
                    if (item is IDeviceCore device)
                        yield return device;

                    if (findDeep && !(item is ITagCore) && !(item is IDeviceCore) && item is IGroupItem childGroup)
                    {
                        foreach (var childDevice in childGroup.GetAllDevices())
                        {
                            if (childDevice is IDeviceCore)
                                yield return childDevice as IDeviceCore;
                        }
                    }
                }
            }
        }
    }
}
