using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyScada.Core.Animate
{
    public class QualityTrigger : TriggerBase
    {
        #region Members
        public string TriggerTagPath { get; set; }
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
            return false;
        }
        #endregion
    }
}
