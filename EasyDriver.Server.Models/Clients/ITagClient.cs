using EasyDriverPlugin;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace EasyDriver.Core
{
    public interface ITagClient : IPath, INotifyPropertyChanged
    {
        string Name { get; }
        string Address { get; }
        string DataType { get; }
        string Value { get; }
        Quality Quality { get; }
        int RefreshRate { get; }
        int RefreshInterval { get; }
        AccessPermission AccessPermission { get; }
        string Error { get; }
        DateTime TimeStamp { get; }
        Dictionary<string, object> Parameters { get; }

        event EventHandler<TagValueChangedEventArgs> ValueChanged;
        event EventHandler<TagQualityChangedEventArgs> QualityChanged;
    }

    [Serializable]
    public sealed class TagClient : ITagClient
    {
        public string Name { get; set; }

        public string Address { get; set; }

        public string DataType { get; set; }

        string value;
        public string Value
        {
            get { return value; }
            set
            {
                if (this.value != value)
                {
                    string oldValue = this.value;
                    this.value = value;
                    RaisePropertyChanged();
                    //if (ValueChanged != null)
                    //{
                    //    var eventListeners = ValueChanged.GetInvocationList();
                    //    for (int i = 0; i < eventListeners.Count(); i++)
                    //    {
                    //        var methodInvoke = (EventHandler<TagValueChangedEventArgs>)eventListeners[i];
                    //        methodInvoke.BeginInvoke(this, new TagValueChangedEventArgs(oldValue, value), ValueChangedAsyncCallback, null);
                    //    }
                    //}
                    ValueChanged?.Invoke(this, new TagValueChangedEventArgs(oldValue, value));
                }
            }
        }

        private void ValueChangedAsyncCallback(IAsyncResult result)
        {
            //var ar = (System.Runtime.Remoting.Messaging.AsyncResult)result;
            //var invokedMethod = (EventHandler<TagValueChangedEventArgs>)ar.AsyncDelegate;

            //try
            //{
            //    invokedMethod.EndInvoke(result);
            //}
            //catch
            //{
            //    // Handle any exceptions that were thrown by the invoked method
            //    Console.WriteLine("An event listener went kaboom!");
            //}
        }

        Quality quality;
        public Quality Quality
        {
            get { return quality; }
            set
            {
                if (quality != value)
                {
                    Quality oldValue = quality;
                    quality = value;
                    RaisePropertyChanged();
                    QualityChanged?.Invoke(this, new TagQualityChangedEventArgs(oldValue, value));
                }
            }
        }

        public int RefreshRate { get; set; }

        int refreshInterval;
        public int RefreshInterval
        {
            get { return refreshInterval; }
            set
            {
                if (refreshInterval != value)
                {
                    refreshInterval = value;
                    RaisePropertyChanged();
                }
            }
        }

        public AccessPermission AccessPermission { get; set; }

        string error;
        public string Error
        {
            get { return error; }
            set
            {
                if (error != value)
                {
                    error = value;
                    RaisePropertyChanged();
                }
            }
        }

        DateTime timeStamp;
        public DateTime TimeStamp
        {
            get { return timeStamp; }
            set
            {
                if (timeStamp != value)
                {
                    timeStamp = value;
                    RaisePropertyChanged();
                }
            }
        }

        public Dictionary<string, object> Parameters { get; set; }

        public string Path { get; set; }

        public bool Checked { get; set; }

        T IPath.GetItem<T>(string pathToObject)
        {
            if (Path == pathToObject)
                return this as T;
            return null;
        }

        [field: NonSerialized]
        public event EventHandler<TagValueChangedEventArgs> ValueChanged;

        [field: NonSerialized]
        public event EventHandler<TagQualityChangedEventArgs> QualityChanged;

        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
