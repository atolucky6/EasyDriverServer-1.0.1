using DevExpress.Mvvm;
using EasyDriverPlugin;
using EasyScada.ServerApplication.Reversible;
using EasyScada.ServerApplication.Workspace;
using System;
using System.Collections.Generic;
using EasyDriver.Core;

namespace EasyScada.ServerApplication
{
    public class ItemPropertiesWorkspaceViewModel : WorkspacePanelViewModelBase
    {
        #region Injected services

        private readonly IReverseService ReverseService;

        #endregion

        #region Constructors

        public ItemPropertiesWorkspaceViewModel(IWorkspaceManagerService workspaceManagerService, IReverseService reverseService) : base(null, workspaceManagerService)
        {
            WorkspaceName = WorkspaceRegion.Properties;
            WorkspaceManagerService = workspaceManagerService;
            ReverseService = reverseService;
            Messenger.Default.Register<ShowPropertiesMessage>(this, OnShowPropertiesMessage);
            Messenger.Default.Register<HidePropertiesMessage>(this, OnHidePropertiesMessage);
            Caption = "Properties";
            IsClosed = true;
        }

        #endregion

        #region Public members

        public override string WorkspaceName { get; protected set; }

        public virtual object PreviousSelectedObject { get; set; }

        public virtual object SelectedObject { get; set; }

        public virtual string SelectedPropertyPath { get; set; }

        public virtual object Context { get; set; }

        #endregion

        #region Message handler

        /// <summary>
        /// The callback to handle recive messaage
        /// </summary>
        /// <param name="message"></param>
        public void OnShowPropertiesMessage(ShowPropertiesMessage message)
        {
            //if (message != null)
            //{
            //    SelectedObject = message.Item;
            //    Context = message.Sender;
            //}
        }

        public void OnHidePropertiesMessage(HidePropertiesMessage message)
        {
            //if (message != null)
            //{
            //    SelectedObject = null;
            //    Context = message.Sender;
            //}
        }

        #endregion

        #region Methods

        /// <summary>
        /// Subscribe all edit events
        /// </summary>
        /// <param name="bindableCore"></param>
        private void SubscribeEditableEvents(BindableCore bindableCore)
        {
            bindableCore.BeganEdit += EditObject_BeganEdit;
            bindableCore.CancelledEdit += EditObject_CancelledEdit;
            bindableCore.EndedEdit += EditObject_EndedEdit;
            if (SelectedObject is ISupportParameters supportParameter)
            {
                if (supportParameter.ParameterContainer is BindableCore coreObject)
                {
                    coreObject.BeganEdit += EditObject_BeganEdit;
                    coreObject.CancelledEdit += EditObject_CancelledEdit;
                    coreObject.EndedEdit += EditObject_EndedEdit;
                }
            }
        }

        /// <summary>
        /// Unsubscribe all edit events
        /// </summary>
        /// <param name="bindableCore"></param>
        private void UnsubscribeEditableEvents(BindableCore bindableCore)
        {
            bindableCore.BeganEdit -= EditObject_BeganEdit;
            bindableCore.CancelledEdit -= EditObject_CancelledEdit;
            bindableCore.EndedEdit -= EditObject_EndedEdit;
            if (SelectedObject is ISupportParameters supportParameter)
            {
                if (supportParameter.ParameterContainer is BindableCore coreObject)
                {
                    coreObject.BeganEdit -= EditObject_BeganEdit;
                    coreObject.CancelledEdit -= EditObject_CancelledEdit;
                    coreObject.EndedEdit -= EditObject_EndedEdit;
                }
            }
        }

        #endregion

        #region Property changed callback

        /// <summary>
        /// The event callback when the current display object was changed
        /// </summary>
        public virtual void OnSelectedObjectChanged()
        {
            //Unsubscribe all event of display object before
            if (PreviousSelectedObject != null && PreviousSelectedObject is BindableCore previousEditObject)
                UnsubscribeEditableEvents(previousEditObject);

            //Subscribe all event of new display object
            if (SelectedObject != null && SelectedObject is BindableCore editObject)
                SubscribeEditableEvents(editObject);

            PreviousSelectedObject = SelectedObject;
        }

        #endregion

        #region Events

        /// <summary>
        /// Whether the object begin edit
        /// </summary>
        bool startEdit = false;

        /// <summary>
        /// The current property name being edit
        /// </summary>
        string propertyName = null;

        /// <summary>
        /// Backup properties use for revese
        /// </summary>
        Dictionary<string, object> backupData;

        /// <summary>
        /// The event callback when end edit
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditObject_EndedEdit(object sender, EventArgs e)
        {
            //Check when editing
            if (startEdit)
            {
                //Cancel edit
                startEdit = false;
                BindableCore editObject = sender as BindableCore;
                bool hasChanges = false;
                //Get current data of edit object
                Dictionary<string, object> currentProperties = editObject.GetData();
                //Check the value of the properties has changes or not
                if (backupData.ContainsKey(propertyName) && !string.IsNullOrEmpty(propertyName))
                {
                    if (!Equals(currentProperties[propertyName], backupData[propertyName]))
                        hasChanges = true;
                }
                propertyName = null;
                //If object has changes
                if (hasChanges)
                {
                    //Start transaction
                    using (var transaction = ReverseService.Begin($"Edit {sender.GetClassName().ToLower()}"))
                    {
                        //Set the properties as revesible
                        this.SetPropertyReversible(x => x.SelectedObject, SelectedObject);
                        foreach (var item in backupData)
                        {
                            if (!Equals(currentProperties[item.Key], item.Value))
                                sender.AddPropertyChangedReversible(item.Key, item.Value, currentProperties[item.Key]);
                        }
                        //Open this view when perform undo or redo
                        transaction.Reversing += (s, args) =>
                        {
                            WorkspaceManagerService.OpenPanel(this);
                        };

                        //Commit the transaction
                        transaction.Commit();
                    }
                }
            }
        }

        /// <summary>
        /// The event callback when cancel edit
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditObject_CancelledEdit(object sender, EventArgs e)
        {
            //Cancel edit
            startEdit = false;
            //Clear backup data
            backupData = null;
            //Clear property name
            propertyName = null;
        }

        /// <summary>
        /// The event callback when begin edit
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditObject_BeganEdit(object sender, EventArgs e)
        {
            //Check the view is active and opened
            if (IsActive && IsOpened)
            {
                //Start edit
                startEdit = true;
                //Set the property name
                propertyName = SelectedPropertyPath;
                //Store backup data of edit object
                backupData = (sender as BindableCore).GetBackupData();
            }
        }

        #endregion
    }
}
