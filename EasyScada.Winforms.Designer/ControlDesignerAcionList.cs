using EasyScada.Winforms.Connector;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Windows.Forms;

namespace EasyScada.Winforms.Designer
{
    /// <summary>
    /// The base class designer action list to provide the action of a control at design-time
    /// </summary>
    /// <typeparam name="TControl"></typeparam>
    public class ControlDesignerAcionList<TControl> : DesignerActionList
        where TControl : Control
    {
        #region Members

        protected TControl BaseControl;
        protected DesignerActionItemCollection actionItems;
        protected DesignerActionUIService designerActionUIservice;

        #endregion

        #region Constructors

        public ControlDesignerAcionList(IComponent component) : base(component)
        {
            BaseControl = component as TControl;
            designerActionUIservice = GetService(typeof(DesignerActionUIService)) as DesignerActionUIService;
        }

        #endregion

        #region Methods

        public override DesignerActionItemCollection GetSortedActionItems()
        {
            actionItems = new DesignerActionItemCollection();

            //Design category
            actionItems.Add(new DesignerActionHeaderItem(DesignerCategory.DESIGN));
            actionItems.Add(new DesignerActionPropertyItem("Name", "Name", DesignerCategory.DESIGN, "Set name of the control"));

            //Appearance category
            actionItems.Add(new DesignerActionHeaderItem(DesignerCategory.APPEARANCE));
            actionItems.Add(new DesignerActionPropertyItem("BackColor", "Back Color", DesignerCategory.APPEARANCE, "Selects the background color."));
            actionItems.Add(new DesignerActionPropertyItem("ForeColor", "Fore Color", DesignerCategory.APPEARANCE, "Selects the fore color."));

            //Layout category
            actionItems.Add(new DesignerActionHeaderItem(DesignerCategory.LAYOUT));
            actionItems.Add(new DesignerActionPropertyItem("Dock", "Dock", DesignerCategory.LAYOUT, "Selects the dock style."));
            actionItems.Add(new DesignerActionPropertyItem("Size", "Size", DesignerCategory.LAYOUT, "Size of the control"));

            if (BaseControl is ISupportScale)
            {
                //Scale category
                actionItems.Add(new DesignerActionHeaderItem(DesignerCategory.SCALE));
                actionItems.Add(new DesignerActionPropertyItem("EnableScale", "Enable Scale", DesignerCategory.SCALE, ""));
                actionItems.Add(new DesignerActionPropertyItem("Gain", "Gain", DesignerCategory.SCALE, ""));
                actionItems.Add(new DesignerActionPropertyItem("Offset", "Offset", DesignerCategory.SCALE, ""));
            }

            AddExtendActionItems();

            //EasyScada category
            if (BaseControl is ISupportConnector)
            {
                actionItems.Add(new DesignerActionHeaderItem(DesignerCategory.EASYSCADA));
                actionItems.Add(new DesignerActionPropertyItem("Connector", "Connector", DesignerCategory.EASYSCADA, "Select connector for Easy Scada control"));
            }

            if (BaseControl is ISupportTag)
            {
                actionItems.Add(new DesignerActionPropertyItem("PathToTag", "Path To Tag", "", "Select path to tag for Easy Scada control"));
                actionItems.Add(new DesignerActionMethodItem(this, "SelectTag", "Select tag...", DesignerCategory.EASYSCADA, "Click here to select tag", true));
            }

            return actionItems;
        }

        protected virtual void AddExtendActionItems()
        {

        }

        #endregion

        #region Designer properties

        #region Design category properties

        public virtual string Name
        {
            get { return BaseControl.Name; }
            set { BaseControl.SetValue(value); }
        }

        #endregion

        #region Appearance category properties

        public virtual Color BackColor
        {
            get { return BaseControl.BackColor; }
            set { BaseControl.SetValue(value); }
        }

        public virtual Color ForeColor
        {
            get { return BaseControl.ForeColor; }
            set { BaseControl.SetValue(value); }
        }

        #endregion

        #region Layout category properties

        public virtual DockStyle Dock
        {
            get { return BaseControl.Dock; }
            set { BaseControl.SetValue(value); }
        }

        public virtual Size Size
        {
            get { return BaseControl.Size; }
            set { BaseControl.SetValue(value); }
        }

        #endregion

        #region Scale category properties

        [Browsable(true), Category(DesignerCategory.SCALE)]
        public virtual bool EnableScale
        {
            get
            {
                if (BaseControl is ISupportScale)
                    return (BaseControl as ISupportScale).EnableScale;
                return false;
            }
            set
            {
                if (BaseControl is ISupportScale)
                    (BaseControl as ISupportScale).SetValue(value);
            }
        }

        [Browsable(true), Category(DesignerCategory.SCALE)]
        public virtual double Gain
        {
            get
            {
                if (BaseControl is ISupportScale)
                    return (BaseControl as ISupportScale).Gain;
                return 1;
            }
            set
            {
                if (BaseControl is ISupportScale)
                    (BaseControl as ISupportScale).SetValue(value);
            }
        }

        [Browsable(true), Category(DesignerCategory.SCALE)]
        public virtual double Offset
        {
            get
            {
                if (BaseControl is ISupportScale)
                    return (BaseControl as ISupportScale).Offset;
                return 0;
            }
            set
            {
                if (BaseControl is ISupportScale)
                    (BaseControl as ISupportScale).SetValue(value);
            }
        }

        #endregion

        #region Easy Scada category properties

        [Description("The driver connector to Easy Driver Server")]
        [Browsable(true), Category(DesignerCategory.EASYSCADA)]
        public virtual EasyDriverConnector Connector
        {
            get
            {
                if (BaseControl is ISupportConnector)
                    return (BaseControl as ISupportConnector).Connector;
                return null;
            }
            set
            {
                if (BaseControl is ISupportConnector)
                    (BaseControl as ISupportConnector).SetValue(value);
            }
        }

        [Description("Path to tag of the control")]
        [TypeConverter(typeof(EasyScadaTagPathConverter)), Category(DesignerCategory.EASYSCADA)]
        public virtual string PathToTag
        {
            get
            {
                if (BaseControl is ISupportTag)
                    return (BaseControl as ISupportTag).PathToTag;
                return string.Empty;
            }
            set
            {
                if (BaseControl is ISupportTag)
                    (BaseControl as ISupportTag).SetValue(value);
            }
        }

        #endregion

        #endregion

        #region Action methods

        /// <summary>
        /// The method to show a select tag form
        /// </summary>
        public void SelectTag()
        {
            //SelectTagForm<ISupportTag> selectTag = new SelectTagForm<ISupportTag>(BaseControl as ISupportTag);
            //selectTag.ShowDialog();
        }

        #endregion
    }
}
