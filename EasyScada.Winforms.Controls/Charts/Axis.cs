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
    public class Axis
    {
        #region Fields
        private EasyChart chart;
        private double minValBuf;
        private double maxValBuf;
        private double dataStep;
        private double maxValueLimit = 1.0;
        private double minValueLimit = 0.0;
        private Color color = Color.FromArgb(51, 51, 51);
        private Brush brush;
        private float maxLabelWidth;
        private float maxLabelHeight;
        private float locationStep;
        #endregion

        #region Public properties
        [Browsable(false)]
        public EasyChart Chart
        {
            get => chart;
            internal set => chart = value; 
        }

        [Browsable(false)]
        public AxisType Type { get; internal set; }

        [Category("Easy Chart"), DefaultValue(1.0)]
        public double MaxValueLimt
        {
            get => maxValueLimit;
            set
            {
                maxValueLimit = value;
                MaxValue = value;
            }
        }

        [Category("Easy Chart"), DefaultValue(0.0)]
        public double MinValueLimt
        {
            get => minValueLimit;
            set
            {
                minValueLimit = value;
                MinValue = value;
            }
        }

        [Category("Easy Chart"), TypeConverter(typeof(CollectionConverter)), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public List<AxisLabel> Labels { get; private set; }

        [Category("Easy Chart"), DefaultValue(false)]
        public bool IsCustomLabels { get; set; } = false;

        [Category("Easy Chart"), DefaultValue(true)]
        public bool Visible { get; set; } = true;

        [Category("Easy Chart"), DefaultValue(true)]
        public bool LabelVisible { get; set; } = true;

        [Category("Easy Chart")]
        public Color Color
        {
            get => color;
            set
            {
                color = value;
                brush = new SolidBrush(color);
            }
        }

        [Category("Easy Chart"), DefaultValue(AxisPosition.LeftBottom)]
        public AxisPosition Position { get; set; } = AxisPosition.LeftBottom;

        [Category("Easy Chart"), DefaultValue("")]
        public string Title { get; set; } = "Axis";

        [Category("Easy Chart"), DefaultValue(false)]
        public bool IsReverse { get; set; }

        [Category("Easy Chart"), DefaultValue(true)]
        public bool AutoAdapter { get; set; } = true;

        [Category("Easy Chart"), DefaultValue(true)]
        public bool ZoomEnabled { get; set; } = true;

        [Browsable(false)]
        public float Width { get; private set; }

        [Browsable(false)]
        public float Height { get; private set; }

        [Browsable(false), DefaultValue(1.0)]
        public double MaxValue { get; private set; }

        [Browsable(false), DefaultValue(0.0)]
        public double MinValue { get; private set; }

        [Browsable(false)]
        public double Range
        {
            get
            {
                double range = MaxValue - MinValue;
                if (range <= 0)
                    return 1.0;
                return range;
            }
        }

        [Browsable(false), DefaultValue(ZoomType.None)]
        public ZoomType ZoomState { get; private set; }

        #endregion

        #region Constructors
        public Axis()
        {
            Labels = new List<AxisLabel>();
            brush = new SolidBrush(Color);
            maxLabelHeight = 0.0f;
            maxLabelWidth = 0.0f;
        }
        #endregion

        #region Private methods
        private float CountAxisXHeight(Graphics g, Font font)
        {
            if (!Visible)
                return 0f;
            float height = string.IsNullOrEmpty(Title) ? 0f : (g.MeasureString(Title, font).Height + 3f);
            if (!LabelVisible || Labels.Count < 1)
                return height;
            maxLabelHeight = 0f;
            maxLabelWidth = 0f;
            for (int i = 0; i < Labels.Count; i++)
            {
                SizeF sf = g.MeasureString(Labels[i].Content, font);
                maxLabelHeight = (sf.Height > maxLabelHeight) ? sf.Height : maxLabelHeight;
                maxLabelWidth = (sf.Width > maxLabelWidth) ? sf.Width : maxLabelWidth;
            }
            return height + maxLabelHeight + 3f;
        }

        private float CountAxisYWidth(Graphics g, Font font)
        {
            if (!Visible)
                return 0f;

            float width = 0f;
            if (!string.IsNullOrEmpty(Title))
                width = ChartHelper.ConvertSize(g.MeasureString(Title, font), -90f).Width + 3f;
            
            if (!LabelVisible || Labels.Count < 1)
                return width;

            maxLabelHeight = 0f;
            maxLabelWidth = 0f;
            for (int i = 0; i < Labels.Count; i++)
            {
                SizeF sf = g.MeasureString(Labels[i].Content, font);
                maxLabelHeight = (sf.Height > maxLabelHeight) ? sf.Height : maxLabelHeight;
                maxLabelWidth = (sf.Width > maxLabelWidth) ? sf.Width : maxLabelWidth;
            }
            return width + maxLabelWidth + 3f;
        }

        internal void DrawAxisX(Graphics g, Font font, Rectangle border, int offset, ref float top, ref float bottom)
        {
            if (Visible)
            {
                StringFormat titleFormatTopAxisX = null;
                StringFormat labelFormatTopAxisX = null;
                float x = border.Left + (border.Width / 2f);
                float y = 0f;
                float num3 = IsCustomLabels ? (float)(border.Left + offset) : (border.Left + (float)(offset % locationStep));
                float yPoint = 0f;
                if (Position == AxisPosition.RightTop)
                {
                    titleFormatTopAxisX = AxisFormat.TitleFormatTopAxisX;
                    labelFormatTopAxisX = AxisFormat.LabelFormatTopAxisX;
                    y = top;
                    yPoint = top + Height - 3f;
                    top += Height;
                }
                else
                {
                    titleFormatTopAxisX = AxisFormat.TitleFormatBottomAxisX;
                    labelFormatTopAxisX = AxisFormat.LabelFormatBottomAxisX;
                    y = border.Bottom + bottom + Height;
                    yPoint = border.Bottom + bottom + 3f;
                    bottom += Height;
                }

                if (LabelVisible)
                {
                    int count = Labels.Count;
                    if (count > 0)
                    {
                        RectangleF rectf = new RectangleF();
                        for (int i = 0; i < count; i++)
                        {
                            float xPoint = 0f;
                            if (IsCustomLabels)
                            {
                                double lbVal = IsReverse ? Labels[count - i - 1].Value : Labels[i].Value;
                                xPoint = Convert.ToSingle((Width / Range) * (lbVal - MinValue));
                                if (IsReverse)
                                    xPoint = Width - xPoint;
                            }
                            else
                            {
                                xPoint = i * locationStep;
                            }

                            xPoint += num3;
                            if (xPoint >= border.Left && xPoint <= border.Right)
                            {
                                PointF pt = new PointF(xPoint, yPoint);
                                if (!rectf.Contains(pt))
                                {
                                    string s = IsReverse ? Labels[count - i - 1].Content : Labels[i].Content;
                                    g.DrawString(s, font, brush, pt, labelFormatTopAxisX);
                                    rectf = new RectangleF(xPoint, yPoint, maxLabelWidth + 21f, maxLabelHeight);
                                }
                            }
                        }
                    }
                }
                if (!string.IsNullOrEmpty(Title))
                    g.DrawString(Title, font, brush, x, y, titleFormatTopAxisX);
            }
        }

        internal void DrawAxisY(Graphics g, Font font, Rectangle border, int offset, ref float left, ref float right)
        {
            if (Visible)
            {
                StringFormat titleFormatRightAxisY = null;
                StringFormat labelFormatRightAxisY = null;
                float x = 0f;
                float y = border.Top + (border.Height / 2);
                float xPoint = 0f;
                float num4 = IsCustomLabels ? (float)(border.Top + offset) : (border.Top + (float)(offset % locationStep));
                if (Position == AxisPosition.RightTop)
                {
                    titleFormatRightAxisY = AxisFormat.TitleFormatRightAxisY;
                    labelFormatRightAxisY = AxisFormat.LabelFormatRightAxisY;
                    x = border.Right + right + Width;
                    xPoint = border.Right + right - 3f;
                    right += Width;
                }
                else
                {
                    titleFormatRightAxisY = AxisFormat.TitleFormatLeftAxisY;
                    labelFormatRightAxisY = AxisFormat.LableFormatLeftAxisY;
                    x = left;
                    xPoint = left + Width - 3f;
                    left += Width;
                }

                if (LabelVisible)
                {
                    int count = Labels.Count;
                    if (count > 0)
                    {
                        RectangleF rectf = new RectangleF();
                        for (int i = 0; i < count; i++)
                        {
                            float yPoint = 0f;
                            if (IsCustomLabels)
                            {
                                double lbVal = IsReverse ? Labels[i].Value : Labels[count - i - 1].Value;
                                yPoint = Convert.ToSingle((Height / Range) * (lbVal - MinValue));
                                if (!IsReverse)
                                    xPoint = Height - xPoint;
                            }
                            else
                            {
                                yPoint = i * locationStep;
                            }

                            yPoint += num4;
                            if (yPoint >= border.Top && yPoint <= border.Bottom)
                            {
                                PointF pt = new PointF(xPoint, yPoint);
                                if (!rectf.Contains(pt))
                                {
                                    string s = IsReverse ? Labels[i].Content : Labels[count - i - 1].Content;
                                    g.DrawString(s, font, brush, pt, labelFormatRightAxisY);
                                    rectf = new RectangleF(xPoint, yPoint, maxLabelWidth, maxLabelHeight + 6f);
                                }
                            }
                        }
                    }
                }
                if (!string.IsNullOrEmpty(Title))
                    ChartHelper.DrawString(g, Title, font, brush, new PointF(x, y), titleFormatRightAxisY, -90f);
            }
        }

        public string GetLabel(double value)
        {
            if (IsCustomLabels)
            {
                IEnumerable<AxisLabel> source = from lab in Labels where lab.Value == value select lab;

                if (source != null && source.Count() > 0)
                {
                    return source.Last().Content;
                }
                return string.Empty;
            }
            return value.ToString();
        }

        internal void ResetRange()
        {
            MinValue = minValBuf = MinValueLimt;
            MaxValue = maxValBuf = MaxValueLimt;
        }

        public void SetLabels(string[] labels, double[] values)
        {
            if (labels != null && values != null)
            {
                IsCustomLabels = true;
                Labels.Clear();
                for (int i = 0; i < labels.Length; i++)
                {
                    AxisLabel axisLabel = new AxisLabel(values[i])
                    {
                        Content = labels[i]
                    };
                    Labels.Add(axisLabel);
                }
            }
            chart?.Invalidate();
        }

        public void SetLabels(DateTime[] dateTimes, double[] values, string format = "yyyy-MM-dd HH:mm:ss")
        {
            if (dateTimes != null && values != null)
            {
                IsCustomLabels = true;
                Labels.Clear();
                for (int i = 0; i < dateTimes.Length; i++)
                {
                    AxisLabel axisLabel = new AxisLabel(values[i])
                    {
                        Content = dateTimes[i].ToString(format)
                    };
                    Labels.Add(axisLabel);
                }
            }
            chart?.Invalidate();
        }

        public void SetLabels(string[] labels, double startValue = 0.0, double valueStep = 0.1)
        {
            if (labels != null)
            {
                IsCustomLabels = true;
                Labels.Clear();
                for (int i = 0; i < labels.Length; i++)
                {
                    AxisLabel axisLabel = new AxisLabel(startValue + (i * valueStep))
                    {
                        Content = labels[i]
                    };
                    Labels.Add(axisLabel);
                }
            }
            chart?.Invalidate();
        }

        internal void UpdateLabel(int separatorSum)
        {
            if (!IsCustomLabels)
            {
                Labels.Clear();
                dataStep = Range / (double)separatorSum;
                for (int i = 0; i <= separatorSum; i++)
                {
                    Labels.Add(new AxisLabel(dataStep * i + MinValue));
                }
            }
        }

        internal void UpdateOffset(int separatorSum, int offset)
        {
            if (!IsCustomLabels)
            {
                int num = (int)((float)offset / locationStep);
                double num2 = num * dataStep;
                if (num2 != 0.0)
                {
                    Labels.Clear();
                    for (int i = 0; i <= separatorSum; i++)
                    {
                        double num4 = dataStep * i + MinValue;
                        num4 = IsReverse ? num4 + num2 : num4 - num2;
                        Labels.Add(new AxisLabel(num4));
                    }
                }
            }
        }

        internal void UpdateRange(SeriesBase series, AxisType axisType, int separatorSum)
        {
            if (AutoAdapter && !IsCustomLabels)
            {
                double? nullable = (axisType == AxisType.X) ? series.MinValueX : series.MinValueY;
                double? nullable2 = (axisType == AxisType.X) ? series.MaxValueX : series.MaxValueY;
                bool flag2 = false;
                if (nullable.HasValue && (minValBuf > nullable.Value))
                {
                    minValBuf = nullable.Value;
                    flag2 = true;
                }
                if (nullable2.HasValue && (maxValBuf < nullable2.Value))
                {
                    maxValBuf = nullable2.Value;
                    flag2 = true;
                }
                if (flag2)
                {
                    UpdateRange(minValBuf, maxValBuf, separatorSum);
                }
            }
        }

        private void UpdateRange(double min, double max, int separatorSum)
        {
            double num6;
            double num = max - min;
            double d = num / ((double)separatorSum);
            double num3 = Math.Pow(10.0, Math.Floor((double)(Math.Log(d) / Math.Log(10.0))));
            double num4 = num / ((double)separatorSum);
            double num5 = num4 / num3;
            if (num5 > 5.0)
            {
                num6 = 10.0 * num3;
            }
            else if (num5 > 2.0)
            {
                num6 = 5.0 * num3;
            }
            else if (num5 > 1.0)
            {
                num6 = 2.0 * num3;
            }
            else
            {
                num6 = num3;
            }
            this.MinValue = Math.Floor((double)(min / num6)) * num6;
            this.MaxValue = Math.Ceiling((double)(max / num6)) * num6;
        }

        internal void UpdateSize1(Graphics g, Font font)
        {
            if (Type == AxisType.X)
            {
                Height = CountAxisXHeight(g, font);
            }
            else if (Type == AxisType.Y)
            {
                Width = CountAxisYWidth(g, font);
            }
        }

        internal void UpdateSize2(Rectangle border, int separatorSum)
        {
            if (Type == AxisType.X)
            {
                Width = border.Width;
                locationStep = Width / (float)separatorSum;
            }
            else if (Type == AxisType.Y)
            {
                Height = border.Height;
                locationStep = Height / (float)separatorSum;
            }
        }

        internal void UpdateZoom(double times)
        {
            if (ZoomEnabled)
            {
                if (times > 0.0)
                {
                    ZoomState = ZoomType.Enlarge;
                }
                else if (times < 0.0)
                {
                    ZoomState = ZoomType.Reduce;
                }
                else
                {
                    ZoomState = ZoomType.None;
                }

                if (!(times == 0.0))
                {
                    double range = Range;
                    MinValue += range * times;
                    MaxValue -= range * times;
                }
            }
        }

        #endregion
    }
}
