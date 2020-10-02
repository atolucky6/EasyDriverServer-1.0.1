using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EasyScada.Winforms.Controls.Charts
{
    [Editor(typeof(SeriesCollectionEditor), typeof(UITypeEditor))]
    public class SeriesCollection : Collection<SeriesBase>
    {
        protected override void ClearItems()
        {
            if (Chart != null)
                Chart.LegendPanel.Controls.Clear();
            base.ClearItems();
            if (Chart != null)
                Chart.Invalidate();
        }

        protected override void InsertItem(int index, SeriesBase item)
        {
            base.InsertItem(index, item);
            item.Chart = Chart;
            item.Points.Chart = Chart;
            item.Points.Series = item;
            if (Chart != null)
            {
                Chart.LegendPanel.Controls.Add(item.Legend);
                Chart.Invalidate();
            }
        }

        protected override void SetItem(int index, SeriesBase item)
        {
            if (Chart != null)
                Chart.LegendPanel.Controls.Remove(base[index].Legend);
            base.SetItem(index, item);
            item.Chart = Chart;
            item.Points.Chart = Chart;
            item.Points.Series = item;
            if (Chart != null)
            {
                Chart.LegendPanel.Controls.Add(item.Legend);
                Chart.Invalidate();
            }
        }

        public EasyChart Chart { get; internal set; }
    }
}
