using ComponentFactory.Easy.Toolkit;
using EasyScada.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace EasyScada.Winforms.Controls
{
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    class EasyButtonDesigner : ControlDesignerBase<EasyButton>
    {
        protected override ControlDesignerAcionList<EasyButton> GetActionList()
        {
            return new EasyButtonDesignerActionList(Component);
        }
    }

    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    class EasyButtonDesignerActionList : ControlDesignerAcionList<EasyButton>
    {
        public EasyButtonDesignerActionList(IComponent component) : base(component)
        {
        }

        protected override void AddExtendActionItems()
        {
            actionItems.Add(new DesignerActionHeaderItem(DesignerCategory.DESIGN));
            actionItems.Add(new DesignerActionPropertyItem("Name", "Name", DesignerCategory.DESIGN, ""));

            actionItems.Add(new DesignerActionHeaderItem(DesignerCategory.APPEARANCE));
            actionItems.Add(new DesignerActionPropertyItem("Text", "Text", DesignerCategory.APPEARANCE, ""));
            actionItems.Add(new DesignerActionPropertyItem("Font", "Font", DesignerCategory.APPEARANCE, ""));
            actionItems.Add(new DesignerActionPropertyItem("BackColor", "BackColor", DesignerCategory.APPEARANCE, ""));
            actionItems.Add(new DesignerActionPropertyItem("ForeColor", "ForeColor", DesignerCategory.APPEARANCE, ""));
            actionItems.Add(new DesignerActionPropertyItem("BorderColor", "BorderColor", DesignerCategory.APPEARANCE, ""));
            actionItems.Add(new DesignerActionPropertyItem("BorderThickness", "BorderThickness", DesignerCategory.APPEARANCE, ""));
        }

        #region Properties

        public virtual string Name
        {
            get { return BaseControl.Name; }
            set { BaseControl.SetValue(value); }
        }

        public string Text
        {
            get { return BaseControl.Text; }
            set { BaseControl.SetValue(value); }
        }

        public Color BackColor
        {
            get => BaseControl.BackColor;
            set => BaseControl.SetValue(value);
        }

        public Color ForeColor
        {
            get => BaseControl.ForeColor;
            set => BaseControl.SetValue(value);
        }

        public Font Font
        {
            get => BaseControl.Font;
            set => BaseControl.SetValue(value);
        }

        public Color BorderColor
        {
            get => BaseControl.BorderColor;
            set => BaseControl.SetValue(value);
        }

        public int BorderThickness
        {
            get => BaseControl.BorderThickness;
            set => BaseControl.SetValue(value);
        }

        #endregion

        #region Actions methods

        #endregion
    }
}
