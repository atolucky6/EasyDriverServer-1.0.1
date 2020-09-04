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

        int RefreshRate { get; set; }
        bool IsSubscribed { get; }
        bool IsStarted { get; }
        bool IsDisposed { get; }

        event EventHandler Started;
        event EventHandler<ConnectionStatusChangedEventArgs> ConnectionStatusChaged;
        event EventHandler ConnectionSlow;

        void Start();
        void Stop();

        ITag GetTag(string pathToTag);
        WriteResponse WriteTag(string pathToTag, string value);
        List<WriteResponse> WriteMultiTag(List<WriteCommand> writeCommands);

        Task<WriteResponse> WriteTagAsync(string pathToTag, string value);
        Task<List<WriteResponse>> WriteMultiTagAsync(List<WriteCommand> writeCommands);
    }
}
