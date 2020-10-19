using EasyScada.Core;
using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Windows.Forms;

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
        protected ITypeDescriptorContext typeDescriptorContext;

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
            if (BaseControl is ISupportScale)
            {
                //Scale category
                actionItems.Add(new DesignerActionHeaderItem(DesignerCategory.SCALE));
                actionItems.Add(new DesignerActionPropertyItem("EnableScale", "Enable Scale", DesignerCategory.SCALE, ""));
                actionItems.Add(new DesignerActionPropertyItem("Gain", "Gain", DesignerCategory.SCALE, ""));
                actionItems.Add(new DesignerActionPropertyItem("Offset", "Offset", DesignerCategory.SCALE, ""));
            }

            if (BaseControl is ISupportWriteSingleTag)
            {
                actionItems.Add(new DesignerActionPropertyItem("WriteTrigger", "WriteTrigger", DesignerCategory.EASYSCADA, ""));
                actionItems.Add(new DesignerActionPropertyItem("WriteDelay", "Write Delay", DesignerCategory.EASYSCADA, ""));
            }

            if (BaseControl is ISupportWriteMultiTag)
            {
                actionItems.Add(new DesignerActionPropertyItem("WriteTagCommands", "Write Tag Commands", DesignerCategory.EASYSCADA, ""));
            }

            if (BaseControl is ISupportTag)
            {
                actionItems.Add(new DesignerActionPropertyItem("TagPath", "Tag Path", DesignerCategory.EASYSCADA, "Select path to tag for Easy Scada control"));
                actionItems.Add(new DesignerActionMethodItem(this, "SelectTag", "Select tag...", DesignerCategory.EASYSCADA, "Click here to select tag", true));
            }

            AddExtendActionItems();
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

        [Category(DesignerCategory.EASYSCADA)]
        [Editor(typeof(PathToTagPropertyEditor), typeof(UITypeEditor))]
        [Description("Path to tag of the control")]
        public virtual string TagPath
        {
            get
            {
                if (BaseControl is ISupportTag)
                    return (BaseControl as ISupportTag).TagPath;
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


        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public virtual WriteTagCommandCollection WriteTagCommands
        {
            get
            {
                if (BaseControl is ISupportWriteMultiTag)
                    return (BaseControl as ISupportWriteMultiTag).WriteTagCommands;
                return null;
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
            SelectTagPathDesignerForm form = new SelectTagPathDesignerForm(Component.Site as IServiceProvider);
            if (form.ShowDialog() == DialogResult.OK)
            {
                if (BaseControl is ISupportTag supportTag)
                    BaseControl.SetValue(form.SelectedTagPath, nameof(TagPath));
                designerActionUIservice.Refresh(Component);
            }
        }

        #endregion
    }
}
