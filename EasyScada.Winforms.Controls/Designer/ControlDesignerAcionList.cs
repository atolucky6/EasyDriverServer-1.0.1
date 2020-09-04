using EasyScada.Core;
using EasyScada.Winforms.Connector;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design.Behavior;

namespace EasyScada.Winforms.Controls
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

            AddExtendActionItems();

            if (BaseControl is ISupportScale)
            {
                //Scale category
                actionItems.Add(new DesignerActionHeaderItem(DesignerCategory.SCALE));
                actionItems.Add(new DesignerActionPropertyItem("EnableScale", "Enable Scale", DesignerCategory.SCALE, ""));
                actionItems.Add(new DesignerActionPropertyItem("Gain", "Gain", DesignerCategory.SCALE, ""));
                actionItems.Add(new DesignerActionPropertyItem("Offset", "Offset", DesignerCategory.SCALE, ""));
            }

            //EasyScada category
            if (BaseControl is ISupportConnector)
            {
                actionItems.Add(new DesignerActionHeaderItem(DesignerCategory.EASYSCADA));
                actionItems.Add(new DesignerActionPropertyItem("Connector", "Connector", DesignerCategory.EASYSCADA, ""));
            }

            if (BaseControl is ISupportWriteSingleTag)
            {
                actionItems.Add(new DesignerActionPropertyItem("WriteTrigger", "Write Trigger", DesignerCategory.EASYSCADA, ""));
                actionItems.Add(new DesignerActionPropertyItem("WriteDelay", "Write Delay", DesignerCategory.EASYSCADA, ""));
            }

            if (BaseControl is ISupportWriteMultiTag)
            {
                actionItems.Add(new DesignerActionPropertyItem("WriteTagCommands", "Write Tag Commands", DesignerCategory.EASYSCADA, ""));
            }

            if (BaseControl is ISupportTag)
            {
                actionItems.Add(new DesignerActionPropertyItem("PathToTag", "Path To Tag", DesignerCategory.EASYSCADA, "Select path to tag for Easy Scada control"));
                actionItems.Add(new DesignerActionMethodItem(this, "SelectTag", "Select tag...", DesignerCategory.EASYSCADA, "Click here to select tag", true));
            }

            if (BaseControl is ISupportPalette)
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
                if (BaseControl is ISupportPalette)
                    return (BaseControl as ISupportPalette).PaletteMode;
                return PaletteMode.Global;
            }
            set
            {
                if (BaseControl is ISupportPalette)
                    (BaseControl as ISupportPalette).SetValue(value);
            }
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
        public virtual IEasyDriverConnector Connector
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

        //[TypeConverter(typeof(TagPathConverter)), Category(DesignerCategory.EASYSCADA)]
        [Description("Path to tag of the control")]
        [EditorAttribute(typeof(PathToTagPropertyEditor), typeof(UITypeEditor))]
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

        public virtual WriteTrigger WriteTrigger
        {
            get
            {
                if (BaseControl is ISupportWriteSingleTag)
                    return (BaseControl as ISupportWriteSingleTag).WriteTrigger;
                return WriteTrigger.OnEnter;
            }
            set
            {
                if (BaseControl is ISupportWriteSingleTag)
                    (BaseControl as ISupportWriteSingleTag).SetValue(value);
            }
        }

        public virtual int WriteDelay
        {
            get
            {
                if (BaseControl is ISupportWriteSingleTag)
                    return (BaseControl as ISupportWriteSingleTag).WriteDelay;
                return 100;
            }
            set
            {
                if (BaseControl is ISupportWriteSingleTag)
                    (BaseControl as ISupportWriteSingleTag).SetValue(value);
            }
        }

        
        public virtual WriteTagCommandCollection WriteTagCommands
        {
            get
            {
                if (BaseControl is ISupportWriteMultiTag)
                    return (BaseControl as ISupportWriteMultiTag).WriteTagCommands;
                return null;
            }
            set
            {
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
