using System;

namespace EasyScada.Winforms.Controls.Charts
{
    public class ChartPointEventArgs : EventArgs
    {
        public ChartPoint[] Points { get; private set; }
        public string Content { get; set; }

        public ChartPointEventArgs(ChartPoint[] points = null, string content = null)
        {
            Points = points;
            Content = content;
        }
    }
}
