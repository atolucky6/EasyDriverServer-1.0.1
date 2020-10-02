using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Security.Permissions;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Windows.Forms.Design.Behavior;

namespace EasyScada.Winforms.Controls
{
    /// <summary>
    /// The base generic designer class support design a control at design-time
    /// </summary>
    /// <typeparam name="TControl"></typeparam>
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    public abstract class ControlDesignerBase<TControl> : ControlDesigner
        where TControl : Control
    {
        #region Members

        protected DesignerActionListCollection actionListCollection;
        protected ControlDesignerAcionList<TControl> actionList;
        protected TControl BaseControl { get { return Component as TControl; } }
        protected ISelectionService SelectionService { get; set; }

        #endregion

        #region Methods

        public override void Initialize(IComponent component)
        {
            base.Initialize(component);
            SelectionService = GetService(typeof(ISelectionService)) as ISelectionService;
            SelectionService.SelectionChanged += OnSelectionChanged;
        }

        private void OnSelectionChanged(object sender, System.EventArgs e)
        {
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        public override DesignerActionListCollection ActionLists
        {
            get
            {
                if (actionListCollection == null)
                {
                    actionListCollection = new DesignerActionListCollection();
                    actionList = GetActionList();
                    actionListCollection.Add(actionList);
                }
                return actionListCollection;
            }
        }

        protected virtual ControlDesignerAcionList<TControl> GetActionList()
        {
            if (actionList == null)
                actionList = new ControlDesignerAcionList<TControl>(Component);
            return actionList;
        }

        #endregion
    }
}
