using Newtonsoft.Json.Bson;
using System;
using System.Collections.Generic;

namespace EasyDriverPlugin
{
    public interface ITagCore : IGroupItem, ISupportParameters, ISupportSynchronization
    {
        /// <summary>
        /// Tên hiển thị của <see cref="ITagCore"/>
        /// </summary>
        string DisplayName { get; set; }

        /// <summary>
        /// Địa chỉ của <see cref="ITagCore"/>
        /// </summary>
        string Address { get; set; }

        /// <summary>
        /// Giá trị của <see cref="ITagCore"/>
        /// </summary>
        string Value { get; set; }

        /// <summary>
        /// Giá trị mặc định cho internal tag
        /// </summary>
        string DefaultValue { get; set; }

        /// <summary>
        /// Thông tin đơn vị của tag
        /// </summary>
        string Unit { get; set; }

        /// <summary>
        /// Trạng thái của <see cref="ITagCore"/>
        /// </summary>
        Quality Quality { get; set; }

        /// <summary>
        /// Thời gian làm mới giá trị của <see cref="ITagCore"/>
        /// </summary>
        int RefreshRate { get; set; }

        /// <summary>
        /// Giới hạn tối đa cho phép ghi
        /// </summary>
        double WriteMaxLimit { get; set; }

        /// <summary>
        /// Giới hạn tối thiểu cho phép ghi
        /// </summary>
        double WriteMinLimit { get; set; }

        /// <summary>
        /// Cho phép giới hạn ghi
        /// </summary>
        bool EnabledWriteLimit { get; set; }

        /// <summary>
        /// Quyền truy cập đến <see cref="ITagCore"/>
        /// </summary>
        AccessPermission AccessPermission { get; set; }

        /// <summary>
        /// Thứ tự sắp xếp byte
        /// </summary>
        ByteOrder ByteOrder { get; set; }

        /// <summary>
        /// Thời gian lần cuối cập nhật đối tượng
        /// </summary>
        DateTime TimeStamp { get; set; }

        /// <summary>
        /// Thời gian giữa 2 lần cập nhật giá trị gần nhất
        /// </summary>
        int RefreshInterval { get; set; }  

        /// <summary>
        /// Hệ số nhân với Value
        /// </summary>
        double Gain { get; set; }

        /// <summary>
        /// Hệ số bù cộng với Value
        /// </summary>
        double Offset { get; set; }

        string ExpressionString { get; set; }

        bool Retain { get; set; }

        string GUID { get; set; }

        /// <summary>
        /// Kiểu dữ liệu của Tag
        /// </summary>
        IDataType DataType { get; set; }

        /// <summary>
        /// Kiểu dữ liệu 
        /// </summary>
        string DataTypeName { get; set; }

        /// <summary>
        /// Thông tin lỗi của tag
        /// </summary>
        string CommunicationError { get; set; }

        /// <summary>
        /// Trạng thái kết nối
        /// </summary>
        ConnectionStatus ConnectionStatus { get; set; }

        /// <summary>
        /// Bit để biết tag là tag nội
        /// </summary>
        bool IsInternalTag { get; set; }

        /// <summary>
        /// Hàm lấy tất cả các <see cref="ITagCore"/> bên trong Tag này
        /// </summary>
        /// <returns></returns>
        IEnumerable<ITagCore> GetAllChildTag();
    }
}
