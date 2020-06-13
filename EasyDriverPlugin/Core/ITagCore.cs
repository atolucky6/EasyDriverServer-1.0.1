﻿using EasyScada.Api.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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
        /// Trạng thái của <see cref="ITagCore"/>
        /// </summary>
        Quality Quality { get; set; }

        /// <summary>
        /// Thời gian làm mới giá trị của <see cref="ITagCore"/>
        /// </summary>
        int RefreshRate { get; set; }

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
        TimeSpan RefreshInterval { get; set; }  

        double Gain { get; set; }

        double Offset { get; set; }

        IDataType DataType { get; set; }

        /// <summary>
        /// Kiểu dữ liệu 
        /// </summary>
        string DataTypeName { get; }

        string CommunicationError { get; set; }

        /// <summary>
        /// Hàm lấy tất cả các <see cref="ITagCore"/> bên trong Tag này
        /// </summary>
        /// <returns></returns>
        IEnumerable<ITagCore> GetAllChildTag();

        /// <summary>
        /// Sự kiện giá trị của <see cref="ITagCore"/> thay đổi
        /// </summary>
        event EventHandler<TagValueChangedEventArgs> ValueChanged;

        /// <summary>
        /// Sự kiện trạng thái của <see cref="ITagCore"/> thay đổi
        /// </summary>
        event EventHandler<TagQualityChangedEventArgs> QualityChanged;

        Indexer<ITagCore> Tags { get; }
    }
}
