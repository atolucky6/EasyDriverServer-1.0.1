using System;
using System.Drawing.Design;
using System.ComponentModel;
using System.Windows.Forms;
using System.Security.Permissions;
using System.Threading.Tasks;
using EasyScada.Core;
using System.Drawing;

namespace EasyScada.Winforms.Controls
{

    [ToolboxItem(true)]
    [DefaultEvent("TextChanged")]
    [DefaultBindingProperty("Text")]
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    [Designer(typeof(EasyTextBoxDesigner))]
    public class EasyTextBox : TextBox, ISupportConnector, ISupportTag, ISupportWriteSingleTag, ISupportInitialize, IAuthorizeControl
    {
        #region ISupportConnector
        [Description("Select driver connector for control")]
        [Browsable(true), Category(DesignerCategory.EASYSCADA)]
        public IEasyDriverConnector Connector => EasyDriverConnectorProvider.GetEasyDriverConnector();
        #endregion

        #region ISupportTag
        [Description("Select path to tag for control")]
        [Browsable(true), Category(DesignerCategory.EASYSCADA)]
        [Editor(typeof(PathToTagPropertyEditor), typeof(UITypeEditor))]
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

        #region ISupportWriteTag

        [Browsable(true), Category(DesignerCategory.EASYSCADA)]
        public WriteTrigger WriteTrigger { get; set; }

        int writeDelay = 200;
        [Browsable(true), Category(DesignerCategory.EASYSCADA)]
        public int WriteDelay
        {
            get { return writeDelay < 0 ? 0 : writeDelay; }
            set
            {
                writeDelay = value;
                writeDelayTimer.Interval = WriteDelay;
            }
        }

        public event EventHandler<TagWritingEventArgs> TagWriting;
        public event EventHandler<TagWritedEventArgs> TagWrited;
        #endregion

        #region IAuthorizeControl
        [Browsable(true), TypeConverter(typeof(RoleConverter)), Category(DesignerCategory.EASYSCADA)]
        public string Role { get; set; }

        [Browsable(false)]
        public bool IsAuthenticated { get; protected set; }
        #endregion

        #region ISupportInitialize
        public void BeginInit()
        {
        }

        public void EndInit()
        {
            if (!DesignMode)
            {
                AuthenticateHelper.Logouted += (s, e) =>
                {
                    IsAuthenticated = false;
                };

                if (Connector.IsStarted)
                    OnConnectorStarted(null, EventArgs.Empty);
                else
                    Connector.Started += OnConnectorStarted;
            }
        }
        #endregion

        #region Constructors
        public EasyTextBox() : base()
        {
            writeDelayTimer.Elapsed += WriteDelayTimer_Elapsed;
            Text = "";
        }
        #endregion

        #region Fields
        private System.Timers.Timer writeDelayTimer = new System.Timers.Timer();
        private string writeValue;
        private Label dropDownLabel = new Label();
        private Panel dropDownPanel = new Panel();
        private Color dropDownBackColor = DefaultBackColor;
        private Color dropDownForeColor = DefaultForeColor;
        private BorderStyle dropDownBorderStyle = BorderStyle.FixedSingle;
        private Font dropDownFont = DefaultFont;
        private DropDownDirection dropDownDirection = DropDownDirection.Bottom;
        private string oldValue;
        #endregion

        #region Properties
        [Browsable(true), Category(DesignerCategory.EASYSCADA)]
        public DropDownDirection DropDownDirection
        {
            get => dropDownDirection;
            set
            {
                if (dropDownDirection != value)
                {
                    dropDownDirection = value;
                    UpdateDropDownPosition();
                }
            }
        }

        [Browsable(true), Category(DesignerCategory.EASYSCADA)]
        public Color DropDownBackColor
        {
            get => dropDownBackColor;
            set
            {
                if (dropDownBackColor != value)
                {
                    dropDownBackColor = value;
                    UpdateDropDownPosition();
                }
            }
        }

        [Browsable(true), Category(DesignerCategory.EASYSCADA)]
        public Color DropDownForeColor
        {
            get => dropDownForeColor;
            set
            {
                if (dropDownForeColor != value)
                {
                    dropDownForeColor = value;
                    UpdateDropDownPosition();
                }
            }
        }

        [Browsable(true), Category(DesignerCategory.EASYSCADA)]
        public BorderStyle DropDownBorderStyle
        {
            get => dropDownBorderStyle;
            set
            {
                if (dropDownBorderStyle != value)
                {
                    dropDownBorderStyle = value;
                    UpdateDropDownPosition();
                }
            }
        }

        [Browsable(true), Category(DesignerCategory.EASYSCADA)]
        public Font DropDownFont
        {
            get => dropDownFont;
            set
            {
                if (dropDownFont != value)
                {
                    dropDownFont = value;
                    UpdateDropDownPosition();
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

                this.SetInvoke((x) =>
                {
                    x.Text = LinkedTag.Value;
                });

                dropDownLabel.SetInvoke(x =>
                {
                    UpdateDropDownPosition();
                    if (x.Text != LinkedTag.Value)
                        x.Text = LinkedTag.Value;
                });
            }
        }

        private void OnTagQualityChanged(object sender, TagQualityChangedEventArgs e)
        {

        }

        private void OnTagValueChanged(object sender, TagValueChangedEventArgs e)
        {
            this.SetInvoke((x) =>
            {
                if (x.Text != e.NewValue && !Focused)
                    x.Text = e.NewValue;
            });

            dropDownLabel.SetInvoke(x =>
            {
                if (x.Text != e.NewValue)
                {
                    x.Text = e.NewValue;
                    UpdateDropDownPosition();
                }
            });
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {         
            base.OnKeyDown(e);
            if (e.KeyCode == Keys.Enter)
            {

                if (WriteTrigger == WriteTrigger.OnEnter)
                {
                    if (CheckAuthenticate())
                    {
                        writeDelayTimer.Stop();
                        writeDelayTimer.Start();
                    }
                    else
                    {
                        if (LinkedTag != null && LinkedTag.Value != Text)
                            Text = LinkedTag.Value;
                    }
                }
            }
            else if (e.KeyCode == Keys.Escape)
            {
                if (LinkedTag != null && LinkedTag.Value != Text)
                    Text = LinkedTag.Value;
            }
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {

            base.OnKeyUp(e);
            if (oldValue != Text)
            {
                if (WriteTrigger == WriteTrigger.ValueChanged)
                {
                    if (CheckAuthenticate())
                    {
                        writeDelayTimer.Stop();
                        writeDelayTimer.Start();
                    }
                    else
                    {
                        if (LinkedTag != null && LinkedTag.Value != Text)
                            Text = LinkedTag.Value;
                    }
                }
            }

        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            base.OnKeyPress(e);
            oldValue = Text;
        }

        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            if (WriteTrigger == WriteTrigger.LostFocus)
            {
                if (CheckAuthenticate())
                {
                    writeDelayTimer.Stop();
                    writeDelayTimer.Start();
                }
            }

            if (LinkedTag != null && LinkedTag.Value != Text)
                Text = LinkedTag.Value;


            if (dropDownPanel != null && dropDownPanel.Visible)
            {
                dropDownPanel.Visible = false;
                dropDownPanel.SendToBack();
            }
        }

        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            UpdateDropDownPosition();

            if (DropDownDirection != DropDownDirection.None)
            {
                if (dropDownPanel.Parent != null)
                    Parent.Controls.Remove(dropDownPanel);

                if (!dropDownPanel.Visible)
                    dropDownPanel.Visible = true;

                if (dropDownPanel.Parent == null)
                {
                    Parent.Controls.Add(dropDownPanel);
                    Parent.Controls.SetChildIndex(dropDownPanel, 0);
                }
            }
            else
            {
                dropDownPanel.Visible = false;
                dropDownPanel.SendToBack();
            }
        }
        #endregion

        #region Methods
        private bool CheckAuthenticate()
        {
            if (!IsAuthenticated)
            {
                if (!AuthenticateHelper.Authenticate(Role))
                {
                    AuthenticateForm form = new AuthenticateForm();
                    if (form.ShowDialog() != DialogResult.OK)
                        return false;
                }

                IsAuthenticated = true;
            }
            return IsAuthenticated;
        }

        private void WriteDelayTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            writeDelayTimer.Stop();
            writeValue = Text;
            WriteTag(writeValue);
        }

        private async void WriteTag(string writeValue)
        {
            if (Connector != null && Connector.IsStarted && LinkedTag != null && LinkedTag.Value != writeValue && IsNumber(writeValue))
            {
                await Task.Delay(WriteDelay);

                TagWriting?.Invoke(this, new TagWritingEventArgs(LinkedTag, writeValue));
                var res = await LinkedTag.WriteAsync(writeValue);
                TagWrited?.Invoke(this, new TagWritedEventArgs(LinkedTag, res.IsSuccess ? Quality.Good : Quality.Bad, writeValue));
            }
        }

        private bool IsNumber(string writeValue)
        {
            return decimal.TryParse(writeValue, out decimal value);
        }

        private void UpdateDropDownPosition()
        {
            if (dropDownPanel == null)
                dropDownPanel = new Panel();
            dropDownPanel.BackColor = DropDownBackColor;
            dropDownPanel.ForeColor = DropDownForeColor;
            dropDownPanel.Font = DropDownFont;
            dropDownPanel.BorderStyle = DropDownBorderStyle;

            if (dropDownLabel.Parent == null)
                dropDownPanel.Controls.Add(dropDownLabel);

            dropDownLabel.BackColor = DropDownBackColor;
            dropDownLabel.ForeColor = DropDownForeColor;
            dropDownLabel.Font = DropDownFont;
            dropDownLabel.Dock = DockStyle.Right;
            dropDownLabel.TextAlign = ContentAlignment.MiddleRight;
            dropDownLabel.Width = dropDownLabel.PreferredSize.Width;
           
            if (Width < dropDownLabel.PreferredSize.Width)
                dropDownPanel.Width = dropDownLabel.PreferredSize.Width;
            else
                dropDownPanel.Width = Width;

            if (Height < dropDownLabel.PreferredSize.Height)
                dropDownPanel.Height = dropDownLabel.PreferredSize.Height;
            else
                dropDownPanel.Height = Height;

            Point location = Point.Empty;
            switch (DropDownDirection)
            {
                case DropDownDirection.Bottom:
                    location.X = Location.X;
                    location.Y = Location.Y + Height;
                    break;
                case DropDownDirection.Top:
                    location.X = Location.X;
                    location.Y = Location.Y - dropDownPanel.Height;
                    break;
                case DropDownDirection.Left:
                    location.X = Location.X - dropDownPanel.Width;
                    location.Y = Location.Y + Height / 2 - dropDownPanel.Height / 2;
                    break;
                case DropDownDirection.Right:
                    location.X = Location.X + Width;
                    location.Y = Location.Y + Height / 2 - dropDownPanel.Height / 2;
                    break;
                default:
                    dropDownPanel.Visible = false;
                    break;
            }
            dropDownPanel.Location = location;
        }
        #endregion
    }
}
