using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace EasyDriverPlugin
{
    [Serializable]
    public class ObservableCoreItems : ObservableCollection<object>
    {
        #region Public members

        [field: NonSerialized]
        public bool DisableNotifyChanged { get; set; }
        public IGroupItem Owner { get; private set; }

        #endregion

        #region Constructors

        public ObservableCoreItems(IGroupItem owner) : base()
        {
            Owner = owner;
        }
        public ObservableCoreItems(IEnumerable<ICoreItem> items, IGroupItem owner) : base(items)
        {
            Owner = owner;
        }
        public ObservableCoreItems(IList<ICoreItem> items, IGroupItem owner) : base(items)
        {
            Owner = owner;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Hàm thêm nhiều đối tượng vào danh sách
        /// </summary>
        /// <param name="collection"></param>
        public void AddRange(IEnumerable<ICoreItem> collection)
        {
            foreach (var item in collection) { Items.Add(item); }
            NotifyResetCollection();
        }

        /// <summary>
        /// Hàm xóa nhiều đối tượng trong danh sách
        /// </summary>
        /// <param name="collection"></param>
        public void RemoveRange(IEnumerable<object> collection)
        {
            foreach (var item in collection) Items.Remove(item);
            NotifyResetCollection();
        }

        /// <summary>
        /// Chạy hàm truyền vào với tất cả các đối tượng trong danh sách
        /// </summary>
        /// <param name="action"></param>
        public void ForEach(Action<ICoreItem> action)
        {
            foreach (var item in Items) { action.Invoke(item as ICoreItem); }
        }

        /// <summary>
        /// Chạy hàm truyền vào với tất cả các đối tượng trong danh sách
        /// </summary>
        /// <param name="action"></param>
        public void ForEach<T>(Action<T> action)
            where T : ICoreItem
        {
            foreach (var item in Items) { action.Invoke((T)item); }
        }

        /// <summary>
        /// Hàm thông báo làm mối đối tượng
        /// </summary>
        public virtual void NotifyResetCollection()
        {
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            OnPropertyChanged(new PropertyChangedEventArgs(nameof(Count)));
        }

        /// <summary>
        /// Hàm thông báo đối tượng cụ thể thay đổi
        /// </summary>
        /// <param name="item"></param>
        public virtual void NotifyItemInCollectionChanged(ICoreItem item)
        {
            if (Contains(item))
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, item, item));
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (!DisableNotifyChanged)
            {
                base.OnCollectionChanged(e);
                Owner.ChildCollectionChangedCallback(e);
            }
        }

        #endregion
    }
}
