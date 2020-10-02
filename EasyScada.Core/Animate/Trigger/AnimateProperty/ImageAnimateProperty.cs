using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Reflection;

namespace EasyScada.Core
{
    public class ImageAnimateProperty : AnimateProperty<Image>
    {
        public ImageAnimateProperty(object targetControl, PropertyInfo property, Image defaultValue) : base(targetControl, property, defaultValue)
        {
        }

        [Editor("System.Drawing.Design.ImageEditor", typeof(UITypeEditor))]
        public override Image Value { get => base.Value; set => base.Value = value; }
    }
}
