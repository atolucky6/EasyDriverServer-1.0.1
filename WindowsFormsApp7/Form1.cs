using EasyScada.Core.Animate;
using EasyScada.Core.Evaluate;
using EasyScada.Winforms.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp7
{
    public partial class Form1 : EasyForm
    {
        TokenExpression token;
        Evaluator evaluator = new Evaluator();
        Task task;
        public Form1()
        {
            
            InitializeComponent();
            Application.Idle += Application_Idle;
            //Type controlType = typeof(UserControl1);

            //var animateTypes = controlType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            //    .Where(x => {
            //        if (x.CanWrite && x.GetSetMethod(true).IsPublic)
            //            return x.IsDefined(typeof(AnimateAttribute), false);
            //        return false;
            //    }).ToList();

            //foreach (var animateProperty in animateTypes)
            //{
            //    //animateProperty.SetValue(userControl11, "ahihi");
            //    AnimateAttribute animateAttribute = animateProperty.GetCustomAttribute(typeof(AnimateAttribute)) as AnimateAttribute;
            //    var valueProperty = animateProperty.PropertyType;
            //    Debug.WriteLine(userControl11.TestProperty);
            //}

            for (int i = 0; i < 500; i++)
            {
                var renderPanel = new RenderPanel();
                renderPanel.Size = new System.Drawing.Size(104, 85);
                flowLayoutPanel1.Controls.Add(renderPanel);
            }

            //timer1.Enabled = true;

            task = Task.Factory.StartNew(RefreshImage, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        private void RefreshImage()
        {
            while(true)
            {
                foreach (var item in flowLayoutPanel1.Controls)
                {
                    if (item is RenderPanel renderPanel)
                        renderPanel.Render();
                }
                Thread.Sleep(40);
            }
        }

        private void Application_Idle(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //string tagName = $"{'"'}RemoteStation1/Channel_1/Device_1/Tag_1{'"'}";
            //token = new TokenExpression($"3 < tag[{tagName}]");
            //evaluator.Evaluate(token, out string value, out string error);
            //label1.Text = value;
            //bool.TryParse(value, out bool result);
            //label2.Text = error;

            //renderPanel1.Render();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            foreach (var item in flowLayoutPanel1.Controls)
            {
                if (item is RenderPanel renderPanel)
                    renderPanel.Render();
            }
        }
    }
}
