using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace EasyScada.Core
{
    public abstract class CoreItemBase : ICoreItem, ICheckable
    {
        [JsonConstructor]
        public CoreItemBase()
        {
            Childs = new CollectionCoreItem(this);
            Properties = new Dictionary<string, string>();
        }

        public string Name { get; internal set; }

        public string Path { get; internal set; }

        public string Description { get; internal set; }

        public string Error { get; internal set; }

        public bool Checked { get; set; }

        public string DisplayInfo { get; internal set; }

        public ItemType ItemType { get; internal set; }

        public ConnectionStatus ConnectionStatus { get; internal set; }

        public Dictionary<string, string> Properties { get; internal set; }

        [JsonIgnore]
        public ICoreItem Parent { get; internal set; }

        public CollectionCoreItem Childs { get; internal set; }

        [field: NonSerialized]
        public virtual event EventHandler<TagValueChangedEventArgs> ValueChanged;

        [field: NonSerialized]
        public virtual event EventHandler<TagQualityChangedEventArgs> QualityChanged;

        internal void SetParent(ICoreItem parent)
        {
            Parent = parent;
            if (Childs == null)
                Childs = new CollectionCoreItem();
            Childs.Parent = this;
        }

        internal void RaiseValueChanged(object sender, TagValueChangedEventArgs e)
        {
            ValueChanged?.Invoke(sender, e);
            if (Parent is CoreItemBase parent)
                parent.RaiseValueChanged(sender, e);  
        }

        internal void RaiseQualityChanged(object sender, TagQualityChangedEventArgs e)
        {
            QualityChanged?.Invoke(sender, e);
            if (Parent is CoreItemBase parent)
                parent.RaiseQualityChanged(sender, e);
        }
    }
}
