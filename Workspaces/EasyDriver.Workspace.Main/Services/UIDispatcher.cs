using EasyDriver.RemoteConnectionPlugin;
using EasyDriverPlugin;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace EasyDriver.Workspace.Main
{
    public class UIDispatcher
    {
        public static UIDispatcher Default { get; } = new UIDispatcher();

        public bool AllowUpdateUI { get; set; } = true;
        private System.Timers.Timer updateTimer;

        protected UIDispatcher()
        {
            
        }

        public void Start()
        {
            if (updateTimer == null)
            {
                updateTimer = new System.Timers.Timer(50);
                updateTimer.Elapsed += UpdateTimer_Elapsed;
                updateTimer.Start();
            }
        }

        private void UpdateTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            updateTimer.Stop();
            try
            {
                Stopwatch sw = new Stopwatch();
                sw.Restart();

                if (AllowUpdateUI)
                {
                    foreach (var panel in ServiceLocator.WorkspaceManagerService.GetAllDocumentPanel().ToArray())
                    {
                        if (panel is TagCollectionViewModel tagCollectionVM)
                        {
                            if (tagCollectionVM.WorkspaceContext is IHaveTag objHaveTags)
                            {
                                if (objHaveTags.HaveTags && objHaveTags.Tags != null)
                                {
                                    foreach (var item in objHaveTags.Tags.ToArray())
                                    {
                                        if (item is TagCore tag)
                                        {
                                            if (tag.NeedToUpdateValue)
                                            {
                                                tag.NeedToUpdateValue = false;
                                                Application.Current.Dispatcher.Invoke(() =>
                                                {
                                                    tag.RaisePropertyChanged("Value");
                                                });
                                            }

                                            if (tag.NeedToUpdateTimeStamp)
                                            {
                                                tag.NeedToUpdateTimeStamp = false;
                                                Application.Current.Dispatcher.Invoke(() =>
                                                {
                                                    tag.RaisePropertyChanged("TimeStamp");
                                                });
                                            }

                                            if (tag.NeedToUpdateQuality)
                                            {
                                                tag.NeedToUpdateQuality = false;
                                                Application.Current.Dispatcher.Invoke(() =>
                                                {
                                                    tag.RaisePropertyChanged("Quality");
                                                });
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                if (ServiceLocator.ProjectManagerService.CurrentProject != null)
                {
                    foreach (var item in ServiceLocator.ProjectManagerService.CurrentProject.Childs.ToArray())
                    {
                        if (item is RemoteStation remoteStation)
                        {
                            RefreshRemoteStationConnection(remoteStation);
                        }
                    }
                }
                sw.Stop();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Update UI error: {ex.ToString()}");
            }
            updateTimer.Start();
        }

        public void RefreshRemoteStationConnection(RemoteStation remoteStation)
        {
            if (remoteStation != null)
            {
                remoteStation.RaisePropertyChanged("ConnectionStatus");
                foreach (var item in remoteStation.Childs)
                {
                    if (item is RemoteStation childRemoteStation)
                    {
                        RefreshRemoteStationConnection(childRemoteStation);
                    }
                }
            }
        }
    }
}
