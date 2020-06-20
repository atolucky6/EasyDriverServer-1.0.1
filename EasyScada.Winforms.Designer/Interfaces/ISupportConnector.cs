using EasyScada.Winforms.Connector;
using System.ComponentModel;

namespace EasyScada.Winforms.Designer
{
    /// <summary>
    /// The interface provide the connector to server for all of the controls in Easy Scada
    /// </summary>
    public interface ISupportConnector
    {
        [Description("The driver connector to Easy Driver Server")]
        [Browsable(true), Category(DesignerCategory.EASYSCADA)]
        EasyDriverConnector Connector { get; set; }
    }
}
