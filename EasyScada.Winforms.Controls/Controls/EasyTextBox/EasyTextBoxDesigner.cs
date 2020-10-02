﻿    using System;
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
    class EasyTextBoxDesigner : ControlDesignerBase<EasyTextBox>
    {
        protected override ControlDesignerAcionList<EasyTextBox> GetActionList()
        {
            return new EasyTextBoxDesignerActionList(Component);
        }
    }

    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    class EasyTextBoxDesignerActionList : ControlDesignerAcionList<EasyTextBox>
    {
        public EasyTextBoxDesignerActionList(IComponent component) : base(component)
        {
        }

        protected override void AddExtendActionItems()
        {
            actionItems.Add(new DesignerActionHeaderItem(DesignerCategory.DESIGN));
            actionItems.Add(new DesignerActionPropertyItem("Name", "Name", DesignerCategory.DESIGN, ""));

            actionItems.Add(new DesignerActionHeaderItem(DesignerCategory.APPEARANCE));
            actionItems.Add(new DesignerActionPropertyItem("Text", "Text", DesignerCategory.APPEARANCE, ""));
            actionItems.Add(new DesignerActionPropertyItem("BackColor", "BackColor", DesignerCategory.APPEARANCE, ""));
            actionItems.Add(new DesignerActionPropertyItem("ForeColor", "ForeColor", DesignerCategory.APPEARANCE, ""));
            actionItems.Add(new DesignerActionPropertyItem("Font", "Font", DesignerCategory.APPEARANCE, ""));
            actionItems.Add(new DesignerActionPropertyItem("TextAlign", "TextAlign", DesignerCategory.APPEARANCE, ""));
            actionItems.Add(new DesignerActionPropertyItem("Multiline", "Multiline", DesignerCategory.APPEARANCE, ""));
            actionItems.Add(new DesignerActionPropertyItem("WordWrap", "WordWrap", DesignerCategory.APPEARANCE, ""));
        }

        #region Properties

        public virtual string Name
        {
            get { return BaseControl.Name; }
            set { BaseControl.SetValue(value); }
        }

        public HorizontalAlignment TextAlign
        {
            get { return BaseControl.TextAlign; }
            set { BaseControl.SetValue(value); }
        }

        public Color BackColor
        {
            get { return BackColor; }
            set { BaseControl.SetValue(value); }
        }

        public Color ForeColor
        {
            get { return BaseControl.ForeColor; }
            set { BaseControl.SetValue(value); }
        }

        public Font Font
        {
            get { return BaseControl.Font; }
            set { BaseControl.SetValue(value); }
        }

        public string Text
        {
            get { return BaseControl.Text; }
            set { BaseControl.SetValue(value); }
        }

        public bool Multiline
        {
            get { return BaseControl.Multiline; }
            set { BaseControl.SetValue(value); }
        }

        public bool WordWrap
        {
            get { return BaseControl.WordWrap; }
            set { BaseControl.SetValue(value); }
        }

        #endregion

        #region Actions methods

        #endregion
    }
}
