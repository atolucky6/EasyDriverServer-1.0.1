using DevExpress.Mvvm.UI.Interactivity;
using DevExpress.Xpf.Editors;
using EasyDriverPlugin;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace EasyScada.ServerApplication
{
    //public class FindDataTypeSourceForAutoSuggestEdit : Behavior<AutoSuggestEdit>
    //{
    //    IDevice Device { get => (AssociatedObject.Tag as ITag).Parent as IDevice; }

    //    protected override void OnAttached()
    //    {
    //        base.OnAttached();
    //        if (AssociatedObject.Tag is IDataTypeIndex dtIndex)
    //        {
    //            var dataType = dtIndex.Parent as IDataType;
    //            if (dataType.IsUserDataType)
    //            {
    //                var dataTypeSource = ((dtIndex.Parent as IDataType).Parent as IDevice).GetDataTypes().Where(x => x != (dtIndex.Parent as IDataType));
    //                AssociatedObject.ItemsSource = dataTypeSource;
    //            }
    //            else
    //                AssociatedObject.ItemsSource = new List<IDataType>() { dtIndex.DataType };
    //        }
    //        else if (AssociatedObject.Tag is ITag tag)
    //        {
    //            var dataTypeSource = Device.GetDataTypes();
    //            AssociatedObject.ItemsSource = dataTypeSource;
    //        }
    //        AssociatedObject.QuerySubmitted += AssociatedObject_QuerySubmitted;
    //    }

    //    private void AssociatedObject_QuerySubmitted(object sender, AutoSuggestEditQuerySubmittedEventArgs e)
    //    {
    //        if (AssociatedObject.Tag is IDataTypeIndex dtIndex)
    //        {
    //            var dataType = dtIndex.Parent as IDataType;
    //            if (dataType.IsUserDataType)
    //            {
    //                var dataTypeSource = ((dtIndex.Parent as IDataType).Owner.Parent as IDevice).GetDataTypes().Where(x => x != (dtIndex.Parent as IDataType));
    //                AssociatedObject.ItemsSource = string.IsNullOrEmpty(e.Text) ? dataTypeSource :
    //                    dataTypeSource.Where(x => Regex.IsMatch(x.Name, Regex.Escape(e.Text), RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace));

    //            }
    //            else
    //                AssociatedObject.ItemsSource = new List<IDataType>() { dtIndex.DataType };
    //        }
    //        else
    //            AssociatedObject.ItemsSource = string.IsNullOrEmpty(e.Text) ? Device.GetDataTypes() :
    //                Device.GetDataTypes().Where(x => Regex.IsMatch(x.Name, Regex.Escape(e.Text), RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace));
    //    }
    //}
}
