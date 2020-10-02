using DevExpress.Mvvm;
using DevExpress.Mvvm.POCO;
using EasyDriver.Core;
using EasyDriverPlugin;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows;

namespace EasyDriver.CoreItemFactory
{
    public class SelectDriverViewModel : IDataErrorInfo, ISupportParameter, ISupportParentViewModel
    {
        #region UI services

        protected ICurrentWindowService CurrentWindowService { get => this.GetService<ICurrentWindowService>(); }
        protected IMessageBoxService MessageBoxService { get => this.GetService<IMessageBoxService>(); }

        #endregion

        #region Constructors

        public SelectDriverViewModel(IGroupItem parent)
        {
            Parent = parent;
            DriverNameSource = DriverNameSource = new List<string>(
                Directory.GetFiles(applicationDir + "\\Drivers\\", "*.dll", SearchOption.AllDirectories).ToList().Select(x => Path.GetFileNameWithoutExtension(x)));
        }

        #endregion

        #region Public members
        public virtual object Result { get; set; }
        public virtual string Name { get; set; }
        public virtual string SelectedDriver { get; set; }
        public List<string> DriverNameSource { get; set; }

        public object Parameter { get; set; }
        public object ParentViewModel { get; set; }
        public IGroupItem Parent { get; private set; }
        public virtual bool IsBusy { get; set; }
        #endregion

        #region Private members

        readonly string applicationDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        #endregion

        #region Commands

        public void Save()
        {
            try
            {
                if (Parent.Childs.FirstOrDefault(x => (x as ICoreItem).Name == Name?.Trim()) != null)
                {
                    MessageBoxService.ShowMessage($"The channel name '{Name?.Trim()}' is already in use.", "Warning", 
                        MessageButton.OK, MessageIcon.Warning);
                }
                else
                {
                    IsBusy = true;
                    IChannelCore channel = new ChannelCore(Parent)
                    {
                        Name = Name,
                        DriverPath = $"{applicationDir}\\Drivers\\{SelectedDriver}.dll"
                    };
                    IEasyDriverPlugin driver = AssemblyHelper.LoadAndCreateInstance<IEasyDriverPlugin>(channel.DriverPath);
                    if (driver != null)
                    {
                        CurrentWindowService.Hide();
                        driver.Channel = channel;

                        var createChannelControl = driver.GetCreateChannelControl(Parent);
                        var result = Helper.ShowContextWindow(createChannelControl, "Add Channel");
                        if (result is IChannelCore channelCore)
                            Result = new List<object>() { result, driver };
                        CurrentWindowService.Close();
                    }
                    else
                    {
                        MessageBoxService.ShowMessage($"Can't load the driver {SelectedDriver}.", "Error", MessageButton.OK, MessageIcon.Error);
                    }
                }
            }
            catch (Exception)
            {
                MessageBoxService.ShowMessage($"Can't load the driver {SelectedDriver}.", "Error", MessageButton.OK, MessageIcon.Error);
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
                        Error = ValidateFileName(Name);
                        break;
                    default:
                        Error = string.Empty;
                        break;
                }
                return Error;
            }
        }

        public static string REGEX_ValidFileName = @"^[\w\-. ]+$";
        public static string ValidateFileName(string str)
        {
            str = str?.Trim();
            if (string.IsNullOrEmpty(str))
                return "The name can't be empty.";
            if (!Regex.IsMatch(str, REGEX_ValidFileName))
                return "The name was not in correct format.";
            return string.Empty;
        }

        #endregion
    }
}
