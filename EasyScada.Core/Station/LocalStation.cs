using EasyDriverPlugin;
using System;
using System.ComponentModel;

namespace EasyScada.Core
{
    [Serializable]
    public class LocalStation : GroupItemBase, IStation
    {
        public LocalStation(IGroupItem parent, bool isReadOnly = false) : base(parent, isReadOnly)
        {
            Name = "Local station";
        }

        [Category(PropertyCategory.General), DisplayName("Parameters")]
        public IParameterContainer ParameterContainer { get; set; }

        [Browsable(false)]
        public object SyncObject { get; set; }

        public override string GetErrorOfProperty(string propertyName)
        {
            return string.Empty;
        }

        public override void GetErrors(ref IErrorInfo errorInfo)
        {
            throw new NotImplementedException();
        }
    }
}
