using DevExpress.Xpf.Bars;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Media;

namespace EasyDriver.MenuPlugin
{
    /// <summary>
    /// Lớp abstract cơ bản cho tất cả các Menu, SubMenu, BarButton.... 
    /// </summary>
    [Serializable]
    internal abstract class BarComponentBase : IBarComponent
    {
        #region Public members
        string displayName;
        public virtual string DisplayName { get => displayName?.Trim(); set { displayName = value; RaisePropertyChanged(); } }
        ImageSource glyph;
        public virtual ImageSource Glyph { get => glyph; set { glyph = value; RaisePropertyChanged(); } }
        public virtual object Parameter { get; set; }
        public virtual BarItemAlignment Alignment { get; set; }
        public ObservableCollection<IBarComponent> BarItems { get; protected set; }
        private bool isChecked;
        public virtual bool IsChecked { get => isChecked; set { isChecked = value; RaisePropertyChanged(); } }
        #endregion

        #region Constructors
        public BarComponentBase(string displayName, ImageSource imageSource = null)
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
        #endregion

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged([CallerMemberName]string propertyChanged = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyChanged));
        }
        #endregion
    }
}
