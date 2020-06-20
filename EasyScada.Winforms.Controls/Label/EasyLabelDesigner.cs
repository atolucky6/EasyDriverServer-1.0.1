using EasyScada.Winforms.Designer;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Security.Permissions;

namespace EasyScada.Winforms.Controls.Designer
{
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    class EasyLabelDesigner : ControlDesignerBase<EasyLabel>
    {
        protected override ControlDesignerAcionList<EasyLabel> GetActionList()
        {
            return new EasyLabelDesignerActionList(Component);
        }
    }

    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    class EasyLabelDesignerActionList : ControlDesignerAcionList<EasyLabel>
    {
        public EasyLabelDesignerActionList(IComponent component) : base(component)
        {
        }

        protected override void AddExtendActionItems()
        {
            actionItems.Add(new DesignerActionPropertyItem("Text", "Text", DesignerCategory.APPEARANCE, ""));
            actionItems.Add(new DesignerActionPropertyItem("TextAlign", "TextAlign", DesignerCategory.APPEARANCE, ""));
            actionItems.Add(new DesignerActionPropertyItem("Font", "Font", DesignerCategory.APPEARANCE, ""));
            actionItems.Add(new DesignerActionPropertyItem("AutoSize", "AutoSize", DesignerCategory.LAYOUT, ""));
        }

        #region Properties

        public bool AutoSize
        {
            get { return BaseControl.AutoSize; }
            set { BaseControl.SetValue(value); }
        }

        public string Text
        {
            get { return BaseControl.Text; }
            set { BaseControl.SetValue(value); }
        }

        public ContentAlignment TextAlign
        {
            get { return BaseControl.TextAlign; }
            set { BaseControl.SetValue(value); }
        }

        public Font Font
        {
            get { return BaseControl.Font; }
            set { BaseControl.SetValue(value); }
        }

        #endregion

        #region Actions methods

        #endregion
    }
}
