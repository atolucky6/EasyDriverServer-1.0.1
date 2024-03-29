﻿using DevExpress.Mvvm;
using DevExpress.Mvvm.POCO;
using EasyDriverPlugin;
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Collections.Generic;
using EasyDriver.Core;
using System.Text.RegularExpressions;
using AssemblyHelper = EasyDriver.Core.AssemblyHelper;

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
        protected ITreeListViewUtilities TreeListViewUtilities { get; set; }

        #endregion

        #region Constructors

        public AddChannelViewModel(
            IProjectManagerService projectManagerService,
            IDriverManagerService driverManagerService,
            ApplicationViewModel applicationViewModel,
            ITreeListViewUtilities treeListViewUtilities)
        {
            Title = "Add Channel";
            SizeToContent = SizeToContent.WidthAndHeight;
            Width = 600;
            Height = 120;
            ProjectManagerService = projectManagerService;
            DriverManagerService = driverManagerService;
            ApplicationViewModel = applicationViewModel;
            TreeListViewUtilities = treeListViewUtilities;
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

        #region Commands

        public void Save()
        {
            try
            {
                if (Parent.Childs.FirstOrDefault(x => (x as ICoreItem).Name == Name?.Trim()) != null)
                {
                    MessageBoxService.ShowMessage($"The channel name '{Name?.Trim()}' is already in use.", "Message", MessageButton.OK, MessageIcon.Warning); 
                }
                else
                {
                    IsBusy = true;
                    string driverPath = $"{ApplicationViewModel.DriverFolderPath}\\{SelectedDriver}.dll";
                    IEasyDriverPlugin driver = AssemblyHelper.LoadAndCreateInstance<IEasyDriverPlugin>(driverPath);
                    if (driver != null)
                    {
                        CurrentWindowService.Hide();
                        if (ContextWindowService.Show(driver.GetCreateChannelControl(ProjectManagerService.CurrentProject.Childs[0] as LocalStation), "Add Channel") is IChannelCore channel)
                        {
                            channel.Name = Name;
                            Parent.Childs.Add(channel);
                            DriverManagerService.AddDriver(channel, driver);
                            driver.Start(channel);
                            TreeListViewUtilities.ExpandNodeByContent(channel);
                        }
                        CurrentWindowService.Close();
                    }
                    else
                    {
                        MessageBoxService.ShowMessage($"Can't load the driver {SelectedDriver}.", "Message", MessageButton.OK, MessageIcon.Error);
                    }
                }
            }
            catch (Exception)
            {
                MessageBoxService.ShowMessage($"Can't load the driver {SelectedDriver}.", "Message", MessageButton.OK, MessageIcon.Error); 
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
            SelectedDriver = DriverNameSource.FirstOrDefault();
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
