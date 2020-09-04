using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyScada.Core.Designer
{
    public enum EdgeBehaviorEnum
    {
        EaseIn,
        EaseOut,
        EaseInOut
    }


    public abstract class EasedInterpolation : Interpolation
    {
        // Methods
        public EasedInterpolation()
        {
            this.EdgeBehavior = EdgeBehaviorEnum.EaseOut;
        }

        public EasedInterpolation(EdgeBehaviorEnum edgeBehavior)
        {
            this.EdgeBehavior = edgeBehavior;
        }

        public override double GetAlpha(double progress)
        {
            switch (this.EdgeBehavior)
            {
                case EdgeBehaviorEnum.EaseIn:
                    return this.GetEaseInAlpha(progress);

                case EdgeBehaviorEnum.EaseOut:
                    return this.GetEaseOutAlpha(progress);
            }
            return this.GetEaseInOutAlpha(progress);
        }

        protected abstract double GetEaseInAlpha(double progress);
        protected virtual double GetEaseInOutAlpha(double timeFraction)
        {
            if (timeFraction <= 0.5)
            {
                return (0.5 * this.GetEaseInAlpha(timeFraction * 2.0));
            }
            return (0.5 + (0.5 * this.GetEaseOutAlpha((timeFraction - 0.5) * 2.0)));
        }

        protected abstract double GetEaseOutAlpha(double progress);

        // Properties
        public EdgeBehaviorEnum EdgeBehavior { get; set; }
    }

}
