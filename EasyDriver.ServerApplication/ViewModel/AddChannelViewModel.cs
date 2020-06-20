using DevExpress.Mvvm;
using DevExpress.Mvvm.POCO;
using EasyDriverPlugin;
using EasyDriver.Server.Models;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;

namespace EasyScada.ServerApplication
{
    public class AddChannelViewModel : IDataErrorInfo, ISupportParameter, ISupportParentViewModel
    {
        #region UI services

        protected ICurrentWindowService CurrentWindowService { get => this.GetService<ICurrentWindowService>(); }
        protected IOpenFileDialogService OpenFileDialogService { get => this.GetService<IOpenFileDialogService>(); }
        protected IMessageBoxService MessageBoxService { get => this.GetService<IMessageBoxService>(); }
        protected IContextWindowService ContextWindowService { get => this.GetService<IContextWindowService>(); }

        #endregion

        #region Inject services

        protected IProjectManagerService ProjectManagerService { get; set; }
        protected IDriverManagerService DriverManagerService { get; set; }

        #endregion

        #region Constructors

        public AddChannelViewModel(
            IProjectManagerService projectManagerService,
            IDriverManagerService driverManagerService)
        {
            Title = "Add Channel";
            SizeToContent = SizeToContent.WidthAndHeight;
            Width = 600;
            Height = 120;
            ProjectManagerService = projectManagerService;
            DriverManagerService = driverManagerService;
        }

        #endregion

        #region Public members

        public string Title { get; set; }
        public SizeToContent SizeToContent { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public virtual string Name { get; set; }
        public virtual string DriverPath { get; set; }

        public object Parameter { get; set; }
        public object ParentViewModel { get; set; }
        public ProjectTreeWorkspaceViewModel ProjectTreeWorkspaceViewModel { get => ParentViewModel as ProjectTreeWorkspaceViewModel; }
        public IStationCore Parent { get => Parameter as IStationCore; }
        public virtual bool IsBusy { get; set; }
        #endregion

        #region Private members

        private string startPath { get; set; }

        #endregion

        #region Commands

        public void Save()
        {
            try
            {
                if (Parent.Childs.FirstOrDefault(x => x.Name == Name?.Trim()) != null)
                {
                    MessageBoxService.ShowMessage($"The channel name '{Name?.Trim()}' is already in use.", "Easy Driver Server", MessageButton.OK, MessageIcon.Warning); 
                }
                else
                {
                    IsBusy = true;
                    IChannelCore channel = new ChannelCore(Parent)
                    {
                        Name = Name,
                        DriverPath = DriverPath
                    };
                    IEasyDriverPlugin driver = AssemblyHelper.LoadAndCreateInstance<IEasyDriverPlugin>(DriverPath);
                    if (driver != null)
                    {
                        CurrentWindowService.Hide();
                        driver.Channel = channel;
                        ContextWindowService.Show(driver.GetCreateChannelControl(), "Add Channel");
                        if (channel.ParameterContainer.Parameters.Count > 0)
                            Parent.Add(channel);
                        DriverManagerService.AddDriver(channel, driver);
                        CurrentWindowService.Close();
                    }
                }

            }
            catch (Exception ex)
            {

            }
            finally { IsBusy = false; }
        }

        public bool CanSave() => string.IsNullOrEmpty(Error) && !string.IsNullOrEmpty(DriverPath) && !IsBusy;

        public void Browse()
        {
            OpenFileDialogService.Filter = "Easy Scada Driver (*.dll)|*.dll";
            OpenFileDialogService.Title = "Select driver";
            if (OpenFileDialogService.ShowDialog())
            {
                DriverPath = OpenFileDialogService.File.GetFullName();
            }
        }

        public bool CanBrowse() => !IsBusy;

        public void Close() => CurrentWindowService.Close();

        public bool CanClose() => !IsBusy;

        #endregion

        #region Event handlers

        public void OnLoaded()
        {
            Name = Parent.GetUniqueNameInGroup("Channel1");
        }

        #endregion

        #region IDataErrorInfo

        public string Error { get; private set; }

        public string this[string columnName]
        {
            get
            {
                switch (columnName)
                {
                    case nameof(Name):
                        Error = Name.ValidateFileName();
                        break;
                    default:
                        Error = string.Empty;
                        break;
                }
                return Error;
            }
        }

        #endregion
    }
}
