using EasyScada.Winforms.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Timer timer = new Timer();
            timer.Interval = 1000;
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        Color shadedColor = Color.Red;
        int angle = 0;
        int flip = 0;
        private void Timer_Tick(object sender, EventArgs e)
        {
            
            if (shadedColor == Color.Red)
                shadedColor = Color.Green;
            else
                shadedColor = Color.Red;
            angle += 10;
            if (angle >= 360)
                angle = 0;
            flip++;
            if (flip > 3)
                flip = 0;
            ImageFlipMode flipMode = (ImageFlipMode)flip;

            foreach (var item in this.Controls)
            {
                if (item is EasyPictureBox picture)
                {
                    picture.ShadedColor = shadedColor;
                    //picture.RotateAngle = angle;
                    //picture.FlipMode = flipMode;
                }
            }
        }

        private void TrackBar1_ValueChanged(object sender, EventArgs e)
        {
        }

        private void w_Paint(object sender, PaintEventArgs e)
        {

        }

        private void easyButton1_Click(object sender, EventArgs e)
        {

        }

        private void easyButton4_Click(object sender, EventArgs e)
        {

        }

        private void easyPictureBox7_Click(object sender, EventArgs e)
        {

        }
    }
}
