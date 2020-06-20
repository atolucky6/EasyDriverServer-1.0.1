using System;

namespace EasyScada.ServerApplication.Reversible
{
    public class ReversingEventArgs : EventArgs
    {
        public bool Handled { get; set; }
        public ReverseDirection Direction { get; private set; }
        public ReversingEventArgs(ReverseDirection direction) => Direction = direction;
    }

    public enum ReverseDirection
    {
        Undo,
        Redo
    }
}
