using System.Collections.Generic;

namespace EasyDriver.Reversible
{
    public class ReversibleCollectionChange<TItem> : Change
    {
        CollectionAction action;
        readonly List<TItem> backupSource;
        readonly IList<TItem> source;
        readonly IList<TItem> changedItems;
        Dictionary<TItem, int> indexDictionary;
        TItem oldItem;
        TItem newItem;

        public ReversibleCollectionChange(CollectionAction action, IList<TItem> source, IList<TItem> changedItems)
        {
            this.action = action;
            this.source = source;
            this.changedItems = changedItems;
            indexDictionary = new Dictionary<TItem, int>();
            foreach (var item in changedItems)
                indexDictionary.Add(item, source.IndexOf(item));
        }

        public ReversibleCollectionChange(CollectionAction action, IList<TItem> source, TItem changedItem)
        {
            this.action = action;
            this.source = source;
            changedItems = new List<TItem>() { changedItem };
            indexDictionary = new Dictionary<TItem, int>();
            foreach (var item in changedItems)
                indexDictionary.Add(item, source.IndexOf(item));
        }

        public ReversibleCollectionChange(IList<TItem> source, TItem oldItem, TItem newItem)
        {
            this.source = source;
            this.oldItem = oldItem;
            this.newItem = newItem;
            action = CollectionAction.Replace;
        }

        public ReversibleCollectionChange(IList<TItem> source)
        {
            this.source = source;
            backupSource = new List<TItem>(source);
        }

        public override bool Reverse()
        {
            switch (action)
            {
                case CollectionAction.Add:
                    {
                        for (int i = changedItems.Count - 1; i >= 0; i--)
                        {
                            int index = indexDictionary[changedItems[i]];
                            source.RemoveAt(index);
                        }
                        action = CollectionAction.Remove;
                        break;
                    }
                case CollectionAction.Remove:
                    {
                        for (int i = 0; i < changedItems.Count; i++)
                        {
                            int index = indexDictionary[changedItems[i]];
                            source.Insert(index, changedItems[i]);
                        }
                        action = CollectionAction.Add;
                        break;
                    }
                case CollectionAction.Replace:
                    {
                        source[source.IndexOf(newItem)] = oldItem;
                        Switch(ref oldItem, ref newItem);
                        break;
                    }
                case CollectionAction.Clear:
                    {
                        if (source.Count > 0)
                            source.Clear();
                        else
                            backupSource.ForEach(x => source.Add(x));
                        break;
                    }
                default:
                    break;
            }
            return true;
        }
    }

    public enum CollectionAction
    {
        Replace,
        Add,
        Remove,
        Clear
    }
}
