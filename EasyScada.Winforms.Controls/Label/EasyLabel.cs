using System.ComponentModel;
using System.Windows.Forms;
using EasyScada.Winforms.Controls.Designer;
using EasyScada.Winforms.Designer;
using EasyScada.Winforms.Connector;
using System.Security.Permissions;
using EasyDriver.Client.Models;

namespace EasyScada.Winforms.Controls
{
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    [Designer(typeof(EasyLabelDesigner))]
    public partial class EasyLabel : Label, ISupportConnector, ISupportTag
    {
        #region Constructors

        public EasyLabel() : base()
        {
            InitializeComponent();
        }

        #endregion

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
                    easyDriverConnector.Started += EasyDriverConnector_Started;
                }
            }
        }

        #endregion

        [Description("Select path to tag for control")]
        [Browsable(true), Category(DesignerCategory.EASYSCADA)]
        [TypeConverter(typeof(EasyScadaTagPathConverter))]
        public string PathToTag { get; set; }

        ITag DriverTag
        {
            get { return easyDriverConnector?.GetTag(PathToTag); }
        }

        private void EasyDriverConnector_Started(object sender, System.EventArgs e)
        {
            if (DriverTag != null)
            {
                DriverTag.ValueChanged += DriverTag_ValueChanged;
                DriverTag.QualityChanged += DriverTag_QualityChanged;
            }
        }

        private void DriverTag_QualityChanged(object sender, TagQualityChangedEventArgs e)
        {

        }

        private void DriverTag_ValueChanged(object sender, TagValueChangedEventArgs e)
        {
            this.SetInvoke((x) =>
            {
                if (x.Text != e.NewValue)
                    x.Text = e.NewValue;
            });
        }
    }
}
