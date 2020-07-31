using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp3
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            var s = easyDriverConnector1.Channels.tag.ToString();
        }

        private void easyLabel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
