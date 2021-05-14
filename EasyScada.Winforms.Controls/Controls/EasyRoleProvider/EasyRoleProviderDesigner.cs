using EasyScada.Core;
using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Windows.Forms;

namespace EasyScada.Winforms.Controls
{
    class EasyRoleProviderDesigner : ComponentDesignerBase<EasyRoleProvider>
    {
        protected override ComponentDesignerAcionList<EasyRoleProvider> GetActionList()
        {
            return new EasyRoleProviderDesignerActionList(Component);
        }
    }

    class EasyRoleProviderDesignerActionList : ComponentDesignerAcionList<EasyRoleProvider>
    {
        public EasyRoleProviderDesignerActionList(IComponent component) : base(component)
        {
        }

        protected override void AddExtendActionItems()
        {
            actionItems.Add(new DesignerActionMethodItem(this, "Settings", "Settings", DesignerCategory.EASYSCADA, "", true));
        }

        public void Settings()
        {
            RoleDesigner designer = new RoleDesigner(Component.Site);
            if (designer.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    DesignerHelper.SaveRoleSettings(Component.Site, designer.Result);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Save role settings fail. {ex.ToString()}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
