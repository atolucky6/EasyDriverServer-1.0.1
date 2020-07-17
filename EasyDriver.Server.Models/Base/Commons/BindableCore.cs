using EasyDriverPlugin;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace EasyDriver.Core
{
    /// <summary>
    /// Lớp cung cấp các hàm Get và Set thuộc tính của Class.
    /// </summary>
    [Serializable]
    public abstract class BindableCore : INotifyPropertyChanged, IChangeTracking, IEditableObject
    {
        #region Set property methods

        /// <summary>
        /// Hàm cài đặt giá trị của thuộc tính cụ thể
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">Giá trị cần cài đặt</param>
        /// <param name="propertyName">Tên của thuộc tính cần cài đặt</param>
        /// <returns></returns>
        public virtual bool SetValue<T>(ref T storage, T value, [CallerMemberName]string propertyName = null)
        {
            return SetValue(ref storage, value, null, propertyName);
        }

        /// <summary>
        /// Hàm cài đặt giá trị của thuộc tính cụ thể
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">Giá trị cần cài đặt</param>
        /// <param name="propertyName">Tên của thuộc tính cần cài đặt</param>
        /// <returns></returns>
        public virtual bool SetValue<T>(ref T storage, T value, Action<string, object, object> propertyChangedCallback = null, [CallerMemberName]string propertyName = null)
        {
            if (!Equals(storage, value))
            {
                storage = value;
                RaisePropertyChanged(propertyName);
                propertyChangedCallback?.Invoke(propertyName, storage, value);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Hàm cài đặt giá trị của thuộc tính trong <see cref="PropertyBag"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">Giá trị cần cài đặt</param>
        /// <param name="propertyName">Tên của thuộc tính cần cài đặt</param>
        /// <returns></returns>
        public virtual bool SetProperty<T>(T value, [CallerMemberName]string propertyName = null)
        {
            return PropertyBag.SetProperty<T>(propertyName, value, RaisePropertyChanged, PropertyChangedCallback);
        }

        public virtual void SetProperty<T>(T value, string propertyName, bool raisePropertyChanged)
        {
            if (raisePropertyChanged)
                PropertyBag.SetProperty(propertyName, value, RaisePropertyChanged, PropertyChangedCallback);
            else
                PropertyBag.SetProperty(propertyName, value, null, null);
        }

        #endregion

        #region Get property methods

        /// <summary>
        /// Hàm lấy giá trị của thuộc tính từ <see cref="PropertyBag"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public virtual T GetProperty<T>([CallerMemberName]string propertyName = null) => PropertyBag.GetProperty<T>(propertyName);

        #endregion

        #region ErrorInfo

        /// <summary>
        /// Hàm lấy thông tin lỗi của một thuộc tính của thể
        /// </summary>
        /// <param name="propertyName">Tên của thuộc tính cần lấy thông tin</param>
        /// <returns></returns>
        public virtual IErrorInfo GetErrorInfo(string propertyName)
        {
            if (!PropertyBag.ErrorDictionary.ContainsKey(propertyName))
                PropertyBag.ErrorDictionary[propertyName] = new ErrorInfo() { PropertyName = propertyName };
            return PropertyBag.ErrorDictionary[propertyName];
        }

        /// <summary>
        /// Hàm cập nhật thông tin lỗi của đối tượng này bằng một <see cref="IErrorInfo"/> khác
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="errorInfo"></param>
        public virtual void UpdateErrorInfo(string propertyName, IErrorInfo errorInfo)
        {
            if (errorInfo.ErrorCode != 0 && !string.IsNullOrEmpty(propertyName))
            {
                errorInfo.PropertyName = propertyName;
                GetErrorInfo(propertyName)?.Replace(errorInfo);
            }
        }

        /// <summary>
        /// Hàm lấy thông tin lỗi của tất cả các thuộc tính trong đối tượng
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<IErrorInfo> GetErrorInfos()
        {
            foreach (var item in PropertyBag.PropertyDictionary)
            {
                if (!PropertyBag.ErrorDictionary.ContainsKey(item.Key))
                    yield return GetErrorInfo(item.Key);
                else
                    yield return PropertyBag.ErrorDictionary[item.Key];
            }
        }

        #endregion

        #region INotifyPropertyChanged

        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;
        public virtual void RaisePropertyChanged([CallerMemberName]string propertyName = null)
        {
            if (!IsInEdit)
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region IChangeTracking

        /// <summary>
        /// Hàm đặt lại trạng thái của đối tượng thành không thay đổi bằng cách chấp nhận sự thay đổi
        /// </summary>
        public virtual void AcceptChanges() => IsChanged = false;

        /// <summary>
        /// Trạng thái cho biết sự thay đổi của đối tượng. Nếu là TRUE thì đối tượng đã bị thay đổi và ngược lại
        /// </summary>
        [Display(AutoGenerateField = false)]
        [Browsable(false)]
        [JsonIgnore]
        public virtual bool IsChanged { get; protected set; }

        #endregion

        #region IEditable

        /// <summary>
        /// Trạng thái cho biết đối tượng có đang trong quá trình chỉnh sửa hay không
        /// </summary>
        [Display(AutoGenerateField = false)]
        [Browsable(false)]
        [JsonIgnore]
        public bool IsInEdit { get; private set; }

        /// <summary>
        /// Bắt đầu chỉnh sửa đối tượng. Hàm sẽ lưu lại tất cả các giá trị của đối tượng vào dữ liệu backup. Đối tượng có thể backup lại 
        /// dữ liệu nếu như ta hủy chỉnh sữa đối tượng.
        /// </summary>
        public virtual void BeginEdit()
        {
            if (!IsInEdit)
            {
                IsInEdit = true;
                IsChanged = true;
                BackupData = PropertyBag.PropertyDictionary.DeepCopy();
                BeganEdit?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Kết thúc chỉnh sửa đối tượng. Hàm này sẽ cập nhật tất cả giá trị sau khi chỉnh sửa vào đối tượng
        /// </summary>
        public virtual void EndEdit()
        {
            if (IsInEdit)
            {
                IsInEdit = false;
                BackupData = null;
                RaisePropertyChanged(null);
                EndedEdit?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Hủy chỉnh sửa đối tượng
        /// </summary>
        public virtual void CancelEdit()
        {
            if (IsInEdit)
            {
                IsInEdit = false;
                Backup(BackupData);
                RaisePropertyChanged(null);
                CancelledEdit?.Invoke(this, EventArgs.Empty);
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Hàm callback nếu như giá trị của các thuộc trong <see cref="PropertyBag"/> thay đổi
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        public virtual void PropertyChangedCallback(string propertyName, object oldValue, object newValue) => IsChanged = true;

        /// <summary>
        /// Sự kiện chạy khi bắt đầu chỉnh sửa
        /// </summary>
        [field: NonSerialized]
        public event EventHandler BeganEdit;

        /// <summary>
        /// Sự kiện hủy bỏ chỉnh sửa
        /// </summary>
        [field: NonSerialized]
        public event EventHandler CancelledEdit;

        /// <summary>
        /// Sự kiện kết thúc chỉnh sửa
        /// </summary>
        [field: NonSerialized]
        public event EventHandler EndedEdit;

        #endregion

        #region Properties manager

        /// <summary>
        /// Dữ liệu backup
        /// </summary>
        [field: NonSerialized]
        protected Dictionary<string, object> BackupData { get; private set; }

        PropertyBag _PropertyBag;
        /// <summary>
        /// Lưu trữ các thuộc tính của đối tượng
        /// </summary>
        [JsonIgnore]
        protected PropertyBag PropertyBag
        {
            get
            {
                if (_PropertyBag == null)
                    _PropertyBag = new PropertyBag();
                return _PropertyBag;
            }
        }

        public Dictionary<string, object> GetBackupData() => BackupData;

        public Dictionary<string, object> GetData() => PropertyBag.PropertyDictionary;

        public void Backup(Dictionary<string, object> backupData)
        {
            foreach (var property in backupData)
                SetProperty(property.Value, property.Key);
        }

        #endregion
    }

    /// <summary>
    /// Quản lý các thuộc tính của đối tượng. Cung cấp các hàm GetProperty và SetProperty. 
    /// Các thông tin của thuộc tính như là tên và giá trị của thuộc tính đó sẽ được lưu trong Dictionary
    /// </summary>
    [Serializable]
    public class PropertyBag
    {
        /// <summary>
        /// Dùng để lưu trữ giá trị và tên của thuộc tính
        /// </summary>
        [JsonIgnore]
        public Dictionary<string, object> PropertyDictionary { get; protected set; }

        /// <summary>
        /// Dùng để lưu trữ thông tin lỗi của từng thuộc tính
        /// </summary>
        [JsonIgnore]
        public Dictionary<string, IErrorInfo> ErrorDictionary { get; protected set; }

        public PropertyBag()
        {
            PropertyDictionary = new Dictionary<string, object>();
            ErrorDictionary = new Dictionary<string, IErrorInfo>();
        }

        #region SetProperty methods

        /// <summary>
        /// Hàm Set thuộc tính cho property. Hàm sẽ gọi callback và NotifyPropertyChanged nếu như thuộc tính thay đổi đồng thời sẽ trả về TRUE. 
        /// Nếu giá trị của thuộc tính không thay đổi thì trả về giá trị False.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyName"></param>
        /// <param name="newValue"></param>
        /// <param name="raiseNotification"></param>
        /// <param name="propertyChangedCallback"></param>
        /// <returns></returns>
        public bool SetProperty<T>(string propertyName, T newValue, Action<string> raiseNotification, Action<string, object, object> propertyChangedCallback)
        {
            if (SetPropertyCore(propertyName, newValue, out T oldValue))
            {
                raiseNotification?.Invoke(propertyName);
                propertyChangedCallback?.Invoke(propertyName, oldValue, newValue);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Hàm Set thuộc tính cơ bản của PropertyManager. Hàm sẽ trả về kết quả là TRUE nếu như giá trị cũ khác với giá trị mới. 
        /// False nếu như 2 giá trị bằng nhau. Đồng thời sẽ lưu lại thuộc tính này vào dictionary nếu nó chưa tồn tại.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyName"></param>
        /// <param name="newValue"></param>
        /// <param name="oldValue"></param>
        /// <returns></returns>
        protected virtual bool SetPropertyCore<T>(string propertyName, T newValue, out T oldValue)
        {
            oldValue = default;
            if (PropertyDictionary.TryGetValue(propertyName, out object oldValueInDictionary))
                oldValue = (T)oldValueInDictionary;
            if (CompareValues(oldValue, newValue))
                return false;
            PropertyDictionary[propertyName] = newValue;
            return true;
        }

        #endregion

        #region GetProperty methods

        /// <summary>
        /// Hàm lấy giá trị của thuộc tính thông qua tên của nó.
        /// </summary>
        /// <typeparam name="T">Kiểu dữ liệu của thuộc tính</typeparam>
        /// <param name="propertyName">Tên thuộc tính</param>
        /// <returns></returns>
        public T GetProperty<T>(string propertyName)
        {
            if (PropertyDictionary.ContainsKey(propertyName))
                return (T)PropertyDictionary[propertyName];
            else
            {
                PropertyDictionary[propertyName] = default(T);
                ErrorDictionary[propertyName] = new ErrorInfo() { PropertyName = propertyName };
            }
            return default;
        }

        #endregion

        /// <summary>
        /// So sánh 2 giá trị. Nếu là TRUE thì bằng nhau, FALSE là không bằng nhau
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="storage"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        static bool CompareValues<T>(T storage, T value)
        {
            return Equals(storage, value);
        }
    }
}
