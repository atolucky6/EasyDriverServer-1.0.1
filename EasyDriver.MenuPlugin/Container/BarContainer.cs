using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media;

namespace EasyDriver.MenuPlugin
{
    [Serializable]
    internal class BarContainer : BarComponentBase
    {
        #region Public members

        public BarContainerType Type { get; protected set; }

        #endregion

        #region Constructors

        public BarContainer(string displayName, BarContainerType type, ImageSource imageSource = null) : base(displayName, imageSource)
        {
            BarItems = new ObservableCollection<IBarComponent>();
            Type = type;
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
            var result = MemberwiseClone() as BarContainer;
            result.BarItems = new ObservableCollection<IBarComponent>();
            return result;
        }

        #endregion
    }
}
