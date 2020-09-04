using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EasyScada.Core.Animate;

namespace WindowsFormsApp7
{
    public partial class UserControl1 : UserControl
    {
        public UserControl1()
        {
            InitializeComponent();

        }

        [Animate()]
        public Padding TestProperty { get; set; }
    }

    public enum TestEnum
    {
        a,
        b,
        c,
    }
}
