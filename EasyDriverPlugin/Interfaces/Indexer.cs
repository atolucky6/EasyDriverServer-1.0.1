using System;

namespace EasyDriverPlugin
{
    [Serializable]
    public class Indexer<T>
        where T : class
    {
        private readonly IGroupItem parent;

        public Indexer(IGroupItem parent)
        {
            this.parent = parent;
        }

        public T this[string name] => (T)parent.FirstOrDefault(x => x.Name == name);
        public T this[int index] => (T)parent.Childs[index];
    }
}
