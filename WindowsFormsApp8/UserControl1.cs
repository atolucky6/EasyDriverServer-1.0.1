using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp8
{
    public partial class UserControl1 : UserControl
    {
        public UserControl1()
        {
            InitializeComponent();
            List<string> itemsSource = new List<string>() { "abc", "def", "ghj", "123", "222", "345" };


            for (int i = 0; i < 5000; i++)
            {
                itemsSource.Add(i.ToString());
            }
            if (searchControl.Child is SearchListBox slb)
            {
                slb.ItemsSource = itemsSource;
            }
        }
    }
}
