using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace EasyScada.Core
{
    [Serializable]
    public sealed class WriteCommand
    {
        public string TagName { get; set; }
        public string Prefix { get; set; }
        public string Value { get; set; }
        public DateTime SendTime { get; set; }
        public DateTime ReceiveTime { get; set; }
        public WritePiority WritePiority { get; set; }
        public WriteMode WriteMode { get; set; }

        [JsonIgnore]
        int tryTimes = 1;
        public int TryTimes
        {
            get => tryTimes;
            set
            {
                if (value < 1 || value > 10)
                    return;
                tryTimes = value;
            }
        }

        public int Timeout { get; set; } = 10000;

        [JsonIgnore]
        int delay = 0;
        public int Delay
        {
            get => delay;
            set
            {
                if (value < 0 || value > 1000)
                    return;
                delay = value;
            }
        }
        public bool IsCustomWrite { get; set; }
        public string CustomWriteAddress { get; set; }
        public byte[] CustomWriteValue { get; set; }
        public bool AllowExecuteNextCommandsWhenFail { get; set; }
        public List<WriteCommand> NextCommands { get; set; }
    }
}
