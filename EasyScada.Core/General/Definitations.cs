using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace EasyScada.Core
{
    /// <summary>
    /// The interface provide the connector to server for all of the controls in Easy Scada
    /// </summary>
    public interface ISupportConnector
    {
        IEasyDriverConnector Connector { get; }
    }

    /// <summary>
    /// The interface provide the select tag action for all of the controls in Easy Scada
    /// </summary>
    public interface ISupportTag
    {
        string TagPath { get; set; }

        ITag LinkedTag { get; }
    }

    /// <summary>
    /// The interface support scale the value received
    /// </summary>
    public interface ISupportScale
    {
        bool EnableScale { get; set; }
        double Gain { get; set; }
        double Offset { get; set; }
        decimal RawValue { get; }
        decimal ScaledValue { get; }
    }

    public interface ISupportWriteSingleTag
    {
        WriteTrigger WriteTrigger { get; set; }
        int WriteDelay { get; set; }
        event EventHandler<TagWritingEventArgs> TagWriting;
        event EventHandler<TagWritedEventArgs> TagWrited;
    }

    public enum WriteTrigger
    {
        OnEnter,
        LostFocus,
        ValueChanged,
    }
}
