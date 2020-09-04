using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace EasyScada.Core.Designer
{
    internal class ScrollData
    {
        // Fields
        internal bool canHorizontallyScroll;
        internal bool canVerticallyScroll;
        internal Vector computedOffset = new Vector(0.0, 0.0);
        internal Size extent = new Size(0.0, 0.0);
        internal Size maxDesiredSize = new Size(0.0, 0.0);
        internal Vector offset = new Vector(0.0, 0.0);
        internal AnimatedScrollViewer _scrollOwner;
        internal Size viewport = new Size(0.0, 0.0);

        // Methods
        internal void ClearLayout()
        {
            this.offset = new Vector(0.0, 0.0);
            this.viewport = this.extent = this.maxDesiredSize = new Size(0.0, 0.0);
        }
    }

}
