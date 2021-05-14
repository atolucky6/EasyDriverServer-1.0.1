using EasyScada.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Security.Permissions;
using System.Timers;

namespace EasyScada.Winforms.Controls
{
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    [Designer(typeof(EasySyncTagDesigner))]
    public partial class EasySyncTag : Component, ISupportInitialize
    {
        #region Constructors
        public EasySyncTag()
        {
            InitializeComponent();
        }

        public EasySyncTag(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }
        #endregion

        #region Public properties
        [Browsable(true), Category(DesignerCategory.EASYSCADA)]
        public bool Enabled { get; set; } = true;

        SyncTargetCollection _targets = new SyncTargetCollection();
        [Description("Select path to tag for control")]
        [Category(DesignerCategory.EASYSCADA), Browsable(true), TypeConverter(typeof(CollectionEditor)), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public SyncTargetCollection Targets { get => _targets; }

        [Browsable(true), Category(DesignerCategory.EASYSCADA)]
        public int SyncInterval { get; set; } = 200;
        #endregion

        #region Events
        #endregion

        #region Members
        protected List<WriteCommand> _syncCommands;
        protected Timer _syncTimer;
        #endregion

        #region ISupportConnector
        [Description("Select driver connector for control")]
        [Browsable(false), Category(DesignerCategory.EASYSCADA)]
        public IEasyDriverConnector Connector => EasyDriverConnectorProvider.GetEasyDriverConnector();
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
            if (_targets.Count > 0)
            {
                _syncTimer = new Timer();
                _syncTimer.Interval = SyncInterval;
                _syncTimer.Elapsed += DoSync;
                _syncTimer.Start();
            }
        }

        private void DoSync(object sender, ElapsedEventArgs e)
        {
            _syncTimer.Stop();
            _syncTimer.Interval = SyncInterval;
            try
            {
                if (Enabled)
                {
                    _syncCommands = new List<WriteCommand>();

                    foreach (var item in _targets)
                    {
                        if (item.TargetTag != null && item.TargetTag.Quality == Quality.Good && 
                            item.SourceTag != null && item.SourceTag.Quality == Quality.Good &&
                            item.SourceTag.Value != item.TargetTag.Value)
                        {
                            WriteCommand cmd = new WriteCommand()
                            {
                                Prefix = item.TargetTag.Parent.Path,
                                TagName = item.TargetTag.Name,
                                WriteMode = WriteMode.WriteLatestValue,
                                WritePiority = WritePiority.High,
                                Value = item.SourceTag.Value
                            };
                            _syncCommands.Add(cmd);
                        }
                    }

                    if (_syncCommands.Count > 0)
                    {
                        Connector.WriteMultiTag(_syncCommands);
                    }
                }
            }
            catch { }
            finally {  _syncTimer.Start(); }
        }
        #endregion
    }
}
