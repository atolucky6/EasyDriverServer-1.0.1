using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyScada.Core.Animate
{
    public class QualityTrigger : TriggerBase
    {
        #region Members
        public Quality TriggerQuality { get; set; }
        #endregion

        #region Constructors
        public QualityTrigger()
        {

        }
        #endregion

        #region Override methods

        protected override bool CanExecute(object parameter = null)
        {
            if (Enabled && Target != null && 
                AnimatePropertyWrapper != null && 
                TriggerTag != null)
            {
                return true;
            }
            return false;
        }

        public override void Execute(object parameter = null)
        {
            if (parameter is Quality quality)
            {
                if (CanExecute())
                {
                    if (quality == TriggerQuality)
                        AnimatePropertyWrapper.UpdateValue();
                }
            }
        }

        public override string Compile()
        {
            return "";
        }
        #endregion
    }
}
