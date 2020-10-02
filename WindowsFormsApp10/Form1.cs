using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace WindowsFormsApp10
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            historicalTrend1.RemoveAllCurve();

            new Thread(new ThreadStart(ThreadReadExample1)) { IsBackground = true }.Start();
        }
        Random random = new Random();

        private void ThreadReadExample1()
        {
            //Thread.Sleep(2000);
            //// 我们假定从数据库中获取到了这些数据信息
            //float[] steps = new float[2000];
            //float[] data = new float[2000];
            //float[] press = new float[2000];
            //DateTime[] times = new DateTime[2000];

            //for (int i = 0; i < data.Length; i++)
            //{
            //    steps[i] = random.Next(10);
            //    data[i] = (float)(Math.Sin(2 * Math.PI * i / 50) * 20 + 120);
            //    times[i] = DateTime.Now.AddSeconds(i - 2000);
            //    press[i] = (float)(Math.Sin(2 * Math.PI * i / 100) * 0.5d + 4.1d);
            //}

            //// 显示出数据信息来
            //Invoke(new Action(() =>
            //{
            //    historicalTrend1.SetLeftCurve("步序", steps);
            //    historicalTrend1.SetLeftCurve("温度", data, Color.DodgerBlue, true, "{0:F1} ℃");
            //    historicalTrend1.SetRightCurve("压力", press, Color.Tomato, true, "{0:F2} Mpa");
            //    historicalTrend1.SetDateTimes(times);
            //    historicalTrend1.RenderCurveUI();
            //}));
        }

    }
}
