using DevExpress.Mvvm;
using DevExpress.Mvvm.POCO;
using EasyDriver.Service.DriverManager;
using EasyDriver.Service.Reversible;
using EasyDriverPlugin;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;

namespace EasyDriver.Workspace.Main
{
    public class ImportTagViewModel : ISupportParameter, ISupportParentViewModel
    {
        #region Constructors
        public ImportTagViewModel()
        {
            SizeToContent = SizeToContent.Manual;
            Title = "Import Tags";
            Width = 800;
            Height = 500;
            DriverManagerService = ServiceLocator.DriverManagerService;
            ReversibleService = ServiceLocator.ReversibleService;
            Tags = new ObservableCollection<object>();
        }
        #endregion

        #region Injected services
        protected IReversibleService ReversibleService { get; set; }
        protected IDriverManagerService DriverManagerService { get; set; }
        #endregion

        #region UI services
        protected IMessageBoxService MessageBoxService { get => this.GetService<IMessageBoxService>(); }
        protected IOpenFileDialogService OpenFileDialogService { get => this.GetService<IOpenFileDialogService>(); }
        protected ICurrentWindowService CurrentWindowService { get => this.GetService<ICurrentWindowService>(); }
        #endregion

        #region Public members

        public string Title { get; set; } = "";
        public SizeToContent SizeToContent { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public virtual string CsvPath { get; set; }
        public virtual IGroupItem Parent { get; set; }
        public virtual ObservableCollection<object> Tags { get; set; }
        public virtual ObservableCollection<object> SelectedItems { get; set; }
        public virtual bool IsBusy { get; set; }
        public object ParentViewModel { get; set; }
        public object Parameter { get; set; }

        #endregion

        #region Methods

        public void Browse()
        {
            try
            {
                OpenFileDialogService.Title = "Import CSV...";
                OpenFileDialogService.Filter = "CSV Files (*.csv)|*.csv";
                if (OpenFileDialogService.ShowDialog())
                {
                    CsvPath = OpenFileDialogService.File.GetFullName();
                    Refresh();
                }
            }
            catch { }
            finally { IsBusy = false; }
        }

        public bool CanBrowse()
        {
            return !IsBusy && Parent != null;
        }

        public void Import()
        {
            try
            {
                IsBusy = true;
                IEasyDriverPlugin driver = DriverManagerService.GetDriver(Parent);
                if (driver != null)
                {
                    List<ITagCore> tags = new List<ITagCore>();
                    foreach (var item in SelectedItems.Select(x => x as ITagCore))
                    {
                        ITagCore tag = null;
                        if (item.IsInternalTag)
                        {
                            tag = new TagCore(Parent);
                        }
                        else
                        {
                            tag = driver.CreateTag(Parent);
                        }
                        
                        tag.Name = item.Name;
                        tag.Address = item.Address;
                        tag.Description = item.Description;
                        tag.Gain = item.Gain;
                        tag.Offset = item.Offset;
                        tag.DataType = item.DataType;
                        tag.RefreshRate = item.RefreshRate;
                        tag.AccessPermission = item.AccessPermission;
                        tag.EnabledWriteLimit = item.Enabled;
                        tag.WriteMinLimit = item.WriteMinLimit;
                        tag.WriteMaxLimit = item.WriteMaxLimit;
                        tag.DefaultValue = item.DefaultValue;
                        tag.Unit = item.Unit;
                        tag.IsInternalTag = tag.IsInternalTag;
                        tag.Retain = item.Address == "Retain";
                        if (tag.IsInternalTag)
                        {
                            tag.Value = item.DefaultValue;
                            if (tag.Retain)
                            {
                                if (string.IsNullOrWhiteSpace(tag.GUID))
                                {
                                    tag.GUID = new Guid().ToString();
                                }
                            }
                        }
                        tags.Add(tag);
                        tag.IsChecked = false;
                    }

                    if (Parent is IHaveTag haveTagObj)
                    {
                        using (Transaction transaction = ReversibleService.Begin("Import tags"))
                        {
                            haveTagObj.Tags.SetPropertyReversible(x => x.DisableNotifyChanged, true);
                            ReversibleCollection<object> reversibleCollection = haveTagObj.Tags.AsReversibleCollection();
                            tags.ForEach(x =>
                            {
                                x.Name = haveTagObj.GetUniqueNameInGroupTags(x.Name, false);
                                reversibleCollection.Add(x);
                            });
                            haveTagObj.Tags.SetPropertyReversible(x => x.DisableNotifyChanged, false);
                            haveTagObj.Tags.NotifyResetCollection();
                            transaction.Reversed += (s, e) =>
                            {
                                haveTagObj.Tags.NotifyResetCollection();
                            };
                            transaction.Commit();
                        }
                    }
                }
                CurrentWindowService.Close();
            }
            catch { }
            finally { IsBusy = false; }
        }

        public bool CanImport()
        {
            return !IsBusy && Parent != null && SelectedItems != null && SelectedItems.Count > 0;
        }

        private void Refresh()
        {
            try
            {
                Tags.Clear();
                if (File.Exists(CsvPath))
                {
                    IChannelCore channel = Parent.FindParent<IChannelCore>(x => x is IChannelCore);
                    var driver = DriverManagerService.GetDriver(channel);
                    if (driver == null)
                        return;
                    List<string> supportDTs = driver.SupportDataTypes.Select(x => x.Name).ToList();

                    string[] lines = File.ReadAllLines(CsvPath);
                    if (lines.Length > 1)
                    {
                        string currentLine = lines[0];
                        string[] columns = currentLine.Split(',');
                        if (columns.Length != 14)
                            return;
                        for (int i = 1; i < lines.Length; i++)
                        {
                            currentLine = lines[i];
                            if (!string.IsNullOrWhiteSpace(currentLine))
                            {
                                string[] rowValues = currentLine.Split(',');
                                string name = rowValues[0]?.Trim();
                                string address = rowValues[1]?.Trim();
                                string dataType = rowValues[2]?.Trim();            
                                string refreshRateStr = rowValues[3]?.Trim();
                                string accessPermissionStr = rowValues[4]?.Trim();
                                string gainStr = rowValues[5]?.Trim();
                                string offsetStr = rowValues[6]?.Trim();
                                string unit = rowValues[7];
                                string isInternalStr = rowValues[8];
                                string defaultValue = rowValues[9]?.Trim();
                                string maxLimitStr = rowValues[10]?.Trim();
                                string minLimitStr = rowValues[11]?.Trim();
                                string enabledWriteLimitStr = rowValues[12]?.Trim();
                                string description = rowValues[13];

                                if (string.IsNullOrEmpty(name))
                                    continue;
                                if (!supportDTs.Contains(dataType))
                                    continue;
                                if (string.IsNullOrEmpty(address))
                                    continue;
                                if (!int.TryParse(refreshRateStr, out int refreshRate))
                                    continue;
                                if (refreshRate < 0)
                                    continue;
                                if (!Enum.TryParse(accessPermissionStr, out AccessPermission accessPermission))
                                    continue;
                                if (!double.TryParse(gainStr, out double gain))
                                    continue;
                                if (!double.TryParse(offsetStr, out double offset))
                                    continue;
                                if (!bool.TryParse(isInternalStr, out bool isInternal))
                                    continue;
                                if (!bool.TryParse(enabledWriteLimitStr, out bool enabledWriteLimit))
                                    continue;
                                if (!double.TryParse(maxLimitStr, out double maxLimit))
                                    continue;
                                if (!double.TryParse(minLimitStr, out double minLimit))
                                    continue;

                                if (rowValues.Length == columns.Length)
                                {
                                    ITagCore tag = new TagCore(Parent);
                                    tag.Name = name;
                                    tag.DataType = driver.SupportDataTypes.FirstOrDefault(x => x.Name == dataType);
                                    tag.Address = address;
                                    tag.RefreshRate = refreshRate;
                                    tag.AccessPermission = accessPermission;
                                    tag.Gain = gain;
                                    tag.Offset = offset;
                                    tag.Unit = unit;
                                    tag.IsInternalTag = isInternal;
                                    tag.DefaultValue = defaultValue;
                                    tag.WriteMaxLimit = maxLimit;
                                    tag.WriteMinLimit = minLimit;
                                    tag.EnabledWriteLimit = enabledWriteLimit;
                                    tag.Description = description;

                                    tag.IsChecked = true;
                                    Tags.Add(tag);
                                }
                            }
                        }
                    }
                }
            }
            catch { }
            finally { this.RaisePropertyChanged(x => x.Tags); }
        }

        #endregion

        #region Event handlers

        public virtual void OnLoaded()
        {
            try
            {
                IsBusy = true;
                if (Parameter is object[] array)
                {
                    Parent = array[0] as IGroupItem;
                    CsvPath = array[1].ToString();
                    Refresh();
                }
            }
            catch { }
            finally { IsBusy = false; }
        }

        #endregion
    }
}
