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
            actionItems.Add(new DesignerActionHeaderItem(DesignerCategory.DESIGN));
            actionItems.Add(new DesignerActionPropertyItem("Name", "Name", DesignerCategory.DESIGN, ""));

            actionItems.Add(new DesignerActionHeaderItem(DesignerCategory.APPEARANCE));
            actionItems.Add(new DesignerActionPropertyItem("LabelStyle", "Label Style", DesignerCategory.APPEARANCE, ""));
            actionItems.Add(new DesignerActionPropertyItem("Orientation", "Orientation", DesignerCategory.APPEARANCE, ""));
            actionItems.Add(new DesignerActionPropertyItem("BackColor", "BackColor", DesignerCategory.APPEARANCE, ""));
            actionItems.Add(new DesignerActionPropertyItem("ForeColor", "ForeColor", DesignerCategory.APPEARANCE, ""));
            actionItems.Add(new DesignerActionPropertyItem("HAlignment", "HAlignment", DesignerCategory.APPEARANCE, ""));
            actionItems.Add(new DesignerActionPropertyItem("VAlignment", "VAlignment", DesignerCategory.APPEARANCE, ""));
            actionItems.Add(new DesignerActionPropertyItem("AllowTransparentBackground", "Use transparent background", DesignerCategory.APPEARANCE, ""));
            actionItems.Add(new DesignerActionPropertyItem("AutoSize", "AutoSize", DesignerCategory.APPEARANCE, ""));

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

        public LabelStyle LabelStyle
        {
            get { return BaseControl.LabelStyle; }
            set { BaseControl.SetValue(value); }
        }

        public VisualOrientation Orientation
        {
            get { return BaseControl.Orientation; }
            set { BaseControl.SetValue(value); }
        }

        public bool AllowTransparentBackground
        {
            get { return BaseControl.AllowTransparentBackground; }
            set { BaseControl.SetValue(value); }
        }

        public Color BackColor
        {
            get { return BaseControl.BackColor; }
            set { BaseControl.SetValue(value); }
        }

        public Color ForeColor
        {
            get { return BaseControl.StateNormal.ShortText.Color1; }
            set
            {
                if (BaseControl.StateNormal.ShortText.Color1 != value)
                {
                    BaseControl.StateNormal.ShortText.Color1 = value;
                }
            }
        }

        public bool AutoSize
        {
            get { return BaseControl.AutoSize; }
            set { BaseControl.SetValue(value); }
        }

        public PaletteRelativeAlign HAlignment
        {
            get { return BaseControl.StateNormal.ShortText.TextH; }
            set
            {
                if (BaseControl.StateNormal.ShortText.TextH != value)
                {
                    BaseControl.StateNormal.ShortText.TextH = value;
                }
            }
        }

        public PaletteRelativeAlign VAlignment
        {
            get { return BaseControl.StateNormal.ShortText.TextV; }
            set
            {
                if (BaseControl.StateNormal.ShortText.TextV != value)
                {
                    BaseControl.StateNormal.ShortText.TextV = value;
                }
            }
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
