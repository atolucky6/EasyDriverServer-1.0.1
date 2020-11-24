namespace EasyDriver.Service.Reversible
{
    public abstract class ReversibleChange
    {
        protected static void Switch<T>(ref T value1, ref T value2)
        {
            T oldValue = value1;
            value1 = value2;
            value2 = oldValue;
        }

        public abstract bool Reverse();

        public virtual string Name { get; protected set; }
    }
}
