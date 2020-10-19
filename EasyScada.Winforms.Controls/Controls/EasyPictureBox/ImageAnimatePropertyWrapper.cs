using EasyScada.Core;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows;

namespace EasyScada.Winforms.Controls
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class ImageAnimatePropertyWrapper : AnimatePropertyWrapper
    {
        public ImageAnimatePropertyWrapper()
        {
            BackColor = new AnimateProperty<Color>();
            ForeColor = new AnimateProperty<Color>();
            ShadedColor = new AnimateProperty<Color>();
            FlipMode = new AnimateImageFlipMode();
            FillMode = new AnimateImageFillMode();
            RotateAngle = new AnimateProperty<int>();
            Size = new AnimateProperty<System.Drawing.Size>();
            Location = new AnimateProperty<System.Drawing.Point>();
            Image = new ImageAnimateProperty();
        }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        [DisplayName("Back Color")]
        public AnimateProperty<Color> BackColor { get; set; }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        [DisplayName("Fore Color")]
        public AnimateProperty<Color> ForeColor { get; set; }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        [DisplayName("Shaded Color")]
        public AnimateProperty<Color> ShadedColor { get; set; }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        [DisplayName("Flip Mode")]
        public AnimateImageFlipMode FlipMode { get; set; }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        [DisplayName("Fill Mode")]
        public AnimateImageFillMode FillMode { get; set; }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        [DisplayName("Rotate Angle")]
        public AnimateProperty<int> RotateAngle { get; set; }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        [DisplayName("Size")]
        public AnimateProperty<System.Drawing.Size> Size { get; set; }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        [DisplayName("Location")]
        public AnimateProperty<System.Drawing.Point> Location { get; set; }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        [DisplayName("Image")]
        public ImageAnimateProperty Image { get; set; }

        public event EventHandler Disposed;

        public void Dispose()
        {
            Disposed?.Invoke(this, EventArgs.Empty);
        }

        public override void Reverse()
        {

        }

        public override void UpdateValue()
        {
            foreach (var item in GetAnimateProperties())
            {
                item.SetValue();
            }
        }
    }
}
