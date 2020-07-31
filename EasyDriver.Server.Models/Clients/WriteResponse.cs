﻿using System;

namespace EasyDriver.Core
{
    public class WriteResponse
    {
        public bool IsSuccess { get; set; }
        public string Error { get; set; }
        public DateTime SendTime { get; set; }
        public DateTime ExecuteTime { get; set; }
        public WriteCommand WriteCommand { get; set; }
    }
}