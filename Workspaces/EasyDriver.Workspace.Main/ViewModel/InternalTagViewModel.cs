using DevExpress.Mvvm;
using DevExpress.Mvvm.POCO;
using EasyDriver.Service.InternalStorage;
using EasyDriver.Service.Reversible;
using EasyDriverPlugin;
using EasyScada.WorkspaceManager;
using System;
using System.ComponentModel;
using System.Windows;

namespace EasyDriver.Workspace.Main
{
    public class InternalTagViewModel : ISupportParameter, ISupportParentViewModel, IDataErrorInfo
    {
        #region Constructors
        public InternalTagViewModel()
        {
            SizeToContent = SizeToContent.Height;
            Width = 1200;
            Height = 800;
            InternalStorageService = ServiceLocator.InternalStorageService;
            ReversibleService = ServiceLocator.ReversibleService;
            WorkspaceManagerService = ServiceLocator.WorkspaceManagerService;
        }
        #endregion

        #region Injected services
        protected IInternalStorageService InternalStorageService { get; set; }
        protected IReversibleService ReversibleService { get; set; }
        protected IWorkspaceManagerService WorkspaceManagerService { get; set; }
        #endregion

        #region UI services
        public ICurrentWindowService CurrentWindowService => this.GetService<ICurrentWindowService>();
        public IMessageBoxService MessageBoxService => this.GetService<IMessageBoxService>();
        #endregion

        #region Fields
        object parameter;
        #endregion

        #region Public properties
        public bool InsertAbove { get; set; }
        public bool InsertBelow { get; set; }
        public int InsertIndex { get; set; }
        public string Title { get; set; }
        public SizeToContent SizeToContent { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public bool IsEdit { get; set; }
        public virtual string Name { get; set; }
        public virtual bool IsRetain { get; set; }
        public virtual string Description { get; set; }
        public virtual string DefaultValue { get; set; }
        public virtual string Unit { get; set; }
        public virtual ITagCore Tag { get; set; }
        public virtual IHaveTag ParentHaveTag { get; set; }
        public object ParentViewModel { get; set; }
        public object Parameter
        {
            get => parameter;
            set
            {
                parameter = value;
                if (parameter is ITagCore tagCore)
                {
                    ParentHaveTag = tagCore.Parent as IHaveTag;
                    IsEdit = true;
                    Name = tagCore.Name;
                    Description = tagCore.Description;
                    DefaultValue = tagCore.DefaultValue;
                    Unit = tagCore.Unit;
                    IsRetain = tagCore.Retain;
                    Tag = tagCore;
                    Title = $"Edit Internal Tag - {Name}";
                }
                else if (parameter is IHaveTag haveTagObj)
                {
                    ParentHaveTag = haveTagObj;
                    Name = haveTagObj.GetUniqueNameInGroupTags("Internal_tag_1");
                    Title = "Add Internal Tag";
                }
            }
        }
        #endregion

        #region Commands
        public void Save()
        {
            if (IsEdit)
            {
                if (parameter is ITagCore tagCore)
                {
                    if (ParentHaveTag != null)
                    {
                        if (Name.IsUniqueNameInGroupTags(ParentHaveTag, tagCore))
                        {
                            string validateMessage = Name.ValidateFileName();
                            if (string.IsNullOrEmpty(validateMessage))
                            {
                                string oldName = tagCore.Name;
                                string oldAddress = tagCore.Address;
                                string oldDescription = tagCore.Description;
                                string oldDefaultValue = tagCore.DefaultValue;
                                string oldUnit = tagCore.Unit;

                                bool oldRetain = tagCore.Retain;

                                if (tagCore.Retain)
                                {
                                    if (string.IsNullOrEmpty(tagCore.GUID))
                                        tagCore.GUID = Guid.NewGuid().ToString();
                                }
                                tagCore.Retain = IsRetain;
                                tagCore.Address = IsRetain ? "Retain" : "Non-Retain";
                                tagCore.Description = Description;
                                tagCore.Unit = Unit;
                                tagCore.DefaultValue = DefaultValue;
                                if (tagCore.Retain)
                                    InternalStorageService.AddOrUpdateStoreValue(tagCore.GUID, tagCore.Value);

                                if (tagCore.HasChanges())
                                {
                                    using (Transaction transaction = ReversibleService.Begin("Edit Tag"))
                                    {
                                        if (oldName != tagCore.Name)
                                            tagCore.AddPropertyChangedReversible(x => x.Name, oldName, tagCore.Name);
                                        if (oldAddress != tagCore.Address)
                                            tagCore.AddPropertyChangedReversible(x => x.Address, oldAddress, tagCore.Address);
                                        if (oldDescription != tagCore.Description)
                                            tagCore.AddPropertyChangedReversible(x => x.Description, oldDescription, tagCore.Description);
                                        if (oldRetain != tagCore.Retain)
                                            tagCore.AddPropertyChangedReversible(x => x.Retain, oldRetain, tagCore.Retain);
                                        if (oldDefaultValue != tagCore.DefaultValue)
                                            tagCore.AddPropertyChangedReversible(x => x.DefaultValue, oldDefaultValue, tagCore.DefaultValue);
                                        if (oldUnit != tagCore.Unit)
                                            tagCore.AddPropertyChangedReversible(x => x.Unit, oldUnit, tagCore.Unit);

                                        transaction.Reversing += (s, e) =>
                                        {
                                            WorkspaceManagerService.OpenPanel(tagCore.Parent);
                                        };
                                        transaction.Commit();
                                    }
                                }

                                Close();
                            }
                            else
                            {
                                MessageBoxService.ShowMessage(validateMessage, "Message", MessageButton.OK, MessageIcon.Information);
                            }
                        }
                        else
                        {
                            MessageBoxService.ShowMessage($"The name '{Name}' is already exists. Please try another name.", "Message", MessageButton.OK, MessageIcon.Information);
                        }
                    }
                }
            }
            else
            {
                if (parameter is IHaveTag haveTagObj)
                {
                    if (Name.IsUniqueNameInGroupTags(haveTagObj, null))
                    {
                        string validateMessage = Name.ValidateFileName();
                        if (string.IsNullOrEmpty(validateMessage))
                        {
                            TagCore tagCore = new TagCore(parameter as IGroupItem);
                            tagCore.DataTypeName = "String";
                            tagCore.IsInternalTag = true;
                            tagCore.Name = Name?.Trim();
                            tagCore.Address = IsRetain ? "Retain" : "Non-Retain";
                            tagCore.Quality = Quality.Good;
                            tagCore.Description = Description?.Trim();
                            tagCore.AccessPermission = AccessPermission.ReadAndWrite;
                            tagCore.RefreshInterval = 100;
                            tagCore.Retain = IsRetain;
                            tagCore.DefaultValue = DefaultValue;
                            tagCore.Value = DefaultValue;
                            tagCore.Unit = Unit;
                            if (string.IsNullOrWhiteSpace(tagCore.GUID))
                                tagCore.GUID = Guid.NewGuid().ToString();
                            if (tagCore.Retain)
                                InternalStorageService.AddOrUpdateStoreValue(tagCore.GUID, tagCore.Value);

                            using (Transaction transaction = ReversibleService.Begin($"Add internal tag"))
                            {
                                int index = InsertIndex;
                                if (InsertBelow)
                                {
                                    haveTagObj.Tags.AsReversibleCollection().Insert(index + 1, tagCore);
                                }
                                else if (InsertAbove)
                                {

                                    haveTagObj.Tags.AsReversibleCollection().Insert(index, tagCore);
                                }
                                else
                                {
                                    haveTagObj.Tags.AsReversibleCollection().Add(tagCore);
                                }
                                (ParentViewModel as TagCollectionViewModel).SetPropertyReversible(x => x.SelectedItem, tagCore);
                                transaction.Reversing += (s, e) =>
                                {
                                    WorkspaceManagerService.OpenPanel((ParentViewModel as TagCollectionViewModel).WorkspaceContext);
                                };
                                transaction.Commit();
                            }
                            Close();
                        }
                        else
                        {
                            MessageBoxService.ShowMessage(validateMessage, "Message", MessageButton.OK, MessageIcon.Information);
                        }
                    }
                    else
                    {
                        MessageBoxService.ShowMessage($"The name '{Name}' is already exists. Please try another name.", "Message", MessageButton.OK, MessageIcon.Information);
                    }

                }
            }
        }
        public bool CanSave()
        {
            return string.IsNullOrWhiteSpace(Error);
        }

        public void Close()
        {
            CurrentWindowService.Close();
        }
        #endregion

        #region IDataErrorInfo
        public string Error { get; set; }

        public string this[string columnName]
        {
            get
            {
                Error = string.Empty;
                switch (columnName)
                {
                    case nameof(Name):
                        Error = Name.ValidateFileName();
                        if (string.IsNullOrWhiteSpace(Error))
                        {
                            if (!Name.IsUniqueNameInGroupTags(ParentHaveTag, Tag))
                            {
                                Error = $"The tag name '{Name}' is already exists";
                            }
                        }
                        break;
                    default:
                        break;
                }
                return Error;
            }
        }
        #endregion
    }
}
