using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EasyScada.Winforms.Controls.Charts
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class ChartPoint
    {
        #region Fields
        private EasyChart chart;
        private double y;
        private double x;
        private string label;
        #endregion

        #region Events
        public event EventHandler LabelChanged;
        public event EventHandler ValueChanged;
        #endregion

        #region Public properties
        [Browsable(false)]
        public EasyChart Chart
        {
            get => chart;
            internal set => chart = value;
        }

        [Browsable(false)]
        public SeriesBase Series { get; internal set; }

        [Browsable(false)]
        public Axis ParentAxisX
        {
            get => Series?.ParentAxisX;
        }

        [Browsable(false)]
        public Axis ParentAxisY
        {
            get => Series?.ParentAxisY;
        }

        public double X
        {
            get => x;
            set
            {
                if (x != value)
                {
                    x = value;
                    Chart?.Invalidate();
                    OnValueChanged(new EventArgs());
                }
            }
        }

        public double Y
        {
            get => y;
            set
            {
                if (y != value)
                {
                    y = value;
                    Chart?.Invalidate();
                    OnValueChanged(new EventArgs());
                }
            }
        }

        public string Label
        {
            get => label;
            set
            {
                if (label != value)
                {
                    label = value;
                    Chart?.Invalidate();
                    OnLabelChanged(new EventArgs());
                }
            }
        }

        [Browsable(false)]
        public int Left
        {
            get
            {
                int left = chart == null ? 0 : chart.OffsetX;
                return ((int)Series.CountLeft(X)) + left;
            }
        }

        [Browsable(false)]
        public int Top
        {
            get
            {
                int top = chart == null ? 0 : chart.OffsetY;
                return ((int)Series.CountTop(Y)) + top;
            }
        }

        [Browsable(false)]
        public Point Location
        {
            get => new Point(Left, Top);
        }

        #endregion

        #region Constructors

        public ChartPoint() : this(0.0, 0.0) { }

        public ChartPoint(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        #endregion

        #region Methods

        public string GetLabelFromAxisX()
        {
            if (ParentAxisX != null)
                return ParentAxisX.GetLabel(Y);
            return null;
        }

        public string GetLabelFromAxisY()
        {
            if (ParentAxisY != null)
                return ParentAxisY.GetLabel(Y);
            return null;
        }

        protected void OnLabelChanged(EventArgs e)
        {
            LabelChanged?.Invoke(this, e);
        }

        protected void OnValueChanged(EventArgs e)
        {
            ValueChanged?.Invoke(this, e);
        }

        #endregion
    }
}
