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
        ObservableCoreItems Childs { get; }

        /// <summary>
        /// Hàm thêm <see cref="ICoreItem"/> vào danh sách con
        /// </summary>
        /// <param name="item">Đối tượng cần thêm</param>
        /// <returns></returns>
        bool Add(ICoreItem item);

        /// <summary>
        /// Hàm thêm một danh sách <see cref="ICoreItem"/> vào danh sách con
        /// </summary>
        /// <param name="items">Danh sách đối tượng cần thêm</param>
        void Add(IEnumerable<ICoreItem> items);

        /// <summary>
        /// Xóa <see cref="ICoreItem"/> khỏi danh sách con
        /// </summary>
        /// <param name="item">Đối tượng cần xóa</param>
        /// <returns></returns>
        bool Remove(ICoreItem item);

        /// <summary>
        /// Hàm xóa một danh sách <see cref="ICoreItem"/> trong danh sách con
        /// </summary>
        /// <param name="items">Danh sách đối tượng cần xóa</param>
        void Remove(IEnumerable<ICoreItem> items);

        /// <summary>
        /// Hàm kiểm tra <see cref="ICoreItem"/> có tồn tại trong danh sách con không
        /// </summary>
        /// <param name="item">Đối tượng cần kiểm tra</param>
        /// <returns></returns>
        bool Contains(ICoreItem item);

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
    }
}
