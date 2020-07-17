using System;
using System.Drawing;
using System.Drawing.Design;
using System.ComponentModel;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using EasyScada.Winforms.Connector;
using System.Threading.Tasks;
using System.Security.Permissions;

namespace EasyScada.Winforms.Controls
{
    /// <summary>
    /// Defines state and events for a single command.
    /// </summary>
    [ToolboxItem(true)]
    [DefaultEvent("TagWrited")]
    [DefaultProperty("WriteValue")]
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    [Designer(typeof(EasyWriteTagCommandDesigner))]
    [DesignerCategory("code")]
    [Description("Defines state and events for a single command.")]
    [ClassInterface(ClassInterfaceType.AutoDispatch)]
    [ComVisible(true)]
    public class EasyWriteTagCommand : Component, IEasyCommand, INotifyPropertyChanged, ISupportConnector, ISupportTag
    {
        #region ISupportConnector

        EasyDriverConnector easyDriverConnector;
        [Description("Select driver connector for control")]
        [Browsable(true), Category(DesignerCategory.EASYSCADA)]
        public EasyDriverConnector Connector
        {
            get { return easyDriverConnector; }
            set
            {
                if (value != null)
                {
                    easyDriverConnector = value;
                    if (!DesignMode)
                    {
                        if (easyDriverConnector.IsStarted)
                            EasyDriverConnector_Started(null, EventArgs.Empty);
                        else
                            easyDriverConnector.Started += EasyDriverConnector_Started;
                    }
                }
            }
        }

        private void EasyDriverConnector_Started(object sender, EventArgs e)
        {

        }

        #endregion

        #region ISupportTag

        [Description("Select path to tag for control")]
        [Browsable(true), Category(DesignerCategory.EASYSCADA)]
        [TypeConverter(typeof(EasyScadaTagPathConverter))]
        public string PathToTag { get; set; }

        ITag linkedTag;
        [Browsable(false)]
        public ITag LinkedTag
        {
            get
            {
                if (linkedTag == null)
                {
                    linkedTag = Connector?.GetTag(PathToTag);
                }
                else
                {
                    if (linkedTag.Path != PathToTag)
                        linkedTag = Connector?.GetTag(PathToTag);
                }
                return linkedTag;
            }
        }

        #endregion

        #region Instance Fields
        private bool _enabled;
        private bool _checked;
        private CheckState _checkState;
        private string _text;
        private string _extraText;
        private string _textLine1;
        private string _textLine2;
        private Image _imageSmall;
        private Image _imageLarge;
        private Color _imageTransparentColor;
        private object _tag;
        private string _writeValue;
        private int _writeDelay;
        #endregion

        #region Events
        /// <summary>
        /// Occurs when the command needs executing.
        /// </summary>
        [Category("Action")]
        [Description("Occurs when the command needs executing.")]
        public event EventHandler Execute;

        /// <summary>
        /// Occurs when a property has changed value.
        /// </summary>
        [Category("Property Changed")]
        [Description("Occurs when the value of property has changed.")]
        public event PropertyChangedEventHandler PropertyChanged;

        public event EventHandler<TagWritingEventArgs> TagWriting;

        public event EventHandler<TagWritedEventArgs> TagWrited;
        #endregion

        #region Identity
        /// <summary>
        /// Initialize a new instance of the EasyWriteTagCommand class.
        /// </summary>
        public EasyWriteTagCommand()
        {
            _enabled = true;
            _checked = false;
            _checkState = CheckState.Unchecked;
            _text = string.Empty;
            _extraText = string.Empty;
            _textLine1 = string.Empty;
            _textLine2 = string.Empty;
            _imageSmall = null;
            _imageLarge = null;
            _imageTransparentColor = Color.Empty;
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
            }

            base.Dispose(disposing);
        }
        #endregion

        #region Public
        /// <summary>
        /// Gets and sets the enabled state of the command.
        /// </summary>
        [Bindable(true)]
        [Category("Behavior")]
        [Description("Indicates whether the command is enabled.")]
        [DefaultValue(true)]
        public bool Enabled
        {
            get { return _enabled; }

            set
            {
                if (_enabled != value)
                {
                    _enabled = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("Enabled"));
                }
            }
        }

        /// <summary>
        /// Gets and sets the checked state of the command.
        /// </summary>
        [Bindable(false)]
        [Browsable(false)]
        [Category("Behavior")]
        [Description("Indicates whether the command is in the checked state.")]
        [DefaultValue(false)]
        public bool Checked
        {
            get { return _checked; }

            set
            {
                if (_checked != value)
                {
                    // Store new values
                    _checked = value;
                    _checkState = (_checked ? CheckState.Checked : CheckState.Unchecked);

                    // Generate events
                    OnPropertyChanged(new PropertyChangedEventArgs("Checked"));
                    OnPropertyChanged(new PropertyChangedEventArgs("CheckState"));
                }
            }
        }

        [Bindable(true)]
        [Browsable(true)]
        [Category(DesignerCategory.EASYSCADA)]
        [DefaultValue("")]
        public string WriteValue
        {
            get { return _writeValue; }
            set { _writeValue = value; }
        }

        [Bindable(true)]
        [Browsable(true)]
        [Category(DesignerCategory.EASYSCADA)]
        public int WriteDelay
        {
            get { return _writeDelay < 0 ? 0 : _writeDelay; }
            set { _writeDelay = value; }
        }

        /// <summary>
        /// Gets and sets the check state of the command.
        /// </summary>
        [Bindable(true)]
        [Browsable(false)]
        [Category("Behavior")]
        [Description("Indicates the checked state of the command.")]
        [DefaultValue(typeof(CheckState), "Unchecked")]
        public CheckState CheckState
        {
            get { return _checkState; }

            set
            {
                if (_checkState != value)
                {
                    // Store new values
                    _checkState = value;
                    bool newChecked = (_checkState != CheckState.Unchecked);
                    bool checkedChanged = (_checked != newChecked);
                    _checked = newChecked;

                    // Generate events
                    if (checkedChanged)
                        OnPropertyChanged(new PropertyChangedEventArgs("Checked"));

                    OnPropertyChanged(new PropertyChangedEventArgs("CheckState"));
                }
            }
        }

        /// <summary>
        /// Gets and sets the command text.
        /// </summary>
        [Bindable(true)]
        [Browsable(false)]
        [Localizable(true)]
        [Category("Appearance")]
        [Description("Command text.")]
        [Editor("System.ComponentModel.Design.MultilineStringEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
        public string Text
        {
            get { return _text; }

            set
            {
                if (_text != value)
                {
                    _text = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("Text"));
                }
            }
        }

        private void ResetText()
        {
            Text = string.Empty;
        }

        private bool ShouldSerializeText()
        {
            return !string.IsNullOrEmpty(Text);
        }

        /// <summary>
        /// Gets and sets the command extra text.
        /// </summary>
        [Bindable(true)]
        [Browsable(false)]
        [Localizable(true)]
        [Category("Appearance")]
        [Description("Command extra text.")]
        [Editor("System.ComponentModel.Design.MultilineStringEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
        public string ExtraText
        {
            get { return _extraText; }

            set
            {
                if (_extraText != value)
                {
                    _extraText = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("ExtraText"));
                }
            }
        }

        private void ResetExtraText()
        {
            ExtraText = string.Empty;
        }

        private bool ShouldSerializeExtraText()
        {
            return !string.IsNullOrEmpty(ExtraText);
        }

        /// <summary>
        /// Gets and sets the command text line 1 for use in EasyRibbon.
        /// </summary>
        [Bindable(true)]
        [Browsable(false)]
        [Localizable(true)]
        [Category("Appearance")]
        [Description("Command text line 1 for use in EasyRibbon.")]
        public string TextLine1
        {
            get { return _textLine1; }

            set
            {
                if (_textLine1 != value)
                {
                    _textLine1 = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("TextLine1"));
                }
            }
        }

        private void ResetTextLine1()
        {
            TextLine1 = string.Empty;
        }

        private bool ShouldSerializeTextLine1()
        {
            return !string.IsNullOrEmpty(TextLine1);
        }

        /// <summary>
        /// Gets and sets the command text line 2 for use in EasyRibbon.
        /// </summary>
        [Bindable(true)]
        [Browsable(false)]
        [Localizable(true)]
        [Category("Appearance")]
        [Description("Command text line 2 for use in EasyRibbon.")]
        public string TextLine2
        {
            get { return _textLine2; }

            set
            {
                if (_textLine2 != value)
                {
                    _textLine2 = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("TextLine2"));
                }
            }
        }

        private void ResetTextLine2()
        {
            TextLine2 = string.Empty;
        }

        private bool ShouldSerializeTextLine2()
        {
            return !string.IsNullOrEmpty(TextLine2);
        }

        /// <summary>
        /// Gets and sets the command small image.
        /// </summary>
        [Bindable(true)]
        [Browsable(false)]
        [Localizable(true)]
        [Category("Appearance")]
        [Description("Command small image.")]
        public Image ImageSmall
        {
            get { return _imageSmall; }

            set
            {
                if (_imageSmall != value)
                {
                    _imageSmall = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("ImageSmall"));
                }
            }
        }

        private void ResetImageSmall()
        {
            ImageSmall = null;
        }

        private bool ShouldSerializeImageSmall()
        {
            return (ImageSmall != null);
        }

        /// <summary>
        /// Gets and sets the command large image.
        /// </summary>
        [Bindable(true)]
        [Browsable(false)]
        [Localizable(true)]
        [Category("Appearance")]
        [Description("Command large image.")]
        public Image ImageLarge
        {
            get { return _imageLarge; }

            set
            {
                if (_imageLarge != value)
                {
                    _imageLarge = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("ImageLarge"));
                }
            }
        }

        private void ResetImageLarge()
        {
            ImageLarge = null;
        }

        private bool ShouldSerializeImageLarge()
        {
            return (ImageLarge != null);
        }

        /// <summary>
        /// Gets and sets the command image transparent color.
        /// </summary>
        [Bindable(true)]
        [Localizable(true)]
        [Browsable(false)]
        [Category("Appearance")]
        [Description("Command image transparent color.")]
        [EasyDefaultColorAttribute()]
        public Color ImageTransparentColor
        {
            get { return _imageTransparentColor; }

            set
            {
                if (_imageTransparentColor != value)
                {
                    _imageTransparentColor = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("ImageTransparentColor"));
                }
            }
        }

        /// <summary>
        /// Gets and sets user-defined data associated with the object.
        /// </summary>
        [Category("Data")]
        [Browsable(false)]
        [Description("User-defined data associated with the object.")]
        [TypeConverter(typeof(StringConverter))]
        [DefaultValue(null)]
        public object Tag
        {
            get { return _tag; }
            set { _tag = value; }
        }

        /// <summary>
        /// Generates a Execute event for a button.
        /// </summary>
        public void PerformExecute()
        {
            if (Enabled)
            {
                OnExecute(EventArgs.Empty);
                OnWriteTag(WriteValue);
            }
        }
        #endregion

        #region Protected
        /// <summary>
        /// Raises the Execute event.
        /// </summary>
        /// <param name="e">An EventArgs containing the event data.</param>
        protected virtual void OnExecute(EventArgs e)
        {
            if (Execute != null)
                Execute(this, e);
        }
        
        protected virtual async void OnWriteTag(string writeValue)
        {
            if (Connector != null && Connector.IsStarted && LinkedTag != null && LinkedTag.Value != writeValue && writeValue.IsNumber())
            {
                await Task.Delay(WriteDelay);

                TagWriting?.Invoke(this, new TagWritingEventArgs(LinkedTag, writeValue));
                Quality writeQuality = Quality.Uncertain;

                writeQuality = await LinkedTag.WriteAsync(writeValue);

                TagWrited?.Invoke(this, new TagWritedEventArgs(LinkedTag, writeQuality, writeValue));
            }
        }

        /// <summary>
        /// Raises the PropertyChanged event.
        /// </summary>
        /// <param name="e">A PropertyChangedEventArgs containing the event data.</param>
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, e);
        }
        #endregion
    }

    /// <summary>
    /// Manages a collection of EasyWriteTagCommand instances.
    /// </summary>
    public class EasyWriteTagCommandCollection : TypedCollection<EasyWriteTagCommand>
    {
        #region Public
        /// <summary>
        /// Gets the item with the provided name.
        /// </summary>
        /// <param name="name">Name to find.</param>
        /// <returns>Item with matching name.</returns>
        public override EasyWriteTagCommand this[string name]
        {
            get
            {
                if (!string.IsNullOrEmpty(name))
                {
                    foreach (EasyWriteTagCommand item in this)
                    {
                        string text = item.Text;
                        if (!string.IsNullOrEmpty(text) && (text == name))
                            return item;

                        text = item.ExtraText;
                        if (!string.IsNullOrEmpty(text) && (text == name))
                            return item;
                    }
                }

                return null;
            }
        }
        #endregion
    }
}
