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
    class EasyPictureBoxDesigner : ControlDesignerBase<EasyPictureBox>
    {
        protected override ControlDesignerAcionList<EasyPictureBox> GetActionList()
        {
            return new EasyPictureBoxActionList(Component);
        }
    }

    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    class EasyPictureBoxActionList : ControlDesignerAcionList<EasyPictureBox>
    {
        public EasyPictureBoxActionList(IComponent component) : base(component)
        {
        }

        protected override void AddExtendActionItems()
        {
            actionItems.Add(new DesignerActionHeaderItem(DesignerCategory.DESIGN));
            actionItems.Add(new DesignerActionPropertyItem("Name", "Name", DesignerCategory.DESIGN, ""));

            actionItems.Add(new DesignerActionHeaderItem(DesignerCategory.APPEARANCE));
            actionItems.Add(new DesignerActionPropertyItem("Padding", "Padding", DesignerCategory.APPEARANCE, ""));

            actionItems.Add(new DesignerActionHeaderItem(DesignerCategory.VALUES));
            actionItems.Add(new DesignerActionPropertyItem("Image", "Image", DesignerCategory.VALUES, ""));
            actionItems.Add(new DesignerActionPropertyItem("FillMode", "Fill Mode", DesignerCategory.VALUES, ""));
            actionItems.Add(new DesignerActionPropertyItem("ShadedColor", "Shaded Color", DesignerCategory.VALUES, ""));
            actionItems.Add(new DesignerActionPropertyItem("RotateAngle", "Rotate Angle", DesignerCategory.VALUES, ""));
            actionItems.Add(new DesignerActionPropertyItem("FlipMode", "Flip Mode", DesignerCategory.VALUES, ""));
            actionItems.Add(new DesignerActionMethodItem(this, "Animate", "Animate", DesignerCategory.EASYSCADA, "", true));
        }

        #region Properties

        public virtual string Name
        {
            get { return BaseControl.Name; }
            set { BaseControl.SetValue(value); }
        }

        [Localizable(true)]
        [TypeConverter(typeof(PaddingConverter))]
        public virtual Padding Padding
        {
            get { return BaseControl.Padding; }
            set { BaseControl.SetValue(value); }
        }

        public virtual Image Image
        {
            get { return BaseControl.Image; }
            set { BaseControl.SetValue(value); }
        }

        public virtual int RotateAngle
        {
            get { return BaseControl.RotateAngle; }
            set { BaseControl.SetValue(value); }
        }

        public virtual ImageFillMode FillMode
        {
            get { return BaseControl.FillMode; }
            set { BaseControl.SetValue(value); }
        }

        public virtual ImageFlipMode FlipMode
        {
            get { return BaseControl.FlipMode; }
            set { BaseControl.SetValue(value); }
        }

        public virtual Color ShadedColor
        {
            get { return BaseControl.ShadedColor; }
            set { BaseControl.SetValue(value); }
        }


        #endregion

        #region Actions methods

        public void Animate()
        {
            //AnimatesDesignerForm form = new AnimatesDesignerForm(BaseControl.Triggers, Component.Site);
            //form.ShowDialog();
            AnimateDesignerForm form = new AnimateDesignerForm(BaseControl.Triggers, Component.Site, BaseControl.TagPath);
            form.ShowDialog();
        }

        #endregion
    }
}
