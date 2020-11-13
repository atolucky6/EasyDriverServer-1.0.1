using DevExpress.Mvvm;
using DevExpress.Mvvm.POCO;
using EasyDriver.Core;
using EasyDriverPlugin;
using EasyScada.ServerApplication.Reversible;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace EasyScada.ServerApplication
{
    public class ImportTagViewModel : ISupportParameter, ISupportParentViewModel
    {
        #region Constructors

        public ImportTagViewModel(
            IDriverManagerService driverManagerService,
            IReverseService reverseService)
        {
            SizeToContent = SizeToContent.Manual;
            Title = "Import Tags";
            Width = 800;
            Height = 500;
            DriverManagerService = driverManagerService;
            ReverseService = reverseService;
            Tags = new ObservableCollection<object>();
        }

        #endregion

        #region Injected services

        IReverseService ReverseService { get; set; }
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
                List<ITagCore> tags = SelectedItems.Select(x => x as ITagCore).ToList();
                if (Parent is IHaveTag haveTagObj)
                {
                    using (Transaction transaction = ReverseService.Begin("Import tags"))
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
                        int nameIndex = -1;
                        int addressIndex = -1;
                        int dtIndex = -1;
                        int refreshRateIndex = -1;
                        int accessPermissionIndex = -1;
                        int gainIndex = -1;
                        int offsetIndex = -1;
                        int descriptionIndex = -1;

                        string[] columns = currentLine.Split(',');
                        if (columns.Length != 8)
                            return;
                        for (int i = 0; i < columns.Length; i++)
                        {
                            switch (columns[i].ToUpper())
                            {
                                case "NAME":
                                    nameIndex = i;
                                    break;
                                case "ADDRESS":
                                    addressIndex = i;
                                    break;
                                case "DATATYPE":
                                    dtIndex = i;
                                    break;
                                case "REFRESHRATE":
                                    refreshRateIndex = i;
                                    break;
                                case "ACCESSPERMISSION":
                                    accessPermissionIndex = i;
                                    break;
                                case "GAIN":
                                    gainIndex = i;
                                    break;
                                case "OFFSET":
                                    offsetIndex = i;
                                    break;
                                case "DESCRIPTION":
                                    descriptionIndex = i;
                                    break;
                                default:
                                    break;
                            }
                        }

                        if (nameIndex == -1 || addressIndex == -1 || dtIndex == -1 ||
                            refreshRateIndex == -1 || accessPermissionIndex == -1 ||
                            gainIndex == -1 || offsetIndex == -1 || descriptionIndex == -1)
                        {
                            return;
                        }

                        for (int i = 1; i < lines.Length; i++)
                        {
                            currentLine = lines[i];
                            if (!string.IsNullOrWhiteSpace(currentLine))
                            {
                                string[] rowValues = currentLine.Split(',');
                                string name = rowValues[nameIndex]?.Trim();
                                string dataType = rowValues[dtIndex]?.Trim();
                                string address = rowValues[addressIndex]?.Trim();
                                string refreshRateStr = rowValues[refreshRateIndex]?.Trim();
                                string accessPermissionStr = rowValues[accessPermissionIndex]?.Trim();
                                string gainStr = rowValues[gainIndex]?.Trim();
                                string offsetStr = rowValues[offsetIndex]?.Trim();

                                if (string.IsNullOrEmpty(name))
                                    continue;
                                if (!supportDTs.Contains(dataType))
                                    continue;
                                if (string.IsNullOrEmpty(address))
                                    continue;
                                if (!int.TryParse(refreshRateStr, out int refreshRate))
                                    continue;
                                if (refreshRate <= 0)
                                    continue;
                                if (!Enum.TryParse(accessPermissionStr, out AccessPermission accessPermission))
                                    continue;
                                if (!double.TryParse(gainStr, out double gain))
                                    continue;
                                if (!double.TryParse(offsetStr, out double offset))
                                    continue;
                                if (rowValues.Length == 8)
                                {
                                    ITagCore tag = new TagCore(Parent);
                                    tag.Name = name;
                                    tag.DataType = driver.SupportDataTypes.FirstOrDefault(x => x.Name == dataType);
                                    tag.Address = address;
                                    tag.RefreshRate = refreshRate;
                                    tag.AccessPermission = accessPermission;
                                    tag.Gain = gain;
                                    tag.Offset = offset;
                                    tag.Description = rowValues[descriptionIndex]?.Trim();
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
