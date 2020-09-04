using EasyScada.Core;
using EasyScada.Winforms.Connector;
using System.ComponentModel;

namespace EasyScada.Winforms.Designer
{
    /// <summary>
    /// The interface provide the select tag action for all of the controls in Easy Scada
    /// </summary>
    public interface ISupportTag
    {
        [Description("Path to tag of the control")]
        [TypeConverter(typeof(EasyScadaTagPathConverter)), Category(DesignerCategory.EASYSCADA)]
        string PathToTag { get; set; }

        ITag LinkedTag { get; }
    }
}
