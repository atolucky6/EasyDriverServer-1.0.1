using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace EasyDriverPlugin
{
    public interface IGroupItem : ICoreItem
    {
        /// <summary>
        /// Danh sách con của đối tượng này
        /// </summary>
        NotifyCollection Childs { get; }

        /// <summary>
        /// Hàm kiểm tra xem <see cref="ICoreItem"/> trong danh sách con có thỏa mãn điều kiện truyền vào hay không
        /// </summary>
        /// <param name="predicate">Điều kiện truyền vào</param>
        /// <param name="findDeep">Kiểm tra trong các đối tượng con</param>
        /// <returns></returns>
        bool Contains(Func<ICoreItem, bool> predicate, bool findDeep = false);

        /// <summary>
        /// Hàm tìm <see cref="ICoreItem"/> đầu tiên thỏa mãn điều kiện trong danh sách con
        /// </summary>
        /// <param name="predicate">Điều kiện truyền vào</param>
        /// <param name="findDeep">Kiểm tra trong các đối tượng con</param>
        /// <returns></returns>
        ICoreItem FirstOrDefault(Func<ICoreItem, bool> predicate, bool findDeep = false);

        /// <summary>
        /// Hàm tìm tất cả các <see cref="ICoreItem"/> thỏa mãn điều kiện trong danh sách con
        /// </summary>
        /// <param name="predicate">Điều kiện truyền vào</param>
        /// <param name="findDeep">Kiểm tra trong các đối tượng con</param>
        /// <returns></returns>
        IEnumerable<ICoreItem> Find(Func<ICoreItem, bool> predicate, bool findDeep = false);

        /// <summary>
        /// Hàm callback khi danh sách con bị thay đổi
        /// </summary>
        /// <param name="e"></param>
        void ChildCollectionChangedCallback(NotifyCollectionChangedEventArgs e);

        /// <summary>
        /// Hàm tìm đối tượng con dựa vào đường dẫn
        /// </summary>
        /// <param name="paths"></param>
        /// <param name="nameIndex"></param>
        /// <returns></returns>
        object Browse(string[] paths, int nameIndex);
    }
}
