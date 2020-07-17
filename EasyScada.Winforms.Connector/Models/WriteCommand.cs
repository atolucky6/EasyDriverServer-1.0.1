﻿using System;

namespace EasyScada.Winforms.Connector
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
