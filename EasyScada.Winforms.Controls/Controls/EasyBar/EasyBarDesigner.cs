using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace EasyScada.Winforms.Controls
{
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    class EasyBarDesigner : ControlDesignerBase<EasyBar>
    {
        protected override ControlDesignerAcionList<EasyBar> GetActionList()
        {
            return new EasyBarDesignerActionList(Component);
        }
    }

    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    class EasyBarDesignerActionList : ControlDesignerAcionList<EasyBar>
    {
        public EasyBarDesignerActionList(IComponent component) : base(component)
        {
        }

        protected override void AddExtendActionItems()
        {
            actionItems.Add(new DesignerActionHeaderItem(DesignerCategory.DESIGN));
            actionItems.Add(new DesignerActionPropertyItem("Name", "Name", DesignerCategory.DESIGN, ""));

            actionItems.Add(new DesignerActionHeaderItem(DesignerCategory.APPEARANCE));
            actionItems.Add(new DesignerActionPropertyItem("Font", "Font", DesignerCategory.APPEARANCE, ""));
            actionItems.Add(new DesignerActionPropertyItem("BackColor", "BackColor", DesignerCategory.APPEARANCE, ""));
            actionItems.Add(new DesignerActionPropertyItem("ForeColor", "ForeColor", DesignerCategory.APPEARANCE, ""));
            actionItems.Add(new DesignerActionPropertyItem("FillColor", "FillColor", DesignerCategory.APPEARANCE, ""));
            actionItems.Add(new DesignerActionPropertyItem("MinValue", "MinValue", DesignerCategory.APPEARANCE, ""));
            actionItems.Add(new DesignerActionPropertyItem("MaxValue", "MaxValue", DesignerCategory.APPEARANCE, ""));
            actionItems.Add(new DesignerActionPropertyItem("Direction", "Direction", DesignerCategory.APPEARANCE, ""));
            actionItems.Add(new DesignerActionPropertyItem("DisplayMode", "DisplayMode", DesignerCategory.APPEARANCE, ""));
        }

        #region Properties

        public virtual string Name
        {
            get { return BaseControl.Name; }
            set { BaseControl.SetValue(value); }
        }

        public Color BackColor
        {
            get { return BaseControl.BackColor; }
            set { BaseControl.SetValue(value); }
        }

        public Color ForeColor
        {
            get { return BaseControl.ForeColor; }
            set { BaseControl.SetValue(value); }
        }

        public Color FillColor
        {
            get { return BaseControl.FillColor; }
            set { BaseControl.SetValue(value); }
        }

        [Editor(typeof(PathToTagPropertyEditor), typeof(UITypeEditor))]
        [Description("Select path to tag for control")]
        [Browsable(true), Category(DesignerCategory.EASYSCADA)]
        public string MinValue
        {
            get { return BaseControl.MinValue; }
            set { BaseControl.SetValue(value); }
        }

        [Editor(typeof(PathToTagPropertyEditor), typeof(UITypeEditor))]
        [Description("Select path to tag for control")]
        [Browsable(true), Category(DesignerCategory.EASYSCADA)]
        public string MaxValue
        {
            get { return BaseControl.MaxValue; }
            set { BaseControl.SetValue(value); }
        }

        public Font Font
        {
            get { return BaseControl.Font; }
            set { BaseControl.SetValue(value); }
        }

        public BarDisplayMode DisplayMode
        {
            get { return BaseControl.DisplayMode; }
            set { BaseControl.SetValue(value); }
        }

        public BarDirection Direction
        {
            get { return BaseControl.Direction; }
            set { BaseControl.SetValue(value); }
        }

        #endregion

        #region Actions methods

        #endregion
    }
}
