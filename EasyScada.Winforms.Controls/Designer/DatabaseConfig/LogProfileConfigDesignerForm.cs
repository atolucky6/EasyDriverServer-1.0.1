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
    public partial class LogProfileConfigDesignerForm : EasyForm
    {
        #region Constructors
        public LogProfileConfigDesignerForm()
        {
            InitializeComponent();
            btnAdd.Click += BtnAdd_Click;
            btnRemove.Click += BtnRemove_Click;
            btnOk.Click += BtnOk_Click;
            btnCancel.Click += BtnCancel_Click;
            btnTest.Click += BtnTest_Click;

            cobDataSourceName.TextChanged += CobDataSourceName_TextChanged;
            cobDatabase.TextChanged += CobDatabase_TextChanged;
            cobDatabaseType.TextChanged += CobDatabaseType_TextChanged;
            txbIpAddress.TextChanged += TxbIpAddress_TextChanged;
            txbPort.TextChanged += TxbPort_TextChanged;
            txbUser.TextChanged += TxbUser_TextChanged;
            txbPassword.TextChanged += TxbPassword_TextChanged;
            cobTableName.TextChanged += CobTableName_TextChanged;

            lbDatabase.SelectedIndexChanged += LbDatabase_SelectedIndexChanged;

            foreach (Control control in easyHeaderGroup1.Panel.Controls)
                control.Enabled = false;
            btnRemove.Enabled = false;

            foreach (var item in Enum.GetValues(typeof(Core.DbType)))
                cobDatabaseType.Items.Add(item.ToString());
        }
        #endregion

        #region Properties
        private bool enableUpdate = true;
        public LogProfileCollection Databases { get; set; }
        public List<LogProfile> ResultDatabases { get; set; } = new List<LogProfile>();
        #endregion

        #region Methods
        public void Start()
        {
            if (Databases.Count > 0)
            {
                foreach (var item in Databases)
                {
                    lbDatabase.Items.Add($"{item.DatabaseType}: {item.DatabaseName} - {item.IpAddress}:{item.Port}");
                    ResultDatabases.Add(item.Clone() as LogProfile);
                }
                lbDatabase.SelectedIndex = 0;
            }
        }

        private void Fill(LogProfile profile)
        {
            enableUpdate = false;
            if (profile != null)
            {
                cobDataSourceName.Text = profile.DataSourceName;
                cobDatabase.Text = profile.DatabaseName;
                cobDatabaseType.Text = profile.DatabaseType.ToString();
                txbIpAddress.Text = profile.IpAddress;
                txbPort.Text = profile.Port.ToString();
                txbUser.Text = profile.Username;
                txbPassword.Text = profile.Password;
                cobTableName.Text = profile.TableName;
            }
            else
            {
                cobDataSourceName.Text = "";
                cobDatabase.Text = "";
                cobDatabaseType.Text = "";
                txbIpAddress.Text = "";
                txbPort.Text = "";
                txbUser.Text = "";
                txbPassword.Text = "";
                cobTableName.Text = "";
            }
            enableUpdate = true;
        }
        #endregion

        #region Event handlers
        private void BtnTest_Click(object sender, EventArgs e)
        {
            if (lbDatabase.SelectedIndex >= 0 && lbDatabase.SelectedIndex < ResultDatabases.Count)
            {
                if (ResultDatabases[lbDatabase.SelectedIndex].TestConnection())
                    MessageBox.Show("Test connection success!", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                else
                    MessageBox.Show("Test connection fail!", "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void BtnRemove_Click(object sender, EventArgs e)
        {
            if (lbDatabase.SelectedIndex > -1 && lbDatabase.SelectedIndex < ResultDatabases.Count)
            {
                var mbr = MessageBox.Show("Do you want to delete the selected object?", "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (mbr == DialogResult.Yes)
                {
                    ResultDatabases.RemoveAt(lbDatabase.SelectedIndex);
                    lbDatabase.Items.RemoveAt(lbDatabase.SelectedIndex);
                }
            }
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            LogProfile newProfile = new LogProfile();
            ResultDatabases.Add(newProfile);
            lbDatabase.Items.Add($"{newProfile.DatabaseType}: {newProfile.DatabaseName} - {newProfile.IpAddress}:{newProfile.Port}");
            lbDatabase.SelectedIndex = lbDatabase.Items.Count - 1;
        }

        private void LbDatabase_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lbDatabase.SelectedIndex > -1 && lbDatabase.SelectedIndex < ResultDatabases.Count)
            {
                foreach (Control control in easyHeaderGroup1.Panel.Controls)
                    control.Enabled = true;

                Fill(ResultDatabases[lbDatabase.SelectedIndex]);
                btnRemove.Enabled = true;
            }
            else
            {
                foreach (Control control in easyHeaderGroup1.Panel.Controls)
                    control.Enabled = false;
                Fill(null);
                btnRemove.Enabled = false;
            }
        }

        private void TxbPassword_TextChanged(object sender, EventArgs e)
        {
            if (enableUpdate && lbDatabase.SelectedIndex > -1 && lbDatabase.SelectedIndex < ResultDatabases.Count)
            {
                ResultDatabases[lbDatabase.SelectedIndex].Password = txbPassword.Text;
            }
        }

        private void CobTableName_TextChanged(object sender, EventArgs e)
        {
            if (enableUpdate && lbDatabase.SelectedIndex > -1 && lbDatabase.SelectedIndex < ResultDatabases.Count)
            {
                ResultDatabases[lbDatabase.SelectedIndex].TableName = cobTableName.Text;
            }
        }

        private void TxbUser_TextChanged(object sender, EventArgs e)
        {
            if (enableUpdate && lbDatabase.SelectedIndex > -1 && lbDatabase.SelectedIndex < ResultDatabases.Count)
            {
                ResultDatabases[lbDatabase.SelectedIndex].Username = txbUser.Text;
            }
        }

        private void TxbPort_TextChanged(object sender, EventArgs e)
        {
            if (enableUpdate && lbDatabase.SelectedIndex > -1 && lbDatabase.SelectedIndex < ResultDatabases.Count)
            {
                if (ushort.TryParse(txbPort.Text, out ushort port))
                {
                    ResultDatabases[lbDatabase.SelectedIndex].Port = port;
                    //lbDatabase.Items[lbDatabase.SelectedIndex] = $"{ResultDatabases[lbDatabase.SelectedIndex].DatabaseType}: {ResultDatabases[lbDatabase.SelectedIndex].DatabaseName} - {ResultDatabases[lbDatabase.SelectedIndex].IpAddress}:{ResultDatabases[lbDatabase.SelectedIndex].Port}";
                }
            }
        }

        private void TxbIpAddress_TextChanged(object sender, EventArgs e)
        {
            if (enableUpdate && lbDatabase.SelectedIndex > -1 && lbDatabase.SelectedIndex < ResultDatabases.Count)
            {
                ResultDatabases[lbDatabase.SelectedIndex].IpAddress = txbIpAddress.Text;
                //lbDatabase.Items[lbDatabase.SelectedIndex] = $"{ResultDatabases[lbDatabase.SelectedIndex].DatabaseType}: {ResultDatabases[lbDatabase.SelectedIndex].DatabaseName} - {ResultDatabases[lbDatabase.SelectedIndex].IpAddress}:{ResultDatabases[lbDatabase.SelectedIndex].Port}";
            }
        }

        private void CobDatabaseType_TextChanged(object sender, EventArgs e)
        {
            if (enableUpdate && lbDatabase.SelectedIndex > -1 && lbDatabase.SelectedIndex < ResultDatabases.Count)
            {
                if (Enum.TryParse(cobDatabaseType.Text, out Core.DbType dbType))
                {
                    ResultDatabases[lbDatabase.SelectedIndex].DatabaseType = dbType;
                    //lbDatabase.Items[lbDatabase.SelectedIndex] = $"{ResultDatabases[lbDatabase.SelectedIndex].DatabaseType}: {ResultDatabases[lbDatabase.SelectedIndex].DatabaseName} - {ResultDatabases[lbDatabase.SelectedIndex].IpAddress}:{ResultDatabases[lbDatabase.SelectedIndex].Port}";
                }
            }
        }

        private void CobDatabase_TextChanged(object sender, EventArgs e)
        {
            if (enableUpdate && lbDatabase.SelectedIndex > -1 && lbDatabase.SelectedIndex < ResultDatabases.Count)
            {
                ResultDatabases[lbDatabase.SelectedIndex].DatabaseName = cobDatabase.Text;
                lbDatabase.SelectedItem = $"{ResultDatabases[lbDatabase.SelectedIndex].DatabaseType}: {ResultDatabases[lbDatabase.SelectedIndex].DatabaseName} - {ResultDatabases[lbDatabase.SelectedIndex].IpAddress}:{ResultDatabases[lbDatabase.SelectedIndex].Port}";
            }
        }

        private void CobDataSourceName_TextChanged(object sender, EventArgs e)
        {
            if (enableUpdate && lbDatabase.SelectedIndex > -1 && lbDatabase.SelectedIndex < ResultDatabases.Count)
            {
                ResultDatabases[lbDatabase.SelectedIndex].DataSourceName = cobDataSourceName.Text;
            }
        }
        #endregion
    }
}
