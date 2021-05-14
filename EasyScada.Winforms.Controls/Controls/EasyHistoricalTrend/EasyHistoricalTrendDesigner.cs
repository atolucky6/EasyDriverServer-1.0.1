using EasyScada.Core;
using OxyPlot;
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
    class EasyHistoricalTrendDesigner : ControlDesignerBase<EasyHistoricalTrend>
    {
        protected override ControlDesignerAcionList<EasyHistoricalTrend> GetActionList()
        {
            return new EasyHistoricalTrendDesignerActionList(Component);
        }
    }

    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    class EasyHistoricalTrendDesignerActionList : ControlDesignerAcionList<EasyHistoricalTrend>
    {
        public EasyHistoricalTrendDesignerActionList(IComponent component) : base(component)
        {
        }

        protected override void AddExtendActionItems()
        {
            actionItems.Add(new DesignerActionHeaderItem(DesignerCategory.DESIGN));
            actionItems.Add(new DesignerActionPropertyItem("Name", "Name", DesignerCategory.DESIGN, ""));

            actionItems.Add(new DesignerActionHeaderItem(DesignerCategory.APPEARANCE));
            actionItems.Add(new DesignerActionPropertyItem("Title", "Title", DesignerCategory.APPEARANCE, ""));
            actionItems.Add(new DesignerActionPropertyItem("ReportTitle", "ReportTitle", DesignerCategory.APPEARANCE, ""));
            actionItems.Add(new DesignerActionPropertyItem("Font", "Font", DesignerCategory.APPEARANCE, ""));
            actionItems.Add(new DesignerActionPropertyItem("BackColor", "BackColor", DesignerCategory.APPEARANCE, ""));
            actionItems.Add(new DesignerActionPropertyItem("BackColorTrend", "BackColorTrend", DesignerCategory.APPEARANCE, ""));
            actionItems.Add(new DesignerActionPropertyItem("Dock", "Dock", DesignerCategory.APPEARANCE, ""));
            actionItems.Add(new DesignerActionPropertyItem("Lines", "Lines", DesignerCategory.APPEARANCE, ""));
            actionItems.Add(new DesignerActionPropertyItem("HorizontalGridLinesVisible", "HorizontalGridLinesVisible", DesignerCategory.APPEARANCE, ""));
            actionItems.Add(new DesignerActionPropertyItem("VerticalGridLinesVisible", "VerticalGridLinesVisible", DesignerCategory.APPEARANCE, ""));
            actionItems.Add(new DesignerActionPropertyItem("Columns", "Columns", DesignerCategory.APPEARANCE, ""));
            actionItems.Add(new DesignerActionPropertyItem("CustomQuery", "CustomQuery", DesignerCategory.APPEARANCE, ""));
            actionItems.Add(new DesignerActionPropertyItem("UseCustomQuery", "UseCustomQuery", DesignerCategory.APPEARANCE, ""));

            actionItems.Add(new DesignerActionHeaderItem("Time Axis"));
            actionItems.Add(new DesignerActionPropertyItem("TimeAxisTitle", "TimeAxisTitle", "Time Axis", ""));
            actionItems.Add(new DesignerActionPropertyItem("TimeAxisTitleColor", "TimeAxisTitleColor", "Time Axis", ""));
            actionItems.Add(new DesignerActionPropertyItem("TimeAxisLabelColor", "TimeAxisLabelColor", "Time Axis", ""));
            actionItems.Add(new DesignerActionPropertyItem("TimeAxisColor", "TimeAxisColor", "Time Axis", ""));
            actionItems.Add(new DesignerActionPropertyItem("TimeAxisFieldName", "TimeAxisFieldName", "Time Axis", ""));

            actionItems.Add(new DesignerActionHeaderItem("Left Axis"));
            actionItems.Add(new DesignerActionPropertyItem("LeftAxisTitle", "LeftAxisTitle", "Left Axis", ""));
            actionItems.Add(new DesignerActionPropertyItem("LeftAxisTitleColor", "LeftAxisTitleColor", "Left Axis", ""));
            actionItems.Add(new DesignerActionPropertyItem("LeftAxisLabelColor", "LeftAxisLabelColor", "Left Axis", ""));
            actionItems.Add(new DesignerActionPropertyItem("LeftAxisColor", "LeftAxisColor", "Left Axis", ""));
            actionItems.Add(new DesignerActionPropertyItem("LeftAxisVisible", "LeftAxisVisible", "Left Axis", ""));

            actionItems.Add(new DesignerActionHeaderItem("Legends"));
            actionItems.Add(new DesignerActionPropertyItem("LegendVisible", "LegendVisible", "Legends", ""));
            actionItems.Add(new DesignerActionPropertyItem("LegendPlacement", "LegendPlacement", "Legends", ""));
            actionItems.Add(new DesignerActionPropertyItem("LegendPosition", "LegendPosition", "Legends", ""));

            actionItems.Add(new DesignerActionMethodItem(this, "DatabaseConfig", "Configure databases", DesignerCategory.EASYSCADA, "Click here to configure databases", true));

        }

        #region Properties

        public virtual string Name
        {
            get { return BaseControl.Name; }
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

        public Font Font
        {
            get { return BaseControl.Font; }
            set { BaseControl.SetValue(value); }
        }

        public Color BackColor
        {
            get { return BaseControl.BackColor; }
            set { BaseControl.SetValue(value); }
        }

        public Color BackColorTrend
        {
            get { return BaseControl.BackColorTrend; }
            set { BaseControl.SetValue(value); }
        }

        public DockStyle Dock
        {
            get { return BaseControl.Dock; }
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
        }

        public bool UseCustomQuery
        {
            get => BaseControl.UseCustomQuery;
            set => BaseControl.SetValue(value);
        }

        public string CustomQuery
        {
            get => BaseControl.CustomQuery;
            set => BaseControl.SetValue(value);
        }

        [Category(DesignerCategory.APPEARANCE), Browsable(true), TypeConverter(typeof(CollectionEditor)), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public LineDefinitionCollection Lines
        {
            get => BaseControl.Lines;
        }

        public bool HorizontalGridLinesVisible
        {
            get { return BaseControl.HorizontalGridLinesVisible; }
            set { BaseControl.SetValue(value); }
        }

        public bool VerticalGridLinesVisible
        {
            get { return BaseControl.VerticalGridLinesVisible; }
            set { BaseControl.SetValue(value); }
        }

        public string TimeAxisTitle
        {
            get { return BaseControl.TimeAxisTitle; }
            set { BaseControl.SetValue(value); }
        }

        public Color TimeAxisTitleColor
        {
            get { return BaseControl.TimeAxisTitleColor; }
            set { BaseControl.SetValue(value); }
        }

        public Color TimeAxisLabelColor
        {
            get { return BaseControl.TimeAxisLabelColor; }
            set { BaseControl.SetValue(value); }
        }

        public Color TimeAxisColor
        {
            get { return BaseControl.TimeAxisColor; }
            set { BaseControl.SetValue(value); }
        }

        public string TimeAxisFieldName
        {
            get { return BaseControl.TimeAxisFieldName; }
            set { BaseControl.SetValue(value); }
        }

        public string LeftAxisTitle
        {
            get { return BaseControl.LeftAxisTitle; }
            set { BaseControl.SetValue(value); }
        }

        public Color LeftAxisTitleColor
        {
            get { return BaseControl.LeftAxisTitleColor; }
            set { BaseControl.SetValue(value); }
        }

        public Color LeftAxisLabelColor
        {
            get { return BaseControl.LeftAxisLabelColor; }
            set { BaseControl.SetValue(value); }
        }

        public Color LeftAxisColor
        {
            get { return BaseControl.LeftAxisColor; }
            set { BaseControl.SetValue(value); }
        }

        public bool LeftAxisVisible
        {
            get { return BaseControl.LeftAxisVisible; }
            set { BaseControl.SetValue(value); }
        }

        public bool LegendVisible
        {
            get { return BaseControl.LegendVisible; }
            set { BaseControl.SetValue(value); }
        }

        public LegendPlacement LegendPlacement
        {
            get { return BaseControl.LegendPlacement; }
            set { BaseControl.SetValue(value); }
        }

        public LegendPosition LegendPosition
        {
            get { return BaseControl.LegendPosition; }
            set { BaseControl.SetValue(value); }
        }

        #endregion

        #region Actions methods
        public void DatabaseConfig()
        {
            LogProfileConfigDesignerForm form = new LogProfileConfigDesignerForm(false, false); // Hide add and remove button
            LogProfile profile = BaseControl.Database;
            if (profile == null)
            {
                profile = new LogProfile() { DatabaseType = DbType.MySql, Port = 3306, TableName = "table" };
            }

            form.Databases = new LogProfileCollection() { profile };
            form.Start();
            if (form.ShowDialog() == DialogResult.OK)
            {
                var results = form.ResultDatabases.ToArray();
                if (results != null && results.Length > 0)
                {
                    BaseControl.Database = results[0];
                }
                else
                {
                    BaseControl.Database = null;
                }

                BaseControl.SetValue(Font, "Font");
            }
        }
        #endregion
    }
}
