using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace EasyDriver.Service.BarManager
{
    [Serializable]
    public class BarItem : BarComponentBase
    {
        #region Public members

        public ICommand Command { get; set; }
        public BarItemType Type { get; set; }
        public virtual object KeyGesture { get; set; }

        #endregion

        #region Constructors

        public BarItem(BarItemType type, string displayName = null, object imageSource = null) : base(displayName, imageSource)
        {
            Type = type;
        }

        #endregion

        #region Public methods

        public override void SetCommand(ICommand command) => Command = command;

        public override IBarComponent Add(IBarComponent barItem)
        {
            return this;
        }

        public override IEnumerable<IBarComponent> Find(Func<IBarComponent, bool> predicate)
        {
            return null;
        }

        public override bool Remove(IBarComponent barItem)
        {
            return false;
        }

        #endregion
    }
}
