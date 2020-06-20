using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace EasyScada.ServerApplication.Reversible
{
    public static class ReversibleExtensions
    {
        #region Convert to reversible

        public static ReversibleObject<TSource> AsReversibleObject<TSource>(this TSource source)
            where TSource : class
        {
            return new ReversibleObject<TSource>(source);
        }

        public static ReversibleCollection<TItem> AsReversibleCollection<TItem>(this IList<TItem> source)
        {
            return new ReversibleCollection<TItem>(source);
        }

        #endregion

        #region Set property reversible

        public static void AddPropertyChangedReversible<TSource, TProperty>(this TSource source, string propertyName, TProperty oldValue, TProperty newValue)
            where TSource : class
        {
            Transaction.AddPropertyChanged(source, propertyName, oldValue, newValue);
        }

        public static void AddPropertyChangedReversible<TSource, TProperty>(this TSource source, Expression<Func<TSource, TProperty>> propertyExpression, TProperty oldValue, TProperty newValue)
            where TSource : class
        {
            Transaction.AddPropertyChanged(source, propertyExpression.GetPropertyName(), oldValue, newValue);

        }

        public static void SetPropertyReversible<TSource, TProperty>(this TSource source, string propertyName, TProperty newValue)
            where TSource : class
        {
            Transaction.AddPropertyChange(propertyName, source, newValue);
            source.GetType().GetProperty(propertyName).SetValue(source, newValue);
        }

        public static void SetPropertyReversible<TSource, TProperty>(this TSource source, Expression<Func<TSource, TProperty>> propertyExpression, TProperty newValue)
            where TSource : class
        {
            SetPropertyReversible(source, propertyExpression.GetPropertyName(), newValue);
        }

        #endregion

        #region Capture property

        public static void CaptureProperty<TSource, T1>(
            this TSource source, 
            Expression<Func<TSource, T1>> propertyExpression1)
            where TSource : class
        {
            SetPropertyReversible(source, propertyExpression1.GetPropertyName(), source.GetPropertyByReflection<TSource, T1>(propertyExpression1.GetPropertyName()));
        }

        public static void CaptureProperty<TSource, T1, T2>(
            this TSource source, 
            Expression<Func<TSource, T1>> propertyExpression1,
            Expression<Func<TSource, T2>> propertyExpression2)
            where TSource : class
        {

            SetPropertyReversible(source, propertyExpression1.GetPropertyName(), source.GetPropertyByReflection<TSource, T1>(propertyExpression1.GetPropertyName()));
            SetPropertyReversible(source, propertyExpression2.GetPropertyName(), source.GetPropertyByReflection<TSource, T2>(propertyExpression2.GetPropertyName()));
        }

        public static void CaptureProperty<TSource, T1, T2, T3>(
            this TSource source,
            Expression<Func<TSource, T1>> propertyExpression1,
            Expression<Func<TSource, T2>> propertyExpression2,
            Expression<Func<TSource, T3>> propertyExpression3)
            where TSource : class
        {
            SetPropertyReversible(source, propertyExpression1.GetPropertyName(), source.GetPropertyByReflection<TSource, T1>(propertyExpression1.GetPropertyName()));
            SetPropertyReversible(source, propertyExpression2.GetPropertyName(), source.GetPropertyByReflection<TSource, T2>(propertyExpression2.GetPropertyName()));
            SetPropertyReversible(source, propertyExpression3.GetPropertyName(), source.GetPropertyByReflection<TSource, T3>(propertyExpression3.GetPropertyName()));

        }

        public static void CaptureProperty<TSource, T1, T2, T3, T4>(
            this TSource source,
            Expression<Func<TSource, T1>> propertyExpression1,
            Expression<Func<TSource, T2>> propertyExpression2,
            Expression<Func<TSource, T3>> propertyExpression3,
            Expression<Func<TSource, T4>> propertyExpression4)
            where TSource : class
        {
            SetPropertyReversible(source, propertyExpression1.GetPropertyName(), source.GetPropertyByReflection<TSource, T1>(propertyExpression1.GetPropertyName()));
            SetPropertyReversible(source, propertyExpression2.GetPropertyName(), source.GetPropertyByReflection<TSource, T2>(propertyExpression2.GetPropertyName()));
            SetPropertyReversible(source, propertyExpression3.GetPropertyName(), source.GetPropertyByReflection<TSource, T3>(propertyExpression3.GetPropertyName()));
            SetPropertyReversible(source, propertyExpression4.GetPropertyName(), source.GetPropertyByReflection<TSource, T4>(propertyExpression4.GetPropertyName()));
        }

        #endregion

        #region Capture collection

        #endregion

        #region Internal use

        /// <summary>
        /// Lấy tên của thuộc tính trong <see cref="LambdaExpression"/>
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        internal static string GetPropertyName(this LambdaExpression expression)
        {
            MemberExpression memberExpression = expression.Body as MemberExpression;
            return memberExpression.Member.Name;
        }

        /// <summary>
        /// Lấy giá trị thuộc tính của đối tượng thông qua Reflection
        /// </summary>
        /// <typeparam name="TSource">Kiểu của đối tượng cần lấy giá trị</typeparam>
        /// <typeparam name="TProperty">Kiểu của thuộc tính cần lấy giá trị</typeparam>
        /// <param name="source">Đối tượng cần lấy giá trị</param>
        /// <param name="propertyName">Tên của thuộc tính cần lấy giá trị</param>
        /// <returns></returns>
        internal static TProperty GetPropertyByReflection<TSource, TProperty>(this TSource source, string propertyName)
            where TSource : class
        {
            return (TProperty)source.GetType().GetProperty(propertyName).GetValue(source);
        }

        /// <summary>
        /// Lấy giá trị thuộc tính của đối tượng thông qua Reflection
        /// </summary>
        /// <typeparam name="TSource">Kiểu của đối tượng cần lấy giá trị</typeparam>
        /// <typeparam name="TProperty">Kiểu của thuộc tính cần lấy giá trị</typeparam>
        /// <param name="source">Đối tượng cần lấy giá trị</param>
        /// <param name="propertyExpression">Tên của thuộc tính cần lấy giá trị</param>
        /// <returns></returns>
        internal static TProperty GetPropertyByReflection<TSource, TProperty>(this TSource source, Expression<Func<TSource, TProperty>> propertyExpression)
            where TSource : class
        {
            return GetPropertyByReflection<TSource, TProperty>(source, GetPropertyName(propertyExpression));
        }

        /// <summary>
        /// Đặt giá trị cho thuộc tính của đối tượng thông qua Reflection
        /// </summary>
        /// <typeparam name="TSource">Kiểu của đối tượng cần đặt giá trị</typeparam>
        /// <typeparam name="TProperty">Kiểu của thuộc tính cần đặt giá trị</typeparam>
        /// <param name="source">Đối tượng cần đặt giá trị</param>
        /// <param name="propertyName">Tên của thuộc tính cần đặt giá trị</param>
        /// <param name="value">Giá trị đặt cho thuộc tính</param>
        internal static void SetPropertyByReflection<TSource, TProperty>(this TSource source, string propertyName, TProperty value)
            where TSource : class
        {
            source.GetType().GetProperty(propertyName).SetValue(source, value);
        }

        /// <summary>
        /// Đặt giá trị cho thuộc tính của đối tượng thông qua Reflection
        /// </summary>
        /// <typeparam name="TSource">Kiểu của đối tượng cần đặt giá trị</typeparam>
        /// <typeparam name="TProperty">Kiểu của thuộc tính cần đặt giá trị</typeparam>
        /// <param name="source">Đối tượng cần đặt giá trị</param>
        /// <param name="propertyName">Tên của thuộc tính cần đặt giá trị</param>
        /// <param name="value">Giá trị đặt cho thuộc tính</param>
        internal static void SetPropertyByReflection<TSource, TProperty>(this TSource source, Expression<Func<TSource, TProperty>> propertyExpression, TProperty value)
            where TSource : class
        {
            SetPropertyByReflection(source, GetPropertyName(propertyExpression), value);
        }

        #endregion
    }

    public class ReversibleObject<TSource>
        where TSource : class
    {
        readonly TSource source;

        public ReversibleObject(TSource source)
        {
            this.source = source;
        }

        public void Set<TProperty>(Expression<Func<TSource, TProperty>> propertyExpression, TProperty newValue)
        {
            Set(propertyExpression.GetPropertyName(), newValue);
        }

        public void Set<TProperty>(string propertyName, TProperty newValue)
        {
            Transaction.AddPropertyChange(propertyName, source, newValue);
            source.SetPropertyByReflection(propertyName, newValue);
        }     
    }

    public class ReversibleCollection<TItem> : IList<TItem>
    {
        #region Members

        /// <summary>
        /// Thành phần trong danh sách
        /// </summary>
        readonly IList<TItem> source;      

        /// <summary>
        /// Chỉ mục đến thành phần con trong danh sách
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public TItem this[int index]
        {
            get => source[index];
            set
            {
                Transaction.Current.AddChange(new ReversibleCollectionChange<TItem>(source, source[index], value));
                source[index] = value;
            }
        }

        /// <summary>
        /// Số lượng các đối tượng trong trong danh sách
        /// </summary>
        public int Count => source.Count;

        /// <summary>
        /// Danh sách chỉ đọc
        /// </summary>
        public bool IsReadOnly => false;

        #endregion

        #region Constructors

        public ReversibleCollection(IList<TItem> source)
        {
            this.source = source;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Thêm 1 đối tượng vào trong danh sách
        /// </summary>
        /// <param name="item">Đối tượng thêm vào</param>
        public void Add(TItem item)
        {
            if (Transaction.Current == null)
                throw new InvalidOperationException("Không thể thêm đối tượng vào danh sách khi chưa có một Transaction nào bắt đầu.");
            source.Add(item);
            Transaction.Current.AddChange(new ReversibleCollectionChange<TItem>(CollectionAction.Add, source, item));
        }

        /// <summary>
        /// Thêm nhiều đối tượng vào danh sách
        /// </summary>
        /// <param name="collection"></param>
        public void AddRange(IList<TItem> collection)
        {
            if (Transaction.Current == null)
                throw new InvalidOperationException("Không thể thêm đối tượng vào danh sách khi chưa có một Transaction nào bắt đầu.");
            foreach (var item in collection)
                source.Add(item);
            Transaction.Current.AddChange(new ReversibleCollectionChange<TItem>(CollectionAction.Add, source, collection));
        }

        /// <summary>
        /// Làm trống danh sách
        /// </summary>
        public void Clear()
        {
            if (Transaction.Current == null)
                throw new InvalidOperationException("Không thể làm trống danh sách khi chưa có một Transaction nào bắt đầu.");
            Transaction.Current.AddChange(new ReversibleCollectionChange<TItem>(source));
            source.Clear();
        }

        /// <summary>
        /// Kiểm tra xem đối tượng truyền vào có tồn tại trong danh sách hay không
        /// </summary>
        /// <param name="item">Đối tượng cần kiểm tra</param>
        /// <returns></returns>
        public bool Contains(TItem item) => source.Contains(item);

        /// <summary>
        /// Lấy vị trí của đối tượng trong danh sách
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int IndexOf(TItem item) => source.IndexOf(item);

        /// <summary>
        /// Thêm 1 đối tượng vào 1 vị trí cụ thể trong danh sách
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        public void Insert(int index, TItem item)
        {
            if (Transaction.Current == null)
                throw new InvalidOperationException("Không thể thêm đối tượng vào danh sách khi chưa có một Transaction nào bắt đầu.");
            source.Insert(index, item);
            Transaction.Current.AddChange(new ReversibleCollectionChange<TItem>(CollectionAction.Add, source, item));
        }

        /// <summary>
        /// Xóa đối tượng khỏi danh sách
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove(TItem item)
        {
            if (Contains(item))
            {
                if (Transaction.Current == null)
                    throw new InvalidOperationException("Không thể xóa đối tượng khỏi danh sách khi chưa có một Transaction nào bắt đầu.");
                Transaction.Current.AddChange(new ReversibleCollectionChange<TItem>(CollectionAction.Remove, source, item));
                return source.Remove(item);
            }
            return false;
        }

        /// <summary>
        /// Xóa nhiều đối tượng trong danh sách
        /// </summary>
        /// <param name="collection"></param>
        public void RemoveRange(IList<TItem> collection)
        {
            if (Transaction.Current == null)
                throw new InvalidOperationException("Không thể xóa đối tượng khỏi danh sách khi chưa có một Transaction nào bắt đầu.");
            Transaction.Current.AddChange(new ReversibleCollectionChange<TItem>(CollectionAction.Remove, source, collection));
            foreach (var item in collection)
                source.Remove(item);
        }

        /// <summary>
        /// Xóa tất cả đối tượng trong danh sách từ vị trí xác định đến cuối danh sách
        /// </summary>
        /// <param name="index">Vị trí bắt đầu xóa</param>
        public void RemoveAt(int index)
        {
            if (Transaction.Current == null)
                throw new InvalidOperationException("Không thể xóa đối tượng khỏi danh sách khi chưa có một Transaction nào bắt đầu.");
            var item = source[index];
            Transaction.Current.AddChange(new ReversibleCollectionChange<TItem>(CollectionAction.Remove, source, item));
            source.Remove(item);
        }

        public void CopyTo(TItem[] array, int arrayIndex) => throw new NotImplementedException();
        public IEnumerator<TItem> GetEnumerator() => source.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => source.GetEnumerator();

        #endregion
    }
}