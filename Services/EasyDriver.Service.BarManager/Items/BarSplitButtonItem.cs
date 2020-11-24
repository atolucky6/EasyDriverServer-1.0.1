using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace EasyDriver.Service.BarManager
{
    [Serializable]
    public class BarSplitButtonItem : BarComponentBase
    {
        #region Members

        public ICommand Command { get; set; }
        public BarItemType Type { get; protected set; }
        public virtual object KeyGesture { get; set; }

        #endregion

        #region Constructors

        public BarSplitButtonItem(string displayName = null, object imageSource = null) : base(displayName, imageSource)
        {
            Type = BarItemType.ButtonSplitItem;
        }

        #endregion

        #region Methods

        public override IBarComponent Add(IBarComponent barItem)
        {
            BarItems.Add(barItem);
            return this;
        }

        public override IEnumerable<IBarComponent> Find(Func<IBarComponent, bool> predicate)
        {
            return BarItems.Where(predicate);
        }

        public override bool Remove(IBarComponent barItem)
        {
            return BarItems.Remove(barItem);
        }

        public override object Clone()
        {
            var cloneItem = MemberwiseClone() as BarSplitButtonItem;
            cloneItem.BarItems = new ObservableCollection<IBarComponent>();
            return cloneItem;
        }

        public override void SetCommand(ICommand command) => Command = command;

        public override void Clear() => BarItems.Clear();

        #endregion
    }
}
