using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;
using MessageBox = System.Windows.Forms.MessageBox;

namespace EasyScada.Winforms.Controls
{
    class EasyAlarmLoggerDesginer : ComponentDesignerBase<EasyAlarmLogger>
    {
        protected override ComponentDesignerAcionList<EasyAlarmLogger> GetActionList()
        {
            return new EasyAlarmLoggerDesginerActionList(Component);
        }
    }

    class EasyAlarmLoggerDesginerActionList : ComponentDesignerAcionList<EasyAlarmLogger>
    {
        public EasyAlarmLoggerDesginerActionList(IComponent component) : base(component)
        {
        }

        protected override void AddExtendActionItems()
        {
            actionItems.Add(new DesignerActionPropertyItem("Enabled", "Enabled", DesignerCategory.EASYSCADA, ""));
            actionItems.Add(new DesignerActionMethodItem(this, "DatabaseConfig", "Configure databases", DesignerCategory.EASYSCADA, "Click here to configure databases", true));
            actionItems.Add(new DesignerActionMethodItem(this, "AlarmConfig", "Configure alarms", DesignerCategory.EASYSCADA, "Click here to configure alarms", true));
        }

        public bool Enabled
        {
            get => BaseComponent.Enabled;
            set => BaseComponent.SetValue(value);
        }

        public void DatabaseConfig()
        {
            LogProfileConfigDesignerForm form = new LogProfileConfigDesignerForm();
            form.Databases = BaseComponent.Databases;
            form.Start();
            if (form.ShowDialog() == DialogResult.OK)
            {
                BaseComponent.Databases.Clear();
                BaseComponent.Databases.AddRange(form.ResultDatabases.ToArray());
            }
        }

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
    }
}
