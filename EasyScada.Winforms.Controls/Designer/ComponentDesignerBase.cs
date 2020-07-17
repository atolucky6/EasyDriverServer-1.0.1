using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Security.Permissions;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace EasyScada.Winforms.Controls
{
    /// <summary>
    /// The base generic designer class support design a control at design-time
    /// </summary>
    /// <typeparam name="TComponent"></typeparam>
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    public abstract class ComponentDesignerBase<TComponent> : ComponentDesigner
        where TComponent : Component
    {
        #region Members

        protected DesignerActionListCollection actionListCollection;
        protected ComponentDesignerAcionList<TComponent> actionList;
        protected TComponent BaseComponent { get { return Component as TComponent; } }

        #endregion

        #region Methods

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

        protected virtual ComponentDesignerAcionList<TComponent> GetActionList()
        {
            if (actionList == null)
                actionList = new ComponentDesignerAcionList<TComponent>(Component);
            return actionList;
        }

        #endregion
    }
}
