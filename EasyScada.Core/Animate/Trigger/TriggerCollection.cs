namespace EasyScada.Core
{
    public class TriggerCollection<TPropertyWrapper> : TriggerCollection
        where TPropertyWrapper : AnimatePropertyWrapper, new()
    {
        public object TargetControl { get; set; }

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
            item.AnimatePropertyWrapper = new TPropertyWrapper();
            item.AnimatePropertyWrapper.TargetControl = TargetControl;
            item.Target = TargetControl;
            base.Add(item);
        }
    }

    public abstract class TriggerCollection : TypedCollection<TriggerBase>
    {

    }
}
