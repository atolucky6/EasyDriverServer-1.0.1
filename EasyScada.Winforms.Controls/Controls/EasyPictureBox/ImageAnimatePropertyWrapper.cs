using EasyScada.Core;
using System.ComponentModel;
using System.Drawing;
using System.Windows;

namespace EasyScada.Winforms.Controls
{
    public class ImageAnimatePropertyWrapper : AnimatePropertyWrapper
    {
        public ImageAnimatePropertyWrapper()
        {
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
        public AnimateProperty<ImageFlipMode> FlipMode { get; set; }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        [DisplayName("Fill Mode")]
        public AnimateProperty<ImageFillMode> FillMode { get; set; }

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

        public override void Reverse()
        {

        }

        public override void UpdateValue()
        {

        }
    }
}
