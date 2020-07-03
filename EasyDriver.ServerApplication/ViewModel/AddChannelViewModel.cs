using DevExpress.Mvvm;
using DevExpress.Mvvm.POCO;
using EasyDriverPlugin;
using EasyDriver.Server.Models;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Collections.Generic;

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
        protected ApplicationViewModel ApplicationViewModel { get; set; }

        #endregion

        #region Constructors

        public AddChannelViewModel(
            IProjectManagerService projectManagerService,
            IDriverManagerService driverManagerService,
            ApplicationViewModel applicationViewModel)
        {
            Title = "Add Channel";
            SizeToContent = SizeToContent.WidthAndHeight;
            Width = 600;
            Height = 120;
            ProjectManagerService = projectManagerService;
            DriverManagerService = driverManagerService;
            ApplicationViewModel = applicationViewModel;
        }

        #endregion

        #region Public members

        public string Title { get; set; }
        public SizeToContent SizeToContent { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public virtual string Name { get; set; }
        public virtual string SelectedDriver { get; set; }
        public List<string> DriverNameSource { get; set; }

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
                if (Parent.Childs.FirstOrDefault(x => (x as ICoreItem).Name == Name?.Trim()) != null)
                {
                    MessageBoxService.ShowMessage($"The channel name '{Name?.Trim()}' is already in use.", "Easy Driver Server", MessageButton.OK, MessageIcon.Warning); 
                }
                else
                {
                    IsBusy = true;
                    IChannelCore channel = new ChannelCore(Parent)
                    {
                        Name = Name,
                        DriverPath = $"{ApplicationViewModel.DriverFolderPath}\\{SelectedDriver}.dll"
                    };
                    IEasyDriverPlugin driver = AssemblyHelper.LoadAndCreateInstance<IEasyDriverPlugin>(channel.DriverPath);
                    if (driver != null)
                    {
                        CurrentWindowService.Hide();
                        driver.Channel = channel;
                        if (ContextWindowService.Show(driver.GetCreateChannelControl(), "Add Channel") == channel)
                        {
                            Parent.Add(channel);
                            DriverManagerService.AddDriver(channel, driver);
                        }
                        CurrentWindowService.Close();
                    }
                    else
                    {
                        MessageBoxService.ShowMessage($"Can't load the driver {SelectedDriver}.", "Easy Driver Server", MessageButton.OK, MessageIcon.Error);
                    }
                }
            }
            catch (Exception)
            {
                MessageBoxService.ShowMessage($"Can't load the driver {SelectedDriver}.", "Easy Driver Server", MessageButton.OK, MessageIcon.Error); 
            }
            finally { IsBusy = false; }
        }

        public bool CanSave() => string.IsNullOrEmpty(Error) && !string.IsNullOrEmpty(SelectedDriver) && !IsBusy;

        public void Close() => CurrentWindowService.Close();

        public bool CanClose() => !IsBusy;

        #endregion

        #region Event handlers

        public void OnLoaded()
        {
            Name = Parent.GetUniqueNameInGroup("Channel1");
            DriverNameSource = ApplicationViewModel.DriverNameSource;
            this.RaisePropertyChanged(x => x.DriverNameSource);
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
