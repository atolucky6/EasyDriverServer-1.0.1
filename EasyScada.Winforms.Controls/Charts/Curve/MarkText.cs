using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyScada.Winforms.Controls.Charts
{
    public class MarkText
    {
        public static readonly int MarkTextOffset = 5;
        
        public MarkText()
        {
            CircleBrush = Brushes.DodgerBlue;
            TextBrush = Brushes.Black;
        }

        public static MarkTextPositionStyle CalculateDirectionFromDataIndex(float[] data, int index)
        {
            MarkTextPositionStyle positionStyle;
            float val1 = index == 0 ? data[index] : data[index - 1];
            float val2 = index == data.Length - 1 ? data[index] : data[index + 1];

            if (val1 < data[index] && data[index] < val2)
                positionStyle = MarkTextPositionStyle.Left;
            else if (val1 > data[index] && data[index] > val2)
                positionStyle = MarkTextPositionStyle.Right;
            else if (val1 <= data[index] && data[index] >= val2)
                positionStyle = MarkTextPositionStyle.Up;
            else if (val1 >= data[index] && data[index] <= val2)
                positionStyle = MarkTextPositionStyle.Down;
            else
                positionStyle = MarkTextPositionStyle.Auto;
            return positionStyle;
        }

        public string CurveKey { get; set; }
        public int Index { get; set; }
        public string Text { get; set; }
        public Brush TextBrush { get; set; }
        public Brush CircleBrush { get; set; }
        public MarkTextPositionStyle PositionStyle { get; set; }
    }
}
