using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace EasyDriverPlugin
{
    [Serializable]
    public abstract class GroupItemBase : BindableCore, IGroupItem
    {
        #region Members

        #region Display properties

        protected string name;
        /// <summary>
        /// Tên của đối tượng
        /// </summary>
        [Category(PropertyCategory.General), DisplayName("Name")]
        public virtual string Name
        {
            get => name?.Trim();
            set
            {
                if (name != value)
                {
                    string oldName = name;
                    name = value;
                    RaisePropertyChanged();
                    NameChanged?.Invoke(this, new NameChangedEventArgs(oldName, value));
                }
            }
        }

        protected bool enabled = true;
        [Category(PropertyCategory.General), DisplayName("Enabled")]
        public virtual bool Enabled
        {
            get
            {
                if (!enabled || Parent == null)
                    return false;
                else
                    return Parent.Enabled;
            }
            set
            {
                if (value != enabled)
                {
                    enabled = value;
                    RaisePropertyChanged();
                }
            }
        }

        public virtual int Level
        {
            get => Parent == null ? 0 : Parent.Level + 1;
        }

        /// <summary>
        /// Đường dẫn đến đối tượng
        /// </summary>
        [Category(PropertyCategory.General), DisplayName("Path"), ReadOnly(true)]
        public virtual string Path { get => Parent == null || string.IsNullOrEmpty(Parent?.Path) ? Name : string.Format("{0}/{1}", Parent.Path, Name); }

        public virtual string DisplayInformation
        {
            get => GetProperty<string>();
            set => SetProperty(value);
        }

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

        [Browsable(false)]
        public abstract ItemType ItemType { get; set; }

        /// <summary>
        /// Đối tượng cha chứa đối tượng này
        /// </summary>
        [Browsable(false)]
        public virtual IGroupItem Parent { get; set; }

        /// <summary>
        /// Bit cho biết đối tượng chỉ được đọc hay không
        /// </summary>
        [Browsable(false)]
        public virtual bool IsReadOnly { get; set; }

        private bool? isChecked;
        public virtual bool? IsChecked
        {
            get => isChecked;
            set
            {
                if (isChecked != value)
                {
                    isChecked = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Danh sách con của đối tượng này
        /// </summary>
        [Browsable(false)]
        public NotifyCollection Childs { get; private set; }

        #endregion

        #endregion

        #region Constructors

        public GroupItemBase(IGroupItem parent, bool isReadOnly = false)
        {
            IsReadOnly = isReadOnly;
            Parent = parent;
            SetProperty(DateTime.Now, nameof(CreatedDate), false);
            SetProperty(DateTime.Now, nameof(ModifiedDate), false);
            Childs = new NotifyCollection(this);
            IsChanged = false;
            Enabled = true;
        }

        #endregion

        #region Events

        public event EventHandler<TagValueChangedEventArgs> ValueChanged;
        public event EventHandler<TagQualityChangedEventArgs> QualityChanged;
        public event EventHandler<NameChangedEventArgs> NameChanged;
        public event EventHandler Added;
        public event EventHandler Removed;

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

        public virtual void RaiseAddedEvent()
        {
            Added?.Invoke(this, EventArgs.Empty);
        }

        public virtual void RaiseRemovedEvent()
        {
            Removed?.Invoke(this, EventArgs.Empty);
            foreach (var item in Childs)
            {
                if (item is ICoreItem coreItem)
                    coreItem.RaiseRemovedEvent();
            }

            if (this is IHaveTag haveTag)
            {
                if (haveTag.HaveTags)
                {
                    foreach (var item in haveTag.Tags)
                    {
                        if (item is ICoreItem coreItem)
                            coreItem.RaiseRemovedEvent();
                    }
                }
            }
        }

        public virtual void RaiseTagValueChanged(ITagCore tagSender, TagValueChangedEventArgs args)
        {
            ValueChanged?.Invoke(tagSender, args);
            if (Parent is GroupItemBase groupItem)
                groupItem.RaiseTagValueChanged(tagSender, args);
        }

        public virtual void RaiseTagQualityChanged(ITagCore tagSender, TagQualityChangedEventArgs args)
        {
            QualityChanged?.Invoke(tagSender, args);
            if (Parent is GroupItemBase groupItem)
                groupItem.RaiseTagQualityChanged(tagSender, args);
        }

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
                if ((item as ICoreItem).HasChanges())
                    return true;
            }
            return false;
        }

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
                        if (predicate(item as ICoreItem))
                            return true;
                    }
                }
                return false;
            }
            return FirstOrDefault(predicate) != null;
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
                    if (predicate(item as ICoreItem))
                        return item as ICoreItem;
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
            return Childs.FirstOrDefault((x) => predicate(x as ICoreItem)) as ICoreItem;
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
                    if (predicate(item as ICoreItem))
                        yield return item as ICoreItem;

                    if (item is IGroupItem)
                    {
                        foreach (var itemInChild in (item as IGroupItem).Find(predicate, findInChildren))
                            yield return itemInChild;
                    }
                }
            }
            else
            {
                foreach (var item in Childs)
                {
                    if (predicate.Invoke(item as ICoreItem))
                        yield return item as ICoreItem;
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

        public virtual object Browse(string[] paths, int nameIndex)
        {
            if (nameIndex < paths.Length)
            {
                if (Childs.Find(paths[nameIndex]) is IGroupItem groupItem)
                {
                    if (nameIndex == paths.Length - 1)
                        return groupItem;
                    else
                    {
                        nameIndex++;
                        return groupItem.Browse(paths, nameIndex);
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Hàm trả về chuỗi của đôi tượng
        /// </summary>
        /// <returns></returns>
        public override string ToString() => Name;

        public virtual bool GetActualEnabledProperty()
        {
            return enabled;
        }

        #endregion

        #region Abstract methods

        /// <summary>
        /// Hàm lấy thông tin của lỗi 
        /// </summary>
        /// <param name="propertyName">Tên thuộc tính cần lấy lỗi</param>
        /// <returns></returns>
        public abstract string GetErrorOfProperty(string propertyName);

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
        public string Error { get; protected set; }

        #endregion
    }
}
