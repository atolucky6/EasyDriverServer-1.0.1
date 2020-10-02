using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EasyScada.Winforms.Controls.Charts
{
    public class ChartPointCollection : Collection<ChartPoint>
    {
        protected override void ClearItems()
        {
            base.ClearItems();
            Chart?.Invalidate();
        }

        protected override void InsertItem(int index, ChartPoint item)
        {
            base.InsertItem(index, item);
            item.Chart = Chart;
            item.Series = Series;
            Chart?.Invalidate();
        }

        protected override void RemoveItem(int index)
        {
            base.RemoveItem(index);
            Chart?.Invalidate();
        }

        protected override void SetItem(int index, ChartPoint item)
        {
            base.SetItem(index, item);
            item.Chart = Chart;
            item.Series = Series;
            Chart?.Invalidate();
        }

        public EasyChart Chart { get; internal set; }
        public SeriesBase Series { get; internal set; }
    }
}
