using Newtonsoft.Json;
using System.Collections.ObjectModel;

namespace EasyScada.Core
{
    public class CollectionCoreItem : ObservableCollection<ICoreItem>
    {
        private ICoreItem parent;
        [JsonIgnore]
        public ICoreItem Parent
        {
            get => parent;
            internal set
            {
                if (parent != value)
                {
                    parent = value;
                    foreach (var item in Items)
                    {
                        if (item is CoreItemBase coreItem)
                            coreItem.SetParent(value);
                    }
                }
            }
        }

        public CollectionCoreItem(ICoreItem coreItem) : base()
        {
            Parent = coreItem;
            CollectionChanged += CollectionCoreItem_CollectionChanged;
        }

        [JsonConstructor]
        public CollectionCoreItem() : base()
        {
            CollectionChanged += CollectionCoreItem_CollectionChanged;
        }

        private void CollectionCoreItem_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (var item in e.NewItems)
                {
                    if (item is CoreItemBase coreItem)
                        coreItem.Parent = Parent;
                }
            }
            if (e.OldItems != null)
            {
                foreach (var item in e.OldItems)
                {
                    if (item is CoreItemBase coreItem)
                        coreItem.Parent = null;
                }
            }
        }
    }
}
