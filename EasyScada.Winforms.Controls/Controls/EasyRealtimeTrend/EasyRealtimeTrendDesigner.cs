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
    class EasyRealtimeTrendDesigner : ControlDesignerBase<EasyRealtimeTrend>
    {
        protected override ControlDesignerAcionList<EasyRealtimeTrend> GetActionList()
        {
            return new EasyRealtimeTrendDesignerActionList(Component);
        }
    }

    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    class EasyRealtimeTrendDesignerActionList : ControlDesignerAcionList<EasyRealtimeTrend>
    {
        public EasyRealtimeTrendDesignerActionList(IComponent component) : base(component)
        {
        }

        protected override void AddExtendActionItems()
        {
            actionItems.Add(new DesignerActionHeaderItem(DesignerCategory.DESIGN));
            actionItems.Add(new DesignerActionPropertyItem("Name", "Name", DesignerCategory.DESIGN, ""));

            actionItems.Add(new DesignerActionHeaderItem(DesignerCategory.APPEARANCE));
            actionItems.Add(new DesignerActionPropertyItem("Title", "Title", DesignerCategory.APPEARANCE, ""));
            actionItems.Add(new DesignerActionPropertyItem("Font", "Font", DesignerCategory.APPEARANCE, ""));
            actionItems.Add(new DesignerActionPropertyItem("BackColor", "BackColor", DesignerCategory.APPEARANCE, ""));
            actionItems.Add(new DesignerActionPropertyItem("Dock", "Dock", DesignerCategory.APPEARANCE, ""));
            actionItems.Add(new DesignerActionPropertyItem("Lines", "Lines", DesignerCategory.APPEARANCE, ""));
            actionItems.Add(new DesignerActionPropertyItem("TimeRange", "TimeRange", DesignerCategory.APPEARANCE, ""));
            actionItems.Add(new DesignerActionPropertyItem("UpdateInterval", "UpdateInterval", DesignerCategory.APPEARANCE, ""));
            actionItems.Add(new DesignerActionPropertyItem("HorizontalGridLinesVisible", "HorizontalGridLinesVisible", DesignerCategory.APPEARANCE, ""));
            actionItems.Add(new DesignerActionPropertyItem("VerticalGridLinesVisible", "VerticalGridLinesVisible", DesignerCategory.APPEARANCE, ""));

            actionItems.Add(new DesignerActionHeaderItem("Time Axis"));
            actionItems.Add(new DesignerActionPropertyItem("TimeAxisTitle", "TimeAxisTitle", "Time Axis", ""));
            actionItems.Add(new DesignerActionPropertyItem("TimeAxisTitleColor", "TimeAxisTitleColor", "Time Axis", ""));
            actionItems.Add(new DesignerActionPropertyItem("TimeAxisLabelColor", "TimeAxisLabelColor", "Time Axis", ""));
            actionItems.Add(new DesignerActionPropertyItem("TimeAxisColor", "TimeAxisColor", "Time Axis", ""));

            actionItems.Add(new DesignerActionHeaderItem("Left Axis"));
            actionItems.Add(new DesignerActionPropertyItem("LeftAxisTitle", "LeftAxisTitle", "Left Axis", ""));
            actionItems.Add(new DesignerActionPropertyItem("LeftAxisTitleColor", "LeftAxisTitleColor", "Left Axis", ""));
            actionItems.Add(new DesignerActionPropertyItem("LeftAxisLabelColor", "LeftAxisLabelColor", "Left Axis", ""));
            actionItems.Add(new DesignerActionPropertyItem("LeftAxisColor", "LeftAxisColor", "Left Axis", ""));
            actionItems.Add(new DesignerActionPropertyItem("LeftAxisVisible", "LeftAxisVisible", "Left Axis", ""));

            //actionItems.Add(new DesignerActionHeaderItem("Right Axis"));
            //actionItems.Add(new DesignerActionPropertyItem("RightAxisTitle", "RightAxisTitle", "Right Axis", ""));
            //actionItems.Add(new DesignerActionPropertyItem("RightAxisTitleColor", "RightAxisTitleColor", "Right Axis", ""));
            //actionItems.Add(new DesignerActionPropertyItem("RightAxisLabelColor", "RightAxisLabelColor", "Right Axis", ""));
            //actionItems.Add(new DesignerActionPropertyItem("RightAxisColor", "RightAxisColor", "Right Axis", ""));
            //actionItems.Add(new DesignerActionPropertyItem("RightAxisVisible", "RightAxisVisible", "Right Axis", ""));

            actionItems.Add(new DesignerActionHeaderItem("Legends"));
            actionItems.Add(new DesignerActionPropertyItem("LegendVisible", "LegendVisible", "Legends", ""));
            actionItems.Add(new DesignerActionPropertyItem("LegendPlacement", "LegendPlacement", "Legends", ""));
            actionItems.Add(new DesignerActionPropertyItem("LegendPosition", "LegendPosition", "Legends", ""));
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

        public DockStyle Dock
        {
            get { return BaseControl.Dock; }
            set { BaseControl.SetValue(value); }
        }

        [Category(DesignerCategory.APPEARANCE), Browsable(true), TypeConverter(typeof(CollectionEditor)), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public LineDefinitionCollection Lines
        {
            get => BaseControl.Lines;
        }

        public TimeSpan TimeRange
        {
            get { return BaseControl.TimeRange; }
            set { BaseControl.SetValue(value); }
        }

        public int UpdateInterval
        {
            get { return BaseControl.UpdateInterval; }
            set { BaseControl.SetValue(value); }
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

        //public string RightAxisTitle
        //{
        //    get { return BaseControl.RightAxisTitle; }
        //    set { BaseControl.SetValue(value); }
        //}

        //public Color RightAxisTitleColor
        //{
        //    get { return BaseControl.RightAxisTitleColor; }
        //    set { BaseControl.SetValue(value); }
        //}

        //public Color RightAxisLabelColor
        //{
        //    get { return BaseControl.RightAxisLabelColor; }
        //    set { BaseControl.SetValue(value); }
        //}

        //public Color RightAxisColor
        //{
        //    get { return BaseControl.RightAxisColor; }
        //    set { BaseControl.SetValue(value); }
        //}

        //public bool RightAxisVisible
        //{
        //    get { return BaseControl.RightAxisVisible; }
        //    set { BaseControl.SetValue(value); }
        //}

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

        #endregion
    }
}
