using EasyScada.Core.Animate;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace EasyScada.Core
{
    public class TriggerCollection<TPropertyWrapper> : TriggerCollection
        where TPropertyWrapper : AnimatePropertyWrapper, new()
    {
        public override int Add(object value)
        {
            if (value is TriggerBase triggerBase)
            {
                triggerBase.AnimatePropertyWrapper = new TPropertyWrapper();
                triggerBase.AnimatePropertyWrapper.TargetControl = TargetControl;
                triggerBase.Target = TargetControl;
            }
            return base.Add(value);
        }

        public override void Add(TriggerBase item)
        {
            if (item != null)
            {
                if (item.AnimatePropertyWrapper == null)
                {
                    item.AnimatePropertyWrapper = new TPropertyWrapper
                    {
                        TargetControl = TargetControl
                    };
                }
                if (item.Target != TargetControl)
                    item.Target = TargetControl;
                base.Add(item);
            }
        }
    }

    [DesignerCategory("code")]
    public abstract class TriggerCollection : TypedCollection<TriggerBase>
    {
        public virtual object TargetControl { get; set; }

        public bool Enabled { get; set; }
        private Dictionary<string, ITag> triggerValueTagDic = new Dictionary<string, ITag>();
        private Dictionary<string, ITag> triggerQualityTagDic = new Dictionary<string, ITag>();
        private IEasyDriverConnector driverConnector = EasyDriverConnectorProvider.GetEasyDriverConnector();

        public virtual void Start()
        {
            Enabled = true;
            foreach (var item in this)
            {
                if (item is QualityTrigger)
                {
                    if (!triggerQualityTagDic.ContainsKey(item.TriggerTagPath))
                        triggerQualityTagDic.Add(item.TriggerTagPath, driverConnector.GetTag(item.TriggerTagPath));
                }
                else
                {
                    if (!triggerValueTagDic.ContainsKey(item.TriggerTagPath))
                        triggerValueTagDic.Add(item.TriggerTagPath, driverConnector.GetTag(item.TriggerTagPath));
                }
                foreach (var animateProperty in item.AnimatePropertyWrapper.GetAnimateProperties())
                {
                    animateProperty.IsDirty = true;
                }
            }

            foreach (var kvp in triggerQualityTagDic)
            {
                if (kvp.Value is ITag tag)
                {
                    tag.QualityChanged += OnTriggerTagQualityChanged;
                    OnTriggerTagQualityChanged(tag, new TagQualityChangedEventArgs(tag, Quality.Uncertain, tag.Quality));
                }
            }

            foreach (var kvp in triggerValueTagDic)
            {
                if (kvp.Value is ITag tag)
                {
                    tag.ValueChanged += OnTriggerTagValueChanged;
                    OnTriggerTagValueChanged(tag, new TagValueChangedEventArgs(tag, "", tag.Value));
                }
            }
        }

        private async void OnTriggerTagValueChanged(object sender, TagValueChangedEventArgs e)
        {
            await Task.Factory.StartNew(() =>
            {
                IEditableObject editableObject = TargetControl as IEditableObject;
                editableObject?.BeginEdit();
                foreach (var item in this)
                {
                    if (!(item is QualityTrigger) && item.TriggerTagPath == e.Tag.Path)
                        item.Execute();
                }
                editableObject?.EndEdit();
            });
        }

        private async void OnTriggerTagQualityChanged(object sender, TagQualityChangedEventArgs e)
        {
            await Task.Factory.StartNew(() =>
            {
                IEditableObject editableObject = TargetControl as IEditableObject;
                editableObject?.BeginEdit();
                foreach (var item in this)
                {
                    if (item is QualityTrigger trigger && item.TriggerTagPath == e.Tag.Path)
                        trigger.Execute();
                }
                editableObject?.EndEdit();
            });
        }

        public virtual void Stop()
        {
            Enabled = false;
        }
    }
}
