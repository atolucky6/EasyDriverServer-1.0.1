using EasyScada.Core;
using EasyScada.Winforms.Controls.Charts;
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
    class HistoricalTrendDesigner : ControlDesignerBase<HistoricalTrend>
    {
        protected override ControlDesignerAcionList<HistoricalTrend> GetActionList()
        {
            return new HistoricalTrendDesignerActionList(Component);
        }
    }

    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    class HistoricalTrendDesignerActionList : ControlDesignerAcionList<HistoricalTrend>
    {
        public HistoricalTrendDesignerActionList(IComponent component) : base(component)
        {
        }

        #region Properties

        public string Name
        {
            get => BaseControl.Name;
            set => BaseControl.SetValue(value);
        }

        public Color ForeColor
        {
            get => BaseControl.ForeColor;
            set => BaseControl.SetValue(value);
        }

        public Color BackColor
        {
            get => BaseControl.BackColor;
            set => BaseControl.SetValue(value);
        }

        public Color AxisColor
        {
            get => BaseControl.AxisColor;
            set => BaseControl.SetValue(value);
        }

        [TypeConverter(typeof(CollectionEditor)), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public List<TrendLine> Lines
        {
            get => BaseControl.Lines;
        }

        [Editor(typeof(LogProfilePropertyEditor), typeof(UITypeEditor))]
        public LogProfile Database
        {
            get => BaseControl.Database;
        }

        public Color GridLineColor
        {
            get => BaseControl.GridLineColor;
            set => BaseControl.SetValue(value);
        }

        public int GridRowCount
        {
            get => BaseControl.GridRowCount;
            set => BaseControl.SetValue(value);
        }

        public int GridColumnWidth
        {
            get => BaseControl.GridColumnWidth;
            set => BaseControl.SetValue(value);
        }

        public float LeftAxisMaxValue
        {
            get => BaseControl.LeftAxisMaxValue;
            set => BaseControl.SetValue(value);
        }

        public float LeftAxisMinValue
        {
            get => BaseControl.LeftAxisMinValue;
            set => BaseControl.SetValue(value);
        }

        public string LeftAxisUnit
        {
            get => BaseControl.LeftAxisUnit;
            set => BaseControl.SetValue(value);
        }

        public float RightAxisMaxValue
        {
            get => BaseControl.RightAxisMaxValue;
            set => BaseControl.SetValue(value);
        }

        public float RightAxisMinValue
        {
            get => BaseControl.RightAxisMinValue;
            set => BaseControl.SetValue(value);
        }

        public string RightAxisUnit
        {
            get => BaseControl.RightAxisUnit;
            set => BaseControl.SetValue(value);
        }

        public bool RightAxisVisible
        {
            get => BaseControl.RightAxisVisible;
            set => BaseControl.SetValue(value);
        }

        public string AxisTimeFormnat
        {
            get => BaseControl.AxisTimeFormat;
            set => BaseControl.SetValue(value);
        }

        public bool RoundAxisValue
        {
            get => BaseControl.RoundAxisValue;
            set => BaseControl.SetValue(value);
        }

        public bool TooltipVisible
        {
            get => BaseControl.TooltipVisible;
            set => BaseControl.SetValue(value);
        }

        public bool TooltipTimeVisible
        {
            get => BaseControl.TooltipTimeVisible;
            set => BaseControl.SetValue(value);
        }

        public string TooltipTimeFormat
        {
            get => BaseControl.TooltipTimeFormat;
            set => BaseControl.SetValue(value);
        }

        public Color TooltipLineColor
        {
            get => BaseControl.TooltipLineColor;
            set => BaseControl.SetValue(value);
        }

        #endregion

        protected override void AddExtendActionItems()
        {
            //actionItems.Add(new DesignerActionPropertyItem("Name", "Name", DesignerCategory.DESIGN, ""));
            actionItems.Add(new DesignerActionPropertyItem("ForeColor", "ForeColor", DesignerCategory.DESIGN, ""));
            actionItems.Add(new DesignerActionPropertyItem("BackColor", "BackColor", DesignerCategory.DESIGN, ""));
            actionItems.Add(new DesignerActionHeaderItem("Historical setting"));
            actionItems.Add(new DesignerActionPropertyItem("Lines", "Lines", "Historical setting", ""));
            actionItems.Add(new DesignerActionPropertyItem("Database", "Database", "Historical setting", ""));
            actionItems.Add(new DesignerActionHeaderItem("Grid"));
            actionItems.Add(new DesignerActionPropertyItem("GridLineColor", "GridLineColor", "Grid", ""));
            actionItems.Add(new DesignerActionPropertyItem("GridRowCount", "GridRowCount", "Grid", ""));
            actionItems.Add(new DesignerActionPropertyItem("GridColumnWidth", "GridColumnWidth", "Grid", ""));

            actionItems.Add(new DesignerActionHeaderItem("Axis"));
            actionItems.Add(new DesignerActionPropertyItem("AxisColor", "AxisColor", "Axis", ""));
            actionItems.Add(new DesignerActionPropertyItem("AxisTimeFormnat", "AxisTimeFormnat", "Axis", ""));
            actionItems.Add(new DesignerActionPropertyItem("RoundAxisValue", "RoundAxisValue", "Axis", ""));

            //actionItems.Add(new DesignerActionHeaderItem("Left axis"));
            //actionItems.Add(new DesignerActionPropertyItem("LeftAxisMaxValue", "LeftAxisMaxValue", "Left axis", ""));
            //actionItems.Add(new DesignerActionPropertyItem("LeftAxisMinValue", "LeftAxisMinValue", "Left axis", ""));
            actionItems.Add(new DesignerActionPropertyItem("LeftAxisUnit", "LeftAxisUnit", "Left axis", ""));

            //actionItems.Add(new DesignerActionHeaderItem("Right axis"));
            //actionItems.Add(new DesignerActionPropertyItem("RightAxisMaxValue", "RightAxisMaxValue", "Right axis", ""));
            //actionItems.Add(new DesignerActionPropertyItem("RightAxisMinValue", "RightAxisMinValue", "Right axis", ""));
            actionItems.Add(new DesignerActionPropertyItem("RightAxisUnit", "RightAxisUnit", "Right axis", ""));
            actionItems.Add(new DesignerActionPropertyItem("RightAxisVisible", "RightAxisVisible", "Right axis", ""));

            actionItems.Add(new DesignerActionHeaderItem("Tooltip"));
            actionItems.Add(new DesignerActionPropertyItem("TooltipVisible", "TooltipVisible", "Tooltip", ""));
            //actionItems.Add(new DesignerActionPropertyItem("TooltipTimeVisible", "TooltipTimeVisible", "Tooltip", ""));
            actionItems.Add(new DesignerActionPropertyItem("TooltipTimeFormat", "TooltipTimeFormat", "Tooltip", ""));
            actionItems.Add(new DesignerActionPropertyItem("TooltipLineColor", "TooltipLineColor", "Tooltip", ""));

        }
    }
}
