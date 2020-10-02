using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;

namespace EasyScada.Winforms.Controls
{
    class EasyDataLoggerDesigner : ComponentDesignerBase<EasyDataLogger>
    {
        protected override ComponentDesignerAcionList<EasyDataLogger> GetActionList()
        {
            return new EasyDataLoggerDesignerActionList(Component);
        }
    }

    class EasyDataLoggerDesignerActionList : ComponentDesignerAcionList<EasyDataLogger>
    {
        public EasyDataLoggerDesignerActionList(IComponent component) : base(component)
        {
        }

        protected override void AddExtendActionItems()
        {
            actionItems.Add(new DesignerActionPropertyItem("Enabled", "Enabled", DesignerCategory.EASYSCADA, ""));
            actionItems.Add(new DesignerActionPropertyItem("AllowLogWhenTagBad", "Allow log when tag bad", DesignerCategory.EASYSCADA, ""));
            actionItems.Add(new DesignerActionPropertyItem("Interval", "Interval", DesignerCategory.EASYSCADA, ""));
            actionItems.Add(new DesignerActionMethodItem(this, "DatabaseConfig", "Configure databases", DesignerCategory.EASYSCADA, "Click here to configure databases", true));
            actionItems.Add(new DesignerActionMethodItem(this, "ColumnsConfig", "Configure columns", DesignerCategory.EASYSCADA, "Click here to configure columns", true));
        }

        public int Interval
        {
            get => BaseComponent.Interval;
            set => BaseComponent.SetValue(value);
        }

        public bool Enabled
        {
            get => BaseComponent.Enabled;
            set => BaseComponent.SetValue(value);
        }

        public bool AllowLogWhenTagBad
        {
            get => BaseComponent.AllowLogWhenTagBad;
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

        public void ColumnsConfig()
        {
            try
            {
                ColumnsConfigDesignerForm form = new ColumnsConfigDesignerForm(BaseComponent.Columns, Component.Site);
                form.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
