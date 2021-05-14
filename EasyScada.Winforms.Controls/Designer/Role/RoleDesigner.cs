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
    public partial class RoleDesigner : Form
    {
        IServiceProvider _serviceProvider;
        BindingList<Role> _source;

        public List<Role> Result { get; set; }

        public RoleDesigner(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            InitializeComponent();
            roleGrid.AllowUserToAddRows = true;

            Load += RoleDesigner_Load;
            btnOk.Click += BtnOk_Click;
            btnCancel.Click += BtnCancel_Click;
        }

        private void RoleDesigner_Load(object sender, EventArgs e)
        {
            Result = DesignerHelper.GetRoleSettings(_serviceProvider);
            if (Result == null)
                Result = new List<Role>();
            _source = new BindingList<Role>(Result);
            roleGrid.DataSource = _source;
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
