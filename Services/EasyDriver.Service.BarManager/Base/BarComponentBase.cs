using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace EasyDriver.Service.BarManager
{
    /// <summary>
    /// Lớp abstract cơ bản cho tất cả các Menu, SubMenu, BarButton.... 
    /// </summary>
    [Serializable]
    public abstract class BarComponentBase : IBarComponent
    {
        #region Public members
        string displayName;
        public virtual string DisplayName { get => displayName?.Trim(); set { displayName = value; RaisePropertyChanged(); } }
        object glyph;
        public virtual object Glyph { get => glyph; set { glyph = value; RaisePropertyChanged(); } }
        public virtual object Parameter { get; set; }

        private BarItemAlignment alignment;
        public virtual BarItemAlignment Alignment
        {
            get => alignment;
            set
            {
                if (alignment != value)
                {
                    alignment = value;
                    RaisePropertyChanged();
                }
            }
        }
        public ObservableCollection<IBarComponent> BarItems { get; protected set; }
        private bool isChecked;
        public virtual bool IsChecked { get => isChecked; set { isChecked = value; RaisePropertyChanged(); } }
        private bool enabled = true;
        public virtual bool Enabled
        { 
            get => enabled; 
            set 
            { 
                enabled = value; 
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(IsVisible));
            }
        }
        private bool isVisible = true;
        public virtual bool IsVisible 
        { 
            get
            {
                if (HideWhenDisable)
                    return Enabled;
                return isVisible;
            }
            set { isVisible = value; RaisePropertyChanged(); } 
        }

        bool hideWhenDisable;
        public virtual bool HideWhenDisable
        {
            get => hideWhenDisable;
            set
            {
                if (value != hideWhenDisable)
                {
                    hideWhenDisable = value;
                    RaisePropertyChanged();
                    RaisePropertyChanged(nameof(IsVisible));
                }
            }
        }
        #endregion

        #region Constructors
        public BarComponentBase(string displayName, object imageSource = null)
        {
            DisplayName = displayName;
            Glyph = imageSource;
            BarItems = new ObservableCollection<IBarComponent>();
        }
        #endregion

        #region Public Methods
        public abstract IBarComponent Add(IBarComponent barItem);
        public abstract bool Remove(IBarComponent barItem);
        public abstract IEnumerable<IBarComponent> Find(Func<IBarComponent, bool> predicate);
        public virtual object Clone() => MemberwiseClone();
        public T Clone<T>() => (T)Clone();
        public virtual void SetCommand(ICommand command) { }
        public virtual void Clear() { }
        public virtual IBarComponent GetOrCreate(string path, bool createIfNotExists)
        {
            if (string.IsNullOrWhiteSpace(path))
                return this;

            string[] pathSplit = path.Split('/');

            string childName = pathSplit[0];
            IBarComponent childItem = BarItems.FirstOrDefault(x => x.DisplayName == childName);
            if (childItem == null)
            {
                if (this is BarSubItem subItem && createIfNotExists)
                {
                    childItem = BarFactory.Default.CreateSubItem(false, childName);
                    BarItems.Add(childItem);
                }
                else { return null; }
            }

            string[] childPathArray = new string[pathSplit.Length - 1];
            Array.Copy(pathSplit, 1, childPathArray, 0, childPathArray.Length);
            return childItem.GetOrCreate(string.Join("/", childPathArray), createIfNotExists);
        }
        #endregion

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged([CallerMemberName] string propertyChanged = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyChanged));
        }
        #endregion
    }
}
