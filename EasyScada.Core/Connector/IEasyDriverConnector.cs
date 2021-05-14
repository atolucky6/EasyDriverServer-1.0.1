using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EasyScada.Core
{
    public interface IEasyDriverConnector : IDisposable
    {
        string ServerAddress { get; set; }
        ushort Port { get; set; }
        CommunicationMode CommunicationMode { get; set; }
        ConnectionStatus ConnectionStatus { get; }

        bool UseMongoDb { get; set; }
        string MongoDb_ConnectionString { get; set; }
        string DatabaseName { get; set; }
        string CollectionName { get; set; }
        string StationName { get; set; }
        int Timeout { get; set; }

        int RefreshRate { get; set; }
        bool IsSubscribed { get; }
        bool IsStarted { get; }
        bool IsDisposed { get; }

        event EventHandler Started;
        event EventHandler<ConnectionStatusChangedEventArgs> ConnectionStatusChaged;
        event EventHandler ConnectionSlow;

        void Start();
        void Stop();

        Task<ConnectionSchema> GetConnectionSchemaAsync(string ipAddress, ushort port);
        Task<ConnectionSchema> GetConnectionSchemaAsync(string url);
        IEnumerable<ITag> GetAllTags();
        ITag GetTag(string pathToTag);

        IEnumerable<ICoreItem> GetAllChannels();
        ICoreItem GetChannel(string path);

        IEnumerable<ICoreItem> GetAllDevices();
        ICoreItem GetDevice(string path);

        WriteResponse WriteTag(WriteCommand cmd);
        WriteResponse WriteTag(string pathToTag, string value, WritePiority writePiority);
        List<WriteResponse> WriteMultiTag(List<WriteCommand> writeCommands);

        Task<WriteResponse> WriteTagAsync(string pathToTag, string value, WritePiority writePiority);
        Task<WriteResponse> WriteTagAsync(WriteCommand cmd);
        Task<List<WriteResponse>> WriteMultiTagAsync(List<WriteCommand> writeCommands);
    }
}
