﻿using DevExpress.Mvvm;
using DevExpress.Mvvm.POCO;
using EasyDriver.Core;
using EasyDriverPlugin;
using System.Windows;

namespace EasyScada.ServerApplication
{
    public class WriteTagViewModel : ISupportParameter, ISupportParentViewModel
    {
        #region Constructors

        public WriteTagViewModel(
            IProjectManagerService projectManagerService,
            ITagWriterService tagWriterService)
        {
            ProjectManagerService = projectManagerService;
            TagWriterService = tagWriterService;

            SizeToContent = SizeToContent.WidthAndHeight;
            Width = 600;
            Height = 120;

        }

        #endregion

        #region Injected services

        protected IProjectManagerService ProjectManagerService { get; set; }
        protected ITagWriterService TagWriterService { get; set; }

        #endregion

        #region Public members

        public string Title { get; set; }
        public SizeToContent SizeToContent { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }

        public object ParentViewModel { get; set; }
        public object Parameter { get; set; }
        public ITagCore TagCore { get; set; }
        public virtual string WriteValue { get; set; }
        public virtual bool IsBusy { get; set; }

        #endregion

        #region Commands

        public async void Write()
        {
            try
            {
                IsBusy = true;
                WriteCommand cmd = new WriteCommand();
                cmd.PathToTag = TagCore.Path;
                cmd.Value = WriteValue;
                await TagWriterService.WriteTag(cmd);
            }
            catch
            {

            }
            finally { IsBusy = false; }
        }

        public bool CanWrite()
        {
            return !IsBusy && TagCore != null && TagWriterService != null;
        }

        #endregion

        #region Event handlers

        public virtual void OnLoaded()
        {
            TagCore = Parameter as ITagCore;
            Title = $"Write Tag - {TagCore.Name}";
            this.RaisePropertiesChanged();
        }

        #endregion
    }
}
