using EasyScada.Winforms.Connector;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;

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

    public interface ISupportLinkable
    {
        List<PinInfo> PinInfos { get; }
        PinInfo SelectedPinInfo { get; }
    }

    public interface ISupportWriteMultiTag
    {
        WriteTagCommandCollection WriteTagCommands { get; }
    }

    public class PinInfo
    {
        public static readonly PinInfo Empty;
        static PinInfo()
        {
            Empty = new PinInfo();
        }

        public Rectangle DesignerRect { get; set; }
        public Point Position { get; set; }
        public PinConnectOrientation ConnectOrientation { get; set; }

        public void UpdatePosition(Point position)
        {
            Position = position;
        }

        public static bool operator ==(PinInfo info1, PinInfo info2)
        {
            return ((info1.DesignerRect == info2.DesignerRect) &&
                    (info1.Position == info2.Position) &&
                    (info1.ConnectOrientation == info2.ConnectOrientation));
        }

        public static bool operator !=(PinInfo info1, PinInfo info2)
        {
            return !(info1 == info2);
        }

        public static bool Equals(PinInfo info1, PinInfo info2)
        {
            return info1 == info2;
        }

        public override bool Equals(object obj)
        {
            if ((obj == null) || !(obj is PinInfo)) return false;
            return Equals(this, (PinInfo)obj);
        }

        public override int GetHashCode()
        {
            return this.DesignerRect.GetHashCode() ^ this.Position.GetHashCode() ^ this.ConnectOrientation.GetHashCode();
        }
    }

    public enum PinConnectOrientation
    {
        None,
        Left,
        Top,
        Right,
        Bottom,
    }

    [TypeConverter(typeof(WriteModeConverter))]
    public enum WriteMode
    {
        OnEnter,
        LostFocus,
        ValueChanged
    }

    public enum AnimatePiority
    {
        Lowest = 0,
        Low = 1,
        Medium = 2,
        High = 3,
        Highest = 4
    }

    public enum AnimateMode
    {
        Discrete,
        Analog,
        Direct,
        Quality
    }

    public enum CompareMode
    {
        Equal,
        NotEqual,
        Larger,
        Smaller,
        EqualOrLarger,
        EqualOrSmaller,        
    }

    public enum CompareValueMode
    {
        Const,
        UseTag
    }

}
