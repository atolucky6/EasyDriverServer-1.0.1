using System;
using System.Collections.Generic;

namespace EasyDriverPlugin
{
    public sealed class WriteCommand
    {
        public string PathToTag { get; set; }
        public string Value { get; set; }
        public DateTime SendTime { get; set; }
        public DateTime ReceiveTime { get; set; }
        public WritePiority WritePiority { get; set; }
        public WriteMode WriteMode { get; set; }
        public int TryTimes { get; set; }
        public bool IsCustomWrite { get; set; }
        public int Delay { get; set; }
        public string CustomWriteAddress { get; set; }
        public byte[] CustomWriteValue { get; set; }
        public List<WriteCommand> NextCommands { get; set; }
        public event EventHandler<CommandExecutedEventArgs> Executed;
    }
}
