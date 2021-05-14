using EasyScada.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EasyScada.Winforms.Controls
{
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    class EasyAlarmViewerDesigner : ControlDesignerBase<EasyAlarmViewer>
    {
        protected override ControlDesignerAcionList<EasyAlarmViewer> GetActionList()
        {
            return new EasyAlarmViewerDesignerActionList(Component);
        }
    }

    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    class EasyAlarmViewerDesignerActionList : ControlDesignerAcionList<EasyAlarmViewer>
    {
        public EasyAlarmViewerDesignerActionList(IComponent component) : base(component)
        {
        }

        protected override void AddExtendActionItems()
        {
            actionItems.Add(new DesignerActionHeaderItem(DesignerCategory.DESIGN));
            actionItems.Add(new DesignerActionPropertyItem("Name", "Name", DesignerCategory.DESIGN, ""));

            actionItems.Add(new DesignerActionHeaderItem(DesignerCategory.APPEARANCE));
            actionItems.Add(new DesignerActionPropertyItem("Font", "Font", DesignerCategory.APPEARANCE, ""));
            actionItems.Add(new DesignerActionPropertyItem("BackColor", "BackColor", DesignerCategory.APPEARANCE, ""));
            actionItems.Add(new DesignerActionPropertyItem("Dock", "Dock", DesignerCategory.APPEARANCE, ""));

            actionItems.Add(new DesignerActionPropertyItem("MaxDisplayRowCount", "MaxDisplayRowCount", DesignerCategory.APPEARANCE, ""));
            actionItems.Add(new DesignerActionPropertyItem("RefreshInterval", "RefreshInterval", DesignerCategory.APPEARANCE, ""));

            actionItems.Add(new DesignerActionMethodItem(this, "DatabaseConfig", "Configure databases", DesignerCategory.EASYSCADA, "Click here to configure databases", true));
        }

        #region Properties

        public virtual string Name
        {
            get { return BaseControl.Name; }
            set { BaseControl.SetValue(value); }
        }

        public Color BackColor
        {
            get { return BaseControl.BackColor; }
            set { BaseControl.SetValue(value); }
        }

        public DockStyle Dock
        {
            get { return BaseControl.Dock; }
            set { BaseControl.SetValue(value); }
        }

        public Font Font
        {
            get { return BaseControl.Font; }
            set { BaseControl.SetValue(value); }
        }

        public int MaxDisplayRowCount
        {
            get { return BaseControl.MaxDisplayRowCount; }
            set { BaseControl.SetValue(value); }
        }

        public int RefreshInterval
        {
            get { return BaseControl.RefreshInterval; }
            set { BaseControl.SetValue(value); }
        }
        #endregion

        #region Actions methods
        public void DatabaseConfig()
        {
            LogProfileConfigDesignerForm form = new LogProfileConfigDesignerForm(false, false); // Hide add and remove button
            LogProfile profile = BaseControl.Database;
            if (profile == null)
            {
                profile = new LogProfile() { DatabaseType = DbType.MySql, Port = 3306, TableName = "alarmTable" };
            }

            form.Databases = new LogProfileCollection() { profile };
            form.Start();
            if (form.ShowDialog() == DialogResult.OK)
            {
                var results = form.ResultDatabases.ToArray();
                if (results != null && results.Length > 0)
                {
                    BaseControl.Database = results[0];
                }
                else
                {
                    BaseControl.Database = null;
                }

                BaseControl.SetValue(Font, "Font");
            }
        }
        #endregion
    }
}
