using System.Windows.Forms;

namespace EasyScada.Winforms.Controls
{
    public partial class AckAlarmComfirm : Form
    {
        public AckAlarmComfirm()
        {
            InitializeComponent();
            txbComment.Focus();
            btnOk.Click += BtnOk_Click;
            StartPosition = FormStartPosition.CenterScreen;
        }

        private void BtnOk_Click(object sender, System.EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        public string Comment
        {
            get => txbComment.Text;
            set => txbComment.Text = value;
        }
    }
}
