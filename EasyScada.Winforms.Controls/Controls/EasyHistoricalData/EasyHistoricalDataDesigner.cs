using EasyScada.Core;
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
using System.Windows.Forms;

namespace EasyScada.Winforms.Controls
{
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    class EasyHistoricalDataDesigner : ControlDesignerBase<EasyHistoricalData>
    {
        protected override ControlDesignerAcionList<EasyHistoricalData> GetActionList()
        {
            return new EasyHistoricalDataDesignerActionList(Component);
        }
    }

    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    class EasyHistoricalDataDesignerActionList : ControlDesignerAcionList<EasyHistoricalData>
    {
        public EasyHistoricalDataDesignerActionList(IComponent component) : base(component)
        {
        }

        protected override void AddExtendActionItems()
        {
            actionItems.Add(new DesignerActionHeaderItem(DesignerCategory.DESIGN));
            actionItems.Add(new DesignerActionPropertyItem("Name", "Name", DesignerCategory.DESIGN, ""));

            actionItems.Add(new DesignerActionHeaderItem(DesignerCategory.APPEARANCE));
            actionItems.Add(new DesignerActionPropertyItem("Font", "Font", DesignerCategory.APPEARANCE, ""));
            actionItems.Add(new DesignerActionPropertyItem("BackColor", "BackColor", DesignerCategory.APPEARANCE, ""));
            actionItems.Add(new DesignerActionPropertyItem("Dock", "Dock", DesignerCategory.APPEARANCE, ""));

            actionItems.Add(new DesignerActionPropertyItem("Title", "Title", DesignerCategory.APPEARANCE, ""));
            actionItems.Add(new DesignerActionPropertyItem("ReportTitle", "ReportTitle", DesignerCategory.APPEARANCE, ""));
            actionItems.Add(new DesignerActionPropertyItem("Columns", "Columns", DesignerCategory.APPEARANCE, ""));
            actionItems.Add(new DesignerActionPropertyItem("CustomQuery", "CustomQuery", DesignerCategory.APPEARANCE, ""));
            actionItems.Add(new DesignerActionPropertyItem("UseCustomQuery", "UseCustomQuery", DesignerCategory.APPEARANCE, ""));

            actionItems.Add(new DesignerActionMethodItem(this, "DatabaseConfig", "Configure databases", DesignerCategory.EASYSCADA, "Click here to configure databases", true));
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

        public DockStyle Dock
        {
            get { return BaseControl.Dock; }
            set { BaseControl.SetValue(value); }
        }

        public Font Font
        {
            get { return BaseControl.Font; }
            set { BaseControl.SetValue(value); }
        }

        public string Title
        {
            get { return BaseControl.Title; }
            set { BaseControl.SetValue(value); }
        }

        public string ReportTitle
        {
            get { return BaseControl.ReportTitle; }
            set { BaseControl.SetValue(value); }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Editor("System.Windows.Forms.Design.ListControlStringCollectionEditor, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
        [Localizable(true)]
        [MergableProperty(false)]
        [Browsable(true), Category(DesignerCategory.EASYSCADA)]
        public List<string> Columns
        {
            get { return BaseControl.Columns; }
            set { BaseControl.SetValue(value); }
        }

        public bool UseCustomQuery
        {
            get { return BaseControl.UseCustomQuery; }
            set { BaseControl.SetValue(value); }
        }

        public string CustomQuery
        {
            get { return BaseControl.CustomQuery; }
            set { BaseControl.SetValue(value); }
        }
        #endregion

        #region Actions methods
        public void DatabaseConfig()
        {
            LogProfileConfigDesignerForm form = new LogProfileConfigDesignerForm();
            form.Databases = BaseControl.Databases;
            form.Start();
            if (form.ShowDialog() == DialogResult.OK)
            {
                BaseControl.Databases.Clear();
                BaseControl.Databases.AddRange(form.ResultDatabases.ToArray());
                designerActionUIservice.Refresh(Component);
                BaseControl.SetValue(BaseControl.Dock, "Dock");
            }
        }
        #endregion
    }
}