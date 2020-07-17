using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EasyScada.Winforms.Controls
{
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    class EasyWriteTagCommandDesigner : ComponentDesignerBase<EasyWriteTagCommand>
    {
        protected override ComponentDesignerAcionList<EasyWriteTagCommand> GetActionList()
        {
            return new EasyWriteTagCommandDesignerActionList(Component);
        }
    }

    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    class EasyWriteTagCommandDesignerActionList : ComponentDesignerAcionList<EasyWriteTagCommand>
    {
        public EasyWriteTagCommandDesignerActionList(IComponent component) : base(component)
        {
        }

        protected override void AddExtendActionItems()
        {
            actionItems.Add(new DesignerActionHeaderItem(DesignerCategory.VALUES));
            actionItems.Add(new DesignerActionPropertyItem("Enabled", "Enabled", DesignerCategory.VALUES, ""));
            actionItems.Add(new DesignerActionPropertyItem("WriteValue", "Write Value", DesignerCategory.VALUES, ""));
            actionItems.Add(new DesignerActionPropertyItem("WriteDelay", "Write Delay", DesignerCategory.VALUES, ""));
        }

        #region Properties

        public virtual string WriteValue
        {
            get { return BaseComponent.WriteValue; }
            set { BaseComponent.SetValue(value); }
        }

        public virtual bool Enabled
        {
            get { return BaseComponent.Enabled; }
            set { BaseComponent.SetValue(value); }
        }

        public override int WriteDelay
        {
            get { return BaseComponent.WriteDelay; }
            set { BaseComponent.SetValue(value); }
        }

        #endregion

        #region Actions methods

        #endregion
    }
}
