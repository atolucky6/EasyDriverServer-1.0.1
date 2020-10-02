using EasyScada.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security;
using System.Security.Permissions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EasyScada.Winforms.Controls
{
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    [Designer(typeof(EasyDriverConnectorDesigner))]
    public sealed partial class EasyDriverConnector : Component, IDisposable, IEasyDriverConnector, ISupportInitialize
    {
        #region Static

        static IEasyDriverConnector EasyDriverConnectorCore
        {
            get { return EasyDriverConnectorProvider.GetEasyDriverConnector(); }
        }

        #endregion

        #region Constructors

        public EasyDriverConnector()
        {
            InitializeComponent();
        }

        public EasyDriverConnector(IContainer container)
        {
            container.Add(this);
            InitializeComponent();
            if (!DesignMode)
                Disposed += OnDisposed;
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

        [Browsable(false)]
        public ConnectionStatus ConnectionStatus
        {
            get { return EasyDriverConnectorCore.ConnectionStatus; }
        }

        [Browsable(false)]
        public bool IsSubscribed { get; private set; }

        [Browsable(false)]
        public bool IsStarted
        {
            get
            {
                if (EasyDriverConnectorCore == null)
                    return false;
                return EasyDriverConnectorCore.IsStarted;
            }
        }

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

        public Task<List<WriteResponse>> WriteMultiTagAsync(List<WriteCommand> writeCommands)
        {
            return EasyDriverConnectorCore.WriteMultiTagAsync(writeCommands);
        }

        public void BeginInit()
        {

        }

        public void EndInit()
        {
            Start();
        }

        public void Start()
        {
            if (!DesignMode)
            {
                if (!EasyDriverConnectorCore.IsStarted)
                {
                    EasyDriverConnectorCore.ConnectionSlow += (s, e) => { ConnectionSlow?.Invoke(this, e); };
                    EasyDriverConnectorCore.ConnectionStatusChaged += (s, e) => { ConnectionStatusChaged?.Invoke(this, e); };
                    EasyDriverConnectorCore.Started += (s, e) => { Started?.Invoke(this, e); };
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

        #endregion
    }

    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    sealed class EasyDriverConnectorDesigner : ComponentDesigner
    {
        #region Members

        DesignerActionListCollection actionListCollection;
        EasyDriverConnectorDesignerActionList actionList;
        EasyDriverConnector EasyDriverConnector { get { return Component as EasyDriverConnector; } }

        #endregion

        #region Methods

        public override DesignerActionListCollection ActionLists
        {
            get
            {
                if (actionListCollection == null)
                {
                    actionListCollection = new DesignerActionListCollection();
                    actionList = GetActionList();
                    actionListCollection.Add(actionList);
                }
                return actionListCollection;
            }
        }

        private EasyDriverConnectorDesignerActionList GetActionList()
        {
            if (actionList == null)
                actionList = new EasyDriverConnectorDesignerActionList(Component);
            return actionList;
        }

        #endregion
    }

    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    sealed class EasyDriverConnectorDesignerActionList : DesignerActionList
    {
        public EasyDriverConnectorDesignerActionList(IComponent component) : base(component)
        {
            designerActionUIservice = GetService(typeof(DesignerActionUIService)) as DesignerActionUIService;
            BaseControl = component as EasyDriverConnector;
            projectPath = GetCurrentDesignPath();
            debugPath = "\\Debug\\ConnectionSchema.json";
            releasePath = "\\Release\\ConnectionSchema.json";
        }

        #region Members


        readonly string projectPath;
        readonly string debugPath;
        readonly string releasePath;
        bool isBusy;
        readonly EasyDriverConnector BaseControl;
        DesignerActionItemCollection actionItems;
        readonly DesignerActionUIService designerActionUIservice;

        #endregion

        #region Methods

        public override DesignerActionItemCollection GetSortedActionItems()
        {
            if (actionItems == null)
            {
                actionItems = new DesignerActionItemCollection
                {
                    new DesignerActionPropertyItem("ServerAddress", "Server Addrees", "Easy Scada", "Set server address for connector"),
                    new DesignerActionPropertyItem("Port", "Port", "Easy Scada", "Set port number for connector"),
                    new DesignerActionPropertyItem("CommunicationMode", "Communication Mode", "Easy Scada", "Set communication mode for connector"),
                    new DesignerActionPropertyItem("RefreshRate", "Refresh Rate", "Easy Scada", "Set refresh rate for connector"),
                    new DesignerActionMethodItem(this, "UpdateConnectionSchema", "Update Connection Schema", "Easy Scada", "Click here to update tag file", true)
                };
            }
            return actionItems;
        }

        #endregion

        #region Desginer properties

        [Description("Set server address for connector")]
        [Browsable(true), Category("Easy Scada")]
        public string ServerAddress
        {
            get { return BaseControl.ServerAddress; }
            set { SetValue(BaseControl, value); SaveTagFile(); }
        }

        [Description("Set port number for connector")]
        [Browsable(true), Category("Easy Scada")]
        public ushort Port
        {
            get { return BaseControl.Port; }
            set { SetValue(BaseControl, value); SaveTagFile(); }
        }

        [Description("Set communication mode for connector")]
        [Browsable(true), Category("Easy Scada")]
        public CommunicationMode CommunicationMode
        {
            get { return BaseControl.CommunicationMode; }
            set { SetValue(BaseControl, value); SaveTagFile(); }
        }

        [Description("Set refresh rate for connector")]
        [Browsable(true), Category("Easy Scada")]
        public int RefreshRate
        {
            get { return BaseControl.RefreshRate; }
            set { SetValue(BaseControl, value); SaveTagFile(); }
        }

        #endregion

        #region Action methods

        private void UpdateConnectionSchema()
        {
            try
            {
                if (isBusy)
                    return;
                isBusy = true;

                ConnectionSchemeDesignerForm form = new ConnectionSchemeDesignerForm(BaseControl, Component.Site);
                form.ShowDialog();

                isBusy = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Easy Driver Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
                isBusy = false;
            }
        }

        private void SaveTagFile()
        {
            try
            {
                if (File.Exists(debugPath))
                {
                    string resJson = File.ReadAllText(debugPath);
                    if (!string.IsNullOrEmpty(resJson))
                    {
                        ConnectionSchema driverConnector = JsonConvert.DeserializeObject<ConnectionSchema>(resJson);
                        if (driverConnector != null)
                        {
                            driverConnector.CommunicationMode = CommunicationMode;
                            driverConnector.RefreshRate = RefreshRate;
                            driverConnector.ServerAddress = ServerAddress;
                            driverConnector.Port = Port;

                            File.WriteAllText(debugPath, JsonConvert.SerializeObject(driverConnector));
                        }
                    }
                }

                if (File.Exists(releasePath))
                {
                    string resJson = File.ReadAllText(releasePath);
                    if (!string.IsNullOrEmpty(resJson))
                    {
                        ConnectionSchema driverConnector = JsonConvert.DeserializeObject<ConnectionSchema>(resJson);
                        if (driverConnector != null)
                        {
                            driverConnector.CommunicationMode = CommunicationMode;
                            driverConnector.RefreshRate = RefreshRate;
                            driverConnector.ServerAddress = ServerAddress;
                            driverConnector.Port = Port;

                            File.WriteAllText(releasePath, JsonConvert.SerializeObject(driverConnector));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error occurs when save tag file. {ex.Message}", "Easy Driver Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region Helper

        private string GetCurrentDesignPath()
        {
            try
            {
                EnvDTE.DTE dte = this.GetService(typeof(EnvDTE.DTE)) as EnvDTE.DTE;
                return Path.GetDirectoryName(dte.ActiveDocument.FullName) + "\\bin";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return string.Empty;
            }
        }

        /// <summary>
        /// The method to get a <see cref="PropertyDescriptor"/> of the control by property name
        /// </summary>
        /// <param name="control"></param>
        /// <param name="propName"></param>
        /// <returns></returns>
        private PropertyDescriptor GetPropertyByName(object control, [CallerMemberName]string propName = null)
        {
            PropertyDescriptor prop;
            prop = TypeDescriptor.GetProperties(control)[propName];
            if (null == prop)
                throw new ArgumentException($"Matching {propName} property not found!", propName);
            else
                return prop;
        }

        /// <summary>
        /// Set the value for property of the control
        /// </summary>
        /// <param name="control"></param>
        /// <param name="value"></param>
        /// <param name="propName"></param>
        private void SetValue(object control, object value = null, [CallerMemberName]string propName = null)
        {
            GetPropertyByName(control, propName).SetValue(control, value);
        }

        #endregion
    }
}
