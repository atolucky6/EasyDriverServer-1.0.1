using System;

namespace EasyScada.Core
{
    [Serializable]
    public sealed class WriteCommand
    {
        public string PathToTag { get; set; }
        public string Value { get; set; }
        public DateTime SendTime { get; internal set; } 
        public WritePiority WritePiority { get; set; } = WritePiority.Default;
        public WriteMode WriteMode { get; set; } = WriteMode.WriteAllValue;
    }
}
