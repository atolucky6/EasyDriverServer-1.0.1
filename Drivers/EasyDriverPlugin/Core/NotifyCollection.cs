using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace EasyDriverPlugin
{
    [Serializable]
    public class NotifyCollection : ObservableCollection<object>
    {
        #region Public members
        [field: NonSerialized]
        public bool DisableNotifyChanged { get; set; }
        public IGroupItem Owner { get; private set; }
        private ConcurrentDictionary<string, ICoreItem> cache = new ConcurrentDictionary<string, ICoreItem>();
        #endregion

        #region Constructors

        public NotifyCollection(IGroupItem owner) : base()
        {
            Owner = owner;
        }
        public NotifyCollection(IEnumerable<ICoreItem> items, IGroupItem owner) : base(items)
        {
            Owner = owner;
        }
        public NotifyCollection(IList<ICoreItem> items, IGroupItem owner) : base(items)
        {
            Owner = owner;
        }

        #endregion

        #region Methods

        protected override void RemoveItem(int index)
        {
            base.RemoveItem(index);
        }

        public ICoreItem Find(string name)
        {
            if (cache.TryGetValue(name, out ICoreItem coreItem))
                return coreItem;
            return null;
        }

        /// <summary>
        /// Hàm thêm nhiều đối tượng vào danh sách
        /// </summary>
        /// <param name="collection"></param>
        public void AddRange(IEnumerable<ICoreItem> collection)
        {
            foreach (var item in collection) 
            { 
                Items.Add(item); 
                item.Parent = Owner;
            }
            NotifyResetCollection();
        }

        /// <summary>
        /// Hàm xóa nhiều đối tượng trong danh sách
        /// </summary>
        /// <param name="collection"></param>
        public void RemoveRange(IEnumerable<object> collection)
        {
            foreach (var item in collection)
            {
                Items.Remove(item);
            }
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

        protected override void InsertItem(int index, object item)
        {
            (item as ICoreItem).Parent = Owner;
            base.InsertItem(index, item);
        }

        protected override void ClearItems()
        {
            base.ClearItems();
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
            if (e.NewItems != null)
            {
                foreach (var item in e.NewItems)
                {
                    if (item is ICoreItem)
                    {
                        (item as ICoreItem).Parent = Owner;
                    }
                }
            }

            if (e.OldItems != null)
            {
                foreach (var item in e.OldItems)
                {
                    if (item is ICoreItem)
                    {
                        (item as ICoreItem).Parent = null;
                    }
                }
            }

            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                if (e.NewItems != null)
                {
                    foreach (var item in e.NewItems)
                    {
                        if (item is ICoreItem coreItem)
                        {
                            var addedItem = cache.AddOrUpdate(coreItem.Name, coreItem, (k, v) =>
                            {
                                v.NameChanged -= OnTagNameChanged;
                                return coreItem;
                            });
                            addedItem.NameChanged += OnTagNameChanged;
                            coreItem.RaiseAddedEvent();
                        }
                    }
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Replace)
            {
                if (e.OldItems != null)
                {
                    foreach (var item in e.OldItems)
                    {
                        if (item is ICoreItem coreItem)
                        {
                            cache.TryRemove(coreItem.Name, out ICoreItem removeCoreItem);
                            if (removeCoreItem != null)
                                removeCoreItem.NameChanged -= OnTagNameChanged;
                            coreItem.RaiseRemovedEvent();
                        }
                    }
                }

                if (e.NewItems != null)
                {
                    foreach (var item in e.NewItems)
                    {
                        if (item is ICoreItem coreItem)
                        {
                            var addedItem = cache.AddOrUpdate(coreItem.Name, coreItem, (k, v) =>
                            {
                                v.NameChanged -= OnTagNameChanged;
                                return coreItem;
                            });
                            addedItem.NameChanged += OnTagNameChanged;
                            coreItem.RaiseAddedEvent();
                        }
                    }
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                if (e.OldItems != null)
                {
                    foreach (var item in e.OldItems)
                    {
                        if (item is ICoreItem coreItem)
                        {
                            cache.TryRemove(coreItem.Name, out ICoreItem removeCoreItem);
                            if (removeCoreItem != null)
                                removeCoreItem.NameChanged -= OnTagNameChanged;
                            coreItem.RaiseRemovedEvent();
                        }
                    }
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                if (e.NewItems != null)
                {
                    foreach (var item in e.NewItems)
                    {
                        if (item is ICoreItem coreItem)
                        {
                            coreItem.RaiseAddedEvent();
                        }
                    }
                }

                if (e.OldItems != null)
                {
                    foreach (var item in e.OldItems)
                    {
                        if (item is ICoreItem coreItem)
                        {
                            coreItem.RaiseRemovedEvent();
                        }
                    }
                }
            }

            if (!DisableNotifyChanged)
            {
                base.OnCollectionChanged(e);
                Owner.ChildCollectionChangedCallback(e);
            }
        }

        private void OnTagNameChanged(object sender, NameChangedEventArgs e)
        {
            if (sender is ICoreItem coreItem)
            {
                if (e.OldValue != null)
                {
                    cache.TryRemove(e.OldValue.ToString(), out ICoreItem removeCoreItem);
                    cache.AddOrUpdate(coreItem.Name, coreItem, (k, v) => coreItem);
                }
            }
        }

        #endregion
    }
}
