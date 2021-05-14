using EasyScada.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EasyScada.Winforms.Controls
{
    [ToolboxItem(true)]
    [Designer(typeof(EasyBarDesigner))]
    public class EasyBar : UserControl, ISupportConnector, ISupportTag, ISupportInitialize
    {
        #region Constructors
        public EasyBar() : base()
        {
            FillColor = Color.Green;
        }
        #endregion

        #region Public properties

        Color _fillColor;
        [Browsable(true), Category(DesignerCategory.EASYSCADA)]
        public Color FillColor
        {
            get => _fillColor;
            set
            {
                if (_fillColor != value)
                {
                    _fillColor = value;
                    Invalidate();
                }
            }
        }

        BarDirection _direction = BarDirection.BottomToTop;
        [Browsable(true), Category(DesignerCategory.EASYSCADA)]
        public BarDirection Direction
        {
            get => _direction;
            set
            {
                if (_direction != value)
                {
                    _direction = value;
                    Invalidate();
                }
            }
        }

        [Browsable(true), Category(DesignerCategory.EASYSCADA)]
        public string ValueStringFormat { get; set; }

        BarDisplayMode _displayMode = BarDisplayMode.None;
        [Browsable(true), Category(DesignerCategory.EASYSCADA)]
        public BarDisplayMode DisplayMode
        {
            get => _displayMode;
            set
            {
                if (_displayMode != value)
                {
                    _displayMode = value;
                    Invalidate();
                }
            }
        }

        string _maxValue;
        [Editor(typeof(PathToTagPropertyEditor), typeof(UITypeEditor))]
        [Description("Select path to tag for control")]
        [Browsable(true), Category(DesignerCategory.EASYSCADA)]
        public string MaxValue
        {
            get => _maxValue;
            set
            {
                if (_maxValue != value)
                {
                    _maxValue = value;
                    Invalidate();
                }
            }
        }

        string _minValue;
        [Editor(typeof(PathToTagPropertyEditor), typeof(UITypeEditor))]
        [Description("Select path to tag for control")]
        [Browsable(true), Category(DesignerCategory.EASYSCADA)]
        public string MinValue
        {
            get => _minValue;
            set
            {
                if (_minValue != value)
                {
                    _minValue = value;
                    Invalidate();
                }
            }
        }
        #endregion

        #region ISupportConnector
        [Description("Select driver connector for control")]
        [Browsable(false), Category(DesignerCategory.EASYSCADA)]
        public IEasyDriverConnector Connector => EasyDriverConnectorProvider.GetEasyDriverConnector();
        #endregion

        #region ISupportTag
        [Editor(typeof(PathToTagPropertyEditor), typeof(UITypeEditor))]
        [Description("Select path to tag for control")]
        [Browsable(true), Category(DesignerCategory.EASYSCADA)]
        public string TagPath { get; set; }

        ITag linkedTag;
        [Browsable(false)]
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

        ITag _maxTag;
        [Browsable(false)]
        public ITag MaxTag
        {
            get
            {
                if (_maxTag == null)
                {
                    _maxTag = Connector?.GetTag(MaxValue);
                }
                else
                {
                    if (_maxTag.Path != MaxValue)
                        _maxTag = Connector?.GetTag(MaxValue);
                }
                return _maxTag;
            }
        }

        ITag _minTag;
        [Browsable(false)]
        public ITag MinTag
        {
            get
            {
                if (_minTag == null)
                {
                    _minTag = Connector?.GetTag(MinValue);
                }
                else
                {
                    if (_minTag.Path != MaxValue)
                        _minTag = Connector?.GetTag(MinValue);
                }
                return _minTag;
            }
        }

        protected override Size DefaultSize => new Size(50, 150);
        #endregion

        #region ISupportInitialize
        public void BeginInit()
        {
        }

        public void EndInit()
        {
            if (!DesignMode)
            {
                if (Connector.IsStarted)
                    OnConnectorStarted(null, EventArgs.Empty);
                else
                    Connector.Started += OnConnectorStarted;
            }
        }
        #endregion

        #region Event handlers
        private void OnConnectorStarted(object sender, EventArgs e)
        {
            if (LinkedTag != null)
            {
                OnTagValueChanged(LinkedTag, new TagValueChangedEventArgs(LinkedTag, null, LinkedTag.Value));
                LinkedTag.ValueChanged += OnTagValueChanged;
                LinkedTag.QualityChanged += OnTagQualityChanged;
                
                if (MinTag != null)
                    MinTag.ValueChanged += OnTagValueChanged;
                if (MaxTag != null)
                    MaxTag.ValueChanged += OnTagValueChanged;
            }
        }

        private void OnTagQualityChanged(object sender, TagQualityChangedEventArgs e)
        {
            if (e.NewQuality == Quality.Good)
            {
                this.SetInvoke((x) =>
                {
                    x.Invalidate();
                });
            }
        }

        private void OnTagValueChanged(object sender, TagValueChangedEventArgs e)
        {
            this.SetInvoke((x) =>
            {
                x.Invalidate();
            });
        }

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            base.OnPaintBackground(pevent);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (LinkedTag != null)
            {
                if (double.TryParse(LinkedTag.Value, out double value))
                {
                    double maxValue = GetMaxValue();
                    double minValue = GetMinValue();

                    if (minValue < maxValue && value > minValue)
                    {
                        float percent = (float)((value - minValue) / (maxValue - minValue));
                        using (SolidBrush brush = new SolidBrush(_fillColor))
                        {
                            float width = percent * this.Width;
                            float height = percent * this.Height;
                            switch (_direction)
                            {
                                case BarDirection.LeftToRight:
                                    e.Graphics.FillRectangle(brush, 0, 0, width, Height);
                                    break;
                                case BarDirection.BottomToTop:
                                    e.Graphics.FillRectangle(brush, 0, Height - height, Width, height);
                                    break;
                                case BarDirection.RightToLeft:
                                    e.Graphics.FillRectangle(brush, Width - width, 0, width, Height);
                                    break;
                                case BarDirection.TopToBottom:
                                    e.Graphics.FillRectangle(brush, 0, 0, Width, height);
                                    break;
                                default:
                                    break;
                            }

                            if (_displayMode == BarDisplayMode.Value)
                            {
                                using (SolidBrush textBrush = new SolidBrush(ForeColor))
                                {
                                    string valueStr = value.ToString(ValueStringFormat);
                                    SizeF valueSize = e.Graphics.MeasureString(valueStr, Font);
                                    float x = (Width - valueSize.Width) / 2.0f;
                                    float y = (Height - valueSize.Height) / 2.0f;
                                    e.Graphics.DrawString(valueStr, Font, textBrush, x, y);
                                }
                            }
                            else if (_displayMode == BarDisplayMode.Percent)
                            {
                                using (SolidBrush textBrush = new SolidBrush(ForeColor))
                                {  
                                    string valueStr = $"{(percent * 100).ToString(ValueStringFormat)} %";
                                    SizeF valueSize = e.Graphics.MeasureString(valueStr, Font);
                                    float x = (Width - valueSize.Width) / 2.0f;
                                    float y = (Height - valueSize.Height) / 2.0f;
                                    e.Graphics.DrawString(valueStr, Font, textBrush, x, y);
                                }
                            }
                        }
                    }
                }
            }
        }
        #endregion

        #region Methods

        private double GetMaxValue()
        {
            if (MaxTag == null)
            {
                if (double.TryParse(MaxValue, out double maxValue))
                    return maxValue;
            }
            else
            {
                if (double.TryParse(MaxTag.Value, out double maxValue))
                    return maxValue;
            }
            return 0;
        }

        private double GetMinValue()
        {
            if (MinTag == null)
            {
                if (double.TryParse(MinValue, out double minValue))
                    return minValue;
            }
            else
            {
                if (double.TryParse(MinTag.Value, out double minValue))
                    return minValue;
            }
            return 0;
        }
        #endregion

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // EasyBar
            // 
            this.Name = "EasyBar";
            this.Size = new System.Drawing.Size(67, 275);
            this.ResumeLayout(false);

        }
    }
}
