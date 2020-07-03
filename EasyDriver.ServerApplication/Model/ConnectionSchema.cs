﻿using EasyDriver.Client.Models;
using System;
using System.Collections.Generic;

namespace EasyScada.ServerApplication
{
    [Serializable]
    public class ConnectionSchema
    {
        public string ServerAddress { get; set; }
        public ushort Port { get; set; }
        public CommunicationMode CommunicationMode { get; set; }
        public int RefreshRate { get; set; }
        public DateTime CreatedDate { get; set; }
        public List<Station> Stations { get; set; }
    }
}
