using System;
using System.Collections.Generic;
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
        
        private string analogRangeString;
        public string AnalogRangeString
        {
            get { return analogRangeString; }
            set
            {
                if (value != analogRangeString)
                {
                    analogRangeString = value;
                    AnalogRange = AnalogRange.Parse(value);
                }
            }
        }
        public AnalogRange AnalogRange { get; private set; }
        public string TagName { get; set; }

        #endregion

        #region Methods

        protected override bool CanExecute(object parameter = null)
        {
            if (string.IsNullOrWhiteSpace(AnalogRangeString))
                return false;

            if (AnalogRange == null)
                return false;

            if (!AnalogRange.IsValid)
                return false;

            if (string.IsNullOrWhiteSpace(TagName))
                return false;

            string expression = $"Tag[{'"'}{TagName}{'"'}] >= {AnalogRange.MinValue}";
            if (AnalogRange.MinValue != AnalogRange.MaxValue)
                expression += $" and Tag[{'"'}{TagName}{'"'}] <= {AnalogRange.MaxValue}";
            TokenExpressionString = expression;
            return base.CanExecute(parameter);
        }

        #endregion
    }
}
