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
        #region Private members
        DisplayMode _displayMode = DisplayMode.Value;
        #endregion


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

        #region Properties
        [Description("Select driver connector for control")]
        [Browsable(true), Category(DesignerCategory.EASYSCADA)]
        public DisplayMode DisplayMode
        {
            get => _displayMode;
            set
            {
                if (_displayMode != value)
                {
                    _displayMode = value;
                    if (!DesignMode && LinkedTag != null)
                    {
                        if (_displayMode == DisplayMode.FullPath)
                        {
                            this.SetInvoke((x) =>
                            {
                                x.Text = LinkedTag.Path;
                            });
                        }
                        else if (_displayMode == DisplayMode.Name)
                        {
                            this.SetInvoke((x) =>
                            {
                                x.Text = LinkedTag.Path;
                            });
                        }
                        else
                        {
                            OnTagValueChanged(LinkedTag, new TagValueChangedEventArgs(LinkedTag, null, LinkedTag.Value));
                            OnTagQualityChanged(LinkedTag, new TagQualityChangedEventArgs(LinkedTag, Quality.Uncertain, LinkedTag.Quality));
                        }
                    }
                }
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

                if (_displayMode == DisplayMode.FullPath)
                {
                    this.SetInvoke((x) =>
                    {
                        x.Text = LinkedTag.Path;
                    });
                }
                else if (_displayMode == DisplayMode.Name)
                {
                    this.SetInvoke((x) =>
                    {
                        x.Text = LinkedTag.Path;
                    });
                }
            }
        }

        private void OnTagQualityChanged(object sender, TagQualityChangedEventArgs e)
        {

            this.SetInvoke((x) =>
            {
                if (_displayMode == DisplayMode.Quality)
                {
                    if (x.Text != e.NewQuality.ToString())
                        x.Text = e.NewQuality.ToString();
                }
                else if (_displayMode == DisplayMode.TimeStamp)
                {
                    string timeStamp = LinkedTag.TimeStamp.ToString("yyyy-MM-dd HH:mm:ss");
                    if (x.Text != timeStamp)
                    {
                        x.Text = timeStamp;
                    }
                }
            });

        }

        private void OnTagValueChanged(object sender, TagValueChangedEventArgs e)
        {
            this.SetInvoke((x) =>
            {
                if (_displayMode == DisplayMode.Value)
                {
                    if (x.Text != e.NewValue)
                        x.Text = e.NewValue;
                }
                else if (_displayMode == DisplayMode.TimeStamp)
                {
                    string timeStamp = LinkedTag.TimeStamp.ToString("yyyy-MM-dd HH:mm:ss");
                    if (x.Text != timeStamp)
                    {
                        x.Text = timeStamp;
                    }
                }
            });
        }

        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);
        }
        #endregion
    }
}
