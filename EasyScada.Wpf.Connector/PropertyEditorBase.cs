using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Forms.Design;


namespace EasyScada.Wpf.Connector
{
    public abstract class UITypeEditor : System.Drawing.Design.UITypeEditor
    {
        protected virtual ITypeDescriptorContext TypeDescriptorContext { get; set; }
        protected virtual IWindowsFormsEditorService EditorService { get; set; }
        protected virtual object EditControl { get; set; }
        protected virtual bool EscapePressed { get; set; }

        protected abstract object GetEditControl(string propertyName, object currentValue);
        protected abstract object GetEditedValue(object editControl, string propertyName, object oldValue);

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            try
            {
                string propName = context.PropertyDescriptor.Name;
                object currentValue = context.PropertyDescriptor.GetValue(context.Instance);
                object control = GetEditControl(propName, currentValue);
                if (control is Form)
                    return UITypeEditorEditStyle.Modal;
                if (control is Window)
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
                        if (EditControl is Control windowFormControl)
                        {
                            EscapePressed = false;

                            if (EditControl is Form editForm)
                                EditorService.ShowDialog(editForm);
                            else
                                EditorService.DropDownControl(windowFormControl);

                            return EscapePressed ? value : GetEditedValue(windowFormControl, propName, value);
                        }
                        else if (EditControl is Window window)
                        {
                            window.ShowDialog();
                        }
                    }
                }
            }
            catch { }
            return base.EditValue(context, provider, value);
        }
    }
}
