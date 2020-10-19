using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace EasyScada.Core
{
    public class UniqueItemCollection<T> : Collection<T>, INotifyCollectionChanged where T : IUniqueNameItem
    {
        public UniqueItemCollection()
        {

        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        protected override void InsertItem(int index, T item)
        {
            base.InsertItem(index, item);
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
        }

        protected override void ClearItems()
        {
            base.ClearItems();
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        protected override void RemoveItem(int index)
        {
            T removedItem = Items[index];
            base.RemoveItem(index);
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, removedItem, index));
        }

        protected override void SetItem(int index, T item)
        {
            T oldItem = Items[index];
            base.SetItem(index, item);
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, oldItem));
        }

        public virtual T this[string name]
        {
            get
            {
                return Items.FirstOrDefault(x => x.Name == name);
            }
        }
    }
}
