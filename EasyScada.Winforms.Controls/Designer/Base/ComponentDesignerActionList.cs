using EasyScada.Core;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Windows.Forms;

namespace EasyScada.Winforms.Controls
{
    /// <summary>
    /// The base class designer action list to provide the action of a control at design-time
    /// </summary>
    /// <typeparam name="TComponent"></typeparam>
    public class ComponentDesignerAcionList<TComponent> : DesignerActionList
        where TComponent : Component
    {
        #region Members

        protected TComponent BaseComponent;
        protected DesignerActionItemCollection actionItems;
        protected DesignerActionUIService designerActionUIservice;

        #endregion

        #region Constructors

        public ComponentDesignerAcionList(IComponent component) : base(component)
        {
            BaseComponent = component as TComponent;
            designerActionUIservice = GetService(typeof(DesignerActionUIService)) as DesignerActionUIService;
        }

        #endregion

        #region Methods

        public override DesignerActionItemCollection GetSortedActionItems()
        {
            actionItems = new DesignerActionItemCollection();

            AddExtendActionItems();

            if (BaseComponent is ISupportScale)
            {
                //Scale category
                actionItems.Add(new DesignerActionHeaderItem(DesignerCategory.SCALE));
                actionItems.Add(new DesignerActionPropertyItem("EnableScale", "Enable Scale", DesignerCategory.SCALE, ""));
                actionItems.Add(new DesignerActionPropertyItem("Gain", "Gain", DesignerCategory.SCALE, ""));
                actionItems.Add(new DesignerActionPropertyItem("Offset", "Offset", DesignerCategory.SCALE, ""));
            }

            //EasyScada category
            if (BaseComponent is ISupportConnector)
            {
                actionItems.Add(new DesignerActionHeaderItem(DesignerCategory.EASYSCADA));
                actionItems.Add(new DesignerActionPropertyItem("Connector", "Connector", DesignerCategory.EASYSCADA, ""));
            }

            if (BaseComponent is ISupportWriteSingleTag)
            {
                actionItems.Add(new DesignerActionPropertyItem("WriteMode", "Write Mode", DesignerCategory.EASYSCADA, ""));
                actionItems.Add(new DesignerActionPropertyItem("WriteDelay", "Write Delay", DesignerCategory.EASYSCADA, ""));
            }

            if (BaseComponent is ISupportTag)
            {
                actionItems.Add(new DesignerActionPropertyItem("PathToTag", "Path To Tag", DesignerCategory.EASYSCADA, "Select path to tag for Easy Scada control"));
                actionItems.Add(new DesignerActionMethodItem(this, "SelectTag", "Select tag...", DesignerCategory.EASYSCADA, "Click here to select tag", true));
            }

            if (BaseComponent is ISupportPalette)
            {
                actionItems.Add(new DesignerActionHeaderItem(DesignerCategory.THEMES));
                actionItems.Add(new DesignerActionPropertyItem("PaletteMode", "Palette", DesignerCategory.THEMES, ""));
            }

            return actionItems;
        }

        protected virtual void AddExtendActionItems()
        {

        }

        #endregion

        #region Designer properties

        #region Themes category properties

        public PaletteMode PaletteMode
        {
            get
            {
                if (BaseComponent is ISupportPalette)
                    return (BaseComponent as ISupportPalette).PaletteMode;
                return PaletteMode.Global;
            }
            set
            {
                if (BaseComponent is ISupportPalette)
                    (BaseComponent as ISupportPalette).SetValue(value);
            }
        }

        #endregion

        #region Scale category properties

        [Browsable(true), Category(DesignerCategory.SCALE)]
        public virtual bool EnableScale
        {
            get
            {
                if (BaseComponent is ISupportScale)
                    return (BaseComponent as ISupportScale).EnableScale;
                return false;
            }
            set
            {
                if (BaseComponent is ISupportScale)
                    (BaseComponent as ISupportScale).SetValue(value);
            }
        }

        [Browsable(true), Category(DesignerCategory.SCALE)]
        public virtual double Gain
        {
            get
            {
                if (BaseComponent is ISupportScale)
                    return (BaseComponent as ISupportScale).Gain;
                return 1;
            }
            set
            {
                if (BaseComponent is ISupportScale)
                    (BaseComponent as ISupportScale).SetValue(value);
            }
        }

        [Browsable(true), Category(DesignerCategory.SCALE)]
        public virtual double Offset
        {
            get
            {
                if (BaseComponent is ISupportScale)
                    return (BaseComponent as ISupportScale).Offset;
                return 0;
            }
            set
            {
                if (BaseComponent is ISupportScale)
                    (BaseComponent as ISupportScale).SetValue(value);
            }
        }

        #endregion

        #region Easy Scada category properties

        [Description("The driver connector to Easy Driver Server")]
        [Browsable(true), Category(DesignerCategory.EASYSCADA)]
        public virtual IEasyDriverConnector Connector
        {
            get
            {
                if (BaseComponent is ISupportConnector)
                    return (BaseComponent as ISupportConnector).Connector;
                return null;
            }
            set
            {
                if (BaseComponent is ISupportConnector)
                    (BaseComponent as ISupportConnector).SetValue(value);
            }
        }

        [Description("Path to tag of the control")]
        [TypeConverter(typeof(TagPathConverter)), Category(DesignerCategory.EASYSCADA)]
        public virtual string PathToTag
        {
            get
            {
                if (BaseComponent is ISupportTag)
                    return (BaseComponent as ISupportTag).TagPath;
                return string.Empty;
            }
            set
            {
                if (BaseComponent is ISupportTag)
                    (BaseComponent as ISupportTag).SetValue(value);
            }
        }

        public virtual WriteTrigger WriteTrigger
        {
            get
            {
                if (BaseComponent is ISupportWriteSingleTag)
                    return (BaseComponent as ISupportWriteSingleTag).WriteTrigger;
                return WriteTrigger.OnEnter;
            }
            set
            {
                if (BaseComponent is ISupportWriteSingleTag)
                    (BaseComponent as ISupportWriteSingleTag).SetValue(value);
            }
        }

        public virtual int WriteDelay
        {
            get
            {
                if (BaseComponent is ISupportWriteSingleTag)
                    return (BaseComponent as ISupportWriteSingleTag).WriteDelay;
                return 100;
            }
            set
            {
                if (BaseComponent is ISupportWriteSingleTag)
                    (BaseComponent as ISupportWriteSingleTag).SetValue(value);
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
