using EasyScada.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyScada.Winforms.Controls
{
    [ToolboxItem(false)]
    public partial class EasyDataUpdater : Component
    {
        public EasyDataUpdater()
        {
            InitializeComponent();
        }

        public EasyDataUpdater(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }

        #region Fields
        private bool enabled = true;
        private int interval = 60000;
        private LogProfileCollection profiles = new LogProfileCollection();
        private LogColumnCollection columns = new LogColumnCollection();
        private UpdateUnit unit = UpdateUnit.Minute;
        private ResetTrigger resetTrigger = ResetTrigger.None;
        #endregion

        #region Public properties
        public bool Enabled
        {
            get => enabled;
            set => enabled = value;
        }

        public int Interval
        {
            get => interval;
            set
            {
                if (value < 100)
                    interval = 100;
                else
                    interval = value;
            }
        }

        public UpdateUnit UpdateUnit
        {
            get => unit;
            set => unit = value;
        }

        public LogProfileCollection Databases { get => profiles; }

        public LogColumnCollection Columns { get => columns; }

        public ResetTrigger ResetTrigger
        {
            get => resetTrigger;
            set => resetTrigger = value;
        }
        #endregion
    }
}
