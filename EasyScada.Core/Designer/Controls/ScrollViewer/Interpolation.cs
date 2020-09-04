using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyScada.Core.Designer
{
    public abstract class Interpolation
    {
        // Methods
        protected Interpolation()
        {
        }

        public abstract double GetAlpha(double progress);
    }

}
