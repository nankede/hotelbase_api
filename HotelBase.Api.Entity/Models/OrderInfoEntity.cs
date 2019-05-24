﻿using HotelBase.Api.Entity.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBase.Api.Entity.Models
{
    public class OrderSearchRequset : BaseRequest
    {
        /// <summary>
        /// 订单号
        /// </summary>
        public string HOCustomerSerialId { get; set; }

        /// <summary>
        /// 查询人员归属  1：预订人  2：入住人
        /// </summary>
        public int CustomerType { get; set; }

        /// <summary>
        /// 人
        /// </summary>
        public string PeopleName { get; set; }

        /// <summary>
        ///手机
        /// </summary>
        public string PeopleMobile { get; set; }

        /// <summary>
        /// 查询时间类型 1:入住时间 2：创建时间
        /// </summary>
        public int TimeType { get; set; } 

        /// <summary>
        /// 查询开始时间(入住开始时间，创建开始时间)
        /// </summary>
        public string StartTime { get; set; }

        /// <summary>
        /// 查询结束时间
        /// </summary>
        public string EndTime { get; set; }

        /// <summary>
        /// 来源id
        /// </summary>
        public int SourceId { get; set;}

        /// <summary>
        /// 酒店名称
        /// </summary>
        public string HotelName { get; set; }

        /// <summary>
        /// 酒店id
        /// </summary>
        public string HIId { get; set; }

        /// <summary>
        /// 第三方流水号
        /// </summary>
        public string HOOutSerialId { get; set; }

        /// <summary>
        /// 订单状态
        /// </summary>
        public List<string> HOStatus { get; set; }
    }

    /// <summary>
    /// 查询返回
    /// </summary>

    public class OrderSearchResponse : BaseResponse
    {
        /// <summary>
        /// 订单id
        /// </summary>
        public int Id { get; set; }

        public string HOCustomerSerialId { get; set; }

        /// <summary>
        /// 供应商来源id
        /// </summary>
        public string HOSupplierSourceId { get; set; }

        /// <summary>
        /// 供应商来源
        /// </summary>
        public string HOSupplierSourceName { get; set; }

        /// <summary>
        /// 酒店Id
        /// </summary>
        public string HIId { get; set; }

        /// <summary>
        /// 酒店名称
        /// </summary>
        public string HName { get; set; }

        /// <summary>
        /// 入住时间
        /// </summary>
        public string HOCheckInDate { get; set; }

        /// <summary>
        /// 离店时间
        /// </summary>
        public string HOCheckOutDate { get; set; }

        /// <summary>
        /// 预订人
        /// </summary>
        public string HOLinkerName { get; set; }

        /// <summary>
        /// 订单状态
        /// </summary>
        public string HOStatus { get; set; }

        /// <summary>
        /// 总金额
        /// </summary>
        public decimal HOSellPrice { get; set; }

        /// <summary>
        /// 结算价
        /// </summary>
        public decimal HOContractPrice { get; set; }

    }


    public class OrdrModel : HO_HotelOrderModel
    {

    }


    public class SeaOrdrModel : HO_HotelOrderModel
    {
        public string OutHotelId { get; set; }

        public string OutRoomId { get; set; }
    }

    public class OrderLogSearchRequset : BaseRequest
    {
        /// <summary>
        /// 订单号
        /// </summary>
        public string CustomerSerialId { get; set; }
    }
}
