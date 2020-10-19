using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyScada.Core.Animate
{
    public class DiscreteTrigger : TriggerBase
    {
        #region Members

        public string TriggerValue { get; set; }

        #endregion

        #region Constructors

        public DiscreteTrigger() : base()
        {

        }

        #endregion

        #region Override methods

        protected override bool CanExecute(object parameter = null)
        {
            if (string.IsNullOrEmpty(TriggerTagPath))
                return false;

            string expression = $"Tag[{'"'}{TriggerTagPath}{'"'}] = {TriggerValue}";
            TokenExpressionString = expression;
            return base.CanExecute(parameter);
        }

        #endregion
    }
}
