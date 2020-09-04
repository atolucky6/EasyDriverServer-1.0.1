using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyScada.Core.Designer
{
    public class ExponentialInterpolation : EasedInterpolation
    {
        // Methods
        public ExponentialInterpolation() : this(2.0, EdgeBehaviorEnum.EaseInOut)
        {
        }

        public ExponentialInterpolation(double power, EdgeBehaviorEnum edgeBehavior) : base(edgeBehavior)
        {
            this.Power = power;
        }

        protected override double GetEaseInAlpha(double timeFraction) =>
            Math.Pow(timeFraction, this.Power);

        protected override double GetEaseOutAlpha(double timeFraction) =>
            Math.Pow(timeFraction, 1.0 / this.Power);

        // Properties
        public double Power { get; set; }
    }

}
