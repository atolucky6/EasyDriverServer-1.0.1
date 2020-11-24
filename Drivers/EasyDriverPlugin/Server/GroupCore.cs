using System;
using System.Collections.Generic;
using EasyDriverPlugin;
using Newtonsoft.Json;

namespace EasyDriverPlugin
{
    public class GroupCore : GroupItemBase, IHaveTag
    {
        #region Constructors
        public GroupCore(IGroupItem parent, bool haveTags, bool isReadOnly = false) : base(parent, isReadOnly)
        {
            Tags = new NotifyCollection(this);
            HaveTags = haveTags;
        }
        #endregion

        public override ItemType ItemType { get; set; } = ItemType.Group;

        #region Methods
        public override string GetErrorOfProperty(string propertyName)
        {
            return null;
        }
        #endregion

        #region IHaveTags

        public bool HaveTags { get; set; }

        public NotifyCollection Tags { get; protected set; }

        #endregion
    }

}
