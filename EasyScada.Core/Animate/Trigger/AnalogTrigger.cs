using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyScada.Core.Animate
{
    public class AnalogTrigger : TriggerBase
    {
        #region Constructors

        public AnalogTrigger() : base()
        {

        }

        #endregion

        #region Public properties
        
        public string MinValue { get; set; }
        public string MaxValue { get; set; }

        #endregion

        #region Methods

        protected override bool CanExecute(object parameter = null)
        {
            if (string.IsNullOrWhiteSpace(TriggerTagPath))
                return false;

            string expression = $"Tag[{'"'}{TriggerTagPath}{'"'}] >= {MinValue}";
            if (MinValue != MaxValue)
                expression += $" and Tag[{'"'}{TriggerTagPath}{'"'}] <= {MaxValue}";
            TokenExpressionString = expression;
            return base.CanExecute(parameter);
        }

        #endregion
    }
}
