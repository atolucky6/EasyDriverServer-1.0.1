using System;
using System.Windows.Forms;

namespace EasyScada.Winforms.Controls
{
    public partial class AuthenticateForm : Form
    {
        public AuthenticateForm()
        {
            InitializeComponent();
            easyLogin1.LoginSuccess += EasyLogin1_LoginSuccess;
            DialogResult = DialogResult.Cancel;
            StartPosition = FormStartPosition.CenterScreen;
        }

        private void EasyLogin1_LoginSuccess(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
