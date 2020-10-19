using EasyScada.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EasyScada.Winforms.Controls
{
    public class AnimateImageFlipMode : AnimateProperty<ImageFlipMode>
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
                            winformControl.FlipMode = Value;
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
