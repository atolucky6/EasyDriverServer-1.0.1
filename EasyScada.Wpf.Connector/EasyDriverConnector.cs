using EasyScada.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace EasyScada.Wpf.Connector
{
    public class EasyDriverConnector : Grid, ISupportInitialize, IEasyDriverConnector
    {
        #region Static

        static IEasyDriverConnector EasyDriverConnectorCore
        {
            get { return EasyDriverConnectorProvider.GetEasyDriverConnector(); }
        }

        #endregion

        #region Constructors

        public EasyDriverConnector() : base()
        {

        }

        #endregion

        #region Public members

        [Browsable(false)]
        public bool IsDisposed { get; private set; }

        [Description("Set server address for connector")]
        [Browsable(true), Category("Easy Scada")]
        public string ServerAddress
        {
            get { return EasyDriverConnectorCore.ServerAddress; }
            set { EasyDriverConnectorCore.ServerAddress = value; }
        }

        [Description("Set port number for connector")]
        [Browsable(true), Category("Easy Scada")]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Editor(typeof(TestEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public ushort Port
        {
            get { return EasyDriverConnectorCore.Port; }
            set { EasyDriverConnectorCore.Port = value; }
        }

        [Description("Set communication mode for connector")]
        [Browsable(true), Category("Easy Scada")]
        public CommunicationMode CommunicationMode
        {
            get { return EasyDriverConnectorCore.CommunicationMode; }
            set { EasyDriverConnectorCore.CommunicationMode = value; }
        }

        [Description("Set refresh rate for connector")]
        [Browsable(true), Category("Easy Scada")]
        public int RefreshRate
        {
            get { return EasyDriverConnectorCore.RefreshRate; }
            set { EasyDriverConnectorCore.RefreshRate = value; }
        }

        [Browsable(true), Category("Easy Scada")]
        public List<DataLogger> DataLoggers { get;set; }

        [Browsable(false)]
        public ConnectionStatus ConnectionStatus
        {
            get { return EasyDriverConnectorCore.ConnectionStatus; }
        }

        [Browsable(false)]
        public bool IsSubscribed { get; private set; }

        [Browsable(false)]
        public bool IsStarted { get; private set; }

        #endregion

        #region Events

        public event EventHandler Started;
        public event EventHandler<ConnectionStatusChangedEventArgs> ConnectionStatusChaged;
        public event EventHandler ConnectionSlow;

        #endregion

        #region Public methods

        public ITag GetTag(string pathToTag)
        {
            return EasyDriverConnectorCore.GetTag(pathToTag);
        }

        public WriteResponse WriteTag(string pathToTag, string value)
        {
            return EasyDriverConnectorCore.WriteTag(pathToTag, value);
        }

        public async Task<WriteResponse> WriteTagAsync(string pathToTag, string value)
        {
            return await EasyDriverConnectorCore.WriteTagAsync(pathToTag, value);
        }

        public List<WriteResponse> WriteMultiTag(List<WriteCommand> writeCommands)
        {
            return EasyDriverConnectorCore.WriteMultiTag(writeCommands);
        }

        public async Task<List<WriteResponse>> WriteMultiTagAsync(List<WriteCommand> writeCommands)
        {
            return await EasyDriverConnectorCore.WriteMultiTagAsync(writeCommands);
        }

        public override void EndInit()
        {
            base.EndInit();
            Start();
        }

        public void Start()
        {
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                if (!EasyDriverConnectorCore.IsStarted)
                {
                    EasyDriverConnectorCore.ConnectionSlow += (s, e) => { ConnectionSlow?.Invoke(this, e); };
                    EasyDriverConnectorCore.ConnectionStatusChaged += (s, e) => { ConnectionStatusChaged?.Invoke(this, e); };
                    EasyDriverConnectorCore.Started += (s, e) => 
                    {
                        Started?.Invoke(this, e);
                    };
                    EasyDriverConnectorCore.Start();
                }
            }
        }

        public void Stop()
        {

        }

        private void OnDisposed(object sender, EventArgs e)
        {
            EasyDriverConnectorCore.Dispose();
            IsDisposed = true;
        }

        public void Dispose()
        {
        }

        #endregion
    }
}
