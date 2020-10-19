using EasyScada.Core;
using System.Collections.Generic;
using System.Windows.Forms;

namespace EasyScada.Winforms.Controls
{
    public class PathToTagPropertyEditor : PropertyEditorBase
    {
        SearchTagControl editControl;
        string selectedTag = "";

        protected override Control GetEditControl(string propertyName, object currentValue)
        {
            editControl = new SearchTagControl();
            TagPathConverter tagPathConverter = new TagPathConverter();
            List<string> tagPaths = new List<string>();
            foreach (var item in tagPathConverter.GetStandardValues(TypeDescriptorContext))
                tagPaths.Add(item?.ToString());
            editControl.TagPathSource = tagPaths;
            editControl.SelectedItemDoubleClick += EditControl_SelectedItemDoubleClick;
            return editControl;
        }

        private void EditControl_SelectedItemDoubleClick(object obj)
        {
            selectedTag = obj?.ToString();
            EditorService.CloseDropDown();
        }

        protected override object GetEditedValue(Control editControl, string propertyName, object oldValue)
        {
            return selectedTag;
        }
    }
}
