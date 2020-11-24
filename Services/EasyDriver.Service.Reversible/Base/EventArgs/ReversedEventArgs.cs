using System;

namespace EasyDriver.Service.Reversible
{
    public class ReversedEventArgs : EventArgs
    {
        public ReverseDirection Direction { get; private set; }
        public ReversedEventArgs(ReverseDirection direction)
        {
            Direction = direction;
        }
    }
}
