using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace EasyDriverPlugin
{
    [Serializable]
    public class TagCollection : ObservableCollection<object>
    {
        #region Public members

        [field: NonSerialized]
        public bool DisableNotifyChanged { get; set; }
        public IGroupItem Owner { get; private set; }
        private ConcurrentDictionary<string, ITagCore> cache = new ConcurrentDictionary<string, ITagCore>();

        #endregion

        #region Constructors

        public TagCollection(IGroupItem owner) : base()
        {
            Owner = owner;
        }
        public TagCollection(IEnumerable<ICoreItem> items, IGroupItem owner) : base(items)
        {
            Owner = owner;
        }
        public TagCollection(IList<ICoreItem> items, IGroupItem owner) : base(items)
        {
            Owner = owner;
        }

        #endregion

        #region Methods

        public ITagCore Find(string tagName)
        {
            if (cache.TryGetValue(tagName, out ITagCore tagCore))
                return tagCore;
            return null;
        }

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
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                if (e.NewItems != null)
                {
                    foreach (var item in e.NewItems)
                    {
                        if (item is ITagCore tagCore)
                        {
                            var addTagCore = cache.AddOrUpdate(tagCore.Name, tagCore, (k, v) => 
                            {
                                v.NameChanged -= OnTagNameChanged;
                                return tagCore;
                            });
                            addTagCore.NameChanged += OnTagNameChanged;
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
                        if (item is ITagCore tagCore)
                        {
                            cache.TryRemove(tagCore.Name, out ITagCore removeTagCore);
                            if (removeTagCore != null)
                                removeTagCore.NameChanged -= OnTagNameChanged;
                        }
                    }
                }

                if (e.NewItems != null)
                {
                    foreach (var item in e.NewItems)
                    {
                        if (item is ITagCore tagCore)
                        {
                            var addTagCore = cache.AddOrUpdate(tagCore.Name, tagCore, (k, v) =>
                            {
                                v.NameChanged -= OnTagNameChanged;
                                return tagCore;
                            });
                            addTagCore.NameChanged += OnTagNameChanged;
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
                        if (item is ITagCore tagCore)
                        {
                            cache.TryRemove(tagCore.Name, out ITagCore removeTagCore);
                            if (removeTagCore != null)
                                removeTagCore.NameChanged -= OnTagNameChanged;
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
            if (sender is ITagCore tagCore)
            {
                if (e.OldValue != null)
                {
                    cache.TryRemove(e.OldValue.ToString(), out ITagCore removeTagCore);
                    cache.AddOrUpdate(tagCore.Name, tagCore, (k, v) => tagCore);
                }
            }
        }

        #endregion
    }
}
