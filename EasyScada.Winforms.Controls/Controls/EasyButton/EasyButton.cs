using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using EasyScada.Core;

namespace EasyScada.Winforms.Controls
{
    [Designer(typeof(EasyButtonDesigner))]
    public class EasyButton : Button, ISupportConnector, ISupportWriteMultiTag, IAuthorizeControl, ISupportInitialize
    {
        #region Constructors
        public EasyButton() : base()
        {
            _writeTagCommands = new WriteTagCommandCollection();
            FlatStyle = FlatStyle.Flat;
        }
        #endregion

        #region ISupportConnector
        [Description("Select driver connector for control")]
        [Browsable(true), Category(DesignerCategory.EASYSCADA)]
        public IEasyDriverConnector Connector => EasyDriverConnectorProvider.GetEasyDriverConnector();
        #endregion

        #region IAuthorizeControl
        [Browsable(true), TypeConverter(typeof(RoleConverter)), Category(DesignerCategory.EASYSCADA)]
        public string Role { get; set; }

        [Browsable(false)]
        public bool IsAuthenticated { get; protected set; }
        #endregion

        #region Fields
        private WriteTagCommandCollection _writeTagCommands;
        private Color borderColor = DefaultBackColor;
        private int borderThickness = 2;
        private bool isPressed;
        #endregion

        #region Public properties
        /// <summary>
        /// Gets the collection of write tag command specifications.
        /// </summary>
        [Category(DesignerCategory.EASYSCADA)]
        [Description("Collection of write tag command specifications.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public WriteTagCommandCollection WriteTagCommands => _writeTagCommands;

        [Category(DesignerCategory.EASYSCADA)]
        public Color BorderColor
        {
            get => borderColor;
            set
            {
                if (borderColor != value)
                {
                    borderColor = value;
                    Invalidate();
                }
            }
        }

        [Category(DesignerCategory.EASYSCADA)]
        public int BorderThickness
        {
            get => borderThickness;
            set
            {
                if (borderThickness != value)
                {
                    borderThickness = value;
                    if (borderThickness < 0)
                        borderThickness = 0;
                    Invalidate();
                }
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
                AuthenticateHelper.Logouted += (s, e) =>
                {
                    IsAuthenticated = false;
                };
            }
        }
        #endregion

        #region Private methods
        private async void OnWriteTags()
        {
            if (WriteTagCommands != null && WriteTagCommands.Count > 0)
            {
                List<WriteCommand> commands = new List<WriteCommand>();
                foreach (var cmd in WriteTagCommands)
                {
                    if (cmd.Enabled)
                    {
                        string[] splitPath = cmd.TagPath.Split('/');
                        var writeCommand = new WriteCommand()
                        {
                            Prefix = string.Join("/", splitPath, 0, splitPath.Length - 1),
                            TagName = splitPath[splitPath.Length - 1],
                            Value = cmd.WriteValue,
                            WritePiority = WritePiority.High,
                            WriteMode = WriteMode.WriteAllValue,
                        };
                        if (cmd.AllowResetValue)
                        {
                            var nextCommand = new WriteCommand()
                            {
                                Prefix = string.Join("/", splitPath, 0, splitPath.Length - 1),
                                TagName = splitPath[splitPath.Length - 1],
                                Value = cmd.ResetValue,
                                Delay = cmd.WriteDelay,
                                WritePiority = WritePiority.High,
                                WriteMode = WriteMode.WriteAllValue,
                            };
                            if (writeCommand.NextCommands == null)
                                writeCommand.NextCommands = new List<WriteCommand>();
                            writeCommand.NextCommands.Add(nextCommand);
                        }
                        commands.Add(writeCommand);
                    }
                }
                await Connector.WriteMultiTagAsync(commands);
            }
        }

        protected override void OnClick(EventArgs e)
        {
            if (!IsAuthenticated)
            {
                if (!AuthenticateHelper.Authenticate(Role))
                {
                    AuthenticateForm form = new AuthenticateForm();
                    if (form.ShowDialog() != DialogResult.OK)
                        return;
                }

                IsAuthenticated = true;
            }

            if (IsAuthenticated)
            {
                base.OnClick(e);
                if (Enabled)
                {
                    OnWriteTags();
                }
            }
        }

        protected override void OnMouseDown(MouseEventArgs mevent)
        {
            isPressed = true;
            base.OnMouseDown(mevent);
        }

        protected override void OnMouseUp(MouseEventArgs mevent)
        {
            isPressed = false;
            base.OnMouseUp(mevent);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (borderThickness > 0 && FlatStyle == FlatStyle.Flat)
            {
                if (isPressed)
                {
                    ControlPaint.DrawBorder(e.Graphics, this.ClientRectangle,
                        BorderColor, BorderThickness, ButtonBorderStyle.Inset,
                        BorderColor, BorderThickness, ButtonBorderStyle.Inset,
                        BorderColor, BorderThickness, ButtonBorderStyle.Inset,
                        BorderColor, BorderThickness, ButtonBorderStyle.Inset);
                }
                else
                {
                    ControlPaint.DrawBorder(e.Graphics, this.ClientRectangle,
                        BorderColor, BorderThickness, ButtonBorderStyle.Outset,
                        BorderColor, BorderThickness, ButtonBorderStyle.Outset,
                        BorderColor, BorderThickness, ButtonBorderStyle.Outset,
                        BorderColor, BorderThickness, ButtonBorderStyle.Outset);
                }
            }
        }
        #endregion
    }
}
