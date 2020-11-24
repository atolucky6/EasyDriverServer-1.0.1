using DevExpress.Mvvm;
using DevExpress.Mvvm.POCO;
using EasyDriver.Core;
using EasyDriverPlugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace EasyScada.ServerApplication
{
    public class InternalTagViewModel : ISupportParameter, ISupportParentViewModel
    {
        #region Constructors
        public InternalTagViewModel(IInternalStorageService internalStorageService)
        {
            SizeToContent = SizeToContent.Height;
            Width = 800;
            InternalStorageService = internalStorageService;
        }
        #endregion

        #region Injected services
        protected IInternalStorageService InternalStorageService { get; set; }
        #endregion

        #region UI services
        public ICurrentWindowService CurrentWindowService => this.GetService<ICurrentWindowService>();
        public IMessageBoxService MessageBoxService => this.GetService<IMessageBoxService>();
        #endregion

        #region Fields
        object parameter;
        #endregion

        #region Public properties
        public string Title { get; set; }
        public SizeToContent SizeToContent { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public bool IsEdit { get; set; }
        public virtual string Name { get; set; }
        public virtual bool IsRetain { get; set; }
        public virtual string Description { get; set; }
        public object ParentViewModel { get; set; }
        public object Parameter
        {
            get => parameter;
            set
            {
                parameter = value;
                if (parameter is ITagCore tagCore)
                {
                    IsEdit = true;
                    Name = tagCore.Name;
                    Description = tagCore.Description;
                    IsRetain = tagCore.Retain;
                    Title = $"Edit Internal Tag - {Name}";
                }
                else if (parameter is IHaveTag haveTagObj)
                {
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
                    if (tagCore.Parent is IHaveTag haveTagObj)
                    {
                        if (Name.IsUniqueNameInGroupTags(haveTagObj, tagCore))
                        {
                            string validateMessage = Name.ValidateFileName();
                            if (string.IsNullOrEmpty(validateMessage))
                            {
                                tagCore.Name = Name?.Trim();
                                tagCore.Description = Description?.Trim();
                                if (string.IsNullOrEmpty(tagCore.GUID))
                                    tagCore.GUID = Guid.NewGuid().ToString();
                                tagCore.Retain = IsRetain;
                                tagCore.Address = IsRetain ? "Retain" : "Non-Retain";
                                if (tagCore.Retain)
                                    InternalStorageService.AddOrUpdateInternalTag(tagCore);
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
                            if (string.IsNullOrWhiteSpace(tagCore.GUID))
                                tagCore.GUID = Guid.NewGuid().ToString();
                            if (tagCore.Retain)
                                InternalStorageService.AddOrUpdateInternalTag(tagCore);
                            haveTagObj.Tags.Add(tagCore);
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

        public void Close()
        {
            CurrentWindowService.Close();
        }
        #endregion
    }
}
