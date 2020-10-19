using EasyScada.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Windows.Forms;

namespace EasyScada.Winforms.Controls.Charts
{
    [ToolboxBitmap(typeof(HistoricalTrend))]
    [Designer(typeof(HistoricalTrendDesigner))]
    public class HistoricalTrend : UserControl
    {
        private LogProfile _database = new LogProfile();
        [Category("EasyScada")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public LogProfile Database { get => _database; set => _database = value; }

        internal TrendLineCollection _lines = new TrendLineCollection();
        [Category("EasyScada"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public TrendLineCollection Lines { get => _lines; }


        #region Fields
        private List<AuxiliaryLabel> auxiliary_Labels;
        private List<MarkLine> markLines;
        private List<MarkText> markTexts;
        internal Dictionary<string, CurveItem> data_dicts = null;
        private List<CurveItem> data_lists = null;
        private List<DateTime> data_times = null;
        private List<string> data_customer = null;
        private List<AuxiliaryLine> auxiliary_lines = new List<AuxiliaryLine>();
        private float data_Scale_Render = 1f;
        private int data_count = 0;
        private string data_time_formate = "yyyy-MM-dd HH:mm:ss";
        private string mouse_hover_time_formate = "dd HH:mm:ss";
        private bool is_mouse_on_picture = false;
        private Point mouse_location = new Point(-1, -1);
        private int m_RowBetweenStart = -1;
        private int m_RowBetweenEnd = -1;
        private int m_RowBetweenStartHeight = -1;
        private int m_RowBetweenHeight = -1;
        private bool m_IsMouseLeftDown = false;
        private List<MarkBackSection> markBackSections = null;
        private List<MarkForeSection> markForeSections = null;
        private List<MarkForeSection> markForeSectionsTmp = null;
        private List<MarkForeSection> markForeActiveSections = null;
        private StringFormat sf = null;
        private StringFormat sf_left = null;
        private StringFormat sf_right = null;
        private StringFormat sf_default = null;
        private StringFormat sf_top = null;
        private Color backColor = Color.White;
        private Random random = null;
        private Color axisColor = Color.LightGray;
        private Brush coordinateBrush = new SolidBrush(Color.LightGray);
        private Pen coordinatePen = new Pen(Color.LightGray);
        private Color gridLineColor = Color.FromArgb(72, 72, 72);
        private Pen coordinateDashPen = null;
        private Color markLineColor = Color.Cyan;
        private Pen markLinePen = new Pen(Color.Cyan);
        private Color markTextColor = Color.Yellow;
        private Brush markTextBrush = new SolidBrush(Color.Yellow);
        private Color moveLineColor = Color.White;
        private Pen moveLinePen = new Pen(Color.White);
        private Color borderTooltipColor = Color.HotPink;
        private Pen borderTooltipPen = new Pen(Color.HotPink);
        private Color foreTooltipColor = Color.Cyan;
        private Brush foreTooltipBrush = new SolidBrush(Color.Cyan);
        private bool tooltipTimeVisible = true;
        private bool tooltipDataVisible = true;
        private bool tooltipVisible = true;
        private bool verticalLineTooltipVisible = true;
        private bool horizontalLineTooltipVisible = true;
        bool isPressedCtrl = false;
        private int data_tip_width = 150;
        private int curveNameWidth = 150;
        private int gridColumnWidth = 200;
        private float value_max_left = 100f;
        private float value_min_left = 0f;
        private float value_max_right = 100f;
        private float value_min_right = 0f;
        private string value_unit_left = string.Empty;
        private string value_unit_right = string.Empty;
        private int gridRowCount = 5;
        private bool isShowTextInfomation = true;
        private bool rightAxisVisible = true;
        private bool isAllowSelectSection = true;
        private bool isRenderTimeData = false;
        private bool isMouseFreeze = false;
        private bool isAoordinateRoundInt = false;
        private IContainer components = null;
        private PictureBox pictureBox3;
        private Panel panel1;
        #endregion

        #region Events
        [Category("Mouse")]
        public event CurveCustomerDoubleClick OnCurveCustomerDoubleClick;

        [Category("Mouse")]
        public event CurveDoubleClick OnCurveDoubleClick;

        [Category("Mouse")]
        public event CurveMouseMove OnCurveMouseMove;

        [Category("Mouse")]
        public event CurveRangeSelect OnCurveRangeSelect;
        #endregion

        #region Constructors
        public HistoricalTrend()
        {
            this.data_dicts = new Dictionary<string, CurveItem>();
            this.data_lists = new List<CurveItem>();
            this.markBackSections = new List<MarkBackSection>();
            this.markForeSections = new List<MarkForeSection>();
            this.markForeSectionsTmp = new List<MarkForeSection>();
            this.markForeActiveSections = new List<MarkForeSection>();
            this.markTexts = new List<MarkText>();
            this.markLines = new List<MarkLine>();
            this.auxiliary_Labels = new List<AuxiliaryLabel>();
            this.data_times = new List<DateTime>(0);
            this.data_customer = new List<string>();
            this.sf_default = new StringFormat();
            this.sf = new StringFormat();
            this.sf.Alignment = StringAlignment.Center;
            this.sf.LineAlignment = StringAlignment.Center;
            this.sf_left = new StringFormat();
            this.sf_left.LineAlignment = StringAlignment.Center;
            this.sf_left.Alignment = StringAlignment.Near;
            this.sf_right = new StringFormat();
            this.sf_right.LineAlignment = StringAlignment.Center;
            this.sf_right.Alignment = StringAlignment.Far;
            this.sf_top = new StringFormat();
            this.sf_top.Alignment = StringAlignment.Center;
            this.sf_top.LineAlignment = StringAlignment.Near;
            this.coordinateDashPen = new Pen(Color.FromArgb(72, 72, 72));
            this.coordinateDashPen.DashPattern = new float[] { 5f, 5f };
            this.coordinateDashPen.DashStyle = DashStyle.Custom;
            this.random = new Random();
            this.InitializeComponent();
            base.SetStyle(ControlStyles.SupportsTransparentBackColor | ControlStyles.UserPaint, true);
            base.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            base.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            base.BackColor = Color.FromArgb(46, 46, 46);
            base.ForeColor = Color.Yellow;
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
            this.CalculateAuxiliaryPaintY();
            if (this.isShowTextInfomation)
            {
                Image image = this.pictureBox3.Image;
                if (image == null)
                {
                    Image local1 = image;
                }
                else
                {
                    image.Dispose();
                }
                this.pictureBox3.Image = this.GetBitmapFromString(this.Text);
            }
        }

        public void AddAuxiliaryLabel(AuxiliaryLabel auxiliaryLable)
        {
            this.auxiliary_Labels.Add(auxiliaryLable);
        }

        public void AddLeftAuxiliary(float value)
        {
            this.AddLeftAuxiliary(value, this.axisColor);
        }

        public void AddLeftAuxiliary(float value, Color lineColor)
        {
            this.AddLeftAuxiliary(value, lineColor, 1f, true);
        }

        public void AddLeftAuxiliary(float value, Color lineColor, float lineThickness, bool isDashLine)
        {
            this.AddAuxiliary(value, lineColor, lineThickness, isDashLine, true);
        }

        public void AddMarkActiveSection(MarkForeSection markActiveSection)
        {
            this.markForeActiveSections.Add(markActiveSection);
        }

        public void AddMarkBackSection(MarkBackSection markBackSection)
        {
            this.markBackSections.Add(markBackSection);
        }

        public void AddMarkForeSection(MarkForeSection markForeSection)
        {
            this.markForeSections.Add(markForeSection);
        }

        public void AddMarkLine(MarkLine markLine)
        {
            this.markLines.Add(markLine);
        }

        public void AddMarkText(MarkText markText)
        {
            this.markTexts.Add(markText);
        }

        public void AddRightAuxiliary(float value)
        {
            this.AddRightAuxiliary(value, this.axisColor);
        }

        public void AddRightAuxiliary(float value, Color lineColor)
        {
            this.AddRightAuxiliary(value, lineColor, 1f, true);
        }

        public void AddRightAuxiliary(float value, Color lineColor, float lineThickness, bool isDashLine)
        {
            this.AddAuxiliary(value, lineColor, lineThickness, isDashLine, false);
        }

        private void CalculateAuxiliaryPaintY()
        {
            for (int i = 0; i < this.auxiliary_lines.Count; i++)
            {
                if (this.auxiliary_lines[i].IsLeftFrame)
                {
                    this.auxiliary_lines[i].PaintValue = ChartHelper.ComputePaintLocationY(this.value_max_left, this.value_min_left, (float)(this.panel1.Height - 60), this.auxiliary_lines[i].Value) + 20f;
                }
                else
                {
                    this.auxiliary_lines[i].PaintValue = ChartHelper.ComputePaintLocationY(this.value_max_right, this.value_min_right, (float)(this.panel1.Height - 60), this.auxiliary_lines[i].Value) + 20f;
                }
            }
        }

        private void CalculateCurveDataMax()
        {
            this.data_count = 0;
            for (int i = 0; i < this.data_lists.Count; i++)
            {
                if (this.data_count < this.data_lists[i].Data.Length)
                {
                    this.data_count = this.data_lists[i].Data.Length;
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void DrawCoordinate(Graphics g, bool isLeft, float dx, float dy, float value_max, float value_min, StringFormat stringFormat, string unit)
        {
            g.TranslateTransform(dx, dy);
            float num = (base.Height - 29) - 3;
            float width = 40f;
            if (isLeft)
            {
                g.DrawLine(this.coordinatePen, (float)(width - 1f), (float)5f, (float)(width - 1f), (float)(num - 35f));
            }
            else
            {
                g.DrawLine(this.coordinatePen, (float)0f, (float)5f, (float)0f, (float)(num - 35f));
            }
            if (isLeft)
            {
                ChartHelper.PaintTriangle(g, this.coordinateBrush, new PointF(width - 1f, 10f), 5, GraphDirection.Upward);
            }
            else
            {
                ChartHelper.PaintTriangle(g, this.coordinateBrush, new PointF(0f, 10f), 5, GraphDirection.Upward);
            }
            g.DrawString(unit, this.Font, this.coordinateBrush, new RectangleF(0f, -15f, width, 30f), this.sf);
            int num3 = 0;
            while (true)
            {
                if (num3 > this.gridRowCount)
                {
                    int num6 = 0;
                    while (true)
                    {
                        if (num6 >= this.auxiliary_lines.Count)
                        {
                            g.TranslateTransform(-dx, -dy);
                            return;
                        }
                        if (this.auxiliary_lines[num6].IsLeftFrame)
                        {
                            float num7 = ChartHelper.ComputePaintLocationY(value_max, value_min, num - 60f, this.auxiliary_lines[num6].Value) + 20f;
                            if (isLeft)
                            {
                                g.DrawLine(this.coordinatePen, width - 5f, num7, width - 2f, num7);
                                RectangleF layoutRectangle = new RectangleF(0f, num7 - 9f, width - 4f, 20f);
                                g.DrawString(this.auxiliary_lines[num6].Value.ToString(), this.Font, this.coordinateBrush, layoutRectangle, stringFormat);
                            }
                            else
                            {
                                g.DrawLine(this.coordinatePen, 1f, num7, 5f, num7);
                                RectangleF layoutRectangle = new RectangleF(6f, num7 - 9f, width - 3f, 20f);
                                g.DrawString(this.auxiliary_lines[num6].Value.ToString(), this.Font, this.coordinateBrush, layoutRectangle, stringFormat);
                            }
                        }
                        num6++;
                    }
                }
                float num4 = ((float)((num3 * (value_max - value_min)) / ((double)this.gridRowCount))) + value_min;
                if (this.RoundAxisValue)
                {
                    num4 = (float)Math.Round((double)num4, 0);
                }
                float paintValue = ChartHelper.ComputePaintLocationY(value_max, value_min, num - 60f, num4) + 20f;
                if (this.IsNeedPaintDash(paintValue))
                {
                    if (isLeft)
                    {
                        g.DrawLine(this.coordinatePen, width - 5f, paintValue, width - 2f, paintValue);
                        RectangleF layoutRectangle = new RectangleF(0f, paintValue - 9f, width - 4f, 20f);
                        g.DrawString(num4.ToString(), this.Font, this.coordinateBrush, layoutRectangle, stringFormat);
                    }
                    else
                    {
                        g.DrawLine(this.coordinatePen, 1f, paintValue, 5f, paintValue);
                        RectangleF layoutRectangle = new RectangleF(5f, paintValue - 9f, width - 3f, 20f);
                        g.DrawString(num4.ToString(), this.Font, this.coordinateBrush, layoutRectangle, stringFormat);
                    }
                }
                num3++;
            }
        }

        private void DrawMarkForeSection(Graphics g, MarkForeSection markForeSection, Font font)
        {
            if (markForeSection != null)
            {
                bool num1;
                float y = (markForeSection.Height > 1f) ? markForeSection.Height : (((this.panel1.Height - 60) * markForeSection.Height) + 20f);
                float num2 = (markForeSection.StartHeight > 1f) ? markForeSection.StartHeight : (((this.panel1.Height - 60) * markForeSection.StartHeight) + 20f);
                if ((markForeSection.StartIndex == -1) || (markForeSection.EndIndex == -1))
                {
                    num1 = false;
                }
                else
                {
                    num1 = (markForeSection.EndIndex <= markForeSection.StartIndex) ? false : ((num2 < y));
                }
                if (num1)
                {
                    bool num9;
                    int num3 = Convert.ToInt32((float)(markForeSection.StartIndex * this.data_Scale_Render));
                    int num4 = Convert.ToInt32((float)(markForeSection.EndIndex * this.data_Scale_Render));
                    if (((markForeSection.StartIndex >= this.data_count) || (markForeSection.EndIndex >= this.data_count)) || (markForeSection.StartIndex >= this.data_times.Count))
                    {
                        num9 = false;
                    }
                    else
                    {
                        num9 = (markForeSection.EndIndex < this.data_times.Count);
                    }
                    if (num9)
                    {
                        g.DrawLine(markForeSection.LinePen, new PointF((float)num3, num2), new PointF((float)num3, y + 30f));
                        g.DrawLine(markForeSection.LinePen, new PointF((float)num4, num2), new PointF((float)num4, y + 30f));
                        int num5 = markForeSection.IsRenderTimeText ? 20 : 0;
                        int num7 = num4 - num3;
                        g.DrawLine(markForeSection.LinePen, new PointF((float)(num3 - (((num4 - num3) > 100) ? num5 : 110)), y), new PointF((float)(num4 + num5), y));
                        PointF[] points = new PointF[] { new PointF((float)(num3 + 20), y + 10f), new PointF((float)num3, y), new PointF((float)(num3 + 20), y - 10f) };
                        g.DrawLines(markForeSection.LinePen, points);
                        PointF[] tfArray2 = new PointF[] { new PointF((float)(num4 - 20), y - 10f), new PointF((float)num4, y), new PointF((float)(num4 - 20), y + 10f) };
                        g.DrawLines(markForeSection.LinePen, tfArray2);
                        TimeSpan span = this.data_times[markForeSection.EndIndex] - this.data_times[markForeSection.StartIndex];
                        if ((num4 - num3) <= 100)
                        {
                            g.DrawString(span.TotalMinutes.ToString("F1") + " min", font, markForeSection.FontBrush, new PointF((float)(num3 - 100), y - 17f));
                        }
                        else
                        {
                            g.DrawString(span.TotalMinutes.ToString("F1") + " min", font, markForeSection.FontBrush, new RectangleF((float)num3, (y - font.Height) - 2f, (float)(num4 - num3), (float)font.Height), this.sf);
                        }
                        if (!string.IsNullOrEmpty(markForeSection.MarkText))
                        {
                            g.DrawString(markForeSection.MarkText, font, markForeSection.FontBrush, new RectangleF((float)num3, y + 3f, (float)(num4 - num3), (float)font.Height), this.sf);
                        }

                        if (markForeSection.IsRenderTimeText)
                        {
                            g.DrawString("Start Time: " + this.data_times[markForeSection.StartIndex].ToString(this.data_time_formate), font, markForeSection.FontBrush, new PointF((float)(num4 + 5), y - 17f));
                            g.DrawString("End Time: " + this.data_times[markForeSection.EndIndex].ToString(this.data_time_formate), font, markForeSection.FontBrush, new PointF((float)(num4 + 5), y + 2f));
                        }
                    }
                }
            }
        }

        private Bitmap GetBitmapFromString(string text)
        {
            int width = (this.panel1.Width > 10) ? this.panel1.Width : 1;
            int height = (this.panel1.Height > 10) ? this.panel1.Height : 10;
            Bitmap image = new Bitmap(width, height);
            Graphics g = Graphics.FromImage(image);
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.Clear(this.backColor);
            this.PaintFromString(g, text, width, height);
            g.Dispose();
            return image;
        }

        private unsafe void GetRenderCurveMain(Graphics g, int m_width, int m_height)
        {
            int num = 0;
            while (true)
            {
                if (num >= this.markBackSections.Count)
                {
                    g.DrawLine(this.coordinatePen, 0, this.panel1.Height - 40, m_width - 1, this.panel1.Height - 40);
                    this.CalculateAuxiliaryPaintY();
                    int num3 = 1;
                    while (true)
                    {
                        if (num3 > this.gridRowCount)
                        {
                            int num6 = 0;
                            while (true)
                            {
                                if (num6 >= this.auxiliary_lines.Count)
                                {
                                    int num7 = this.gridColumnWidth;
                                    while (true)
                                    {
                                        if (num7 >= m_width)
                                        {
                                            foreach (MarkText text in this.markTexts)
                                            {
                                                foreach (KeyValuePair<string, CurveItem> pair in this.data_dicts)
                                                {
                                                    if (!pair.Value.Visible)
                                                    {
                                                        continue;
                                                    }
                                                    if (pair.Value.LineRenderVisible && (pair.Key == text.CurveKey))
                                                    {
                                                        bool flag1;
                                                        if (pair.Value.Data != null)
                                                        {
                                                            flag1 = pair.Value.Data.Length > 1;
                                                        }
                                                        else
                                                        {
                                                            float[] data = pair.Value.Data;
                                                            flag1 = false;
                                                        }
                                                        if ((flag1 && ((text.Index >= 0) && (text.Index < pair.Value.Data.Length))) && !float.IsNaN(pair.Value.Data[text.Index]))
                                                        {
                                                            var t = new PointF(text.Index * this.data_Scale_Render, ChartHelper.ComputePaintLocationY(pair.Value.IsLeftFrame ? this.value_max_left : this.value_max_right, pair.Value.IsLeftFrame ? this.value_min_left : this.value_min_right, (float)(this.panel1.Height - 60), pair.Value.Data[text.Index]) + 20f);
                                                            PointF tf;
                                                            PointF* tfPtr2 = &t;
                                                            tfPtr2 = &tf;
                                                            g.FillEllipse(text.CircleBrush, new RectangleF(tf.X - 3f, tf.Y - 3f, 6f, 6f));
                                                            switch (((text.PositionStyle == MarkTextPositionStyle.Auto) ? MarkText.CalculateDirectionFromDataIndex(pair.Value.Data, text.Index) : text.PositionStyle))
                                                            {
                                                                case MarkTextPositionStyle.Up:
                                                                    g.DrawString(text.Text, this.Font, text.TextBrush, new RectangleF(tf.X - 100f, (tf.Y - this.Font.Height) - MarkText.MarkTextOffset, 200f, (float)(this.Font.Height + 2)), this.sf);
                                                                    break;

                                                                case MarkTextPositionStyle.Right:
                                                                    g.DrawString(text.Text, this.Font, text.TextBrush, new RectangleF(tf.X + MarkText.MarkTextOffset, tf.Y - this.Font.Height, 100f, (float)(this.Font.Height * 2)), this.sf_left);
                                                                    break;

                                                                case MarkTextPositionStyle.Down:
                                                                    g.DrawString(text.Text, this.Font, text.TextBrush, new RectangleF(tf.X - 100f, tf.Y + MarkText.MarkTextOffset, 200f, (float)(this.Font.Height + 2)), this.sf);
                                                                    break;

                                                                case MarkTextPositionStyle.Left:
                                                                    g.DrawString(text.Text, this.Font, text.TextBrush, new RectangleF(tf.X - 100f, tf.Y - this.Font.Height, (float)(100 - MarkText.MarkTextOffset), (float)(this.Font.Height * 2)), this.sf_right);
                                                                    break;

                                                                default:
                                                                    break;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            goto DRAW;
                                        }
                                        int num8 = Convert.ToInt32((float)(((float)num7) / this.data_Scale_Render));
                                        g.DrawLine(this.coordinateDashPen, num7, this.panel1.Height - 38, num7, 0);
                                        if (this.isRenderTimeData)
                                        {
                                            if (num8 < this.data_times.Count)
                                            {
                                                g.DrawString(this.data_times[num8].ToString(this.data_time_formate), this.Font, this.coordinateBrush, new Rectangle(num7 - 100, this.panel1.Height - 40, 200, 22), this.sf);
                                            }
                                        }
                                        else if (num8 < this.data_customer.Count)
                                        {
                                            g.DrawString(this.data_customer[num8], this.Font, this.coordinateBrush, new Rectangle(num7 - 100, this.panel1.Height - 40, 200, 22), this.sf);
                                        }
                                        num7 += this.gridColumnWidth;
                                    }
                                }

                                if (num6 < auxiliary_lines.Count)
                                    g.DrawLine(this.auxiliary_lines[num6].GetPen(), 0f, this.auxiliary_lines[num6].PaintValue, (float)(m_width - 1), this.auxiliary_lines[num6].PaintValue);
                                else
                                    break;
                                num6++;
                            }
                        }
                        float num4 = ((float)((num3 * (this.value_max_left - this.value_min_left)) / ((double)this.gridRowCount))) + this.value_min_left;
                        if (this.RoundAxisValue)
                        {
                            num4 = (float)Math.Round((double)num4, 0);
                        }
                        float paintValue = ChartHelper.ComputePaintLocationY(this.value_max_left, this.value_min_left, (float)(this.panel1.Height - 60), num4) + 20f;
                        if (this.IsNeedPaintDash(paintValue))
                        {
                            g.DrawLine(this.coordinateDashPen, 0f, paintValue, (float)(m_width - 1), paintValue);
                        }
                        num3++;
                    }
                }
                RectangleF rect = new RectangleF(this.markBackSections[num].StartIndex * this.data_Scale_Render, 0f, (this.markBackSections[num].EndIndex - this.markBackSections[num].StartIndex) * this.data_Scale_Render, (float)((this.panel1.Height - 60) + 20));
                using (Brush brush = new SolidBrush(this.markBackSections[num].BackColor))
                {
                    g.FillRectangle(brush, rect);
                }
                g.DrawRectangle(Pens.DimGray, rect.X, rect.Y, rect.Width, rect.Height);
                string markText = this.markBackSections[num].MarkText;
                string s = markText ?? string.Empty;
                if ((this.markBackSections[num].StartIndex < this.data_times.Count) && (this.markBackSections[num].EndIndex < this.data_times.Count))
                {
                    s = s + " (" + (this.data_times[this.markBackSections[num].EndIndex] - this.data_times[this.markBackSections[num].StartIndex]).TotalMinutes.ToString("F1") + " min)";
                }
                g.DrawString(s, this.Font, Brushes.DimGray, new RectangleF(rect.X, 3f, rect.Width, (float)(this.panel1.Height - 60)), this.sf_top);
                num++;
            }
            DRAW:
            foreach (MarkLine line in this.markLines)
            {
                bool flag31;
                PointF[] points = line.Points;
                if (points != null)
                {
                    flag31 = points.Length > 1;
                }
                else
                {
                    PointF[] local2 = points;
                    flag31 = false;
                }
                if (flag31)
                {
                    PointF[] tfArray = new PointF[line.Points.Length];
                    int index = 0;
                    while (true)
                    {
                        if (index >= line.Points.Length)
                        {
                            if (!line.IsLineClosed)
                            {
                                g.DrawLines(line.LinePen, tfArray);
                            }
                            else
                            {
                                g.DrawLines(line.LinePen, tfArray);
                                g.DrawLine(line.LinePen, tfArray[0], tfArray[tfArray.Length - 1]);
                            }
                            break;
                        }
                        
                        tfArray[index].X = line.Points[index].X * this.data_Scale_Render;
                        tfArray[index].Y = ChartHelper.ComputePaintLocationY(line.IsLeftFrame ? this.value_max_left : this.value_max_right, line.IsLeftFrame ? this.value_min_left : this.value_min_right, (float)(this.panel1.Height - 60), line.Points[index].Y) + 20f;
                        g.FillEllipse(line.CircleBrush, new RectangleF(tfArray[index].X - 3f, tfArray[index].Y - 3f, 6f, 6f));
                        MarkTextPositionStyle style4 = MarkText.CalculateDirectionFromDataIndex((from m in line.Points select m.Y).ToArray<float>(), index);
                        if (line.Marks != null)
                        {
                            switch (style4)
                            {
                                case MarkTextPositionStyle.Up:
                                    g.DrawString(line.Marks[index], this.Font, line.TextBrush, new RectangleF(tfArray[index].X - 100f, (tfArray[index].Y - this.Font.Height) - MarkText.MarkTextOffset, 200f, (float)(this.Font.Height + 2)), this.sf);
                                    break;

                                case MarkTextPositionStyle.Right:
                                    g.DrawString(line.Marks[index], this.Font, line.TextBrush, new RectangleF(tfArray[index].X + MarkText.MarkTextOffset, tfArray[index].Y - this.Font.Height, 100f, (float)(this.Font.Height * 2)), this.sf_left);
                                    break;

                                case MarkTextPositionStyle.Down:
                                    g.DrawString(line.Marks[index], this.Font, line.TextBrush, new RectangleF(tfArray[index].X - 100f, tfArray[index].Y + MarkText.MarkTextOffset, 200f, (float)(this.Font.Height + 2)), this.sf);
                                    break;

                                case MarkTextPositionStyle.Left:
                                    g.DrawString(line.Marks[index], this.Font, line.TextBrush, new RectangleF(tfArray[index].X - 100f, tfArray[index].Y - this.Font.Height, (float)(100 - MarkText.MarkTextOffset), (float)(this.Font.Height * 2)), this.sf_right);
                                    break;

                                default:
                                    break;
                            }
                        }
                        index++;
                    }
                }
            }
            using (Dictionary<string, CurveItem>.ValueCollection.Enumerator enumerator4 = this.data_dicts.Values.GetEnumerator())
            {
                CurveItem current;
                List<PointF> list;
                int num10;
                goto TR_0028;
            TR_000F:
                num10++;
                goto TR_001E;
            TR_0011:
                list.Clear();
                goto TR_000F;
            TR_001E:
                while (true)
                {
                    if (num10 >= current.Data.Length)
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
                        break;
                    }
                    if (float.IsNaN(current.Data[num10]))
                    {
                        if (list.Count > 1)
                        {
                            using (Pen pen = new Pen(current.LineColor, current.LineThickness))
                            {
                                if (current.IsSmoothCurve)
                                {
                                    g.DrawCurve(pen, list.ToArray());
                                }
                                else
                                {
                                    g.DrawLines(pen, list.ToArray());
                                }
                            }
                        }
                    }
                    else
                    {
                        PointF* tfPtr1;
                        PointF tf2 = new PointF
                        {
                            X = num10 * this.data_Scale_Render
                        };
                        tf2.Y = ChartHelper.ComputePaintLocationY(current.IsLeftFrame ? this.value_max_left : this.value_max_right, current.IsLeftFrame ? this.value_min_left : this.value_min_right, (float)(this.panel1.Height - 60), current.Data[num10]) + 20f;
                        tfPtr1 = &tf2;
                        list.Add(tf2);
                        goto TR_000F;
                    }
                    goto TR_0011;
                }
            TR_0028:
                while (true)
                {
                    if (enumerator4.MoveNext())
                    {
                        bool flag32;
                        current = enumerator4.Current;
                        if (!current.Visible)
                        {
                            continue;
                        }
                        if (!current.LineRenderVisible)
                        {
                            continue;
                        }
                        if (current.Data != null)
                        {
                            flag32 = current.Data.Length > 1;
                        }
                        else
                        {
                            float[] data = current.Data;
                            flag32 = false;
                        }
                        if (!flag32)
                        {
                            continue;
                        }
                        list = new List<PointF>(current.Data.Length);
                        num10 = 0;
                    }
                    else
                    {
                        for (int i = 0; i < this.markForeSections.Count; i++)
                        {
                            this.DrawMarkForeSection(g, this.markForeSections[i], this.Font);
                        }
                        return;
                    }
                    break;
                }
                goto TR_001E;
            }
        }

        private void CurveHistory_Load(object sender, EventArgs e)
        {
            if (this.isShowTextInfomation)
            {
                Image image = this.pictureBox3.Image;
                if (image == null)
                {
                    Image local1 = image;
                }
                else
                {
                    image.Dispose();
                }
                this.pictureBox3.Image = this.GetBitmapFromString(this.Text);
            }
            this.pictureBox3.MouseMove += new MouseEventHandler(this.PictureBox3_MouseMove);
            this.pictureBox3.MouseLeave += new EventHandler(this.PictureBox3_MouseLeave);
            this.pictureBox3.MouseEnter += new EventHandler(this.PictureBox3_MouseEnter);
            this.pictureBox3.MouseDown += new MouseEventHandler(this.PictureBox3_MouseDown);
            this.pictureBox3.MouseUp += new MouseEventHandler(this.PictureBox3_MouseUp);
            this.pictureBox3.Paint += new PaintEventHandler(this.PictureBox3_Paint);
            this.pictureBox3.MouseDoubleClick += new MouseEventHandler(this.PictureBox3_MouseDoubleClick);
            this.panel1.Scroll += new ScrollEventHandler(this.Panel1_Scroll);
            this.panel1.PreviewKeyDown += Panel1_PreviewKeyDown;
            this.panel1.MouseWheel += Panel1_MouseWheel;
            base.MouseMove += new MouseEventHandler(this.CurveHistory_MouseMove);
            base.MouseDown += new MouseEventHandler(this.CurveHistory_MouseDown);
        }

        private void Panel1_MouseWheel(object sender, MouseEventArgs e)
        {
            if (panel1.DisplayRectangle.Contains(e.Location) && isPressedCtrl)
            {
                if (e.Delta > 0 && data_Scale_Render < 3)
                {
                    data_Scale_Render += 0.05f;
                }
                else if (e.Delta < 0 && data_Scale_Render > -1.95f)
                {
                    if (data_Scale_Render > 0.1f)
                        data_Scale_Render -= 0.05f;
                }
                data_Scale_Render = (float)Math.Round(data_Scale_Render, 2);
                RenderCurveUI();
            }
        }

        private void Panel1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
        }

        private void CurveHistory_MouseDown(object sender, MouseEventArgs e)
        {
            foreach (KeyValuePair<string, CurveItem> pair in this.data_dicts)
            {
                RectangleF titleRegion = pair.Value.TitleRegion;
                if (titleRegion.Contains((PointF)e.Location))
                {
                    pair.Value.LineRenderVisible = !pair.Value.LineRenderVisible;
                    this.RenderCurveUI();
                    break;
                }
            }
        }

        private void CurveHistory_MouseMove(object sender, MouseEventArgs e)
        {
            bool flag = false;
            foreach (KeyValuePair<string, CurveItem> pair in this.data_dicts)
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

        private void CurveHistory_SizeChanged(object sender, EventArgs e)
        {
            if (!this.isShowTextInfomation)
            {
                this.RenderCurveUI();
            }
            else
            {
                Image image = this.pictureBox3.Image;
                if (image == null)
                {
                    Image local1 = image;
                }
                else
                {
                    image.Dispose();
                }
                this.pictureBox3.Image = this.GetBitmapFromString(this.Text);
                base.Invalidate();
            }
        }

        private void InitializeComponent()
        {
            this.pictureBox3 = new PictureBox();
            this.panel1 = new Panel();
            ((ISupportInitialize)this.pictureBox3).BeginInit();
            this.panel1.SuspendLayout();
            base.SuspendLayout();
            this.pictureBox3.Location = new Point(0, 0);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new Size(351, 209);
            this.pictureBox3.SizeMode = PictureBoxSizeMode.AutoSize;
            this.pictureBox3.TabIndex = 2;
            this.pictureBox3.TabStop = false;
            this.panel1.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Top;
            this.panel1.AutoScroll = true;
            this.panel1.BackColor = Color.Transparent;
            this.panel1.Controls.Add(this.pictureBox3);
            this.panel1.Location = new Point(43, 29);
            this.panel1.Name = "panel1";
            this.panel1.Size = new Size(766, 446);
            this.panel1.TabIndex = 3;
            base.AutoScaleMode = AutoScaleMode.None;
            this.BackColor = Color.FromArgb(46, 46, 46);
            base.Controls.Add(this.panel1);
            base.Name = "history";
            base.Size = new Size(852, 478);
            base.Load += new EventHandler(this.CurveHistory_Load);
            base.SizeChanged += new EventHandler(this.CurveHistory_SizeChanged);
            ((ISupportInitialize)this.pictureBox3).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
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
                    if (Math.Abs((float)(this.auxiliary_lines[num].PaintValue - paintValue)) >= this.Font.Height)
                    {
                        num++;
                        continue;
                    }
                    flag2 = false;
                }
                return flag2;
            }
        }

        protected override void OnDockChanged(EventArgs e)
        {
            base.OnDockChanged(e);
            this.CurveHistory_SizeChanged(this, e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            this.PaintMain(g, (float)base.Width, (float)base.Height);
            base.OnPaint(e);
        }

        private void PaintFromString(Graphics g, string text, int m_width, int m_height)
        {
            Font font = new Font(this.Font.FontFamily, 18f);
            if ((text != null) ? (text.Length > 400) : false)
            {
                font = new Font(this.Font.FontFamily, 12f);
            }
            int num = m_height - 60;
            g.DrawLine(this.coordinatePen, 0, m_height - 40, m_width - 1, m_height - 40);
            int num2 = 1;
            while (true)
            {
                if (num2 > this.gridRowCount)
                {
                    this.CalculateAuxiliaryPaintY();
                    int num5 = 0;
                    while (true)
                    {
                        if (num5 >= this.auxiliary_lines.Count)
                        {
                            int num6 = this.gridColumnWidth;
                            while (true)
                            {
                                if (num6 >= m_width)
                                {
                                    Rectangle layoutRectangle = new Rectangle(0, 0, m_width, m_height);
                                    using (Brush brush = new SolidBrush(this.ForeColor))
                                    {
                                        g.DrawString(text, font, brush, layoutRectangle, this.sf);
                                    }
                                    font.Dispose();
                                    return;
                                }
                                g.DrawLine(this.coordinateDashPen, num6, m_height - 40, num6, 0);
                                num6 += this.gridColumnWidth;
                            }
                        }
                        g.DrawLine(this.auxiliary_lines[num5].GetPen(), 0f, this.auxiliary_lines[num5].PaintValue, (float)(m_width - 1), this.auxiliary_lines[num5].PaintValue);
                        num5++;
                    }
                }
                float num3 = ((float)((num2 * (this.value_max_left - this.value_min_left)) / ((double)this.gridRowCount))) + this.value_min_left;
                float paintValue = ChartHelper.ComputePaintLocationY(this.value_max_left, this.value_min_left, (float)(m_height - 60), num3) + 20f;
                if (this.IsNeedPaintDash(paintValue))
                {
                    g.DrawLine(this.coordinateDashPen, 0f, paintValue, (float)(m_width - 1), paintValue);
                }
                num2++;
            }
        }

        private void PaintHeadText(Graphics g)
        {
            float num = 50f;
            foreach (KeyValuePair<string, CurveItem> pair in this.data_dicts)
            {
                if (pair.Value.Visible)
                {
                    Pen pen = pair.Value.LineRenderVisible ? new Pen(pair.Value.LineColor) : new Pen(Color.FromArgb(80, pair.Value.LineColor));
                    g.DrawLine(pen, num, 11f, num + 30f, 11f);
                    g.DrawEllipse(pen, (float)(num + 8f), (float)4f, (float)14f, (float)14f);
                    pen.Dispose();
                    SolidBrush brush = pair.Value.LineRenderVisible ? new SolidBrush(pair.Value.LineColor) : new SolidBrush(Color.FromArgb(80, pair.Value.LineColor));
                    g.DrawString(pair.Key, this.Font, brush, new RectangleF(num + 35f, 2f, (float)(this.curveNameWidth - 30), 18f), this.sf_left);
                    pair.Value.TitleRegion = new RectangleF(num, 2f, 60f, 18f);
                    brush.Dispose();
                    num += this.curveNameWidth;
                }
            }
            for (int i = 0; i < this.auxiliary_Labels.Count; i++)
            {
                if (!string.IsNullOrEmpty(this.auxiliary_Labels[i].Text))
                {
                    int x = (this.auxiliary_Labels[i].LocationX > 1f) ? ((int)this.auxiliary_Labels[i].LocationX) : ((int)(this.auxiliary_Labels[i].LocationX * this.panel1.Width));
                    int num4 = ((int)g.MeasureString(this.auxiliary_Labels[i].Text, this.Font).Width) + 3;
                    Point[] points = new Point[] { new Point(x, 11), new Point(x + 10, 20), new Point((x + num4) + 10, 20), new Point((x + num4) + 10, 0), new Point(x + 10, 0), new Point(x, 11) };
                    g.FillPolygon(this.auxiliary_Labels[i].TextBack, points);
                    g.DrawString(this.auxiliary_Labels[i].Text, this.Font, this.auxiliary_Labels[i].TextBrush, new Rectangle(x + 7, 0, num4 + 3, 20), this.sf);
                }
            }
        }

        private void PaintMain(Graphics g, float width, float height)
        {
            this.DrawCoordinate(g, true, 3f, 29f, this.value_max_left, this.value_min_left, this.sf_right, this.value_unit_left);
            if (this.rightAxisVisible)
            {
                this.DrawCoordinate(g, false, (float)(base.Width - 43), 29f, this.value_max_right, this.value_min_right, this.sf_left, this.value_unit_right);
            }
            this.PaintHeadText(g);
        }

        private void Panel1_Scroll(object sender, ScrollEventArgs e)
        {
            this.OnScroll(e);
            
        }

        private void PictureBox3_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (!this.isShowTextInfomation)
            {
                this.isMouseFreeze = true;
                int index = Convert.ToInt32((float)(((e.X >= 0) ? ((float)e.X) : ((float)(65536 + e.X))) / this.data_Scale_Render));
                if (this.isRenderTimeData)
                {
                    if ((index >= 0) && (index < this.data_times.Count))
                    {
                        if (this.OnCurveDoubleClick == null)
                        {
                            CurveDoubleClick onCurveDoubleClick = this.OnCurveDoubleClick;
                        }
                        else
                        {
                            this.OnCurveDoubleClick(this, index, this.data_times[index]);
                        }
                    }
                }
                else if ((index >= 0) && (index < this.data_customer.Count))
                {
                    if (this.OnCurveCustomerDoubleClick == null)
                    {
                        CurveCustomerDoubleClick onCurveCustomerDoubleClick = this.OnCurveCustomerDoubleClick;
                    }
                    else
                    {
                        this.OnCurveCustomerDoubleClick(this, index, this.data_customer[index]);
                    }
                }
            }
        }

        private void PictureBox3_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                this.isMouseFreeze = false;
                this.markForeSectionsTmp.Clear();
                this.m_RowBetweenStart = -1;
                this.m_RowBetweenEnd = -1;
                this.m_RowBetweenHeight = -1;
                this.m_RowBetweenStartHeight = -1;
                this.pictureBox3.Invalidate();
            }
            else if (this.isAllowSelectSection)
            {
                this.m_IsMouseLeftDown = true;
                this.m_RowBetweenStart = Convert.ToInt32((float)(((e.X >= 0) ? ((float)e.X) : ((float)(65536 + e.X))) / this.data_Scale_Render));
                this.m_RowBetweenStartHeight = e.Y;
                this.m_RowBetweenHeight = e.Y;
            }
        }

        private void PictureBox3_MouseEnter(object sender, EventArgs e)
        {
            this.is_mouse_on_picture = true;
            this.Focus();
        }

        private void PictureBox3_MouseLeave(object sender, EventArgs e)
        {
            if (!this.isMouseFreeze)
            {
                this.is_mouse_on_picture = false;
            }
            if (this.OnCurveMouseMove == null)
            {
                CurveMouseMove onCurveMouseMove = this.OnCurveMouseMove;
            }
            else
            {
                this.OnCurveMouseMove(this, -1, -1);
            }
            this.pictureBox3.Invalidate();
        }

        private void PictureBox3_MouseMove(object sender, MouseEventArgs e)
        {
            bool num1;
            if (!this.is_mouse_on_picture || this.isShowTextInfomation)
            {
                num1 = false;
            }
            else
            {
                num1 = !this.isMouseFreeze;
            }
            if (num1)
            {
                this.mouse_location = e.Location;
                if (this.mouse_location.X < 0)
                {
                    this.mouse_location.X = 65536 + this.mouse_location.X;
                }
                if (this.OnCurveMouseMove == null)
                {
                    CurveMouseMove onCurveMouseMove = this.OnCurveMouseMove;
                }
                else
                {
                    this.OnCurveMouseMove(this, this.mouse_location.X, this.mouse_location.Y);
                }
                this.pictureBox3.Invalidate();
            }
        }

        private void PictureBox3_MouseUp(object sender, MouseEventArgs e)
        {
            if ((e.Button == MouseButtons.Left) && this.isAllowSelectSection)
            {
                MarkForeSection section1 = new MarkForeSection();
                section1.StartIndex = this.m_RowBetweenStart;
                section1.EndIndex = this.m_RowBetweenEnd;
                section1.Height = this.m_RowBetweenHeight;
                section1.StartHeight = this.m_RowBetweenStartHeight;
                section1.LinePen = this.markLinePen;
                section1.FontBrush = this.markTextBrush;
                MarkForeSection item = section1;
                this.markForeSectionsTmp.Add(item);
                this.m_IsMouseLeftDown = false;
                this.pictureBox3.Invalidate();
                if (this.OnCurveRangeSelect == null)
                {
                    CurveRangeSelect onCurveRangeSelect = this.OnCurveRangeSelect;
                }
                else
                {
                    this.OnCurveRangeSelect(this, this.m_RowBetweenStart, this.m_RowBetweenEnd);
                }
                this.m_RowBetweenStart = -1;
                this.m_RowBetweenEnd = -1;
                this.m_RowBetweenHeight = -1;
                this.m_RowBetweenStartHeight = -1;
            }
        }

        private void PictureBox3_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            Font font = new Font(this.Font.FontFamily, 12f);
            Brush brush = new SolidBrush(Color.FromArgb(220, Color.FromArgb(52, 52, 52)));
            int index = Convert.ToInt32((float)(((float)this.mouse_location.X) / this.data_Scale_Render));
            int num2 = 0;
            while (true)
            {
                if (num2 < this.markForeSectionsTmp.Count)
                {
                    this.DrawMarkForeSection(g, this.markForeSectionsTmp[num2], font);
                    num2++;
                    continue;
                }
                if ((this.m_IsMouseLeftDown && (this.m_RowBetweenStart != -1)) && (index < this.data_count))
                {
                    this.m_RowBetweenEnd = Convert.ToInt32((float)(((float)this.mouse_location.X) / this.data_Scale_Render));
                    this.m_RowBetweenHeight = this.mouse_location.Y;
                }
                MarkForeSection markForeSection = new MarkForeSection();
                markForeSection.StartIndex = this.m_RowBetweenStart;
                markForeSection.EndIndex = this.m_RowBetweenEnd;
                markForeSection.Height = this.m_RowBetweenHeight;
                markForeSection.StartHeight = this.m_RowBetweenStartHeight;
                markForeSection.LinePen = this.markLinePen;
                markForeSection.FontBrush = this.markTextBrush;
                this.DrawMarkForeSection(g, markForeSection, font);
                int num3 = 0;
                while (true)
                {
                    if (num3 < this.markForeActiveSections.Count)
                    {
                        if ((index >= this.markForeActiveSections[num3].StartIndex) && (index <= this.markForeActiveSections[num3].EndIndex))
                        {
                            this.DrawMarkForeSection(g, this.markForeActiveSections[num3], font);
                        }
                        num3++;
                        continue;
                    }
                    if (!(this.is_mouse_on_picture && !this.isShowTextInfomation))
                    {
                        goto TR_0001;
                    }
                    else
                    {
                        if (tooltipVisible)
                        {
                            if (verticalLineTooltipVisible)
                                g.DrawLine(this.moveLinePen, this.mouse_location.X, 15, this.mouse_location.X, this.panel1.Height - 38);
                            if (horizontalLineTooltipVisible)
                                g.DrawLine(this.moveLinePen, 0, mouse_location.Y, mouse_location.X + panel1.Width, mouse_location.Y);
                        }
                        if ((index < this.data_count) && (index >= 0))
                        {
                            int y = (this.mouse_location.Y - (this.data_dicts.Count * 20)) - 20;
                            int x = this.mouse_location.X;
                            if ((x + 250) > (this.panel1.Width + this.panel1.HorizontalScroll.Value))
                            {
                                x = (this.mouse_location.X - this.data_tip_width) - 10;
                            }
                            Rectangle rectangle;
                            if (tooltipDataVisible && tooltipVisible)
                            {
                                rectangle = new Rectangle(x + 5, y - 5, this.data_tip_width, (this.data_dicts.Count * 20) + 10);
                                g.FillRectangle(brush, rectangle);
                                g.DrawRectangle(borderTooltipPen, rectangle);
                                foreach (KeyValuePair<string, CurveItem> pair in this.data_dicts)
                                {
                                    Rectangle layoutRectangle = new Rectangle(rectangle.X + 3, y, this.data_tip_width - 6, 20);
                                    g.DrawString(pair.Key, font, foreTooltipBrush, layoutRectangle, this.sf_left);
                                    if (index < pair.Value.Data.Length)
                                    {
                                        g.DrawString(string.Format(pair.Value.RenderFormat, pair.Value.Data[index]), font, foreTooltipBrush, layoutRectangle, this.sf_right);
                                    }
                                    y += 20;
                                }
                            }
                            y = this.mouse_location.Y + 25;
                            int num6 = 0;
                            while (true)
                            {
                                if (num6 < this.markForeActiveSections.Count)
                                {
                                    SizeF ef;
                                    if ((index < this.markForeActiveSections[num6].StartIndex) || (index > this.markForeActiveSections[num6].EndIndex))
                                    {
                                        num6++;
                                        continue;
                                    }
                                    float height = 0f;
                                    foreach (KeyValuePair<string, string> pair2 in this.markForeActiveSections[num6].CursorTexts)
                                    {
                                        ef = g.MeasureString(pair2.Key + " : " + pair2.Value, this.Font, 244);
                                        height += ef.Height + 8f;
                                    }
                                    rectangle = new Rectangle(x + 5, y - 5, 250, ((int)height) + 10);
                                    if (x < this.mouse_location.X)
                                    {
                                        rectangle.X = (this.mouse_location.X - 250) - 10;
                                    }
                                    g.FillRectangle(brush, rectangle);
                                    g.DrawRectangle(borderTooltipPen, rectangle);
                                    foreach (KeyValuePair<string, string> pair3 in this.markForeActiveSections[num6].CursorTexts)
                                    {
                                        ef = g.MeasureString(pair3.Key + " : " + pair3.Value, this.Font, 244);
                                        height = ef.Height;
                                        Rectangle layoutRectangle = new Rectangle(rectangle.X + 3, y, 244, 200);
                                        g.DrawString(pair3.Key + " : " + pair3.Value, font, Brushes.Yellow, layoutRectangle, this.sf_default);
                                        y += ((int)height) + 8;
                                    }
                                }
                                break;
                            }
                            break;
                        }
                        return;
                    }
                }
                break;
            }
            Rectangle rect = new Rectangle(this.mouse_location.X - 50, this.panel1.Height - 38, 100, 18);
            if (rect.X < this.panel1.HorizontalScroll.Value)
            {
                rect.X = this.panel1.HorizontalScroll.Value;
            }
            if (rect.X > ((this.panel1.HorizontalScroll.Value + this.panel1.Width) - 101))
            {
                rect.X = (this.panel1.HorizontalScroll.Value + this.panel1.Width) - 101;
            }
            if (this.isRenderTimeData && tooltipTimeVisible && tooltipVisible)
            {
                if (index < this.data_times.Count)
                {
                    g.FillRectangle(brush, rect);
                    g.DrawRectangle(borderTooltipPen, rect);
                    g.DrawString(this.data_times[index].ToString(this.mouse_hover_time_formate), this.Font, foreTooltipBrush, rect, this.sf);
                }
            }
            else if (index < this.data_customer.Count)
            {
                g.FillRectangle(brush, rect);
                g.DrawRectangle(borderTooltipPen, rect);
                g.DrawString(this.data_customer[index], this.Font, foreTooltipBrush, rect, this.sf);
            }
        TR_0001:
            font.Dispose();
            brush.Dispose();
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
            this.data_dicts.Clear();
            this.data_lists.Clear();
            this.markBackSections.Clear();
            this.markForeSections.Clear();
            this.markForeSectionsTmp.Clear();
            this.markForeActiveSections.Clear();
            this.markTexts.Clear();
            this.auxiliary_Labels.Clear();
            this.markLines.Clear();
            if (this.data_dicts.Count == 0)
            {
                this.data_times = new List<DateTime>(0);
            }
            this.CalculateCurveDataMax();
        }

        public void RemoveAllMarkBackSection()
        {
            this.markBackSections.Clear();
        }

        public void RemoveAllMarkForeSection()
        {
            this.markForeSections.Clear();
        }

        public void RemoveAllMarkLine()
        {
            this.markLines.Clear();
        }

        public void RemoveAllMarkMouseSection()
        {
            this.markForeSectionsTmp.Clear();
            this.m_RowBetweenStart = -1;
            this.m_RowBetweenEnd = -1;
            this.m_RowBetweenHeight = -1;
            this.m_RowBetweenStartHeight = -1;
            this.pictureBox3.Invalidate();
        }

        public void RemoveAllMarkText()
        {
            this.markTexts.Clear();
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
            if (this.data_dicts.ContainsKey(key))
            {
                this.data_lists.Remove(this.data_dicts[key]);
                this.data_dicts.Remove(key);
            }
            if (this.data_dicts.Count == 0)
            {
                this.data_times = new List<DateTime>(0);
                this.data_customer = new List<string>();
            }
            this.CalculateCurveDataMax();
        }

        public void RemoveMarkBackSection(MarkBackSection markBackSection)
        {
            this.markBackSections.Remove(markBackSection);
        }

        public void RemoveMarkForeSection(MarkForeSection markForeSection)
        {
            this.markForeSections.Remove(markForeSection);
        }

        public void RemoveMarkLine(MarkLine markLine)
        {
            this.markLines.Remove(markLine);
        }

        public void RemoveMarkText(MarkText markText)
        {
            this.markTexts.Remove(markText);
        }

        public void RenderCurveUI()
        {
            int width = ((int)(this.data_count * this.data_Scale_Render)) + 200;
            if (width < this.panel1.Width)
            {
                width = this.panel1.Width;
            }
            int height = this.panel1.Height - 18;
            if (height < 10)
            {
                height = 10;
            }
            Bitmap image = new Bitmap(width, height);
            Graphics g = Graphics.FromImage(image);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            g.Clear(this.backColor);
            this.GetRenderCurveMain(g, width, height);
            g.Dispose();
            Image image1 = this.pictureBox3.Image;
            if (image1 == null)
            {
                Image local1 = image1;
            }
            else
            {
                image1.Dispose();
            }
            this.pictureBox3.Image = image;
            base.Invalidate();
            this.isShowTextInfomation = false;
        }

        public Bitmap SaveToBitmap()
        {
            Bitmap bitmap2;
            if (this.isShowTextInfomation)
            {
                Bitmap image = new Bitmap(base.Width, base.Height);
                Graphics g = Graphics.FromImage(image);
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                g.Clear(this.backColor);
                this.PaintMain(g, (float)base.Width, (float)base.Height);
                g.TranslateTransform(43f, 29f);
                int num = (this.panel1.Width > 10) ? this.panel1.Width : 1;
                this.PaintFromString(g, this.Text, num, (this.panel1.Height > 10) ? this.panel1.Height : 10);
                g.TranslateTransform(-43f, -29f);
                g.Dispose();
                bitmap2 = image;
            }
            else
            {
                Bitmap image = new Bitmap(this.pictureBox3.Width + 86, base.Height);
                Graphics g = Graphics.FromImage(image);
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                g.Clear(this.backColor);
                this.DrawCoordinate(g, true, 3f, 29f, this.value_max_left, this.value_min_left, this.sf_right, this.value_unit_left);
                if (this.rightAxisVisible)
                {
                    this.DrawCoordinate(g, false, (float)(image.Width - 43), 29f, this.value_max_right, this.value_min_right, this.sf_left, this.value_unit_right);
                }
                this.PaintHeadText(g);
                int width = ((int)(this.data_count * this.data_Scale_Render)) + 200;
                if (width < this.panel1.Width)
                {
                    width = this.panel1.Width;
                }
                int num4 = this.panel1.Height - 18;
                if (num4 < 10)
                {
                    num4 = 10;
                }
                g.TranslateTransform(43f, 29f);
                this.GetRenderCurveMain(g, width, num4);
                g.TranslateTransform(-43f, -29f);
                g.Dispose();
                bitmap2 = image;
            }
            return bitmap2;
        }

        public void ScrollToRight()
        {
            this.panel1.HorizontalScroll.Value = this.panel1.HorizontalScroll.Maximum - 1;
            this.panel1.HorizontalScroll.Value = this.panel1.HorizontalScroll.Maximum - 1;
        }

        public void SetCurve(string key, bool isLeft, float[] data, Color lineColor, float thickness, bool isSmooth)
        {
            this.SetCurve(key, isLeft, data, lineColor, thickness, isSmooth, "{0}");
        }

        public void SetCurve(string key, bool isLeft, float[] data, Color lineColor, float thickness, bool isSmooth, string renderFormat)
        {
            if (this.data_dicts.ContainsKey(key))
            {
                if (data == null)
                {
                    data = new float[0];
                }
                this.data_dicts[key].Data = data;
            }
            else
            {
                if (data == null)
                {
                    data = new float[0];
                }
                CurveItem item1 = new CurveItem();
                item1.Data = data;
                item1.LineThickness = thickness;
                item1.LineColor = lineColor;
                item1.IsLeftFrame = isLeft;
                item1.IsSmoothCurve = isSmooth;
                item1.RenderFormat = renderFormat;
                item1.Visible = true;
                item1.LineRenderVisible = true;
                this.data_dicts.Add(key, item1);
                this.data_lists.Add(this.data_dicts[key]);
            }
            this.CalculateCurveDataMax();
        }

        public void SetCurveMousePosition(int x, int y)
        {
            if ((x < 0) || (y < 0))
            {
                this.is_mouse_on_picture = false;
            }
            else
            {
                this.is_mouse_on_picture = true;
            }
            this.mouse_location.X = x;
            this.mouse_location.Y = y;
            this.pictureBox3.Invalidate();
        }

        public void SetCurveVisible(string key, bool visible)
        {
            if (this.data_dicts.ContainsKey(key))
            {
                this.data_dicts[key].Visible = visible;
            }
        }

        public void SetCurveVisible(string[] keys, bool visible)
        {
            foreach (string str in keys)
            {
                if (this.data_dicts.ContainsKey(str))
                {
                    this.data_dicts[str].Visible = visible;
                }
            }
        }

        public void SetDateCustomer(string[] customers)
        {
            this.data_customer = new List<string>(customers);
            this.isRenderTimeData = false;
        }

        public void SetDateTimes(DateTime[] times)
        {
            this.data_times = new List<DateTime>(times);
            this.isRenderTimeData = true;
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

        public void SetLeftCurve(string key, float[] data, Color lineColor, bool isSmooth, string renderFormat)
        {
            this.SetCurve(key, true, data, lineColor, 1f, isSmooth, renderFormat);
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

        public void SetRightCurve(string key, float[] data, Color lineColor, bool isSmooth, string renderFormat)
        {
            this.SetCurve(key, false, data, lineColor, 1f, isSmooth, renderFormat);
        }

        public void SetScaleByXAxis(float scale)
        {
            this.data_Scale_Render = scale;
        }

        public void SetScrollPosition(ScrollEventArgs e)
        {
            this.panel1.HorizontalScroll.Value = e.NewValue;
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
        }

        protected override void OnPreviewKeyDown(PreviewKeyDownEventArgs e)
        {
            base.OnPreviewKeyDown(e);
            isPressedCtrl = e.Control;
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            isPressedCtrl = false;
            base.OnKeyUp(e);
        }

        #endregion

        #region Properties
        [Browsable(true), Category("EasyScada"), DefaultValue(typeof(Color), "[46, 46, 46]"), EditorBrowsable(EditorBrowsableState.Always)]
        public override Color BackColor
        {
            get
            {
                return base.BackColor;
            }
            set
            {
                this.backColor = value;
                base.BackColor = value;
                if (this.isShowTextInfomation)
                {
                    Image image = this.pictureBox3.Image;
                    if (image == null)
                    {
                        Image local1 = image;
                    }
                    else
                    {
                        image.Dispose();
                    }
                    this.pictureBox3.Image = this.GetBitmapFromString(this.Text);
                }
            }
        }

        [Browsable(true), Category("EasyScada"), Bindable(true), DefaultValue(typeof(Color), "Yellow"), EditorBrowsable(EditorBrowsableState.Always)]
        public override Color ForeColor
        {
            get
            {
                return base.ForeColor;
            }
            set
            {
                base.ForeColor = value;
                if (this.isShowTextInfomation)
                {
                    Image image = this.pictureBox3.Image;
                    if (image == null)
                    {
                        Image local1 = image;
                    }
                    else
                    {
                        image.Dispose();
                    }
                    this.pictureBox3.Image = this.GetBitmapFromString(this.Text);
                }
            }
        }

        [Browsable(true), Category("EasyScada"), DefaultValue(true)]
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

        [Category("EasyScada"), Browsable(true), DefaultValue((float)100f)]
        public float LeftAxisMaxValue
        {
            get
            {
                return this.value_max_left;
            }
            set
            {
                if (value > this.value_min_left)
                {
                    this.value_max_left = value;
                    base.Invalidate();
                }
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
                if (value < this.value_max_left)
                {
                    this.value_min_left = value;
                    base.Invalidate();
                }
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
                if (value > this.value_min_right)
                {
                    this.value_max_right = value;
                    base.Invalidate();
                }
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
                if (value < this.value_max_right)
                {
                    this.value_min_right = value;
                    base.Invalidate();
                }
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
                if (this.isShowTextInfomation)
                {
                    Image image = this.pictureBox3.Image;
                    if (image == null)
                    {
                        Image local1 = image;
                    }
                    else
                    {
                        image.Dispose();
                    }
                    this.pictureBox3.Image = this.GetBitmapFromString(this.Text);
                }
            }
        }

        [Browsable(true), Category("EasyScada"), DefaultValue(""), EditorBrowsable(EditorBrowsableState.Always), Bindable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public override string Text
        {
            get
            {
                return base.Text;
            }
            set
            {
                this.isShowTextInfomation = true;
                base.Text = value;
                if (this.isShowTextInfomation)
                {
                    Image image = this.pictureBox3.Image;
                    if (image == null)
                    {
                        Image local1 = image;
                    }
                    else
                    {
                        image.Dispose();
                    }
                    this.pictureBox3.Image = this.GetBitmapFromString(this.Text);
                }
            }
        }

        [Browsable(true), Category("EasyScada"), DefaultValue("")]
        public string LeftAxisUnit
        {
            get
            {
                return this.value_unit_left;
            }
            set
            {
                this.value_unit_left = value;
                base.Invalidate();
            }
        }

        [Browsable(true), Category("EasyScada"), DefaultValue("")]
        public string RightAxisUnit
        {
            get
            {
                return this.value_unit_right;
            }
            set
            {
                this.value_unit_right = value;
                base.Invalidate();
            }
        }

        [Browsable(true), Category("EasyScada"), DefaultValue(typeof(Color), "LightGray")]
        public Color AxisColor
        {
            get
            {
                return this.axisColor;
            }
            set
            {
                this.axisColor = value;
                this.coordinatePen.Dispose();
                this.coordinatePen = new Pen(this.axisColor);
                this.coordinateBrush.Dispose();
                this.coordinateBrush = new SolidBrush(this.axisColor);
                base.Invalidate();
                if (this.isShowTextInfomation)
                {
                    Image image = this.pictureBox3.Image;
                    if (image == null)
                    {
                        Image local1 = image;
                    }
                    else
                    {
                        image.Dispose();
                    }
                    this.pictureBox3.Image = this.GetBitmapFromString(this.Text);
                }
            }
        }

        [Browsable(true), Category("EasyScada"), DefaultValue(typeof(Color), "[72, 72, 72]")]
        public Color GridLineColor
        {
            get
            {
                return this.gridLineColor;
            }
            set
            {
                this.gridLineColor = value;
                this.coordinateDashPen.Dispose();
                this.coordinateDashPen = new Pen(this.gridLineColor);
                this.coordinateDashPen.DashPattern = new float[] { 5f, 5f };
                this.coordinateDashPen.DashStyle = DashStyle.Custom;
                base.Invalidate();
                if (this.isShowTextInfomation)
                {
                    Image image = this.pictureBox3.Image;
                    if (image == null)
                    {
                        Image local1 = image;
                    }
                    else
                    {
                        image.Dispose();
                    }
                    this.pictureBox3.Image = this.GetBitmapFromString(this.Text);
                }
            }
        }

        [Browsable(true), Category("EasyScada"), DefaultValue(typeof(Color), "Cyan")]
        public Color MeasureLineColor
        {
            get
            {
                return this.markLineColor;
            }
            set
            {
                this.markLineColor = value;
                this.markLinePen.Dispose();
                this.markLinePen = new Pen(this.markLineColor);
            }
        }

        [Browsable(true), Category("EasyScada"), DefaultValue(typeof(Color), "Cyan")]
        public Color MeasureTextColor
        {
            get
            {
                return this.markTextColor;
            }
            set
            {
                this.markTextColor = value;
                this.markTextBrush.Dispose();
                this.markTextBrush = new SolidBrush(this.markTextColor);
            }
        }

        [Browsable(true), Category("EasyScada"), DefaultValue(150)]
        public int DataTipWidth
        {
            get
            {
                return this.data_tip_width;
            }
            set
            {
                if ((value > 20) && (value < 500))
                {
                    this.data_tip_width = value;
                }
            }
        }

        [Browsable(true), Category("EasyScada"), DefaultValue(150)]
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

        [Category("EasyScada"), Browsable(true), DefaultValue(200)]
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
                if (this.isShowTextInfomation)
                {
                    Image image = this.pictureBox3.Image;
                    if (image == null)
                    {
                        Image local1 = image;
                    }
                    else
                    {
                        image.Dispose();
                    }
                    this.pictureBox3.Image = this.GetBitmapFromString(this.Text);
                }
            }
        }

        [Category("EasyScada"), Browsable(true), DefaultValue("yyyy-MM-dd HH:mm:ss")]
        public string AxisTimeFormat
        {
            get
            {
                return this.data_time_formate;
            }
            set
            {
                this.data_time_formate = value;
                base.Invalidate();
                if (this.isShowTextInfomation)
                {
                    Image image = this.pictureBox3.Image;
                    if (image == null)
                    {
                        Image local1 = image;
                    }
                    else
                    {
                        image.Dispose();
                    }
                    this.pictureBox3.Image = this.GetBitmapFromString(this.Text);
                }
            }
        }

        [Category("EasyScada"), Browsable(true), DefaultValue("dd HH:mm:ss")]
        public string TooltipTimeFormat
        {
            get
            {
                return this.mouse_hover_time_formate;
            }
            set
            {
                this.mouse_hover_time_formate = value;
            }
        }

        [Browsable(true), Category("EasyScada"), EditorBrowsable(EditorBrowsableState.Always)]
        public bool TooltipVisible
        {
            get => tooltipVisible;
            set => tooltipVisible = value;
        }

        [Browsable(true), Category("EasyScada"), EditorBrowsable(EditorBrowsableState.Always)]
        public bool TooltipDataVisible
        {
            get => tooltipDataVisible;
            set => tooltipDataVisible = value;
        }

        [Browsable(true), Category("EasyScada"), EditorBrowsable(EditorBrowsableState.Always)]
        public bool TooltipTimeVisible
        {
            get => tooltipTimeVisible;
            set => tooltipTimeVisible = value;
        }

        [Browsable(true), Category("EasyScada"), EditorBrowsable(EditorBrowsableState.Always)]
        public bool TooltipVerticalLineVisible
        {
            get => verticalLineTooltipVisible;
            set => verticalLineTooltipVisible = value;
        }

        [Browsable(true), Category("EasyScada"), EditorBrowsable(EditorBrowsableState.Always)]
        public bool TooltipHorizontalLineVisible
        {
            get => horizontalLineTooltipVisible;
            set => horizontalLineTooltipVisible = value;
        }

        [Browsable(true), Category("EasyScada"), DefaultValue(typeof(Color), "White")]
        public Color TooltipLineColor
        {
            get
            {
                return this.moveLineColor;
            }
            set
            {
                if (value == null)
                    return;

                this.moveLineColor = value;
                this.moveLinePen.Dispose();
                this.moveLinePen = new Pen(this.moveLineColor);
            }
        }

        [Browsable(true), Category("EasyScada"), EditorBrowsable(EditorBrowsableState.Always)]
        public Color TooltipForeColor
        {
            get => foreTooltipColor;
            set
            {
                if (value != null)
                {
                    foreTooltipColor = value;
                    foreTooltipBrush.Dispose();
                    foreTooltipBrush = new SolidBrush(foreTooltipColor);
                }
            }
        }

        [Browsable(true), Category("EasyScada"), EditorBrowsable(EditorBrowsableState.Always)]
        public Color TooltipBorderColor
        {
            get => borderTooltipColor;
            set
            {
                if (value != null)
                {
                    borderTooltipPen.Dispose();
                    borderTooltipColor = value;
                    borderTooltipPen = new Pen(borderTooltipColor);
                }
            }
        }

        [Category("EasyScada"), Browsable(true), DefaultValue(true)]
        public bool AllowMeasure
        {
            get
            {
                return this.isAllowSelectSection;
            }
            set
            {
                this.isAllowSelectSection = value;
            }
        }

        [Category("EasyScada"), Browsable(true), DefaultValue(false)]
        public bool RoundAxisValue
        {
            get
            {
                return this.isAoordinateRoundInt;
            }
            set
            {
                this.isAoordinateRoundInt = value;
                base.Invalidate();
                if (this.isShowTextInfomation)
                {
                    Image image = this.pictureBox3.Image;
                    if (image == null)
                    {
                        Image local1 = image;
                    }
                    else
                    {
                        image.Dispose();
                    }
                    this.pictureBox3.Image = this.GetBitmapFromString(this.Text);
                }
            }
        }

        #endregion
    }

    public delegate void CurveCustomerDoubleClick(HistoricalTrend chart, int index, string customer);

    public delegate void CurveDoubleClick(HistoricalTrend chart, int index, DateTime dateTime);

    public delegate void CurveMouseMove(HistoricalTrend chart, int x, int y);

    public delegate void CurveRangeSelect(HistoricalTrend chart, int index, int end);
}
