using EasyScada.Core.Designer;
using System.Windows.Forms;

namespace EasyScada.Winforms.Controls
{
    public class PathToTagPropertyEditor : PropertyEditorBase
    {
        protected override Control GetEditControl(string propertyName, object currentValue)
        {
            PathToTagPropertyControl editControl = new PathToTagPropertyControl();
            return editControl;
        }

        protected override object GetEditedValue(Control editControl, string propertyName, object oldValue)
        {
            return "";
        }
    }
}
