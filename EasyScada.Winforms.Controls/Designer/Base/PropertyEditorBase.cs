using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace EasyScada.Winforms.Controls
{
    public abstract class PropertyEditorBase : UITypeEditor
    {
        protected virtual ITypeDescriptorContext TypeDescriptorContext { get; set; }
        protected virtual IWindowsFormsEditorService EditorService { get; set; }
        protected virtual Control EditControl { get; set; }
        protected virtual bool EscapePressed { get; set; }

        protected abstract Control GetEditControl(string propertyName, object currentValue);
        protected abstract object GetEditedValue(Control editControl, string propertyName, object oldValue);

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            try
            {
                string propName = context.PropertyDescriptor.Name;
                object currentValue = context.PropertyDescriptor.GetValue(context.Instance);
                Control control = GetEditControl(propName, currentValue);
                if (control is Form)
                    return UITypeEditorEditStyle.Modal;
            }
            catch { }
            return UITypeEditorEditStyle.DropDown;
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            try
            {
                TypeDescriptorContext = context;
                EditorService = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;
                if (EditorService != null)
                {
                    string propName = context.PropertyDescriptor.Name;
                    EditControl = GetEditControl(propName, value);
                    if (EditControl != null)
                    {
                        EscapePressed = false;

                        if (EditControl is Form editForm)
                            EditorService.ShowDialog(editForm);
                        else
                            EditorService.DropDownControl(EditControl);

                        return EscapePressed ? value : GetEditedValue(EditControl, propName, value);
                    }

                }
            }
            catch { }
            return base.EditValue(context, provider, value);
        }
    }
}
