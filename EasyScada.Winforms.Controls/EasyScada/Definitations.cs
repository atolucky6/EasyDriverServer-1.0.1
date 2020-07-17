using EasyScada.Winforms.Connector;
using System;
using System.ComponentModel;

namespace EasyScada.Winforms.Controls
{
    /// <summary>
    /// The interface provide the connector to server for all of the controls in Easy Scada
    /// </summary>
    public interface ISupportConnector
    {
        EasyDriverConnector Connector { get; set; }
    }

    /// <summary>
    /// The interface provide the select tag action for all of the controls in Easy Scada
    /// </summary>
    public interface ISupportTag
    {
        string PathToTag { get; set; }

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

    /// <summary>
    /// The interface support control applies palette when drawing
    /// </summary>
    public interface ISupportPalette
    {
        PaletteMode PaletteMode { get; set; }
    }

    public interface ISupportWriteSingleTag
    {
        WriteMode WriteMode { get; set; }
        int WriteDelay { get; set; }
        event EventHandler<TagWritingEventArgs> TagWriting;
        event EventHandler<TagWritedEventArgs> TagWrited;
    }

    public interface ISupportWriteMultiTag
    {
        WriteTagCommandCollection WriteTagCommands { get; }
    }

    [TypeConverter(typeof(WriteModeConverter))]
    public enum WriteMode
    {
        OnEnter,
        LostFocus,
        ValueChanged
    }
}
