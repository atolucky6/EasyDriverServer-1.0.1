using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            source = new List<string>()
            {
                "Tag1",
                "Pump",
                "Tag2",
                "Lamp",
                "Alarm"
            };

            autoCompleComboBox1.DataSource = source;
            AutoCompleteStringCollection autoCompleteStringCollection = new AutoCompleteStringCollection();
            autoCompleteStringCollection.AddRange(source.ToArray());
            autoCompleComboBox1.AutoCompleteCustomSource = autoCompleteStringCollection;
        }

        List<string> source;
    }
}
