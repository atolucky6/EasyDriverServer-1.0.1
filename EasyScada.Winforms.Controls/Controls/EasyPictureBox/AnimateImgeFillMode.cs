using EasyScada.Core;
using System;

namespace EasyScada.Winforms.Controls
{
    public class AnimateImageFillMode : AnimateProperty<ImageFillMode>
    {
        public override void SetValue()
        {
            try
            {
                if (Enabled && TargetControl != null && AnimatePropertyInfo != null)
                {
                    if (TargetControl is EasyPictureBox winformControl)
                    {
                        winformControl.Invoke(new Action(() =>
                        {
                            winformControl.FillMode = Value;
                        }));
                    }
                    else
                    {
                        AnimatePropertyInfo.SetValue(TargetControl, Value);
                    }
                }
            }
            catch { }
        }
    }
}
