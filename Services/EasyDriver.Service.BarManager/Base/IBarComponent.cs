using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace EasyDriver.Service.BarManager
{
    /// <summary>
    /// Interface cơ bản cho tất cả các Menu, SubMenu, BarButton....
    /// </summary>
    public interface IBarComponent : ICloneable, INotifyPropertyChanged
    {
        ObservableCollection<IBarComponent> BarItems { get; }
        string DisplayName { get; set; }
        object Glyph { get; set; }
        object Parameter { get; set; }
        BarItemAlignment Alignment { get; set; }
        bool Enabled { get; set; }
        bool IsChecked { get; set; }
        bool IsVisible { get; set; }

        IBarComponent Add(IBarComponent barItem);
        IEnumerable<IBarComponent> Find(Func<IBarComponent, bool> predicate);
        IBarComponent GetOrCreate(string path, bool createIfNotExists);
        bool Remove(IBarComponent barItem);
        void SetCommand(ICommand command);
        void Clear();
        T Clone<T>();
    }
}
