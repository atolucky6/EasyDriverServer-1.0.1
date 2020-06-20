﻿using System;
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
            index = insertBrackets ? 0 : index;
            patternName = insertBrackets ? hasBracketsSurround ? patternName.RemoveLastNumberFromString() : patternName?.Trim() : patternName.RemoveLastNumberFromString();
            string name = string.Format("{0}{1}", patternName, hasBracketsSurround || insertBrackets ? $"({index})" : $"{index}");
            while (!name.IsUniqueNameInGroup(group, null))
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
            if (item.Childs.FirstOrDefault(x => x.Name == name && x != ignoreItem) == null)
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
    }
}