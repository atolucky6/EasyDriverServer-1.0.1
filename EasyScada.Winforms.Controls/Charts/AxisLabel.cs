using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyScada.Winforms.Controls.Charts
{
    public class AxisLabel
    {
        public string Content { get; set; }
        public double Value { get; set; }

        public AxisLabel() : this(0.0)
        {

        }

        public AxisLabel(double value)
        {
            Value = value;
            Content = value.ToString();
        }
    }
}
