using System;
using System.Text;
using System.Data;
using System.Drawing;
using System.Drawing.Design;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using EasyScada.Winforms.Connector;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Collections;
using System.Linq;

namespace EasyScada.Winforms.Controls
{
    [ToolboxItem(true)]
    //[DefaultEvent("Paint")]
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    [Designer(typeof(EasyPictureBoxDesigner))]
    [DesignerCategory("code")]
    [Description("Displays images")]
    [ClassInterface(ClassInterfaceType.AutoDispatch)]
    [ComVisible(true)]
    public class EasyPictureBox : Control
    {
        #region CACHE
        protected static Hashtable SHADED_IMAGE_CACHE = new Hashtable();
        protected static Hashtable IMAGE_CACHE = new Hashtable();
        #endregion

        #region Private members
        private string _keyCache;
        private int _rotateAngle;
        private PictureBoxSizeMode _sizeMode;
        private ImageFillMode _fillMode;
        private ImageFlipMode _flipMode;
        private Color _shadedColor = Color.Gray;

        #endregion

        #region Public members
        public Image Image
        {
            get
            {;
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
                    _keyCache = string.Join("", GetHash((Bitmap)value).Select(b => b ? "1" : "0").ToArray());
                    if (!IMAGE_CACHE.ContainsKey(_keyCache))
                        IMAGE_CACHE[_keyCache] = value;
                }
                Invalidate();
            }
        }

        public int RotateAngle
        {
            get { return _rotateAngle; }
            set
            {
                if (_rotateAngle != value)
                {
                    _rotateAngle = value;
                    Invalidate();
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
                    Invalidate();
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
                    Invalidate();
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
                    Invalidate();
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
                    Invalidate();
                }
            }
        }

        #endregion

        #region Constructors
        public EasyPictureBox()
        {
           
        }
        #endregion

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

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.Clear(BackColor);

            if (Image != null)
            {
                Bitmap drawBM;
                if (FillMode == ImageFillMode.Shaded)
                {
                    if (!SHADED_IMAGE_CACHE.ContainsKey(_keyCache))
                        SHADED_IMAGE_CACHE[_keyCache] = new Hashtable();
                    if (!(SHADED_IMAGE_CACHE[_keyCache] as Hashtable).ContainsKey(ShadedColor))
                        (SHADED_IMAGE_CACHE[_keyCache] as Hashtable)[ShadedColor] = CommonHelper.BitmapColorShade((Bitmap)Image, ShadedColor);
                    drawBM = (SHADED_IMAGE_CACHE[_keyCache] as Hashtable)[ShadedColor] as Bitmap;
                }
                else
                {
                    drawBM = (Bitmap)Image;
                }

                int width = e.ClipRectangle.Width - Padding.Left - Padding.Right;
                int height = e.ClipRectangle.Height - Padding.Top - Padding.Bottom;

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
                            newBM.Dispose();
                            break;
                        case ImageFlipMode.Vertical:
                            newBM = new Bitmap(drawBM);
                            newBM.RotateFlip(RotateFlipType.RotateNoneFlipY);
                            e.Graphics.DrawImage(newBM, destination);
                            newBM.Dispose(); break;
                        case ImageFlipMode.Both:
                            newBM = new Bitmap(drawBM);
                            newBM.RotateFlip(RotateFlipType.RotateNoneFlipXY);
                            e.Graphics.DrawImage(newBM, destination);
                            newBM.Dispose();
                            break;
                        default:
                            e.Graphics.DrawImage(drawBM, destination);
                            break;
                    }
                    
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
                            newBM.Dispose();
                            break;
                        case ImageFlipMode.Vertical:
                            newBM = new Bitmap(drawBM);
                            newBM.RotateFlip(RotateFlipType.RotateNoneFlipY);
                            e.Graphics.DrawImage(newBM, destination);
                            newBM.Dispose();
                            break;
                        case ImageFlipMode.Both:
                            newBM = new Bitmap(drawBM);
                            newBM.RotateFlip(RotateFlipType.RotateNoneFlipXY);
                            e.Graphics.DrawImage(newBM, destination);
                            newBM.Dispose();
                            break;
                        default:
                            e.Graphics.DrawImage(drawBM, destination);
                            break;
                    }
                }
            }
        }

        protected override void OnPaddingChanged(EventArgs e)
        {
            base.OnPaddingChanged(e);
            Invalidate();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            Invalidate();
        }
    }
    #endregion
}


