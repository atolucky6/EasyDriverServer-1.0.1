namespace EasyDriverPlugin
{
    public interface IDevice : IGroupItem, ISupportParameters, ISupportSynchronization
    {
        Indexer<ITag> Tags { get; }
        ByteOrder ByteOrder { get; set; }
    }
}
