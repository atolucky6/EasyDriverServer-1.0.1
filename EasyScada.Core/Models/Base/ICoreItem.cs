using System;
using System.Collections.Generic;

namespace EasyScada.Core
{
    public interface ICoreItem
    {
        string Name { get; }
        string Path { get; }
        string Description { get; }
        string Error { get; }
        string DisplayInfo { get; }
        ItemType ItemType { get; }
        ConnectionStatus ConnectionStatus { get; }
        Dictionary<string, string> Properties { get; }
        ICoreItem Parent { get; }
        CollectionCoreItem Childs { get; }

        event EventHandler<TagValueChangedEventArgs> ValueChanged;
        event EventHandler<TagQualityChangedEventArgs> QualityChanged;
    }
}
