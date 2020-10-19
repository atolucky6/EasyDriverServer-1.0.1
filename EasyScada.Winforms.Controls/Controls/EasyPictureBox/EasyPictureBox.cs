using EasyScada.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Windows.Forms;

namespace EasyScada.Winforms.Controls
{
    [ToolboxItem(true)]
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    [Designer(typeof(EasyPictureBoxDesigner))]
    [DesignerCategory("code")]
    [Description("Displays images")]
    [ClassInterface(ClassInterfaceType.AutoDispatch)]
    public class EasyPictureBox : Control, ISupportTag, ISupportConnector, ISupportInitialize, IEditableObject, IChangeTracking
    {
        #region CACHE
        protected static Dictionary<string, Dictionary<Color, Bitmap>> SHADED_IMAGE_CACHE = new Dictionary<string, Dictionary<Color, Bitmap>>();
        protected static Dictionary<string, Image> IMAGE_CACHE = new Dictionary<string, Image>();
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
        #endregion

        #region ISupportConnector
        [Description("Select driver connector for control")]
        [Browsable(true), Category(DesignerCategory.EASYSCADA)]
        public IEasyDriverConnector Connector => EasyDriverConnectorProvider.GetEasyDriverConnector();

        private void OnConnectorStarted(object sender, EventArgs e)
        {
            triggers.Start();
        }

        #endregion

        #region Private members
        private bool isEditing;
        private string _keyCache;
        private int _rotateAngle;
        private PictureBoxSizeMode _sizeMode;
        private ImageFillMode _fillMode;
        private ImageFlipMode _flipMode;
        private Color _shadedColor = Color.Gray;
        private Image defaultImage;
        private Image currentImage;
        private TriggerCollection<ImageAnimatePropertyWrapper> triggers = new TriggerCollection<ImageAnimatePropertyWrapper>();
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
            Invalidate();
        }
        #endregion

        #region IChangeTracking
        public bool IsChanged { get; protected set; }

        public void AcceptChanges()
        {
            IsChanged = false;
        }
        #endregion

        #region IEditableObject
        public void BeginEdit()
        {
            isEditing = true;
        }

        public void EndEdit()
        {
            isEditing = false;
            if (IsChanged)
            {
                Invoke(new Action(() =>
                {
                    RecreateHandle();
                }));
                AcceptChanges();
            }
        }

        public void CancelEdit()
        {
            EndEdit();
        }
        #endregion

        #region Public members

        [Category(DesignerCategory.EASYSCADA)]
        [Description("Collection of write tag command specifications.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public TriggerCollection<ImageAnimatePropertyWrapper> Triggers { get => triggers; }

        public Image Image
        {
            get
            {
                if (_keyCache != null)
                {
                    if (IMAGE_CACHE.ContainsKey(_keyCache))
                        return IMAGE_CACHE[_keyCache] as Image;
                }
                return null;
            }
            set
            {
                if (value == null)
                {
                    _keyCache = null;
                }
                else
                {
                    if (defaultImage != null)
                        defaultImage = value;

                    if (currentImage != value)
                    {  
                        currentImage = value;
                        _keyCache = string.Join("", GetHash((Bitmap)value).Select(b => b ? "1" : "0").ToArray());
                        if (!IMAGE_CACHE.ContainsKey(_keyCache))
                        {
                            IMAGE_CACHE[_keyCache] = value;
                        }


                        if (!isEditing)
                        {
                            RecreateHandle();
                        }
                        IsChanged = true;
                    }
                }
            }
        }

        protected override Size DefaultSize => new Size(100, 100);

        public int RotateAngle
        {
            get { return _rotateAngle; }
            set
            {
                if (_rotateAngle != value)
                {
                    _rotateAngle = value;

                    if (!isEditing)
                    {
                        RecreateHandle();
                    }
                    IsChanged = true;
                }
            }
        }

        public PictureBoxSizeMode SizeMode
        {
            get { return _sizeMode; }
            set
            {
                if (_sizeMode != value)
                {
                    _sizeMode = value;

                    if (!isEditing)
                    {
                        RecreateHandle();
                    }
                    IsChanged = true;
                }
            }
        }

        public ImageFillMode FillMode
        {
            get { return _fillMode; }
            set
            {
                if (_fillMode != value)
                {
                    _fillMode = value;

                    if (!isEditing)
                    {
                        RecreateHandle();
                    }
                    IsChanged = true;
                }
            }
        }

        public ImageFlipMode FlipMode
        {
            get { return _flipMode; }
            set
            {
                if (_flipMode != value)
                {
                    _flipMode = value;

                    if (!isEditing)
                    {
                        RecreateHandle();
                    }
                    IsChanged = true;
                }
            }
        }

        public Color ShadedColor
        {
            get { return _shadedColor; }
            set
            {
                if (_shadedColor != value)
                {
                    _shadedColor = value;


                    if (!isEditing)
                    {
                        RecreateHandle();
                    }
                    IsChanged = true;
                }
            }
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x20;
                return cp;
            }
        }

        #endregion

        #region Constructors
        public EasyPictureBox()
        { 
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, false);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.Selectable, true);
            BackColor = Color.Transparent;
            triggers.TargetControl = this;

            refresher = new Timer();
            refresher.Interval = 100;
            refresher.Tick += Refresher_Tick;
            refresher.Start();
        }

        private void Refresher_Tick(object sender, EventArgs e)
        {
            RecreateHandle();
            refresher.Stop();
        }
        #endregion
        private Timer refresher;
        #region Protected

        protected List<bool> GetHash(Bitmap bmpSource)
        {
            List<bool> lResult = new List<bool>();
            Bitmap bmpMin = new Bitmap(bmpSource, new Size(16, 16));
            for (int j = 0; j < bmpMin.Height; j++)
            {
                for (int i = 0; i < bmpMin.Width; i++)
                {        
                    lResult.Add(bmpMin.GetPixel(i, j).GetBrightness() < 0.5f);
                }
            }
            return lResult;
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            // Paint background with underlying graphics from other controls
            //Graphics g = e.Graphics;

            //if (Parent != null)
            //{
            //    // Take each control in turn
            //    int index = Parent.Controls.GetChildIndex(this);
            //    for (int i = Parent.Controls.Count - 1; i >= index + 1; i += -1)
            //    {
            //        if (i < index)
            //        {
            //            Control c = Parent.Controls[i];
            //            // Check it's visible and overlaps this control
            //            if (c.Bounds.IntersectsWith(Bounds) && c.Visible)
            //            {
            //                // Load appearance of underlying control and redraw it on this background
            //                Bitmap bmp = new Bitmap(c.Width, c.Height, g);
            //                c.DrawToBitmap(bmp, c.ClientRectangle);
            //                g.TranslateTransform(c.Left - Left, c.Top - Top);
            //                g.DrawImageUnscaled(bmp, Point.Empty);
            //                g.TranslateTransform(Left - c.Left, Top - c.Top);
            //                bmp.Dispose();
            //            }
            //        }
            //    }
            //}
            e.Graphics.FillRectangle(Brushes.Transparent, DisplayRectangle);
        }

        protected override void OnMove(EventArgs e)
        {
            RecreateHandle();
        }

        public void Redraw()
        {
            RecreateHandle();
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            Bitmap bm = new Bitmap(Width, Height);
            this.DrawToBitmap(bm, DisplayRectangle);
            Color pixel = bm.GetPixel(e.X, e.Y);
            if (pixel.A != 0)
            {
                base.OnMouseClick(e);
            }
            bm.Dispose();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (Image != null)
            {
                Bitmap drawBM;
                if (FillMode == ImageFillMode.Shaded)
                {
                    if (!SHADED_IMAGE_CACHE.ContainsKey(_keyCache))
                        SHADED_IMAGE_CACHE[_keyCache] = new Dictionary<Color, Bitmap>();

                    if (!SHADED_IMAGE_CACHE[_keyCache].ContainsKey(ShadedColor))
                        SHADED_IMAGE_CACHE[_keyCache][ShadedColor] = EasyScadaHelper.BitmapColorShade((Bitmap)Image, ShadedColor);

                    drawBM = SHADED_IMAGE_CACHE[_keyCache][ShadedColor] as Bitmap;
                }
                else
                {
                    drawBM = (Bitmap)Image;
                }

                int width = Width - Padding.Left - Padding.Right;
                int height = Height - Padding.Top - Padding.Bottom;

                if (width <= 0 || height <= 0)
                    return;

                if (RotateAngle != 0)
                {
                    int newWidth = 0;
                    int newHeight = 0;
                    int angle = RotateAngle % 360;
                    angle = angle < 0 ? 360 + angle : angle;

                    double radian = angle * Math.PI / 180d;

                    double cos = Math.Abs(Math.Cos(radian));
                    double sin = Math.Abs(Math.Sin(radian));
                    newWidth = (int)Math.Round(width * cos + height * sin);
                    newHeight = (int)Math.Round(width * sin + height * cos);
                    e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    e.Graphics.SmoothingMode = SmoothingMode.HighQuality;

                    e.Graphics.TranslateTransform(Width / 2f, Height / 2f);
                    e.Graphics.RotateTransform(angle);
                    e.Graphics.TranslateTransform(-Width / 2f, -Height / 2f);
                    
                    Rectangle destination = new Rectangle(Padding.Left, Padding.Top, width, height);
                    Bitmap newBM;
                    switch (FlipMode)
                    {
                        case ImageFlipMode.Horizontal:
                            newBM = new Bitmap(drawBM);
                            newBM.RotateFlip(RotateFlipType.RotateNoneFlipX);
                            e.Graphics.DrawImage(newBM, destination);
                            break;
                        case ImageFlipMode.Vertical:
                            newBM = new Bitmap(drawBM);
                            newBM.RotateFlip(RotateFlipType.RotateNoneFlipY);
                            e.Graphics.DrawImage(newBM, destination);
                            break;
                        case ImageFlipMode.Both:
                            newBM = new Bitmap(drawBM);
                            newBM.RotateFlip(RotateFlipType.RotateNoneFlipXY);
                            e.Graphics.DrawImage(newBM, destination);
                            break;
                        default:
                            e.Graphics.DrawImage(drawBM, destination);
                            currentImage = drawBM;
                            return;
                    }
                    currentImage = newBM;
                    return;
                }
                else
                {
                    Bitmap newBM;
                    // Draw the result 
                    Rectangle destination = new Rectangle(Padding.Left, Padding.Top, width, height);

                    switch (FlipMode)
                    {
                        case ImageFlipMode.Horizontal:
                            newBM = new Bitmap(drawBM);
                            newBM.RotateFlip(RotateFlipType.RotateNoneFlipX);
                            e.Graphics.DrawImage(newBM, destination);
                            break;
                        case ImageFlipMode.Vertical:
                            newBM = new Bitmap(drawBM);
                            newBM.RotateFlip(RotateFlipType.RotateNoneFlipY);
                            e.Graphics.DrawImage(newBM, destination);
                            break;
                        case ImageFlipMode.Both:
                            newBM = new Bitmap(drawBM);
                            newBM.RotateFlip(RotateFlipType.RotateNoneFlipXY);
                            e.Graphics.DrawImage(newBM, destination);
                            break;
                        default:
                            e.Graphics.DrawImage(drawBM, destination);
                            currentImage = drawBM;
                            return;
                    }
                    currentImage = newBM;
                    return;
                }
            }
        }

        protected override void OnPaddingChanged(EventArgs e)
        {
            base.OnPaddingChanged(e);
            RecreateHandle();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
        }
        #endregion
    }
}


