using DevExpress.Mvvm;
using DevExpress.Mvvm.POCO;
using EasyDriverPlugin;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace EasyScada.ServerApplication
{
    public class GroupViewModel : ISupportParentViewModel, ISupportParameter, IDataErrorInfo
    {
        #region Constructors
        public GroupViewModel()
        {
            Title = "Edit Group";
            SizeToContent = SizeToContent.WidthAndHeight;
            Width = 600;
            Height = 80;
        }
        #endregion

        #region UI services

        protected ICurrentWindowService CurrentWindowService { get => this.GetService<ICurrentWindowService>(); }
        protected IMessageBoxService MessageBoxService { get => this.GetService<IMessageBoxService>(); }

        #endregion

        #region Public properties
        public virtual string Name { get; set; }
        public virtual IGroupItem GroupItem { get; set; }
        public object Parameter { get; set; }
        public object ParentViewModel { get; set; }
        public string Title { get; set; }
        public SizeToContent SizeToContent { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public virtual bool IsBusy { get; set; }
        #endregion

        #region Commands

        public void Save()
        {
            try
            {
                if (GroupItem.Parent.Childs.FirstOrDefault(x => x != GroupItem && (x as ICoreItem).Name == Name?.Trim()) != null)
                {
                    MessageBoxService.ShowMessage($"The group name '{Name?.Trim()}' is already in use.", "Easy Driver Server", MessageButton.OK, MessageIcon.Warning);
                }
                else
                {
                    IsBusy = true;
                    GroupItem.Name = Name;
                    CurrentWindowService.Close();
                }
            }
            catch (Exception) { }
            finally { IsBusy = false; }
        }

        public bool CanSave() => string.IsNullOrEmpty(Error) && !IsBusy;

        public void Close() => CurrentWindowService.Close();

        public bool CanClose() => !IsBusy;

        #endregion

        #region Event handlers

        public void OnLoaded()
        {
            if (Parameter == null)
                CurrentWindowService.Close();

            if (Parameter is IGroupItem groupItem)
            {
                GroupItem = groupItem;
                Name = groupItem.Name;
                this.RaisePropertyChanged(x => x.Name);
            }
            else
            {
                CurrentWindowService.Close();
            }
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
