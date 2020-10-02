using System;
using System.Collections.Generic;

namespace EasyDriver.Reversible
{
    public class Transaction : Change, IDisposable
    {
        #region Static

        /// <summary>
        /// <see cref="Transaction"/> hiện tại đang làm việc
        /// </summary>
        public static Transaction Current { get; protected set; }

        /// <summary>
        /// Bắt đầu một <see cref="Transaction"/>
        /// </summary>
        /// <returns></returns>
        public static Transaction Begin() => Begin(null);

        /// <summary>
        /// Bắt đầu một <see cref="Transaction"/>
        /// </summary>
        /// <param name="transactionName"></param>
        /// <returns></returns>
        public static Transaction Begin(string transactionName)
        {
            Transaction newTransaction = new Transaction(Current, transactionName);
            //Nếu Transaction hiện tại đã tồn tại thì transaction kế tiếp của nó sẽ là transaction ta chuẩn bị bắt đầu
            if (Current != null)
                Current.Next = newTransaction;
            //Cập nhật lại Transaction hiện tại
            Current = newTransaction;
            return newTransaction;
        }

        /// <summary>
        /// Thêm 1 sự thay đổi thuộc tính vào <see cref="Transaction"/> hiện tại
        /// </summary>
        /// <typeparam name="TSource">Kiểu của đối tượng sỡ hữu thuộc tính bị thay đổi</typeparam>
        /// <typeparam name="TProperty">Kiểu của thuộc tính bị thay đổi</typeparam>
        /// <param name="setter">Hàm để đặt giá trị của thuộc tính</param>
        /// <param name="source">Đối tượng sỡ hữu thuộc tính</param>
        /// <param name="oldValue">Giá trị cũ của thuộc tính</param>
        /// <param name="newValue">Giá trị mới của thuộc tính</param>
        public static void AddPropertyChange<TSource, TProperty>(
            ReversiblePropertySetter<TSource, TProperty> setter,
            TSource source, 
            TProperty oldValue,
            TProperty newValue) where TSource : class
        {
            Current?.AddChange(new ReversiblePropertyChange<TSource, TProperty>(setter, source, oldValue, newValue));
        }

        /// <summary>
        /// <summary>
        /// Thêm 1 sự thay đổi thuộc tính vào <see cref="Transaction"/> hiện tại
        /// </summary>
        /// <typeparam name="TSource">Kiểu của đối tượng sỡ hữu thuộc tính bị thay đổi</typeparam>
        /// <typeparam name="TProperty">Kiểu của thuộc tính bị thay đổi</typeparam>
        /// <param name="source">Đối tượng sỡ hữu thuộc tính</param>
        /// <param name="newValue">Giá trị mới của thuộc tính</param>
        public static void AddPropertyChange<TSource, TProperty>(
            string propertyName,
            TSource source,
            TProperty newValue) where TSource : class
        {
            Current?.AddChange(new ReversiblePropertyObjectChange<TSource, TProperty>(propertyName, source, newValue));
        }

        /// <summary>
        /// <summary>
        /// Thêm 1 sự thay đổi thuộc tính vào <see cref="Transaction"/> hiện tại
        /// </summary>
        /// <typeparam name="TSource">Kiểu của đối tượng sỡ hữu thuộc tính bị thay đổi</typeparam>
        /// <typeparam name="TProperty">Kiểu của thuộc tính bị thay đổi</typeparam>
        /// <param name="source">Đối tượng sỡ hữu thuộc tính</param>
        /// <param name="newValue">Giá trị mới của thuộc tính</param>

        public static void AddPropertyChanged<TSource, TProperty>(
            TSource source,
            string propertyName,
            TProperty oldValue, 
            TProperty newValue) where TSource : class
        {
            Current?.AddChange(new ReversiblePropertyObjectChange<TSource, TProperty>(propertyName, source, oldValue, newValue));
        }
        #endregion

        #region Members

        /// <summary>
        /// Tên của <see cref="Transaction"/>
        /// </summary>
        public override string Name { get; protected set; }

        /// <summary>
        /// Thể hiện <see cref="Transaction"/> này đã hoàn tất
        /// </summary>
        public bool IsFinished { get; private set; }

        /// <summary>
        /// Thể hiện <see cref="Transaction"/> này là <see cref="Transaction"/> cuối cùng trong danh sách các <see cref="Transaction"/>
        /// </summary>
        public bool IsLast { get => Next == null; }

        /// <summary>
        /// <see cref="Transaction"/> trước đó
        /// </summary>
        private Transaction Previous { get; set; }

        /// <summary>
        /// <see cref="Transaction"/> kế tiếp
        /// </summary>
        protected Transaction Next { get; private set; }

        /// <summary>
        /// Danh sách các thay đổi lúc <see cref="Transaction"/> bắt đầu cho đến khi kết thúc
        /// </summary>
        protected List<Change> Changes = new List<Change>();

        /// <summary>
        /// Vị trí của <see cref="Transaction"/> trong danh sách <see cref="Transaction"/>
        /// </summary>
        protected int Index { get; set; }

        #endregion

        #region Constructors

        protected Transaction(Transaction previous, string name)
        {
            Previous = previous;
            Name = name;
        }

        #endregion

        #region Static methods

        #endregion

        #region Methods

        /// <summary>
        /// Thêm một sự thay đổi vào <see cref="Transaction"/>
        /// </summary>
        /// <param name="change">Sự thay đổi thêm vào Transaction</param>
        /// <exception cref="InvalidOperationException">Nếu Transaction này đã hoàn tất thì không thể thêm sự thay đổi vào nó</exception>
        public virtual void AddChange(Change change)
        {
            if (IsFinished)
                throw new InvalidOperationException("Không thể thêm sự thay đổi vào một Transaction đã hoàn tất.");
            Changes.RemoveRange(Index, Changes.Count - Index);
            Changes.Add(change);
            Index++;
        }

        /// <summary>
        /// Phục hồi lại tất cả sự thay đổi trong <see cref="Transaction"/> này và kết thúc nó sau đó quay lại <see cref="Transaction"/> trước đó
        /// </summary>
        /// <exception cref="InvalidOperationException">Nếu Transaction này đã hoàn tất hoặc hàm đã được gọi trước đó thì không thể gọi hàm này 1 lần nữa</exception>
        public void Rollback()
        {
            if (IsFinished)
                throw new InvalidOperationException("Sự phục hồi không thể thực hiện khi Transaction đã hoàn tất. " +
                    "Hãy chắc chắn rằng hàm này chỉ gọi 1 lần duy nhất và phải gọi trước khi xác nhận Transaction");
            IsFinished = true; //Kết thúc Transaction này
            Current = null;
            if (Next != null) 
                Next.Rollback(); //Phục hồi lại Transaction kế tiếp của nó nếu như tồn tại
            if (Index > 0)
                Reverse();  //Phục hồi tất cả các sự thay đổi đã xảy ra khi Transaction này bắt đầu
            if (Previous != null)
                Previous.OnNextTransactionFinished(this);
            else
                Current = null; //Nếu Transaction trước đó không tồn tại thì có nghĩa là chưa có Transaction nào được bắt đầu
        }

        /// <summary>
        /// Xác nhận sự thay đổi và đánh dấu <see cref="Transaction"/> này đã hoàn tất
        /// </summary>
        /// <exception cref="InvalidOperationException">Nếu Transaction đã hoàn tất hoặc đã gọi hàm Rollback thì không thể xác nhận sự thay đổi</exception>
        public void Commit()
        {
            if (IsFinished)
                throw new InvalidOperationException("Sự xác nhận không thể thực hiện khi Transaction đã hoàn tất hoặc Transaction này đã gọi hàm Rollback trước đó.");
            IsFinished = true;
            if (Next != null)
                Next.Commit(); //Xác nhận Transaction kế tiếp của Transaction này nếu nó tồn tại
            if (Previous != null)
            {
                if (Changes.Count > 0)
                    Previous.AddChange(this);
                Previous.OnNextTransactionFinished(this);
            }
            else
                Current = null;
        }

        /// <summary>
        /// Hủy <see cref="Transaction"/> hiện tại
        /// </summary>
        public void Cancel()
        {
            Transaction transaction = Current;
            while (transaction != null)
            {
                if (transaction == this)
                {
                    Current = null;
                    return;
                }
                transaction = transaction.Previous;
            }
        }

        /// <summary>
        /// Phục hồi lại tất cả sự thay đổi trong <see cref="Transaction"/> này
        /// </summary>
        /// <returns></returns>
        public override bool Reverse()
        {
            if (Index > 0)
            {
                var reversingArgs = new ReversingEventArgs(ReverseDirection.Undo);
                Reversing?.Invoke(this, reversingArgs);
                if ((Reversing != null && !reversingArgs.Handled) || Reversing == null)
                {
                    for (int i = Index - 1; i >= 0; i--)
                    {
                        if (!Changes[i].Reverse())
                            return false;
                    }
                    Index = 0;
                    OnReversed(ReverseDirection.Undo);
                    return true;
                }               
                return false;
            }
            else
            {
                var reversingArgs = new ReversingEventArgs(ReverseDirection.Redo);
                Reversing?.Invoke(this, reversingArgs);
                if ((Reversing != null && !reversingArgs.Handled) || Reversing == null)
                {
                    for (int i = 0; i < Changes.Count; i++)
                    {
                        if (!Changes[i].Reverse())
                            return false;
                    }
                    Index = Changes.Count;
                    OnReversed(ReverseDirection.Redo);
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Hàm được gọi khi đã phục hồi sự thay đổi trong <see cref="Transaction"/>
        /// </summary>
        protected virtual void OnReversed(ReverseDirection direction) => Reversed?.Invoke(this, new ReversedEventArgs(direction));

        /// <summary>
        /// Hàm được gọi khi đang phục hồi sự thay đổi trong <see cref="Transaction"/>
        /// </summary>
        protected virtual void OnReversing(ReverseDirection direction) => Reversing?.Invoke(this, new ReversingEventArgs(direction));

        /// <summary>
        /// Hàm được gọi khi <see cref="Transaction"/> kế tiếp hoàn tất
        /// </summary>
        /// <param name="nextTransaction"></param>
        protected virtual void OnNextTransactionFinished(Transaction nextTransaction)
        {
            if (nextTransaction == Next)
            {
                Next = null;
                Current = this;
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Sự kiện xảy ra khi <see cref="Transaction"/> đã hoàn tất phục hồi lại sự thay đổi
        /// </summary>
        public event EventHandler<ReversedEventArgs> Reversed;

        /// <summary>
        /// Sự kiện xảy ra khi <see cref="Transaction"/> đang phục hồi lại sự thay đổi
        /// </summary>
        public event EventHandler<ReversingEventArgs> Reversing;

        #endregion

        #region IDisposable

        /// <summary>
        /// Loại bỏ <see cref="Transaction"/> này. Nếu nó chưa hoàn tất thì phục hồi lại thay đổi 
        /// </summary>
        void IDisposable.Dispose()
        {
            if (!IsFinished)
                Rollback();
        }

        #endregion
    }
}
