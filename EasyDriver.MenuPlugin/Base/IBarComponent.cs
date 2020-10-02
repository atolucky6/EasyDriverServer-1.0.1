using DevExpress.Xpf.Bars;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Media;

namespace EasyDriver.MenuPlugin
{
    /// <summary>
    /// Interface cơ bản cho tất cả các Menu, SubMenu, BarButton....
    /// </summary>
    public interface IBarComponent : ICloneable, INotifyPropertyChanged
    {
        ObservableCollection<IBarComponent> BarItems { get; }
        string DisplayName { get; set; }
        ImageSource Glyph { get; set; }
        object Parameter { get; set; }
        BarItemAlignment Alignment { get; set; }
        bool IsChecked { get; set; }
        IBarComponent Add(IBarComponent barItem);
        IEnumerable<IBarComponent> Find(Func<IBarComponent, bool> predicate);
        bool Remove(IBarComponent barItem);
        void SetCommand(ICommand command);
        void Clear();
        T Clone<T>();
    }
}
