using System;
using System.ComponentModel;
using EasyScada.Core;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Security.Permissions;

namespace EasyScada.Winforms.Controls
{
    [ToolboxItem(true)]
    [Designer(typeof(EasyLabelDesigner))]
    public class EasyLabel : Label, ISupportConnector, ISupportTag, ISupportInitialize
    {
        #region Constructors
        public EasyLabel() : base()
        {

        }
        #endregion

        #region ISupportConnector
        [Description("Select driver connector for control")]
        [Browsable(true), Category(DesignerCategory.EASYSCADA)]
        public IEasyDriverConnector Connector => EasyDriverConnectorProvider.GetEasyDriverConnector();
        #endregion

        #region ISupportTag
        [Editor(typeof(PathToTagPropertyEditor), typeof(UITypeEditor))]
        [Description("Select path to tag for control")]
        [Browsable(true), Category(DesignerCategory.EASYSCADA)]
        public string TagPath { get; set; }

        ITag linkedTag;
        [Browsable(false)]
        public ITag LinkedTag
        {
            get
            {
                if (linkedTag == null)
                {
                    linkedTag = Connector?.GetTag(TagPath);
                }
                else
                {
                    if (linkedTag.Path != TagPath)
                        linkedTag = Connector?.GetTag(TagPath);
                }
                return linkedTag;
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
                if (Connector.IsStarted)
                    OnConnectorStarted(null, EventArgs.Empty);
                else
                    Connector.Started += OnConnectorStarted;
            }
        }
        #endregion

        #region Event handlers
        private void OnConnectorStarted(object sender, EventArgs e)
        {
            if (LinkedTag != null)
            {
                OnTagValueChanged(LinkedTag, new TagValueChangedEventArgs(LinkedTag, null, LinkedTag.Value));
                OnTagQualityChanged(LinkedTag, new TagQualityChangedEventArgs(LinkedTag, Quality.Uncertain, LinkedTag.Quality));
                LinkedTag.ValueChanged += OnTagValueChanged;
                LinkedTag.QualityChanged += OnTagQualityChanged;
            }
        }

        private void OnTagQualityChanged(object sender, TagQualityChangedEventArgs e)
        {

        }

        private void OnTagValueChanged(object sender, TagValueChangedEventArgs e)
        {
            this.SetInvoke((x) =>
            {
                if (x.Text != e.NewValue)
                    x.Text = e.NewValue;
            });
        }
        #endregion
    }
}
