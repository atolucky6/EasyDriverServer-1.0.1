using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EasyScada.Wpf.Connector
{
    public class TestEditor : UITypeEditor
    {
        protected override object GetEditControl(string propertyName, object currentValue)
        {
            return new UserControl2();
        }

        protected override object GetEditedValue(object editControl, string propertyName, object oldValue)
        {
            return "";
        }
    }

    //public class TestEditor : CollectionEditor
    //{
    //    public TestEditor() : base(typeof(EasyDriverConnector))
    //    {
    //    }

    //    public override bool IsDropDownResizable => base.IsDropDownResizable;

    //    protected override string HelpTopic => base.HelpTopic;

    //    public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
    //    {
    //        return base.EditValue(context, provider, value);
    //    }

    //    public override bool Equals(object obj)
    //    {
    //        return base.Equals(obj);
    //    }

    //    public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
    //    {
    //        return UITypeEditorEditStyle.Modal;
    //    }

    //    public override int GetHashCode()
    //    {
    //        return base.GetHashCode();
    //    }

    //    public override bool GetPaintValueSupported(ITypeDescriptorContext context)
    //    {
    //        return base.GetPaintValueSupported(context);
    //    }

    //    public override void PaintValue(PaintValueEventArgs e)
    //    {
    //        base.PaintValue(e);
    //    }

    //    public override string ToString()
    //    {
    //        return base.ToString();
    //    }

    //    protected override void CancelChanges()
    //    {
    //        base.CancelChanges();
    //    }

    //    protected override bool CanRemoveInstance(object value)
    //    {
    //        return base.CanRemoveInstance(value);
    //    }

    //    protected override bool CanSelectMultipleInstances()
    //    {
    //        return base.CanSelectMultipleInstances();
    //    }

    //    protected override CollectionForm CreateCollectionForm()
    //    {
    //        return new TestForm(this);
    //        //return base.CreateCollectionForm();
    //    }

    //    protected override Type CreateCollectionItemType()
    //    {
    //        return base.CreateCollectionItemType();
    //    }

    //    protected override object CreateInstance(Type itemType)
    //    {
    //        return base.CreateInstance(itemType);
    //    }

    //    protected override Type[] CreateNewItemTypes()
    //    {
    //        return base.CreateNewItemTypes();
    //    }

    //    protected override void DestroyInstance(object instance)
    //    {
    //        base.DestroyInstance(instance);
    //    }

    //    protected override string GetDisplayText(object value)
    //    {
    //        return base.GetDisplayText(value);
    //    }

    //    protected override object[] GetItems(object editValue)
    //    {
    //        return base.GetItems(editValue);
    //    }

    //    protected override IList GetObjectsFromInstance(object instance)
    //    {
    //        return base.GetObjectsFromInstance(instance);
    //    }

    //    protected override object SetItems(object editValue, object[] value)
    //    {
    //        return base.SetItems(editValue, value);
    //    }

    //    protected override void ShowHelp()
    //    {
    //        base.ShowHelp();
    //    }

    //     class TestForm : CollectionForm
    //    {
    //        public TestForm(CollectionEditor editor) : base(editor)
    //        {
    //        }

    //        protected override void OnEditValueChanged()
    //        {
    //            throw new NotImplementedException();
    //        }
    //    }
    //}
}
