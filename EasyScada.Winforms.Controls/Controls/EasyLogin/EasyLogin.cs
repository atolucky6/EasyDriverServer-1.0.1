using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EasyScada.Core;
using System.ComponentModel.Design;
using System.Data.Common;
using System.Diagnostics;

namespace EasyScada.Winforms.Controls
{
    [Designer(typeof(EasyLoginDesigner))]
    public partial class EasyLogin : UserControl
    {
        #region Constructors
        public EasyLogin() 
        {
            InitializeComponent();
            if (!DesignMode)
            {
                txbPassword.KeyDown += OnKeyDown;
                txbUsername.KeyDown += OnKeyDown;
                btnLogin.Click += OnLoginClick;
                Load += OnLoaded;
            }
        }
        #endregion

        #region Non display properties
        [Browsable(false)]
        public LogProfile Database { get => AuthenticateHelper.GetDbProfile(Site); }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public TextBox UsernameTextBox { get => txbUsername; }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public TextBox PasswordTextBox { get => txbPassword; }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public Button LoginButton { get => btnLogin; }
        #endregion

        public event EventHandler LoginSuccess;

        #region Event handlers
        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                OnLoginClick(btnLogin, EventArgs.Empty);
        }

        private void OnLoginClick(object sender, EventArgs e)
        {
            if (AuthenticateHelper.Login(txbUsername.Text, txbPassword.Text, out string error))
            {
                LoginSuccess?.Invoke(this, new EventArgs());
            }
            else
            {
                MessageBox.Show(error, "Message", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void OnLoaded(object sender, EventArgs e)
        {
            try
            {
                if (!DesignMode)
                {
                    LogProfile profile = Database;
                    if (profile != null)
                    {
                        // Create database schema if not exists
                        profile.GetCommand(out DbConnection conn, out DbCommand cmd, false);
                        conn.Open();
                        cmd.CommandText = profile.GetCreateSchemaQuery();
                        cmd.ExecuteNonQuery();
                        conn.Close();
                        conn.Dispose();
                        cmd.Dispose();
                        // Create table if not exists
                        profile.GetCommand(out conn, out cmd, out DbDataAdapter adp, true);
                        conn.Open();
                        cmd.CommandText = $"create table if not exists {profile.TableName} (Id int not null primary key AUTO_INCREMENT, Username varchar(255) not null, Password varchar(255) not null, Role varchar(100));";
                        int res = cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }
        #endregion
    }
}
