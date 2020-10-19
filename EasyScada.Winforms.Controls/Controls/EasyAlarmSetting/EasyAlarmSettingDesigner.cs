using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EasyScada.Winforms.Controls
{
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    class EasyAlarmSettingDesigner : ControlDesignerBase<EasyAlarmSetting>
    {
        protected override ControlDesignerAcionList<EasyAlarmSetting> GetActionList()
        {
            return new EasyAlarmSettingDesignerActionList(Component);
        }
    }

    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    class EasyAlarmSettingDesignerActionList : ControlDesignerAcionList<EasyAlarmSetting>
    {
        public EasyAlarmSettingDesignerActionList(IComponent component) : base(component)
        {
        }

        protected override void AddExtendActionItems()
        {
            actionItems.Add(new DesignerActionMethodItem(this, "AlarmConfig", "Configure alarms", DesignerCategory.EASYSCADA, "Click here to configure alarms", true));
        }

        #region Properties

        #endregion

        #region Actions methods
        public void AlarmConfig()
        {
            try
            {
                EasyAlarmSettingDesignerForm form = new EasyAlarmSettingDesignerForm(Component.Site);
                form.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        #endregion
    }
}
