using EasyScada.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EasyScada.Winforms.Controls
{ 
    public partial class EasyAlarmSettingDesignerForm : EasyForm
    {
        public EasyAlarmSettingDesignerForm(IServiceProvider serviceProvider)
        {
            InitializeComponent();
            easyAlarmSetting1.InitializeAlarmSetting(serviceProvider);
            FormClosing += EasyAlarmSettingDesignerForm_FormClosing;
        }

        private void EasyAlarmSettingDesignerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (easyAlarmSetting1.IsChanged)
            {
                var mbr = MessageBox.Show("The alarm setting is changed. Do you want to save now ?", "Message", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (mbr == DialogResult.Yes)
                {
                    easyAlarmSetting1.Save();
                }
                else if (mbr == DialogResult.Cancel)
                {
                    e.Cancel = true;
                }
            }
        }
    }
}
