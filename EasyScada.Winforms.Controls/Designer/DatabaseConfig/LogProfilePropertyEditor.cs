using EasyScada.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EasyScada.Winforms.Controls
{
    public class LogProfilePropertyEditor : PropertyEditorBase
    {
        //LogProfileConfigControl editControl;
        LogProfile currentValue;

        protected override Control GetEditControl(string propertyName, object currentValue)
        {
            //this.currentValue = currentValue as LogProfile;
            //editControl = new LogProfileConfigControl(this.currentValue);
            //editControl.btnOk.Click += BtnOk_Click;
            //editControl.btnCancel.Click += BtnCancel_Click;
            //return editControl;
            return null;
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            EditorService.CloseDropDown();
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            //Enum.TryParse(editControl.cobDatabaseType.Text, out Core.DbType dbtype);
            //currentValue.DatabaseType = dbtype;
            //ushort.TryParse(editControl.txbPort.Text, out ushort port);
            //currentValue.Port = port;
            //currentValue.IpAddress = editControl.txbIpAddress.Text;
            //currentValue.Username = editControl.txbUser.Text;
            //currentValue.Password = editControl.txbPassword.Text;
            //currentValue.DatabaseName = editControl.cobDatabase.Text;
            //currentValue.TableName = editControl.cobTable.Text;
            //currentValue.DataSourceName = editControl.cobDataSourceName.Text;
            //EditorService.CloseDropDown();
        }

        protected override object GetEditedValue(Control editControl, string propertyName, object oldValue)
        {
            return currentValue;
        }
    }
}
