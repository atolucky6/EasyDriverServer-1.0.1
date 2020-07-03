using System;

namespace EasyDriver.Client.Models
{
    [Serializable]
    public sealed class WriteCommand
    {
        public string PathToTag { get; set; }
        public string Value { get; set; }
        public DateTime SendTime { get; set; }
        public WritePiority WritePiority { get; set; }
        public WriteMode WriteMode { get; set; }
    }
}
