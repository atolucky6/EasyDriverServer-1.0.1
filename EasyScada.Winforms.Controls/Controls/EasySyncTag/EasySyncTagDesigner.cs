using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Windows.Forms;

namespace EasyScada.Winforms.Controls
{
    class EasySyncTagDesigner : ComponentDesignerBase<EasySyncTag>
    {
        protected override ComponentDesignerAcionList<EasySyncTag> GetActionList()
        {
            return new EasySyncTagDesignerActionList(Component);
        }
    }

    class EasySyncTagDesignerActionList : ComponentDesignerAcionList<EasySyncTag>
    {
        public EasySyncTagDesignerActionList(IComponent component) : base(component)
        {
        }

        protected override void AddExtendActionItems()
        {
            actionItems.Add(new DesignerActionPropertyItem("Enabled", "Enabled", DesignerCategory.EASYSCADA, ""));
            actionItems.Add(new DesignerActionPropertyItem("SyncInterval", "SyncInterval", DesignerCategory.EASYSCADA, ""));
            actionItems.Add(new DesignerActionPropertyItem("Targets", "Targets", DesignerCategory.EASYSCADA, ""));
        }

        public int SyncInterval
        {
            get => BaseComponent.SyncInterval;
            set => BaseComponent.SetValue(value);
        }

        public bool Enabled
        {
            get => BaseComponent.Enabled;
            set => BaseComponent.SetValue(value);
        }

        public virtual SyncTargetCollection Targets
        {
            get => BaseComponent.Targets;
            set => BaseComponent.SetValue(value);
        }
    }
}
