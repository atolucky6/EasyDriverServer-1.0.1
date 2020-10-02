using EasyScada.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyScada.Winforms.Controls.Controls.EasyAlarmLogger
{
    public partial class EasyAlarmLogger : Component
    {
        #region Constructors
        public EasyAlarmLogger()
        {
            InitializeComponent();
        }

        public EasyAlarmLogger(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }
        #endregion

        #region Fields
        private bool enabled;
        private LogProfileCollection profiles = new LogProfileCollection();

        #endregion
    }
}
