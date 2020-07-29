using System;
using System.Text;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Collections.Generic;

namespace EasyScada.Winforms.Controls
{
    public class ImageValues : Storage, IContentValues
    {
        #region Static Fields
        private const string _defaultText = "";
        private static readonly string _defaultExtraText = string.Empty;
        #endregion

        #region Instance Fields
        private Control _parent;
        private Image _drawImage;
        private Image _image;
        private Color _transparent;
        private string _text;
        private string _extraText;
        private PictureBoxSizeMode _sizeMode;
        private ImageFillMode _imageFillMode;
        private Color _shadedColor;
        private int _rotateAngle;
        #endregion

        #region Events
        /// <summary>
        /// Occures when the value of the Text property changes.
        /// </summary>
        public event EventHandler TextChanged;

        /// <summary>
        /// Occures when the value of the SizeMode property changes.
        /// </summary>
        public event EventHandler SizeModeChanged;
        #endregion

        #region Identity
        /// <summary>
		/// Initialize a new instance of the LabelValues class.
		/// </summary>
        /// <param name="needPaint">Delegate for notifying paint requests.</param>
        public ImageValues(Control parent, NeedPaintHandler needPaint)
        {
            _parent = parent;
            // Store the provided paint notification delegate
            NeedPaint = needPaint;

            // Set initial values
            _image = null;
            _transparent = Color.Empty;
            _text = _defaultText;
            _extraText = _defaultExtraText;
        }
        #endregion

        #region IsDefault
        /// <summary>
        /// Gets a value indicating if all values are default.
        /// </summary>
        [Browsable(false)]
        public override bool IsDefault
        {
            get
            {
                return ((Image == null) &&
                        (ImageTransparentColor == Color.Empty) &&
                        (Text == _defaultText) &&
                        (ExtraText == _defaultExtraText));
            }
        }
        #endregion

        #region SizeMode
        public PictureBoxSizeMode SizeMode
        {
            get { return _sizeMode; }
            set
            {
                if (_sizeMode != value)
                {
                    _sizeMode = value;
                    SizeModeChanged?.Invoke(this, EventArgs.Empty);
                    PerformNeedPaint(true);
                }
            }
        }
        #endregion

        #region ImageFillMode
        public ImageFillMode ImageFillMode
        {
            get { return _imageFillMode; }
            set
            {
                if (_imageFillMode != value)
                {
                    _imageFillMode = value;
                    PerformNeedPaint(true);
                }
            }
        }
        #endregion

        #region ShadedColor
        public Color ShadedColor
        {
            get { return _shadedColor; }
            set
            {
                if (_shadedColor != value)
                {
                    _shadedColor = value;
                    if (_imageFillMode == ImageFillMode.Shaded)
                        PerformNeedPaint(true);
                }
            }
        }
        #endregion

        #region RotateAngle
        public int RotateAngle
        {
            get { return _rotateAngle; }
            set
            {
                if (_rotateAngle != value)
                {
                    _rotateAngle = value;
                    PerformNeedPaint(true);
                }
            }
        }
        #endregion

        #region Image
        /// <summary>
        /// Gets and sets the label image.
        /// </summary>
        [Localizable(true)]
        [Category("Visuals")]
        [Description("Image of picture box")]
        [RefreshPropertiesAttribute(RefreshProperties.All)]
        public Image Image
        {
            get { return _image; }
            
            set
            {
                if (_image != value)
                {
                    _image = value;
                    PerformNeedPaint(true);
                }
            }
        }

        private bool ShouldSerializeImage()
        {
            return Image != null;
        }

        /// <summary>
        /// Resets the Image property to its default value.
        /// </summary>
        public void ResetImage()
        {
            Image = null;
        }

        /// <summary>
        /// Gets the content image.
        /// </summary>
        /// <param name="state">The state for which the image is needed.</param>
        /// <returns>Image value.</returns>
        public Image GetImage(PaletteState state)
        {
            if (Image != null)
            {

                switch (SizeMode)
                {
                    case PictureBoxSizeMode.Normal:
                        _drawImage = Image;
                        break;
                    case PictureBoxSizeMode.StretchImage:
                        _drawImage = CommonHelper.ResizeImage(
                            Image,
                            _parent.Width - +_parent.Padding.Top - _parent.Padding.Bottom,
                            _parent.Height - _parent.Padding.Left - _parent.Padding.Right);
                        break;
                    case PictureBoxSizeMode.AutoSize:
                        _drawImage = Image;
                        _parent.Width = Image.Width + _parent.Padding.Top + _parent.Padding.Bottom;
                        _parent.Height = Image.Height + _parent.Padding.Left + _parent.Padding.Right;
                        break;
                    case PictureBoxSizeMode.CenterImage:
                        _drawImage = Image;
                        break;
                    case PictureBoxSizeMode.Zoom:
                        double zoomFactor = (double)(_parent.Width - +_parent.Padding.Top - _parent.Padding.Bottom) / (double)Image.Width;
                        int height = (int)(Image.Height * zoomFactor);
                        int width = (int)(Image.Width * zoomFactor);
                        if (height > _parent.Height - _parent.Padding.Left - _parent.Padding.Right)
                        {
                            zoomFactor = (double)(_parent.Height - _parent.Padding.Left - _parent.Padding.Right) / (double)Image.Height;
                            height = (int)(Image.Height * zoomFactor);
                            width = (int)(Image.Width * zoomFactor);
                        }
                        _drawImage = CommonHelper.ResizeImage(
                            Image,
                            width,
                            height);
                        break;
                    default:
                        break;
                }
            }

            if (_drawImage != null)
            {
                if (ImageFillMode == ImageFillMode.Shaded)
                {
                    _drawImage = CommonHelper.BitmapColorShade((Bitmap)_drawImage, ShadedColor);
                }
                _drawImage = CommonHelper.RotateBitmap((Bitmap)_drawImage, (float)_rotateAngle);
            }

            return _drawImage;
        }
        #endregion

        #region ImageTransparentColor
        /// <summary>
        /// Gets and sets the label image transparent color.
        /// </summary>
        [Localizable(true)]
        [Category("Visuals")]
        [Description("Label image transparent color.")]
        [RefreshPropertiesAttribute(RefreshProperties.All)]
        [EasyDefaultColorAttribute()]
        public Color ImageTransparentColor
        {
            get { return _transparent; }

            set
            {
                if (_transparent != value)
                {
                    _transparent = value;
                    PerformNeedPaint(true);
                }
            }
        }

        private bool ShouldSerializeImageTransparentColor()
        {
            return ImageTransparentColor != Color.Empty;
        }

        /// <summary>
        /// Resets the ImageTransparentColor property to its default value.
        /// </summary>
        public void ResetImageTransparentColor()
        {
            ImageTransparentColor = Color.Empty;
        }

        /// <summary>
        /// Gets the content image transparent color.
        /// </summary>
        /// <param name="state">The state for which the image color is needed.</param>
        /// <returns>Color value.</returns>
        public Color GetImageTransparentColor(PaletteState state)
        {
            return ImageTransparentColor;
        }
        #endregion

        #region Text
        /// <summary>
        /// Gets and sets the label text.
        /// </summary>
        [Localizable(true)]
        [Browsable(false)]
        [ReadOnly(true)]
        [Category("Visuals")]
        [RefreshPropertiesAttribute(RefreshProperties.All)]
        [Editor("System.ComponentModel.Design.MultilineStringEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
        public string Text
        {
            get { return _text; }

            set
            {
                if (_text != value)
                {
                    _text = value;
                    PerformNeedPaint(true);
                    if (TextChanged != null)
                        TextChanged(this, EventArgs.Empty);
                }
            }
        }

        private bool ShouldSerializeText()
        {
            return Text != _defaultText;
        }

        /// <summary>
        /// Resets the Text property to its default value.
        /// </summary>
        public void ResetText()
        {
            Text = _defaultText;
        }

        /// <summary>
        /// Gets the content short text.
        /// </summary>
        public string GetShortText()
        {
            return Text;
        }
        #endregion

        #region ExtraText
        /// <summary>
        /// Gets and sets the label extra text.
        /// </summary>
        [Localizable(true)]
        [Category("Visuals")]
        [Browsable(false)]
        [ReadOnly(true)]
        [RefreshPropertiesAttribute(RefreshProperties.All)]
        [Editor("System.ComponentModel.Design.MultilineStringEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
        [DefaultValue("")]
        public string ExtraText
        {
            get { return _extraText; }

            set
            {
                if (_extraText != value)
                {
                    _extraText = value;
                    PerformNeedPaint(true);
                }
            }
        }

        private bool ShouldSerializeExtraText()
        {
            return ExtraText != _defaultExtraText;
        }

        /// <summary>
        /// Resets the Description property to its default value.
        /// </summary>
        public void ResetExtraText()
        {
            ExtraText = ExtraText;
        }

        /// <summary>
        /// Gets the content long text.
        /// </summary>
        public string GetLongText()
        {
            return ExtraText;
        }
        #endregion
    }
}
