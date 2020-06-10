using EasyDriverPlugin;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace EasyScada.Core
{
    [Serializable]
    public abstract class GroupItemBase : BindableCore, IGroupItem
    {
        #region Members

        #region Display properties

        /// <summary>
        /// Tên của đối tượng
        /// </summary>
        [Category(PropertyCategory.General), DisplayName("Name")]
        public virtual string Name
        {
            get => GetProperty<string>()?.Trim();
            set => SetProperty(value);
        }

        /// <summary>
        /// Đường dẫn đến đối tượng
        /// </summary>
        [Category(PropertyCategory.General), DisplayName("Path"), ReadOnly(true)]
        public virtual string Path { get => Parent == null ? Name : string.Format("{0}/{1}", Parent.Path, Name); }

        /// <summary>
        /// Thời gian tạo đối tượng
        /// </summary>
        [Category(PropertyCategory.General), DisplayName("Created date"), ReadOnly(true)]
        public virtual DateTime CreatedDate
        {
            get => GetProperty<DateTime>();
            set => SetProperty(value);
        }

        /// <summary>
        /// Thời gian lần cuối cùng chỉnh sữa đối tượng
        /// </summary>
        [Category(PropertyCategory.General), DisplayName("Modified date"), ReadOnly(true)]
        public virtual DateTime ModifiedDate
        {
            get => GetProperty<DateTime>();
            set => SetProperty(value);
        }

        /// <summary>
        /// Mô tả
        /// </summary>
        [Category(PropertyCategory.General), DisplayName("Description")]
        public virtual string Description
        {
            get => GetProperty<string>()?.Trim();
            set => SetProperty(value);
        }

        #endregion

        #region Non display properties

        /// <summary>
        /// Đối tượng cha chứa đối tượng này
        /// </summary>
        [Browsable(false)]
        public virtual IGroupItem Parent { get; set; }

        /// <summary>
        /// Bit cho biết đối tượng chỉ được đọc hay không
        /// </summary>
        [Browsable(false)]
        public virtual bool IsReadOnly { get; private set; }

        /// <summary>
        /// Danh sách con của đối tượng này
        /// </summary>
        [Browsable(false)]
        public ObservableCoreItems Childs { get; private set; }

        #endregion

        #endregion

        #region Constructors

        internal GroupItemBase(IGroupItem parent, bool isReadOnly = false)
        {
            IsReadOnly = isReadOnly;
            Parent = parent;
            SetProperty(DateTime.Now, nameof(CreatedDate), false);
            SetProperty(DateTime.Now, nameof(ModifiedDate), false);
            Childs = new ObservableCoreItems(this);
            IsChanged = false;
        }

        #endregion

        #region Child collection changed callback

        /// <summary>
        /// Hàm callback khi danh sách con bị thay đổi
        /// </summary>
        /// <param name="e"></param>
        public virtual void ChildCollectionChangedCallback(NotifyCollectionChangedEventArgs e)
        {
            IsChanged = true;
            SetProperty(DateTime.Now, nameof(ModifiedDate), false);
        }

        #endregion

        #region Methods       

        /// <summary>
        /// Hàm đặt lại trạng thái của đối tượng thành không thay đổi bằng cách chấp nhận sự thay đổi
        /// </summary>
        public override void AcceptChanges()
        {
            if (IsChanged)
                SetProperty(DateTime.Now, nameof(ModifiedDate), false);
            base.AcceptChanges();
            Childs.ForEach(x => x.AcceptChanges());
            IsChanged = false;
        }

        /// <summary>
        /// Hàm kiểm tra đối tượng đã bị thay đổi hay chưa
        /// </summary>
        /// <returns></returns>
        public virtual bool HasChanges()
        {
            if (IsChanged)
                return true;
            foreach (var item in Childs)
            {
                if (item.HasChanges())
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Hàm kiểm tra đối tượng có lỗi hay không
        /// </summary>
        /// <returns></returns>
        public virtual bool HasError()
        {
            if (GetErrorInfos().FirstOrDefault(x =>
            {
                if (!string.IsNullOrEmpty(GetErrorOfProperty(x.PropertyName)))
                    return true;
                return false;
            }) != null)
                return true;
            foreach (var item in Childs)
            {
                if (item.HasError())
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Hàm thêm <see cref="ICoreItem"/> vào danh sách con
        /// </summary>
        /// <param name="item">Đối tượng cần thêm</param>
        /// <returns></returns>
        public bool Add(ICoreItem item)
        {
            if (Contains(item))
                return false;
            Childs.Add(item);
            return true;
        }

        /// <summary>
        /// Hàm thêm một danh sách <see cref="ICoreItem"/> vào danh sách con
        /// </summary>
        /// <param name="items">Danh sách đối tượng cần thêm</param>
        public void Add(IEnumerable<ICoreItem> items) => Childs.AddRange(items);

        /// <summary>
        /// Xóa <see cref="ICoreItem"/> khỏi danh sách con
        /// </summary>
        /// <param name="item">Đối tượng cần xóa</param>
        /// <returns></returns>
        public bool Remove(ICoreItem item) => Childs.Remove(item);

        /// <summary>
        /// Hàm xóa một danh sách <see cref="ICoreItem"/> trong danh sách con
        /// </summary>
        /// <param name="items">Danh sách đối tượng cần xóa</param>
        public void Remove(IEnumerable<ICoreItem> items) => Childs.RemoveRange(items);

        /// <summary>
        /// Hàm kiểm tra <see cref="ICoreItem"/> có tồn tại trong danh sách con không
        /// </summary>
        /// <param name="item">Đối tượng cần kiểm tra</param>
        /// <returns></returns>
        public bool Contains(ICoreItem item) => Childs.Contains(item);

        /// <summary>
        /// Hàm kiểm tra xem <see cref="ICoreItem"/> trong danh sách con có thỏa mãn điều kiện truyền vào hay không
        /// </summary>
        /// <param name="predicate">Điều kiện truyền vào</param>
        /// <param name="findDeep">Kiểm tra trong các đối tượng con</param>
        /// <returns></returns>
        public bool Contains(Func<ICoreItem, bool> predicate, bool findDeep = false)
        {
            if (findDeep)
            {
                foreach (var item in Childs)
                {
                    if (item is IGroupItem)
                    {
                        if ((item as IGroupItem).Contains(predicate, findDeep))
                            return true;
                    }
                    else if (item is ICoreItem)
                    {
                        if (predicate(item))
                            return true;
                    }
                }
                return false;
            }
            return Childs.FirstOrDefault(predicate) != null;
        }

        /// <summary>
        /// Hàm tìm <see cref="ICoreItem"/> đầu tiên thỏa mãn điều kiện trong danh sách con
        /// </summary>
        /// <param name="predicate">Điều kiện truyền vào</param>
        /// <param name="findDeep">Kiểm tra trong các đối tượng con</param>
        /// <returns></returns>
        public virtual ICoreItem FirstOrDefault(Func<ICoreItem, bool> predicate, bool findDeep = false)
        {
            if (findDeep)
            {
                foreach (var item in Childs)
                {
                    if (predicate(item))
                        return item;
                }
                foreach (var item in Childs)
                {
                    if (item is IGroupItem)
                    {
                        ICoreItem result = (item as IGroupItem).FirstOrDefault(predicate, findDeep);
                        if (result != null)
                            return result;
                    }
                }
                return null;
            }
            return Childs.FirstOrDefault(predicate);
        }

        /// <summary>
        /// Hàm tìm tất cả các <see cref="ICoreItem"/> thỏa mãn điều kiện trong danh sách con
        /// </summary>
        /// <param name="predicate">Điều kiện truyền vào</param>
        /// <param name="findDeep">Kiểm tra trong các đối tượng con</param>
        /// <returns></returns>
        public IEnumerable<ICoreItem> Find(Func<ICoreItem, bool> predicate, bool findInChildren = false)
        {
            if (findInChildren)
            {
                foreach (var item in Childs)
                {
                    if (predicate(item))
                        yield return item;
                    if (item is IGroupItem)
                    {
                        foreach (var itemInChild in (item as IGroupItem).Find(predicate, findInChildren))
                        {
                            yield return itemInChild;
                        }
                    }
                }
            }
            else
            {
                foreach (var item in Childs)
                {
                    if (predicate.Invoke(item))
                        yield return item;
                }
            }
        }

        /// <summary>
        /// Cập nhật lại đường dẫn
        /// </summary>
        public virtual void RefreshPath()
        {
            RaisePropertyChanged(nameof(Path));
            Childs.ForEach(x => x.RefreshPath());
        }

        /// <summary>
        /// Hàm trả về chuỗi của đôi tượng
        /// </summary>
        /// <returns></returns>
        public override string ToString() => Name;

        #endregion

        #region Abstract methods

        /// <summary>
        /// Hàm lấy thông tin của lỗi 
        /// </summary>
        /// <param name="propertyName">Tên thuộc tính cần lấy lỗi</param>
        /// <returns></returns>
        public abstract string GetErrorOfProperty(string propertyName);

        /// <summary>
        /// Hàm lấy <see cref="IErrorInfo"/> của đối tượng
        /// </summary>
        /// <param name="errorInfo"></param>
        public abstract void GetErrors(ref IErrorInfo errorInfo);

        #endregion

        #region Copy methods

        /// <summary>
        /// Clone đối tượng và trả về kết quả. Hàm chỉ clone lại các kiểu dữ liệu nguyên thủy
        /// </summary>
        /// <returns></returns>
        public virtual ICoreItem ShallowCopy() => MemberwiseClone() as ICoreItem;

        /// <summary>
        /// Clone đối tượng và trả về kết quả. Hàm sẽ clone tất cả các thuộc tính
        /// </summary>
        /// <returns></returns>
        public virtual ICoreItem DeepCopy() => CopyHelper.DeepCopy<ICoreItem>(this);

        #endregion

        #region IDateErrorInfo

        string IDataErrorInfo.this[string columnName] => GetErrorOfProperty(columnName);

        [field: NonSerialized]
        [Browsable(false)]
        [Display(AutoGenerateField = false)]
        public string Error { get; protected set; }

        #endregion
    }
}
