using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Reflection;

namespace EasyScada.Core
{
    public class ImageAnimateProperty : AnimateProperty<Image>
    {
        public ImageAnimateProperty(object targetControl, Image defaultValue) : base(targetControl, defaultValue)
        {
        }

        public ImageAnimateProperty() : base()
        {

        }

        [Editor("System.Drawing.Design.ImageEditor", typeof(UITypeEditor))]
        public override Image Value { get => base.Value; set => base.Value = value; }
    }
}
