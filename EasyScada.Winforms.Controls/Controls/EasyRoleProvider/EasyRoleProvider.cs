using EasyScada.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace EasyScada.Winforms.Controls
{
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    [Designer(typeof(EasyRoleProviderDesigner))]
    public partial class EasyRoleProvider : Component, ISupportInitialize
    {
        #region Constructors
        public EasyRoleProvider()
        {
            InitializeComponent();


        }

        public EasyRoleProvider(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }
        #endregion

        #region Public properties
        RoleCollection _roles = new RoleCollection();
        [Category(DesignerCategory.EASYSCADA)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Browsable(false)]
        public RoleCollection Roles
        {
            get => _roles;
            protected set
            {
                if (_roles != value)
                {
                    _roles = value;
                }
            }
        }
        #endregion

        #region ISupportInitialize
        public void BeginInit()
        {
        }

        public void EndInit()
        {
            if (!DesignMode)
            {
                var roles = DesignerHelper.GetRoleSettings(Site);
                if (roles != null)
                {
                    roles.ForEach(x => Roles.Add(x));
                }
            }
        }
        #endregion

    }
}
