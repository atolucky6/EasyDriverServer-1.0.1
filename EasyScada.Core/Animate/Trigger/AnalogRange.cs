using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyScada.Core.Animate
{
    public class AnalogRange
    {
        public decimal MinValue { get; set; }
        public decimal MaxValue { get; set; }

        public bool IsValid
        {
            get
            {
                if (MinValue > MaxValue)
                    return false;
                return true;
            }
        }

        public static bool TryParse(string input, out AnalogRange analogRange)
        {
            analogRange = null;
            if (string.IsNullOrWhiteSpace(input))
                return false;
            string[] inputSplit = input.Split('-');
            if (inputSplit.Length != 2)
                return false;
            if (decimal.TryParse(inputSplit[0], out decimal min) &&
                decimal.TryParse(inputSplit[1], out decimal max))
            {
                if (min > max)
                    return false;
                return true;
            }
            return false;
        }

        public static AnalogRange Parse(string input)
        {
            TryParse(input, out AnalogRange analogRange);
            return analogRange;
        }
    }
}
