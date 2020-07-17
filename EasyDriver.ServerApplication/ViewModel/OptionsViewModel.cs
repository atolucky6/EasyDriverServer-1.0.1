using DevExpress.Mvvm;
using DevExpress.Mvvm.POCO;
using EasyDriver.Core;
using System.Collections.Generic;
using System.Windows;

namespace EasyScada.ServerApplication
{
    public class OptionsViewModel
    {
        #region Constructors

        public OptionsViewModel(ApplicationViewModel applicationViewModel,
            IServerBroadcastService serverBroadcastService)
        {
            Title = "Options";
            SizeToContent = SizeToContent.Manual;
            Width = 600;
            Height = 450;

            BroadcastModeSource = new List<string>()
            {
                "Only send asked data",
                "Send all data"
            };

            ApplicationViewModel = applicationViewModel;
            ServerBroadcastService = serverBroadcastService;
            RestoreGeneral();
        }

        #endregion

        #region Injected services

        protected ApplicationViewModel ApplicationViewModel { get; set; }
        protected IServerBroadcastService ServerBroadcastService { get; set; }

        #endregion

        #region UI services

        protected IMessageBoxService MessageBoxService { get => this.GetService<IMessageBoxService>(); }

        #endregion

        #region Public members

        public string Title { get; set; }
        public SizeToContent SizeToContent { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public virtual bool IsBusy { get; set; }

        public virtual ushort Port { get; set; }
        public virtual int BroadcastRate { get; set; }
        public virtual string BroadcastMode { get; set; }
        public virtual int MaximumAllowConnection { get; set; }

        public List<string> BroadcastModeSource { get; set; }
        public ServerConfiguration ServerConfiguration { get; private set; }

        #endregion

        #region Commands

        public void ApplyGeneral()
        {
            IsBusy = true;
            try
            {
                ServerConfiguration.Port = Port;
                ServerConfiguration.BroadcastMode = (BroadcastMode)BroadcastModeSource.FindIndex(x => x == BroadcastMode);
                ServerConfiguration.BroadcastRate = BroadcastRate;
                ServerConfiguration.MaximumAllowConnection = MaximumAllowConnection;
                if (ServerConfiguration.Save())
                {
                    ServerBroadcastService.BroadcastMode = ServerConfiguration.BroadcastMode;
                    ServerBroadcastService.BroadcastRate = ServerConfiguration.BroadcastRate;
                    IsBusy = false;
                    MessageBoxService.ShowMessage("The configuration was saved successfully. Please restart the application to apply all these changes.", "Easy Driver Server", MessageButton.OK, MessageIcon.Information);
                }
                else
                {
                    MessageBoxService.ShowMessage("Some error occurs when save the configuration!", "Easy Driver Server", MessageButton.OK, MessageIcon.Error);
                    IsBusy = false;
                }
            }
            catch { }
            finally { IsBusy = false; }
        }

        public bool CanApplyGeneral()
        {
            return !IsBusy && ServerConfiguration != null;
        }

        public void RestoreGeneral()
        {
            IsBusy = true;
            try
            {
                ServerConfiguration = new ServerConfiguration(ApplicationViewModel.ServerIniPath);
                Port = ServerConfiguration.Port;
                BroadcastMode = BroadcastModeSource[(int)ServerConfiguration.BroadcastMode];
                BroadcastRate = ServerConfiguration.BroadcastRate;
                MaximumAllowConnection = ServerConfiguration.MaximumAllowConnection;
                this.RaisePropertiesChanged();
            }
            catch { }
            finally { IsBusy = false; }
        }

        public bool CanRestoreGeneral()
        {
            return !IsBusy;
        }

        #endregion
    }
}
