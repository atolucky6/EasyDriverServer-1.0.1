using EasyDriverPlugin;
using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace EasyDriver.Server.Models
{
    [Serializable]
    public abstract class CoreItemBase : BindableCore, ICoreItem
    {
        #region Members

        #region Display properties

        /// <summary>
        /// Tên của đối tượng
        /// </summary>
        [Category(PropertyCategory.General), DisplayName("Name")]
        [JsonIgnore]
        public virtual string Name
        {
            get => GetProperty<string>()?.Trim();
            set => SetProperty(value);
        }

        /// <summary>
        /// Đường dẫn đến đối tượng
        /// </summary>
        [Category(PropertyCategory.General), DisplayName("Path"), ReadOnly(true)]
        [JsonIgnore]
        public virtual string Path { get => string.Format("{0}/{1}", Parent.Path, Name); }

        /// <summary>
        /// Thời gian tạo đối tượng
        /// </summary>
        [Category(PropertyCategory.General), DisplayName("Created date"), ReadOnly(true)]
        [JsonIgnore]
        public virtual DateTime CreatedDate
        {
            get => GetProperty<DateTime>();
            set => SetProperty(value);
        }

        /// <summary>
        /// Thời gian lần cuối cùng chỉnh sữa đối tượng
        /// </summary>
        [Category(PropertyCategory.General), DisplayName("Modified date"), ReadOnly(true)]
        [JsonIgnore]
        public virtual DateTime ModifiedDate
        {
            get => GetProperty<DateTime>();
            set => SetProperty(value);
        }

        /// <summary>
        /// Mô tả
        /// </summary>
        [Category(PropertyCategory.General), DisplayName("Description")]
        [JsonIgnore]
        public virtual string Description
        {
            get => GetProperty<string>()?.Trim();
            set => SetProperty(value);
        }

        #endregion

        /// <summary>
        /// Đối tượng cha chứa đối tượng này
        /// </summary>
        [Display(AutoGenerateField = false)]
        [Browsable(false)]
        [JsonIgnore]
        public virtual IGroupItem Parent { get; set; }

        /// <summary>
        /// Bit cho biết đối tượng chỉ được đọc hay không
        /// </summary>
        [Display(AutoGenerateField = false)]
        [Browsable(false)]
        [JsonIgnore]
        public virtual bool IsReadOnly { get; private set; }

        #endregion

        #region Constructors

        internal CoreItemBase(IGroupItem parent, bool isReadOnly = false)
        {
            Parent = parent;
            IsReadOnly = isReadOnly;
            SetProperty(DateTime.Now, nameof(CreatedDate), false);
            SetProperty(DateTime.Now, nameof(ModifiedDate), false);
            IsChanged = false;
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
            IsChanged = false;
        }

        /// <summary>
        /// Hàm kiểm tra đối tượng đã bị thay đổi hay chưa
        /// </summary>
        /// <returns></returns>
        public virtual bool HasChanges() => IsChanged;

        /// <summary>
        /// Hàm kiểm tra đối tượng có lỗi hay không
        /// </summary>
        /// <returns></returns>
        public virtual bool HasError() => GetErrorInfos().FirstOrDefault(x => !string.IsNullOrEmpty(GetErrorOfProperty(x.PropertyName))) != null;

        /// <summary>
        /// Cập nhật lại đường dẫn
        /// </summary>
        public virtual void RefreshPath() => RaisePropertyChanged(nameof(Path));


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
        public virtual ICoreItem ShallowCopy()
        {
            ICoreItem result = MemberwiseClone() as ICoreItem;
            (result as CoreItemBase).CreatedDate = DateTime.Now;
            (result as CoreItemBase).ModifiedDate = DateTime.Now;
            return result;
        }

        /// <summary>
        /// Clone đối tượng và trả về kết quả. Hàm sẽ clone tất cả các thuộc tính
        /// </summary>
        /// <returns></returns>
        public virtual ICoreItem DeepCopy()
        {
            ICoreItem result = CopyHelper.DeepCopy<ICoreItem>(this);
            (result as CoreItemBase).CreatedDate = DateTime.Now;
            (result as CoreItemBase).ModifiedDate = DateTime.Now;
            return result;
        }

        #endregion

        #region IDataErrorInfo

        string IDataErrorInfo.this[string columnName] => GetErrorOfProperty(columnName);

        [field: NonSerialized]
        [Browsable(false)]
        [Display(AutoGenerateField = false)]
        public string Error { get; protected set; }

        #endregion
    }
}
