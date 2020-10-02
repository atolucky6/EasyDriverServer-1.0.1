using System.Collections.ObjectModel;
using System.Windows.Forms;

namespace EasyScada.Winforms.Controls.Charts
{
    public class AxisCollection : Collection<Axis>
    {
        protected override void ClearItems()
        {
            base.ClearItems();
            Chart?.Invalidate();
        }

        protected override void InsertItem(int index, Axis item)
        {
            base.InsertItem(index, item);
            item.Chart = Chart;
            item.Type = Type;
            Chart?.Invalidate();
        }

        protected override void RemoveItem(int index)
        {
            base.RemoveItem(index);
            Chart?.Invalidate();
        }

        protected override void SetItem(int index, Axis item)
        {
            base.SetItem(index, item);
            item.Chart = Chart;
            item.Type = Type;
            Chart?.Invalidate();
        }

        public EasyChart Chart { get; internal set; }
        public AxisType Type { get; internal set; }
    }
}
