using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;

namespace EasyScada.Winforms.Controls
{
    class EasyLoginLoggerDesigner : ComponentDesignerBase<EasyLoginLogger>
    {
        protected override ComponentDesignerAcionList<EasyLoginLogger> GetActionList()
        {
            return new EasyLoginLoggerDesignerActionList(Component);
        }
    }

    class EasyLoginLoggerDesignerActionList : ComponentDesignerAcionList<EasyLoginLogger>
    {
        public EasyLoginLoggerDesignerActionList(IComponent component) : base(component)
        {
        }

        protected override void AddExtendActionItems()
        {
            actionItems.Add(new DesignerActionPropertyItem("Enabled", "Enabled", DesignerCategory.EASYSCADA, ""));
            actionItems.Add(new DesignerActionMethodItem(this, "DatabaseConfig", "Configure databases", DesignerCategory.EASYSCADA, "Click here to configure databases", true));
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
                designerActionUIservice.Refresh(Component);
                BaseComponent.SetValue(BaseComponent.Enabled, "Enabled");
            }
        }
    }
}
