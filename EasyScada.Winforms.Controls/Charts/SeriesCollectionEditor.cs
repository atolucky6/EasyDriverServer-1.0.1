using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyScada.Winforms.Controls.Charts
{
    public class SeriesCollectionEditor : CollectionEditor
    {
        public SeriesCollectionEditor(Type type) : base(type)
        {
        }

        protected override bool CanSelectMultipleInstances()
        {
            return true;
        }

        protected override object CreateInstance(Type itemType)
        {
            if (itemType == typeof(BezierSeries))
            {
                return new BezierSeries();
            }
            if (itemType == typeof(ColumnSeries))
            {
                return new ColumnSeries();
            }
            if (itemType == typeof(LabelSeries))
            {
                return new LabelSeries();
            }
            if (itemType == typeof(LineSeries))
            {
                return new LineSeries();
            }
            if (itemType == typeof(PointSeries))
            {
                return new PointSeries();
            }
            if (itemType == typeof(StandardLineSeries))
            {
                return new StandardLineSeries();
            }
            return null;
        }

        protected override Type[] CreateNewItemTypes()
        {
            return new Type[] { typeof(BezierSeries), typeof(ColumnSeries), typeof(LabelSeries), typeof(LineSeries), typeof(PointSeries), typeof(StandardLineSeries) };
        }
    }
}
