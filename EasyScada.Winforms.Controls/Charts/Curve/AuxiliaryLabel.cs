using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyScada.Winforms.Controls.Charts
{
    public class AuxiliaryLabel
    {
        public AuxiliaryLabel()
        {
            TextBrush = Brushes.Black;
            TextBack = Brushes.Transparent;
            LocationX = 0.5f;
        }

        public string Text { get; set; }
        public Brush TextBrush { get; set; }
        public Brush TextBack { get; set; }
        public float LocationX { get; set; }
    }
}
