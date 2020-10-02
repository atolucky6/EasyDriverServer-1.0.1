using System;
using System.Drawing;

namespace EasyScada.Winforms.Controls
{
    [Serializable]
    public class TrendLine
    {
        public bool Enabled { get; set; } = true;
        public TrendLineAlignment Alignment { get; set; }
        private Color _Color = Color.Empty;
        public Color Color
        {
            get => _Color;
            set
            {
                if (value != Color.Empty && value != Color.Transparent)
                {
                    _Color = value;
                }
            }
        }
        public string ColumnName { get; set; }

        public TrendLine()
        {
            _Color = EasyChart.Colors[EasyChart.ColorIndex];
            EasyChart.ColorIndex++;
            if (EasyChart.ColorIndex >= EasyChart.Colors.Count)
                EasyChart.ColorIndex = 0;
        }

        public override string ToString()
        {
            return ColumnName;
        }
    }

    public enum TrendLineAlignment
    {
        Left,
        Right,
    }
}
