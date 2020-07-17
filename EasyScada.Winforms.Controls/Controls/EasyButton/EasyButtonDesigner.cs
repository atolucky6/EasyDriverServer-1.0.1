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
            actionItems.Add(new DesignerActionPropertyItem("Orientation", "Orientation", DesignerCategory.APPEARANCE, ""));
            actionItems.Add(new DesignerActionPropertyItem("BackColor", "BackColor", DesignerCategory.APPEARANCE, ""));
            actionItems.Add(new DesignerActionPropertyItem("ForeColor", "ForeColor", DesignerCategory.APPEARANCE, ""));
            actionItems.Add(new DesignerActionPropertyItem("BorderColor", "BorderColor", DesignerCategory.APPEARANCE, ""));

            actionItems.Add(new DesignerActionHeaderItem(DesignerCategory.VALUES));
            actionItems.Add(new DesignerActionPropertyItem("Text", "Text", DesignerCategory.VALUES, ""));
            actionItems.Add(new DesignerActionPropertyItem("ExtraText", "Extra Text", DesignerCategory.VALUES, ""));
            actionItems.Add(new DesignerActionPropertyItem("Image", "Image", DesignerCategory.VALUES, ""));

        }

        #region Properties

        public virtual string Name
        {
            get { return BaseControl.Name; }
            set { BaseControl.SetValue(value); }
        }

        public VisualOrientation Orientation
        {
            get { return BaseControl.Orientation; }
            set { BaseControl.SetValue(value); }
        }

        public Color BackColor
        {
            get { return BaseControl.StateCommon.Back.Color1; }
            set { BaseControl.StateCommon.Back.Color1 = value; }
        }

        public Color ForeColor
        {
            get { return BaseControl.StateCommon.Content.ShortText.Color1; }
            set { BaseControl.StateCommon.Content.ShortText.Color1 = value; }
        }

        public Color BorderColor
        {
            get { return BaseControl.StateCommon.Border.Color1; }
            set { BaseControl.StateCommon.Border.Color1 = value; }
        }

        public string Text
        {
            get { return BaseControl.Text; }
            set { BaseControl.SetValue(value); }
        }

        public string ExtraText
        {
            get { return BaseControl.Values.ExtraText; }
            set
            {
                if (BaseControl.Values.ExtraText != value)
                {
                    BaseControl.Values.ExtraText = value;
                }
            }
        }

        public Image Image
        {
            get { return BaseControl.Values.Image; }

            set
            {
                if (BaseControl.Values.Image != value)
                {
                    BaseControl.Values.Image = value;
                }
            }
        }

        #endregion

        #region Actions methods

        #endregion
    }
}
