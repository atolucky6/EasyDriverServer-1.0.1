using EasyScada.Core;
using EasyScada.Winforms.Controls.Charts;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EasyScada.Winforms.Controls
{
    public class RealtimeTrend : UserControl
    {
        #region Extends
        #endregion

        #region Core
        #region Fields
        private const int value_count_max = 4096;
        private float value_max_left = 100f;
        private float value_min_left = 0f;
        private float value_max_right = 100f;
        private float value_min_right = 0f;
        private int gridRowCount = 5;
        private bool autoSizeGridColumn = false;
        private int gridColumnMaxWidth = 300;
        private bool gridVisible = true;
        private string textFormat = "HH:mm";
        private int gridColumnWidth = 100;
        private Random random = null;
        private string title = "";
        private int leftRight = 50;
        private int upDowm = 25;
        private bool smoothLine;
        private Dictionary<string, CurveItem> data_list = null;
        private string[] data_text = null;
        private List<AuxiliaryLine> auxiliary_lines;
        private List<AuxiliaryLabel> auxiliary_Labels;
        private List<MarkText> hslMarkTexts;
        private Font font_size9 = null;
        private Font font_size12 = null;
        private Brush brush_deep = null;
        private Pen pen_normal = null;
        private Pen pen_dash = null;
        private Color color_normal = Color.DeepPink;
        private Color color_deep = Color.DimGray;
        private Color color_dash = Color.Gray;
        private Color color_mark_font = Color.DodgerBlue;
        private Brush brush_mark_font = Brushes.DodgerBlue;
        private StringFormat format_left = null;
        private StringFormat format_right = null;
        private StringFormat format_center = null;
        private bool rightAxisVisible = true;
        private int curveNameWidth = 100;
        private IContainer components = null;

        #endregion

        #region Constructors
        public RealtimeTrend()
        {
            this.InitializeComponent();
            this.random = new Random();
            this.data_list = new Dictionary<string, CurveItem>();
            this.auxiliary_lines = new List<AuxiliaryLine>();
            this.hslMarkTexts = new List<MarkText>();
            this.auxiliary_Labels = new List<AuxiliaryLabel>();
            StringFormat format1 = new StringFormat();
            format1.LineAlignment = StringAlignment.Center;
            format1.Alignment = StringAlignment.Near;
            this.format_left = format1;
            StringFormat format2 = new StringFormat();
            format2.LineAlignment = StringAlignment.Center;
            format2.Alignment = StringAlignment.Far;
            this.format_right = format2;
            StringFormat format3 = new StringFormat();
            format3.LineAlignment = StringAlignment.Center;
            format3.Alignment = StringAlignment.Center;
            this.format_center = format3;
            this.font_size9 = new Font(FontFamily.GenericSansSerif, 9f);
            this.font_size12 = new Font(FontFamily.GenericSansSerif, 12f);
            this.InitializationColor();
            this.pen_dash = new Pen(this.color_deep);
            this.pen_dash.DashStyle = DashStyle.Custom;
            this.pen_dash.DashPattern = new float[] { 5f, 5f };
            base.SetStyle(ControlStyles.SupportsTransparentBackColor | ControlStyles.UserPaint, true);
            base.SetStyle(ControlStyles.ResizeRedraw, true);
            base.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            base.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
        }
        #endregion

        #region Methods
        private void AddAuxiliary(float value, Color lineColor, float lineThickness, bool isDashLine, bool isLeft)
        {
            AuxiliaryLine item = new AuxiliaryLine
            {
                Value = value,
                LineColor = lineColor
            };
            Pen pen = new Pen(lineColor)
            {
                DashStyle = DashStyle.Custom
            };
            pen.DashPattern = new float[] { 5f, 5f };
            item.PenDash = pen;
            item.PenSolid = new Pen(lineColor);
            item.IsDashStyle = isDashLine;
            item.IsLeftFrame = isLeft;
            item.LineThickness = lineThickness;
            item.LineTextBrush = new SolidBrush(lineColor);
            this.auxiliary_lines.Add(item);
            base.Invalidate();
        }

        public void AddAuxiliaryLabel(AuxiliaryLabel auxiliaryLable)
        {
            this.auxiliary_Labels.Add(auxiliaryLable);
        }

        public void AddCurveData(string key, float value)
        {
            float[] values = new float[] { value };
            this.AddCurveData(key, values);
        }

        public void AddCurveData(string key, float[] values)
        {
            this.AddCurveData(key, values, (string[])null);
        }

        public void AddCurveData(string[] keys, float[] values)
        {
            this.AddCurveData(keys, values, null);
        }

        public void AddCurveData(string key, float value, string markText)
        {
            float[] values = new float[] { value };
            string[] markTexts = new string[] { markText };
            this.AddCurveData(key, values, markTexts);
        }

        public void AddCurveData(string key, float[] values, string[] markTexts)
        {
            if (markTexts == null)
            {
                markTexts = new string[values.Length];
            }
            this.AddCurveData(key, values, markTexts, false);
            if ((values != null) ? (values.Length != 0) : false)
            {
                this.AddCurveTime(values.Length);
            }
            base.Invalidate();
        }

        public void AddCurveData(string[] keys, float[] values, string[] markTexts)
        {
            if (keys == null)
            {
                throw new ArgumentNullException("keys");
            }
            if (values == null)
            {
                throw new ArgumentNullException("values");
            }
            if (markTexts == null)
            {
                markTexts = new string[keys.Length];
            }
            if (keys.Length != values.Length)
            {
                throw new Exception("两个参数的数组长度不一致。");
            }
            if (keys.Length != markTexts.Length)
            {
                throw new Exception("两个参数的数组长度不一致。");
            }
            int index = 0;
            while (true)
            {
                if (index >= keys.Length)
                {
                    this.AddCurveTime(1);
                    base.Invalidate();
                    return;
                }
                float[] singleArray1 = new float[] { values[index] };
                string[] textArray1 = new string[] { markTexts[index] };
                this.AddCurveData(keys[index], singleArray1, textArray1, false);
                index++;
            }
        }

        public void AddCurveData(string axisText, string[] keys, float[] values)
        {
            this.AddCurveData(axisText, keys, values, null);
        }

        private void AddCurveData(string key, float[] values, string[] markTexts, bool isUpdateUI)
        {
            if (!((values != null) ? (values.Length < 1) : false) && this.data_list.ContainsKey(key))
            {
                CurveItem item = this.data_list[key];
                if (item.Data != null)
                {
                    if (this.autoSizeGridColumn)
                    {
                        ChartHelper.AddArrayData<float>(ref item.Data, values, this.gridColumnMaxWidth);
                        ChartHelper.AddArrayData<string>(ref item.Text, markTexts, this.gridColumnMaxWidth);
                    }
                    else
                    {
                        ChartHelper.AddArrayData<float>(ref item.Data, values, 4096);
                        ChartHelper.AddArrayData<string>(ref item.Text, markTexts, 4096);
                    }
                    if (isUpdateUI)
                    {
                        base.Invalidate();
                    }
                }
            }
        }

        public void AddCurveData(string axisText, string[] keys, float[] values, string[] markTexts)
        {
            if (keys == null)
            {
                throw new ArgumentNullException("keys");
            }
            if (values == null)
            {
                throw new ArgumentNullException("values");
            }
            if (markTexts == null)
            {
                markTexts = new string[keys.Length];
            }
            if (keys.Length != values.Length)
            {
                throw new Exception("两个参数的数组长度不一致。");
            }
            if (keys.Length != markTexts.Length)
            {
                throw new Exception("两个参数的数组长度不一致。");
            }
            int index = 0;
            while (true)
            {
                if (index >= keys.Length)
                {
                    this.AddCurveTime(1, axisText);
                    base.Invalidate();
                    return;
                }
                float[] singleArray1 = new float[] { values[index] };
                string[] textArray1 = new string[] { markTexts[index] };
                this.AddCurveData(keys[index], singleArray1, textArray1, false);
                index++;
            }
        }

        private void AddCurveTime(int count)
        {
            this.AddCurveTime(count, DateTime.Now.ToString(this.textFormat));
        }

        private void AddCurveTime(int count, string text)
        {
            if (this.data_text != null)
            {
                string[] data = new string[count];
                int index = 0;
                while (true)
                {
                    if (index >= data.Length)
                    {
                        if (this.autoSizeGridColumn)
                        {
                            ChartHelper.AddArrayData<string>(ref this.data_text, data, this.gridColumnMaxWidth);
                        }
                        else
                        {
                            ChartHelper.AddArrayData<string>(ref this.data_text, data, 4096);
                        }
                        break;
                    }
                    data[index] = text;
                    index++;
                }
            }
        }

        public void AddLeftAuxiliary(float value)
        {
            this.AddLeftAuxiliary(value, AxisColor);
        }

        public void AddLeftAuxiliary(float value, Color lineColor)
        {
            this.AddLeftAuxiliary(value, lineColor, 1f, true);
        }

        public void AddLeftAuxiliary(float value, Color lineColor, float lineThickness, bool isDashLine)
        {
            this.AddAuxiliary(value, lineColor, lineThickness, isDashLine, true);
        }

        public void AddMarkText(MarkText markText)
        {
            this.hslMarkTexts.Add(markText);
            if (this.data_list.Count > 0)
            {
                base.Invalidate();
            }
        }

        public void AddRightAuxiliary(float value)
        {
            this.AddRightAuxiliary(value, AxisColor);
        }

        public void AddRightAuxiliary(float value, Color lineColor)
        {
            this.AddRightAuxiliary(value, lineColor, 1f, true);
        }

        public void AddRightAuxiliary(float value, Color lineColor, float lineThickness, bool isDashLine)
        {
            this.AddAuxiliary(value, lineColor, lineThickness, isDashLine, false);
        }

        private int CalculateDataCountByOffect(float offect)
        {
            int num;
            if (this.gridColumnWidth > 0)
            {
                num = this.gridColumnWidth;
            }
            else if (offect > 40f)
            {
                num = 1;
            }
            else
            {
                offect = 40f / offect;
                num = (int)Math.Ceiling((double)offect);
            }
            return num;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void DrawMarkPoint(Graphics g, string markText, PointF center, Brush brush, MarkTextPositionStyle markTextPosition)
        {
            if (!string.IsNullOrEmpty(markText))
            {
                g.FillEllipse(brush, new RectangleF(center.X - 3f, center.Y - 3f, 6f, 6f));
                switch (markTextPosition)
                {
                    case MarkTextPositionStyle.Up:
                        g.DrawString(markText, this.Font, brush, new RectangleF(center.X - 100f, (center.Y - this.Font.Height) - MarkText.MarkTextOffset, 200f, (float)(this.Font.Height + 2)), this.format_center);
                        break;

                    case MarkTextPositionStyle.Right:
                        g.DrawString(markText, this.Font, brush, new RectangleF(center.X + MarkText.MarkTextOffset, center.Y - this.Font.Height, 100f, (float)(this.Font.Height * 2)), this.format_left);
                        break;

                    case MarkTextPositionStyle.Down:
                        g.DrawString(markText, this.Font, brush, new RectangleF(center.X - 100f, center.Y + MarkText.MarkTextOffset, 200f, (float)(this.Font.Height + 2)), this.format_center);
                        break;

                    case MarkTextPositionStyle.Left:
                        g.DrawString(markText, this.Font, brush, new RectangleF(center.X - 100f, center.Y - this.Font.Height, (float)(100 - MarkText.MarkTextOffset), (float)(this.Font.Height * 2)), this.format_right);
                        break;

                    default:
                        break;
                }
            }
        }

        private void DrawMarkPoint(Graphics g, string markText, PointF center, Color color, MarkTextPositionStyle markTextPosition)
        {
            if (!string.IsNullOrEmpty(markText))
            {
                using (Brush brush = new SolidBrush(color))
                {
                    this.DrawMarkPoint(g, markText, center, brush, markTextPosition);
                }
            }
        }

        public CurveItem GetCurveItem(string key)
        {
            CurveItem item;
            if (this.data_list.ContainsKey(key))
            {
                item = this.data_list[key];
            }
            else
            {
                item = null;
            }
            return item;
        }

        private void InitializationColor()
        {
            if (this.pen_normal == null)
            {
                Pen local1 = this.pen_normal;
            }
            else
            {
                this.pen_normal.Dispose();
            }
            if (this.brush_deep == null)
            {
                Brush local2 = this.brush_deep;
            }
            else
            {
                this.brush_deep.Dispose();
            }
            this.pen_normal = new Pen(this.color_deep);
            this.brush_deep = new SolidBrush(this.color_deep);
        }

        private void InitializeComponent()
        {
            base.SuspendLayout();
            base.AutoScaleMode = AutoScaleMode.None;
            this.BackColor = Color.Transparent;
            base.Name = "HslCurve";
            base.Size = new Size(417, 205);
            base.ResumeLayout(false);
        }

        private bool IsNeedPaintDash(float paintValue)
        {
            int num = 0;
            while (true)
            {
                bool flag2;
                if (num >= this.auxiliary_lines.Count)
                {
                    flag2 = true;
                }
                else
                {
                    if (Math.Abs((float)(this.auxiliary_lines[num].PaintValue - paintValue)) >= this.font_size9.Height)
                    {
                        num++;
                        continue;
                    }
                    flag2 = false;
                }
                return flag2;
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            foreach (KeyValuePair<string, CurveItem> pair in this.data_list)
            {
                RectangleF titleRegion = pair.Value.TitleRegion;
                if (titleRegion.Contains((PointF)e.Location))
                {
                    pair.Value.LineRenderVisible = !pair.Value.LineRenderVisible;
                    base.Invalidate();
                    break;
                }
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            bool flag = false;
            foreach (KeyValuePair<string, CurveItem> pair in this.data_list)
            {
                RectangleF titleRegion = pair.Value.TitleRegion;
                if (titleRegion.Contains((PointF)e.Location))
                {
                    flag = true;
                    break;
                }
            }
            this.Cursor = flag ? Cursors.Hand : Cursors.Arrow;
        }

        protected override unsafe void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            if (this.BackColor != Color.Transparent)
            {
                g.Clear(this.BackColor);
            }
            int width = base.Width;
            int height = base.Height;
            if ((width >= 120) && (height >= 60))
            {
                Point[] pointArray = new Point[] { new Point(this.leftRight - 1, this.upDowm - 8), new Point(this.leftRight - 1, height - this.upDowm), new Point(width - this.leftRight, height - this.upDowm), new Point(width - this.leftRight, this.upDowm - 8) };
                g.DrawLine(this.pen_normal, pointArray[0], pointArray[1]);
                g.DrawLine(this.pen_normal, pointArray[1], pointArray[2]);
                if (this.rightAxisVisible)
                {
                    g.DrawLine(this.pen_normal, pointArray[2], pointArray[3]);
                }
                if (!string.IsNullOrEmpty(this.title))
                {
                    g.DrawString(this.title, this.font_size9, this.brush_deep, new Rectangle(0, 0, width - 1, 20), this.format_center);
                }
                else if (this.data_list.Count > 0)
                {
                    float num3 = this.leftRight + 10;
                    foreach (KeyValuePair<string, CurveItem> pair in this.data_list)
                    {
                        if (pair.Value.Visible)
                        {
                            Pen pen = pair.Value.LineRenderVisible ? new Pen(pair.Value.LineColor) : new Pen(Color.FromArgb(80, pair.Value.LineColor));
                            g.DrawLine(pen, num3, 11f, num3 + 30f, 11f);
                            g.DrawEllipse(pen, (float)(num3 + 8f), (float)4f, (float)14f, (float)14f);
                            pen.Dispose();
                            SolidBrush brush = pair.Value.LineRenderVisible ? new SolidBrush(pair.Value.LineColor) : new SolidBrush(Color.FromArgb(80, pair.Value.LineColor));
                            g.DrawString(pair.Key, this.Font, brush, new RectangleF(num3 + 35f, 2f, 120f, 18f), this.format_left);
                            pair.Value.TitleRegion = new RectangleF(num3, 2f, 60f, 18f);
                            brush.Dispose();
                            num3 += this.curveNameWidth;
                        }
                    }
                }
                int num4 = 0;
                while (true)
                {
                    if (num4 < this.auxiliary_Labels.Count)
                    {
                        if (!string.IsNullOrEmpty(this.auxiliary_Labels[num4].Text))
                        {
                            int x = (this.auxiliary_Labels[num4].LocationX > 1f) ? ((int)this.auxiliary_Labels[num4].LocationX) : ((int)(this.auxiliary_Labels[num4].LocationX * width));
                            int num6 = ((int)g.MeasureString(this.auxiliary_Labels[num4].Text, this.Font).Width) + 3;
                            Point[] points = new Point[] { new Point(x, 11), new Point(x + 10, 20), new Point((x + num6) + 10, 20), new Point((x + num6) + 10, 0), new Point(x + 10, 0), new Point(x, 11) };
                            g.FillPolygon(this.auxiliary_Labels[num4].TextBack, points);
                            g.DrawString(this.auxiliary_Labels[num4].Text, this.Font, this.auxiliary_Labels[num4].TextBrush, new Rectangle(x + 7, 0, num6 + 3, 20), this.format_center);
                        }
                        num4++;
                        continue;
                    }
                    ChartHelper.PaintTriangle(g, this.brush_deep, new Point(this.leftRight - 1, this.upDowm - 8), 4, GraphDirection.Upward);
                    if (this.rightAxisVisible)
                    {
                        ChartHelper.PaintTriangle(g, this.brush_deep, new Point(width - this.leftRight, this.upDowm - 8), 4, GraphDirection.Upward);
                    }
                    int num7 = 0;
                    while (true)
                    {
                        if (num7 < this.auxiliary_lines.Count)
                        {
                            if (this.auxiliary_lines[num7].IsLeftFrame)
                            {
                                this.auxiliary_lines[num7].PaintValue = ChartHelper.ComputePaintLocationY(this.value_max_left, this.value_min_left, (float)((height - this.upDowm) - this.upDowm), this.auxiliary_lines[num7].Value) + this.upDowm;
                            }
                            else
                            {
                                this.auxiliary_lines[num7].PaintValue = ChartHelper.ComputePaintLocationY(this.value_max_right, this.value_min_right, (float)((height - this.upDowm) - this.upDowm), this.auxiliary_lines[num7].Value) + this.upDowm;
                            }
                            num7++;
                            continue;
                        }
                        int num8 = 0;
                        while (true)
                        {
                            if (num8 <= this.gridRowCount)
                            {
                                float num9 = ((float)((num8 * (this.value_max_left - this.value_min_left)) / ((double)this.gridRowCount))) + this.value_min_left;
                                float paintValue = ChartHelper.ComputePaintLocationY(this.value_max_left, this.value_min_left, (float)((height - this.upDowm) - this.upDowm), num9) + this.upDowm;
                                if (this.IsNeedPaintDash(paintValue))
                                {
                                    g.DrawLine(this.pen_normal, (float)(this.leftRight - 4), paintValue, (float)(this.leftRight - 1), paintValue);
                                    RectangleF layoutRectangle = new RectangleF(0f, paintValue - 9f, (float)(this.leftRight - 4), 20f);
                                    g.DrawString(num9.ToString(), this.font_size9, this.brush_deep, layoutRectangle, this.format_right);
                                    if (this.rightAxisVisible)
                                    {
                                        float num11 = ((num8 * (this.value_max_right - this.value_min_right)) / ((float)this.gridRowCount)) + this.value_min_right;
                                        g.DrawLine(this.pen_normal, (float)((width - this.leftRight) + 1), paintValue, (float)((width - this.leftRight) + 4), paintValue);
                                        layoutRectangle.Location = new PointF((float)((width - this.leftRight) + 4), paintValue - 9f);
                                        g.DrawString(num11.ToString(), this.font_size9, this.brush_deep, layoutRectangle, this.format_left);
                                    }
                                    if ((num8 > 0) && this.gridVisible)
                                    {
                                        g.DrawLine(this.pen_dash, (float)this.leftRight, paintValue, (float)(width - this.leftRight), paintValue);
                                    }
                                }
                                num8++;
                                continue;
                            }
                            if (this.gridVisible)
                            {
                                if (this.autoSizeGridColumn)
                                {
                                    float offect = ((width - (this.leftRight * 2)) * 1f) / ((float)(this.gridColumnMaxWidth - 1));
                                    int num13 = this.CalculateDataCountByOffect(offect);
                                    int index = 0;
                                    while (true)
                                    {
                                        if (index >= this.gridColumnMaxWidth)
                                        {
                                            bool flag73;
                                            if (this.data_text != null)
                                            {
                                                flag73 = this.data_text.Length > 1;
                                            }
                                            else
                                            {
                                                string[] local1 = this.data_text;
                                                flag73 = false;
                                            }
                                            if (flag73)
                                            {
                                                if (this.data_text.Length < this.gridColumnMaxWidth)
                                                {
                                                    g.DrawLine(this.pen_dash, ((this.data_text.Length - 1) * offect) + this.leftRight, (float)this.upDowm, ((this.data_text.Length - 1) * offect) + this.leftRight, (float)((height - this.upDowm) - 1));
                                                }
                                                Rectangle layoutRectangle = new Rectangle((((int)((this.data_text.Length - 1) * offect)) + this.leftRight) - this.leftRight, (height - this.upDowm) + 1, this.leftRight * 2, this.upDowm);
                                                g.DrawString(this.data_text[this.data_text.Length - 1], this.font_size9, this.brush_deep, layoutRectangle, this.format_center);
                                            }
                                            break;
                                        }
                                        if ((index > 0) && (index < (this.gridColumnMaxWidth - 1)))
                                        {
                                            g.DrawLine(this.pen_dash, (index * offect) + this.leftRight, (float)this.upDowm, (index * offect) + this.leftRight, (float)((height - this.upDowm) - 1));
                                        }
                                        if ((this.data_text != null) && ((index < this.data_text.Length) && (((index * offect) + this.leftRight) < ((((this.data_text.Length - 1) * offect) + this.leftRight) - 40f))))
                                        {
                                            Rectangle layoutRectangle = new Rectangle((int)(index * offect), (height - this.upDowm) + 1, this.leftRight * 2, this.upDowm);
                                            g.DrawString(this.data_text[index], this.font_size9, this.brush_deep, layoutRectangle, this.format_center);
                                        }
                                        index += num13;
                                    }
                                }
                                else
                                {
                                    int num15 = (width - (2 * this.leftRight)) + 1;
                                    int leftRight = this.leftRight;
                                    while (true)
                                    {
                                        if (leftRight >= (width - this.leftRight))
                                        {
                                            bool flag74;
                                            if (this.data_text != null)
                                            {
                                                flag74 = this.data_text.Length > 1;
                                            }
                                            else
                                            {
                                                string[] local6 = this.data_text;
                                                flag74 = false;
                                            }
                                            if (flag74)
                                            {
                                                if (this.data_text.Length >= num15)
                                                {
                                                    Rectangle layoutRectangle = new Rectangle((width - this.leftRight) - this.leftRight, (height - this.upDowm) + 1, this.leftRight * 2, this.upDowm);
                                                    g.DrawString(this.data_text[this.data_text.Length - 1], this.font_size9, this.brush_deep, layoutRectangle, this.format_center);
                                                }
                                                else
                                                {
                                                    g.DrawLine(this.pen_dash, (this.data_text.Length + this.leftRight) - 1, this.upDowm, (this.data_text.Length + this.leftRight) - 1, (height - this.upDowm) - 1);
                                                    Rectangle layoutRectangle = new Rectangle(((this.data_text.Length + this.leftRight) - 1) - this.leftRight, (height - this.upDowm) + 1, this.leftRight * 2, this.upDowm);
                                                    g.DrawString(this.data_text[this.data_text.Length - 1], this.font_size9, this.brush_deep, layoutRectangle, this.format_center);
                                                }
                                            }
                                            break;
                                        }
                                        if (leftRight != this.leftRight)
                                        {
                                            g.DrawLine(this.pen_dash, leftRight, this.upDowm, leftRight, (height - this.upDowm) - 1);
                                        }
                                        if (this.data_text != null)
                                        {
                                            int num17 = (num15 > this.data_text.Length) ? this.data_text.Length : num15;
                                            if (((leftRight - this.leftRight) < this.data_text.Length) && ((num17 - (leftRight - this.leftRight)) > 40))
                                            {
                                                if (this.data_text.Length <= num15)
                                                {
                                                    Rectangle layoutRectangle = new Rectangle(leftRight - this.leftRight, (height - this.upDowm) + 1, this.leftRight * 2, this.upDowm);
                                                    g.DrawString(this.data_text[leftRight - this.leftRight], this.font_size9, this.brush_deep, layoutRectangle, this.format_center);
                                                }
                                                else
                                                {
                                                    Rectangle layoutRectangle = new Rectangle(leftRight - this.leftRight, (height - this.upDowm) + 1, this.leftRight * 2, this.upDowm);
                                                    g.DrawString(this.data_text[((leftRight - this.leftRight) + this.data_text.Length) - num15], this.font_size9, this.brush_deep, layoutRectangle, this.format_center);
                                                }
                                            }
                                        }
                                        leftRight += this.gridColumnWidth;
                                    }
                                }
                            }
                            int num18 = 0;
                            while (true)
                            {
                                if (num18 < this.auxiliary_lines.Count)
                                {
                                    if (this.auxiliary_lines[num18].IsLeftFrame)
                                    {
                                        g.DrawLine(this.auxiliary_lines[num18].GetPen(), (float)(this.leftRight - 4), this.auxiliary_lines[num18].PaintValue, (float)(this.leftRight - 1), this.auxiliary_lines[num18].PaintValue);
                                        RectangleF layoutRectangle = new RectangleF(0f, this.auxiliary_lines[num18].PaintValue - 9f, (float)(this.leftRight - 4), 20f);
                                        g.DrawString(this.auxiliary_lines[num18].Value.ToString(), this.font_size9, this.auxiliary_lines[num18].LineTextBrush, layoutRectangle, this.format_right);
                                    }
                                    else
                                    {
                                        g.DrawLine(this.auxiliary_lines[num18].GetPen(), (float)((width - this.leftRight) + 1), this.auxiliary_lines[num18].PaintValue, (float)((width - this.leftRight) + 4), this.auxiliary_lines[num18].PaintValue);
                                        RectangleF layoutRectangle = new RectangleF((float)((width - this.leftRight) + 4), this.auxiliary_lines[num18].PaintValue - 9f, (float)(this.leftRight - 4), 20f);
                                        g.DrawString(this.auxiliary_lines[num18].Value.ToString(), this.font_size9, this.auxiliary_lines[num18].LineTextBrush, layoutRectangle, this.format_left);
                                    }
                                    g.DrawLine(this.auxiliary_lines[num18].GetPen(), (float)this.leftRight, this.auxiliary_lines[num18].PaintValue, (float)(width - this.leftRight), this.auxiliary_lines[num18].PaintValue);
                                    num18++;
                                    continue;
                                }
                                if (!this.autoSizeGridColumn)
                                {
                                    foreach (MarkText text2 in this.hslMarkTexts)
                                    {
                                        foreach (KeyValuePair<string, CurveItem> pair3 in this.data_list)
                                        {
                                            if (!pair3.Value.Visible)
                                            {
                                                continue;
                                            }
                                            if (pair3.Value.LineRenderVisible && (pair3.Key == text2.CurveKey))
                                            {
                                                bool flag71;
                                                if (pair3.Value.Data != null)
                                                {
                                                    flag71 = pair3.Value.Data.Length > 1;
                                                }
                                                else
                                                {
                                                    float[] data = pair3.Value.Data;
                                                    flag71 = false;
                                                }
                                                if (flag71 && ((text2.Index >= 0) && (text2.Index < pair3.Value.Data.Length)))
                                                {
                                                    PointF tf3;
                                                    var t = new PointF((float)(this.leftRight + text2.Index), ChartHelper.ComputePaintLocationY(pair3.Value.IsLeftFrame ? this.value_max_left : this.value_max_right, pair3.Value.IsLeftFrame ? this.value_min_left : this.value_min_right, (float)((height - this.upDowm) - this.upDowm), pair3.Value.Data[text2.Index]) + this.upDowm);
                                                    PointF* tfPtr5 = &t;
                                                    tfPtr5 = &tf3;
                                                    g.FillEllipse(text2.CircleBrush, new RectangleF(tf3.X - 3f, tf3.Y - 3f, 6f, 6f));
                                                    switch (((text2.PositionStyle == MarkTextPositionStyle.Auto) ? MarkText.CalculateDirectionFromDataIndex(pair3.Value.Data, text2.Index) : text2.PositionStyle))
                                                    {
                                                        case MarkTextPositionStyle.Up:
                                                            g.DrawString(text2.Text, this.Font, text2.TextBrush, new RectangleF(tf3.X - 100f, (tf3.Y - this.Font.Height) - MarkText.MarkTextOffset, 200f, (float)(this.Font.Height + 2)), this.format_center);
                                                            break;

                                                        case MarkTextPositionStyle.Right:
                                                            g.DrawString(text2.Text, this.Font, text2.TextBrush, new RectangleF(tf3.X + MarkText.MarkTextOffset, tf3.Y - this.Font.Height, 100f, (float)(this.Font.Height * 2)), this.format_left);
                                                            break;

                                                        case MarkTextPositionStyle.Down:
                                                            g.DrawString(text2.Text, this.Font, text2.TextBrush, new RectangleF(tf3.X - 100f, tf3.Y + MarkText.MarkTextOffset, 200f, (float)(this.Font.Height + 2)), this.format_center);
                                                            break;

                                                        case MarkTextPositionStyle.Left:
                                                            g.DrawString(text2.Text, this.Font, text2.TextBrush, new RectangleF(tf3.X - 100f, tf3.Y - this.Font.Height, (float)(100 - MarkText.MarkTextOffset), (float)(this.Font.Height * 2)), this.format_right);
                                                            break;

                                                        default:
                                                            break;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    break;
                                }
                                else
                                {
                                    foreach (MarkText text in this.hslMarkTexts)
                                    {
                                        foreach (KeyValuePair<string, CurveItem> pair2 in this.data_list)
                                        {
                                            if (!pair2.Value.Visible)
                                            {
                                                continue;
                                            }
                                            if (pair2.Value.LineRenderVisible && (pair2.Key == text.CurveKey))
                                            {
                                                bool flag1;
                                                if (pair2.Value.Data != null)
                                                {
                                                    flag1 = pair2.Value.Data.Length > 1;
                                                }
                                                else
                                                {
                                                    float[] data = pair2.Value.Data;
                                                    flag1 = false;
                                                }
                                                if (flag1)
                                                {
                                                    float num20 = ((width - (this.leftRight * 2)) * 1f) / ((float)(this.gridColumnMaxWidth - 1));
                                                    if ((text.Index >= 0) && (text.Index < pair2.Value.Data.Length))
                                                    {
                                                        PointF tf;
                                                        var t = new PointF(this.leftRight + (text.Index * num20), ChartHelper.ComputePaintLocationY(pair2.Value.IsLeftFrame ? this.value_max_left : this.value_max_right, pair2.Value.IsLeftFrame ? this.value_min_left : this.value_min_right, (float)((height - this.upDowm) - this.upDowm), pair2.Value.Data[text.Index]) + this.upDowm);
                                                        PointF* tfPtr2 = &t;
                                                        tfPtr2 = &tf;
                                                        g.FillEllipse(text.CircleBrush, new RectangleF(tf.X - 3f, tf.Y - 3f, 6f, 6f));
                                                        switch (((text.PositionStyle == MarkTextPositionStyle.Auto) ? MarkText.CalculateDirectionFromDataIndex(pair2.Value.Data, text.Index) : text.PositionStyle))
                                                        {
                                                            case MarkTextPositionStyle.Up:
                                                                g.DrawString(text.Text, this.Font, text.TextBrush, new RectangleF(tf.X - 100f, (tf.Y - this.Font.Height) - MarkText.MarkTextOffset, 200f, (float)(this.Font.Height + 2)), this.format_center);
                                                                break;

                                                            case MarkTextPositionStyle.Right:
                                                                g.DrawString(text.Text, this.Font, text.TextBrush, new RectangleF(tf.X + MarkText.MarkTextOffset, tf.Y - this.Font.Height, 100f, (float)(this.Font.Height * 2)), this.format_left);
                                                                break;

                                                            case MarkTextPositionStyle.Down:
                                                                g.DrawString(text.Text, this.Font, text.TextBrush, new RectangleF(tf.X - 100f, tf.Y + MarkText.MarkTextOffset, 200f, (float)(this.Font.Height + 2)), this.format_center);
                                                                break;

                                                            case MarkTextPositionStyle.Left:
                                                                g.DrawString(text.Text, this.Font, text.TextBrush, new RectangleF(tf.X - 100f, tf.Y - this.Font.Height, (float)(100 - MarkText.MarkTextOffset), (float)(this.Font.Height * 2)), this.format_right);
                                                                break;

                                                            default:
                                                                break;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    using (Dictionary<string, CurveItem>.ValueCollection.Enumerator enumerator4 = this.data_list.Values.GetEnumerator())
                                    {
                                        CurveItem current;
                                        float num21;
                                        List<PointF> list;
                                        int num22;
                                        goto TR_002F;
                                    TR_000B:
                                        num22++;
                                        goto TR_0025;
                                    TR_0018:
                                        list.Clear();
                                        goto TR_000B;
                                    TR_0025:
                                        while (true)
                                        {
                                            if (num22 >= current.Data.Length)
                                            {
                                                if (list.Count > 1)
                                                {
                                                    using (Pen pen3 = new Pen(current.LineColor, current.LineThickness))
                                                    {
                                                        if (current.IsSmoothCurve)
                                                        {
                                                            g.DrawCurve(pen3, list.ToArray());
                                                        }
                                                        else
                                                        {
                                                            g.DrawLines(pen3, list.ToArray());
                                                        }
                                                    }
                                                }
                                                break;
                                            }
                                            if (float.IsNaN(current.Data[num22]))
                                            {
                                                if (list.Count > 1)
                                                {
                                                    using (Pen pen2 = new Pen(current.LineColor, current.LineThickness))
                                                    {
                                                        if (current.IsSmoothCurve)
                                                        {
                                                            g.DrawCurve(pen2, list.ToArray());
                                                        }
                                                        else
                                                        {
                                                            g.DrawLines(pen2, list.ToArray());
                                                        }
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                PointF* tfPtr1;
                                                PointF tf2 = new PointF
                                                {
                                                    X = this.leftRight + (num22 * num21)
                                                };
                                                tf2.Y = ChartHelper.ComputePaintLocationY(current.IsLeftFrame ? this.value_max_left : this.value_max_right, current.IsLeftFrame ? this.value_min_left : this.value_min_right, (float)((height - this.upDowm) - this.upDowm), current.Data[num22]) + this.upDowm;
                                                tfPtr1 = &tf2;
                                                list.Add(tf2);
                                                if (!string.IsNullOrEmpty(current.Text[num22]))
                                                {
                                                    using (Brush brush2 = new SolidBrush(current.LineColor))
                                                    {
                                                        g.FillEllipse(brush2, new RectangleF(tf2.X - 3f, tf2.Y - 3f, 6f, 6f));
                                                        switch (MarkText.CalculateDirectionFromDataIndex(current.Data, num22))
                                                        {
                                                            case MarkTextPositionStyle.Up:
                                                                g.DrawString(current.Text[num22], this.Font, brush2, new RectangleF(tf2.X - 100f, (tf2.Y - this.Font.Height) - MarkText.MarkTextOffset, 200f, (float)(this.Font.Height + 2)), this.format_center);
                                                                break;

                                                            case MarkTextPositionStyle.Right:
                                                                g.DrawString(current.Text[num22], this.Font, brush2, new RectangleF(tf2.X + MarkText.MarkTextOffset, tf2.Y - this.Font.Height, 100f, (float)(this.Font.Height * 2)), this.format_left);
                                                                break;

                                                            case MarkTextPositionStyle.Down:
                                                                g.DrawString(current.Text[num22], this.Font, brush2, new RectangleF(tf2.X - 100f, tf2.Y + MarkText.MarkTextOffset, 200f, (float)(this.Font.Height + 2)), this.format_center);
                                                                break;

                                                            case MarkTextPositionStyle.Left:
                                                                g.DrawString(current.Text[num22], this.Font, brush2, new RectangleF(tf2.X - 100f, tf2.Y - this.Font.Height, (float)(100 - MarkText.MarkTextOffset), (float)(this.Font.Height * 2)), this.format_right);
                                                                break;

                                                            default:
                                                                break;
                                                        }
                                                    }
                                                }
                                                goto TR_000B;
                                            }
                                            goto TR_0018;
                                        }
                                    TR_002F:
                                        while (true)
                                        {
                                            if (!enumerator4.MoveNext())
                                            {
                                                break;
                                            }
                                            current = enumerator4.Current;
                                            if (current.Visible && current.LineRenderVisible)
                                            {
                                                bool flag70;
                                                if (current.Data != null)
                                                {
                                                    flag70 = current.Data.Length > 1;
                                                }
                                                else
                                                {
                                                    float[] data = current.Data;
                                                    flag70 = false;
                                                }
                                                if (flag70)
                                                {
                                                    num21 = ((width - (this.leftRight * 2)) * 1f) / ((float)(this.gridColumnMaxWidth - 1));
                                                    list = new List<PointF>(current.Data.Length);
                                                    num22 = 0;
                                                    goto TR_0025;
                                                }
                                            }
                                        }
                                    }
                                    goto TR_0001;
                                }
                            }
                            break;
                        }
                        break;
                    }
                    break;
                }
                using (Dictionary<string, CurveItem>.ValueCollection.Enumerator enumerator7 = this.data_list.Values.GetEnumerator())
                {
                    CurveItem current;
                    int num23;
                    List<PointF> list2;
                    int num24;
                    int num25;
                    goto TR_0081;
                TR_0054:
                    if (list2.Count > 1)
                    {
                        using (Pen pen6 = new Pen(current.LineColor, current.LineThickness))
                        {
                            if (smoothLine)
                                g.DrawCurve(pen6, list2.ToArray());
                            else
                                g.DrawLines(pen6, list2.ToArray());
                            //if (current.IsSmoothCurve)
                            //{
                            //    g.DrawCurve(pen6, list2.ToArray());
                            //}
                            //else
                            //{
                            //    g.DrawLines(pen6, list2.ToArray());
                            //}
                        }
                    }
                    goto TR_0081;
                TR_0055:
                    num24++;
                    goto TR_0064;
                TR_0057:
                    list2.Clear();
                    goto TR_0055;
                TR_0064:
                    while (true)
                    {
                        if (num24 < current.Data.Length)
                        {
                            if (float.IsNaN(current.Data[num24]))
                            {
                                if (list2.Count > 1)
                                {
                                    using (Pen pen4 = new Pen(current.LineColor, current.LineThickness))
                                    {
                                        if (smoothLine)
                                            g.DrawCurve(pen4, list2.ToArray());
                                        else
                                            g.DrawLines(pen4, list2.ToArray());

                                        //if (current.IsSmoothCurve)
                                        //{
                                        //    g.DrawCurve(pen4, list2.ToArray());
                                        //}
                                        //else
                                        //{
                                        //    g.DrawLines(pen4, list2.ToArray());
                                        //}
                                    }
                                }
                            }
                            else
                            {
                                PointF item = new PointF
                                {
                                    X = this.leftRight + num24
                                };
                                item.Y = ChartHelper.ComputePaintLocationY(current.IsLeftFrame ? this.value_max_left : this.value_max_right, current.IsLeftFrame ? this.value_min_left : this.value_min_right, (float)((height - this.upDowm) - this.upDowm), current.Data[num24]) + this.upDowm;
                                list2.Add(item);
                                this.DrawMarkPoint(g, current.Text[num24], item, current.LineColor, MarkText.CalculateDirectionFromDataIndex(current.Data, num24));
                                goto TR_0055;
                            }
                        }
                        else
                        {
                            goto TR_0054;
                        }
                        break;
                    }
                    goto TR_0057;
                TR_0066:
                    num25++;
                    goto TR_0075;
                TR_0068:
                    list2.Clear();
                    goto TR_0066;
                TR_0075:
                    while (true)
                    {
                        if (num25 < num23)
                        {
                            int index = (num25 + current.Data.Length) - num23;
                            if (float.IsNaN(current.Data[index]))
                            {
                                if (list2.Count > 1)
                                {
                                    using (Pen pen5 = new Pen(current.LineColor, current.LineThickness))
                                    {
                                        if (smoothLine)
                                            g.DrawCurve(pen5, list2.ToArray());
                                        else
                                            g.DrawLines(pen5, list2.ToArray());
                                        //if (current.IsSmoothCurve)
                                        //{
                                        //    g.DrawCurve(pen5, list2.ToArray());
                                        //}
                                        //else
                                        //{
                                        //    g.DrawLines(pen5, list2.ToArray());
                                        //}
                                    }
                                }
                            }
                            else
                            {
                                PointF item = new PointF
                                {
                                    X = this.leftRight + num25
                                };
                                item.Y = ChartHelper.ComputePaintLocationY(current.IsLeftFrame ? this.value_max_left : this.value_max_right, current.IsLeftFrame ? this.value_min_left : this.value_min_right, (float)((height - this.upDowm) - this.upDowm), current.Data[index]) + this.upDowm;
                                list2.Add(item);
                                this.DrawMarkPoint(g, current.Text[index], item, current.LineColor, MarkText.CalculateDirectionFromDataIndex(current.Data, index));
                                goto TR_0066;
                            }
                        }
                        else
                        {
                            goto TR_0054;
                        }
                        break;
                    }
                    goto TR_0068;
                TR_0081:
                    while (true)
                    {
                        if (!enumerator7.MoveNext())
                        {
                            break;
                        }
                        current = enumerator7.Current;
                        if (current.Visible && current.LineRenderVisible)
                        {
                            bool flag72;
                            if (current.Data != null)
                            {
                                flag72 = current.Data.Length > 1;
                            }
                            else
                            {
                                float[] data = current.Data;
                                flag72 = false;
                            }
                            if (flag72)
                            {
                                num23 = (width - (2 * this.leftRight)) + 1;
                                list2 = new List<PointF>(current.Data.Length);
                                if (current.Data.Length > num23)
                                {
                                    num25 = 0;
                                }
                                else
                                {
                                    num24 = 0;
                                    goto TR_0064;
                                }
                                goto TR_0075;
                            }
                        }
                    }
                }
                goto TR_0001;
            }
            return;
        TR_0001:
            base.OnPaint(e);
        }

        public void RemoveAllAuxiliary()
        {
            int count = this.auxiliary_lines.Count;
            this.auxiliary_lines.Clear();
            if (count > 0)
            {
                base.Invalidate();
            }
        }

        public void RemoveAllAuxiliaryLabel()
        {
            int count = this.auxiliary_Labels.Count;
            this.auxiliary_Labels.Clear();
            if (count > 0)
            {
                base.Invalidate();
            }
        }

        public void RemoveAllCurve()
        {
            int count = this.data_list.Count;
            this.data_list.Clear();
            if (this.data_list.Count == 0)
            {
                this.data_text = new string[0];
            }
            if (count > 0)
            {
                base.Invalidate();
            }
        }

        public void RemoveAllCurveData()
        {
            int count = this.data_list.Count;
            foreach (KeyValuePair<string, CurveItem> pair in this.data_list)
            {
                pair.Value.Data = new float[0];
                pair.Value.Text = new string[0];
            }
            this.data_text = new string[0];
            if (count > 0)
            {
                base.Invalidate();
            }
        }

        public void RemoveAllMarkText()
        {
            this.hslMarkTexts.Clear();
            if (this.data_list.Count > 0)
            {
                base.Invalidate();
            }
        }

        public void RemoveAuxiliary(float value)
        {
            int num = 0;
            int index = this.auxiliary_lines.Count - 1;
            while (true)
            {
                if (index < 0)
                {
                    if (num > 0)
                    {
                        base.Invalidate();
                    }
                    return;
                }
                if (this.auxiliary_lines[index].Value == value)
                {
                    this.auxiliary_lines[index].Dispose();
                    this.auxiliary_lines.RemoveAt(index);
                    num++;
                }
                index--;
            }
        }

        public void RemoveAuxiliaryLabel(AuxiliaryLabel auxiliaryLable)
        {
            if (this.auxiliary_Labels.Remove(auxiliaryLable))
            {
                base.Invalidate();
            }
        }

        public void RemoveCurve(string key)
        {
            if (this.data_list.ContainsKey(key))
            {
                this.data_list.Remove(key);
            }
            if (this.data_list.Count == 0)
            {
                this.data_text = new string[0];
            }
            base.Invalidate();
        }

        public void RemoveMarkText(MarkText markText)
        {
            this.hslMarkTexts.Remove(markText);
            if (this.data_list.Count > 0)
            {
                base.Invalidate();
            }
        }

        public Bitmap SaveToBitmap()
        {
            return this.SaveToBitmap(base.Width, base.Height);
        }

        public Bitmap SaveToBitmap(int width, int height)
        {
            Bitmap image = new Bitmap(width, height);
            Graphics graphics = Graphics.FromImage(image);
            this.OnPaint(new PaintEventArgs(graphics, new Rectangle(0, 0, width, height)));
            return image;
        }

        public void SetCurve(string key, bool isLeft, float[] data, Color lineColor, float thickness, bool isSmooth)
        {
            if (this.data_list.ContainsKey(key))
            {
                if (data == null)
                {
                    data = new float[0];
                }
                this.data_list[key].Data = data;
            }
            else
            {
                if (data == null)
                {
                    data = new float[0];
                }
                CurveItem item = new CurveItem
                {
                    Data = data,
                    Text = new string[data.Length],
                    LineThickness = thickness,
                    LineColor = lineColor,
                    IsLeftFrame = isLeft,
                    IsSmoothCurve = isSmooth
                };
                this.data_list.Add(key, item);
                if (this.data_text == null)
                {
                    this.data_text = new string[data.Length];
                }
            }
            base.Invalidate();
        }

        public void SetCurveText(string[] descriptions)
        {
            this.data_text = descriptions;
            base.Invalidate();
        }

        public void SetCurveVisible(string key, bool visible)
        {
            if (this.data_list.ContainsKey(key))
            {
                this.data_list[key].Visible = visible;
                base.Invalidate();
            }
        }

        public void SetCurveVisible(string[] keys, bool visible)
        {
            foreach (string str in keys)
            {
                if (this.data_list.ContainsKey(str))
                {
                    this.data_list[str].Visible = visible;
                }
            }
            base.Invalidate();
        }

        public void SetLeftCurve(string key, float[] data)
        {
            this.SetLeftCurve(key, data, Color.FromArgb(this.random.Next(256), this.random.Next(256), this.random.Next(256)));
        }

        public void SetLeftCurve(string key, float[] data, Color lineColor)
        {
            this.SetCurve(key, true, data, lineColor, 1f, false);
        }

        public void SetLeftCurve(string key, float[] data, Color lineColor, bool isSmooth)
        {
            this.SetCurve(key, true, data, lineColor, 1f, isSmooth);
        }

        public void SetRightCurve(string key, float[] data)
        {
            this.SetRightCurve(key, data, Color.FromArgb(this.random.Next(256), this.random.Next(256), this.random.Next(256)));
        }

        public void SetRightCurve(string key, float[] data, Color lineColor)
        {
            this.SetCurve(key, false, data, lineColor, 1f, false);
        }

        public void SetRightCurve(string key, float[] data, Color lineColor, bool isSmooth)
        {
            this.SetCurve(key, false, data, lineColor, 1f, isSmooth);
        }
        #endregion

        #region Properties

        [Browsable(true), Category("EasyScada"), DefaultValue(typeof(Font), "Microsoft Sans Serif")]
        public override Font Font
        {
            get => font_size9;
            set
            {
                if (value != null)
                {
                    font_size9 = value;
                    Invalidate();
                }
            }
        }

        [Browsable(true), Category("EasyScada"), DefaultValue(typeof(Color), "Transparent"), EditorBrowsable(EditorBrowsableState.Always)]
        public override Color BackColor
        {
            get
            {
                return base.BackColor;
            }
            set
            {
                base.BackColor = value;
            }
        }

        [Category("EasyScada"), Browsable(true), DefaultValue((float)100f)]
        public float LeftAxisMaxValue
        {
            get
            {
                return this.value_max_left;
            }
            set
            {
                this.value_max_left = value;
                base.Invalidate();
            }
        }

        [Category("EasyScada"), Browsable(true), DefaultValue((float)0f)]
        public float LeftAxisMinValue
        {
            get
            {
                return this.value_min_left;
            }
            set
            {
                this.value_min_left = value;
                base.Invalidate();
            }
        }

        [Category("EasyScada"), Browsable(true), DefaultValue((float)100f)]
        public float RightAxisMaxValue
        {
            get
            {
                return this.value_max_right;
            }
            set
            {
                this.value_max_right = value;
                base.Invalidate();
            }
        }

        [Category("EasyScada"), Browsable(true), DefaultValue((float)0f)]
        public float RightAxisMinValue
        {
            get
            {
                return this.value_min_right;
            }
            set
            {
                this.value_min_right = value;
                base.Invalidate();
            }
        }

        [Category("EasyScada"), Browsable(true), DefaultValue(5)]
        public int GridRowCount
        {
            get
            {
                return this.gridRowCount;
            }
            set
            {
                this.gridRowCount = value;
                if (gridRowCount < 1)
                    gridRowCount = 1;
                base.Invalidate();
            }
        }

        [Category("EasyScada"), Browsable(true), DefaultValue(true)]
        public bool AutoSizeGridColumn
        {
            get
            {
                return this.autoSizeGridColumn;
            }
            set
            {
                this.autoSizeGridColumn = value;
                base.Invalidate();
            }
        }

        [Category("EasyScada"), Browsable(true), DefaultValue(300)]
        public int GridColumnMaxWidth
        {
            get
            {
                return this.gridColumnMaxWidth;
            }
            set
            {
                this.gridColumnMaxWidth = value;
                base.Invalidate();
            }
        }

        [Category("EasyScada"), Browsable(true), DefaultValue(true)]
        public bool GridVisible
        {
            get
            {
                return this.gridVisible;
            }
            set
            {
                this.gridVisible = value;
                base.Invalidate();
            }
        }

        [Category("EasyScada"), Browsable(true), DefaultValue(typeof(Color), "DimGray")]
        public Color AxisColor
        {
            get
            {
                return this.color_deep;
            }
            set
            {
                this.color_deep = value;
                this.InitializationColor();
                base.Invalidate();
            }
        }

        [Category("EasyScada"), Browsable(true), DefaultValue(typeof(Color), "Gray")]
        public Color GridLineColor
        {
            get
            {
                return this.color_dash;
            }
            set
            {
                this.color_dash = value;
                if (this.pen_dash == null)
                {
                    Pen local1 = this.pen_dash;
                }
                else
                {
                    this.pen_dash.Dispose();
                }
                this.pen_dash = new Pen(this.color_dash);
                this.pen_dash.DashStyle = DashStyle.Custom;
                this.pen_dash.DashPattern = new float[] { 5f, 5f };
                base.Invalidate();
            }
        }

        [Category("EasyScada"), Browsable(true), DefaultValue(100)]
        public int GridColumnWidth
        {
            get
            {
                return this.gridColumnWidth;
            }
            set
            {
                this.gridColumnWidth = value;
                if (gridColumnWidth < 1)
                    gridColumnWidth = 1;
                base.Invalidate();
            }
        }

        [Category("EasyScada"), Browsable(true), DefaultValue("HH:mm:ss")]
        public string TimeStringFormat
        {
            get
            {
                return this.textFormat;
            }
            set
            {
                this.textFormat = value;
                base.Invalidate();
            }
        }

        [Category("EasyScada"), Browsable(true), DefaultValue("")]
        public string Title
        {
            get
            {
                return this.title;
            }
            set
            {
                this.title = value;
                base.Invalidate();
            }
        }

        [Category("EasyScada"), Browsable(true), DefaultValue(true)]
        public bool RightAxisVisible
        {
            get
            {
                return this.rightAxisVisible;
            }
            set
            {
                this.rightAxisVisible = value;
                base.Invalidate();
            }
        }

        [Category("EasyScada"), Browsable(true), DefaultValue(false)]
        public bool SmoothLine
        {
            get { return smoothLine; }
            set
            {
                smoothLine = value;
                Invalidate();
            }
        }

        [Browsable(true),Category("EasyScada"), DefaultValue(100)]
        public int CurveNameWidth
        {
            get
            {
                return this.curveNameWidth;
            }
            set
            {
                if (value > 10)
                {
                    this.curveNameWidth = value;
                }
            }
        }
        #endregion
        #endregion
    }
}
