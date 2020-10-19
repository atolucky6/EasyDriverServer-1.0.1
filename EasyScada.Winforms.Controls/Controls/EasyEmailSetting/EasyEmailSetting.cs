using System.ComponentModel;

namespace EasyScada.Winforms.Controls.Controls.EasyEmailSetting
{
    public partial class EasyEmailSetting : Component
    {
        #region Constructors

        public EasyEmailSetting()
        {
            InitializeComponent();
        }

        public EasyEmailSetting(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }

        #endregion

        #region Public properties

        public string Host { get; set; } = "smptp.gmail.com";
        public int Port { get; set; } = 587;
        public int Timeout { get; set; } = 100000;
        public bool EnableSSL { get; set; } = true;
        public string CredentialEmail { get; set; } 
        public string CredentialPassword { get; set; } 

        #endregion
    }
}
