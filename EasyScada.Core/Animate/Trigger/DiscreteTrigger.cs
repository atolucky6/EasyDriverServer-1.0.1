using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyScada.Core.Animate
{
    public class DiscreteTrigger : TriggerBase
    {
        #region Members

        public string TagName { get; set; }
        public BoolValue CompareValue { get; set; }

        #endregion

        #region Constructors

        public DiscreteTrigger() : base()
        {

        }

        #endregion

        #region Override methods

        protected override bool CanExecute(object parameter = null)
        {
            if (string.IsNullOrEmpty(TagName))
                return false;

            string expression = $"Tag[{'"'}{TagName}{'"'}] = {(int)CompareValue}";
            TokenExpressionString = expression;
            return base.CanExecute(parameter);
        }

        #endregion
    }
}
