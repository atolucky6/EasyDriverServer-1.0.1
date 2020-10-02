using EasyDriver.ServiceContainer;
using EasyDriver.ServicePlugin;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace EasyDriver.CopyPaste
{
    public class CopyPasteService : EasyServicePlugin, ICopyPasteService, INotifyPropertyChanged
    {
        public CopyPasteService(IServiceContainer serviceContainer) : base(serviceContainer)
        {
        }

        object context;
        public object Context
        {
            get => context;
            set
            {
                if (context != value)
                {
                    context = value;
                    RaisePropertyChanged();
                }
            }
        }

        object objectToCopy;
        public object ObjectToCopy
        {
            get => objectToCopy;
            set
            {
                if (objectToCopy != value)
                {
                    objectToCopy = value;
                    RaisePropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public bool ContainData()
        {
            return Context != null && ObjectToCopy != null;
        }

        public void CopyToClipboard(object objToCopy, object context)
        {
            Clear();
            Context = context;
            ObjectToCopy = objToCopy;
        }

        public void Clear()
        {
            Context = null;
            ObjectToCopy = null;
        }

        public override void BeginInit()
        {
        }

        public override void EndInit()
        {
        }

        protected void RaisePropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
