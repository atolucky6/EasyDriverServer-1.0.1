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
    class EasyGroupBoxDesigner : ControlDesignerBase<EasyGroupBox>
    {
        protected override ControlDesignerAcionList<EasyGroupBox> GetActionList()
        {
            return new EasyGroupBoxDesignerActionList(Component);
        }
    }

    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    class EasyGroupBoxDesignerActionList : ControlDesignerAcionList<EasyGroupBox>
    {
        public EasyGroupBoxDesignerActionList(IComponent component) : base(component)
        {
        }

        protected override void AddExtendActionItems()
        {
            actionItems.Add(new DesignerActionHeaderItem(DesignerCategory.DESIGN));
            actionItems.Add(new DesignerActionPropertyItem("Name", "Name", DesignerCategory.DESIGN, ""));

            actionItems.Add(new DesignerActionHeaderItem(DesignerCategory.APPEARANCE));
            actionItems.Add(new DesignerActionPropertyItem("Caption", "Caption", DesignerCategory.APPEARANCE, ""));
            actionItems.Add(new DesignerActionPropertyItem("Font", "Font", DesignerCategory.APPEARANCE, ""));
            actionItems.Add(new DesignerActionPropertyItem("ForeColor", "ForeColor", DesignerCategory.APPEARANCE, ""));
            actionItems.Add(new DesignerActionPropertyItem("Description", "Description", DesignerCategory.APPEARANCE, ""));
            actionItems.Add(new DesignerActionPropertyItem("Image", "Image", DesignerCategory.APPEARANCE, ""));
            actionItems.Add(new DesignerActionPropertyItem("CaptionLocation", "Caption Location", DesignerCategory.APPEARANCE, ""));
            actionItems.Add(new DesignerActionPropertyItem("CaptionOverlap", "Caption Overlap", DesignerCategory.APPEARANCE, ""));
            actionItems.Add(new DesignerActionPropertyItem("CaptionVisible", "Caption Visible", DesignerCategory.APPEARANCE, ""));
            actionItems.Add(new DesignerActionPropertyItem("Anchor", "Anchor", DesignerCategory.APPEARANCE, ""));
        }

        #region Properties

        public virtual string Name
        {
            get { return BaseControl.Name; }
            set { BaseControl.SetValue(value); }
        }

        public virtual string Caption
        {
            get { return BaseControl.Values.Heading; }
            set { BaseControl.Values.Heading = value; }
        }

        public AnchorStyles Anchor
        {
            get { return BaseControl.Anchor; }
            set { BaseControl.Anchor = value; }
        }

        public Font Font
        {
            get { return BaseControl.StateNormal.Content.ShortText.Font; }
            set { BaseControl.StateNormal.Content.ShortText.Font = value; }
        }

        public Color ForeColor
        {
            get { return BaseControl.StateNormal.Content.ShortText.Color1; }
            set
            {
                if (BaseControl.StateNormal.Content.ShortText.Color1 != value)
                {
                    BaseControl.StateNormal.Content.ShortText.Color1 = value;
                }
            }
        }

        public string Description
        {
            get { return BaseControl.Values.Description; }
            set { BaseControl.Values.Description = value; }
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

        public VisualOrientation CaptionLocation
        {
            get { return BaseControl.CaptionEdge; }
            set { BaseControl.CaptionEdge = value; }
        }

        public double CaptionOverlap
        {
            get { return BaseControl.CaptionOverlap; }
            set { BaseControl.CaptionOverlap = value; }
        }

        public bool CaptionVisible
        {
            get { return BaseControl.CaptionVisible; }
            set { BaseControl.CaptionVisible = value; }
        }

        #endregion

        #region Actions methods

        #endregion
    }
}
