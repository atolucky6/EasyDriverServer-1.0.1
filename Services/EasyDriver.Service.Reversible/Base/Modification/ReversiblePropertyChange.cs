namespace EasyDriver.Service.Reversible
{
    /// <summary>
    /// Đại diện cho hàm dùng để đặt giá trị cho thuộc tính
    /// </summary>
    /// <typeparam name="TSource">Kiểu của đối tượng sỡ hữu thuộc tính</typeparam>
    /// <typeparam name="TProperty">Kiểu của thuộc tính</typeparam>
    /// <param name="source">Đối tượng sỡ hữu thuộc tính</param>
    /// <param name="newValue">Giá trị mới của thuộc tính</param>
    public delegate void ReversiblePropertySetter<TSource, TProperty>(TSource source, TProperty newValue) where TSource : class;

    /// <summary>
    /// Đại diện cho hàm dùng để đặt giá trị cho thuộc tính
    /// </summary>
    /// <typeparam name="TProperty">Kiểu của thuộc tính</typeparam>
    /// <param name="newValue">Giá trị mới của thuộc tính</param>
    public delegate void ReversibleInstancePropertySetter<TProperty>(TProperty newValue);

    /// <summary>
    /// Lớp đại diện cho sự thay đổi của 1 thuộc tính trong 1 đối tượng
    /// </summary>
    /// <typeparam name="TSource">Kiểu của đối tượng sỡ hữu thuộc tính</typeparam>
    /// <typeparam name="TProperty">Kiểu của thuộc tính bị thay đổi</typeparam>
    public class ReversiblePropertyChange<TSource, TProperty> : ReversibleChange
        where TSource : class
    {
        protected readonly ReversiblePropertySetter<TSource, TProperty> setter;
        protected readonly TSource source;
        protected TProperty newValue;
        protected TProperty oldValue;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="setter"></param>
        /// <param name="source"></param>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        public ReversiblePropertyChange(ReversiblePropertySetter<TSource, TProperty> setter, TSource source, TProperty oldValue, TProperty newValue)
        {
            this.setter = setter;
            this.source = source;
            this.oldValue = oldValue;
            this.newValue = newValue;
        }

        /// <summary>
        /// Đảo ngược lại sự thay đổi của đối tượng
        /// </summary>
        /// <returns></returns>
        public override bool Reverse()
        {
            setter(source, oldValue);
            Switch(ref oldValue, ref newValue);
            return true;
        }
    }

    /// <summary>
    /// Lớp đại diện cho sự thay đổi của 1 thuộc tính trong 1 đối tượng
    /// </summary>
    /// <typeparam name="TSource">Kiểu của đối tượng sỡ hữu thuộc tính</typeparam>
    /// <typeparam name="TProperty">Kiểu của thuộc tính bị thay đổi</typeparam>
    public class ReversiblePropertyObjectChange<TSource, TProperty> : ReversiblePropertyChange<TSource, TProperty>
        where TSource : class
    {
        public ReversiblePropertyObjectChange(string propertyName, TSource source, TProperty newValue)
            : base((s, v) =>
            {
                Transaction.AddPropertyChange(propertyName, s, v);
                s.SetPropertyByReflection(propertyName, v);
            }, source, source.GetPropertyByReflection<TSource, TProperty>(propertyName), newValue)
        {
        }

        public ReversiblePropertyObjectChange(string propertyName, TSource source, TProperty oldValue, TProperty newValue)
            : base((s, v) =>
            {
                Transaction.AddPropertyChange(propertyName, s, v);
                s.SetPropertyByReflection(propertyName, v);
            }, source, oldValue, newValue)
        {
        }
    }
}
