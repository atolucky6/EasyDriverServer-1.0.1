using EasyScada.Core;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using OxyPlot;
using OxyPlot.Series;

namespace EasyScada.Winforms.Controls
{
    public class LineDefinition
    {
        [Browsable(true), Category(DesignerCategory.EASYSCADA)]
        public bool ShowInLegend { get; set; } = true;

        [Browsable(true), Category(DesignerCategory.EASYSCADA)]
        public double StrokeThickness { get; set; } = 1;

        [Browsable(true), Category(DesignerCategory.EASYSCADA)]
        public double FontSize { get; set; } = double.NaN;

        [Browsable(true), Category(DesignerCategory.EASYSCADA)]
        public string LabelFormatString { get; set; }

        [Browsable(true), Category(DesignerCategory.EASYSCADA)]
        public string Title { get; set; } = "Title";

        [Editor(typeof(PathToTagPropertyEditor), typeof(UITypeEditor))]
        [Description("Select path to tag for control")]
        [Browsable(true), Category(DesignerCategory.EASYSCADA)] 
        public string TagPath { get; set; }

        [Browsable(true), Category(DesignerCategory.EASYSCADA)]
        public string ColumnName { get; set; }

        [Browsable(true), Category(DesignerCategory.EASYSCADA)]
        public MarkerType MarkerType { get; set; } = MarkerType.None;

        [Browsable(true), Category(DesignerCategory.EASYSCADA)]
        public LineStyle LineStyle { get; set; } = LineStyle.Solid;

        [Browsable(true), Category(DesignerCategory.EASYSCADA)]
        public LineStyle EmptyLineStyle { get; set; } = LineStyle.None;

        [Browsable(true), Category(DesignerCategory.EASYSCADA)]
        public Color LineColor { get; set; } = Color.Green;

        [Browsable(true), Category(DesignerCategory.EASYSCADA)]
        public Color EmptyLineColor { get; set; } = Color.Red;

        [Browsable(true), Category(DesignerCategory.EASYSCADA)]
        public Color MarkerFill { get; set; } = Color.Green;

        [Browsable(true), Category(DesignerCategory.EASYSCADA)]
        public Color MarkerStroke { get; set; } = Color.Green;

        ITag linkedTag;
        [Browsable(false)]
        [JsonIgnore]
        public ITag LinkedTag
        {
            get
            {
                if (linkedTag == null)
                {
                    linkedTag = Connector?.GetTag(TagPath);
                }
                else
                {
                    if (linkedTag.Path != TagPath)
                        linkedTag = Connector?.GetTag(TagPath);
                }
                return linkedTag;
            }
        }

        [Description("Select driver connector for control")]
        [Browsable(false), Category(DesignerCategory.EASYSCADA)]
        public IEasyDriverConnector Connector => EasyDriverConnectorProvider.GetEasyDriverConnector();

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        internal List<DateTimePoint> Points { get; set; }

        public LineDefinition()
        {
            Points = new List<DateTimePoint>();
        }

        public override string ToString()
        {
            return TagPath;
        }
    }
}
