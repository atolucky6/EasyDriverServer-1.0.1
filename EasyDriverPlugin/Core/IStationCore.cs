﻿using EasyDriver.Client.Models;
using System;

namespace EasyDriverPlugin
{
    public interface IStationCore : IGroupItem, ISupportParameters, ISupportSynchronization
    {
        string RemoteAddress { get; set; }
        ushort Port { get; set; }
        CommunicationMode CommunicationMode { get; set; }
        int RefreshRate { get; set; }
        string CommunicationError { get; set; }
        DateTime LastRefreshTime { get; set; }
        bool IsLocalStation { get; set; }
    }
}