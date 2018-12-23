﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBase.Api.Entity.Models
{
    public class OrderStaticEntity
    {

    }

    /// <summary>
    /// 订单统计请求
    /// </summary>
    public class OrderStaticRequest : BaseRequest
    {
        /// <summary>
        /// 统计类型（1：时间，2：省市，3：酒店，4：供应商 5：分销商）
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// 时间类型（1：创建时间 2：离店时间）
        /// </summary>
        public int TimeType { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public string StartTime { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public string EndTime { get; set; }

        /// <summary>
        /// 省份id
        /// </summary>
        public int PrivoceId { get; set; }

        /// <summary>
        /// 城市id
        /// </summary>
        public int CityId { get; set; }

        /// <summary>
        /// 来源1
        /// </summary>
        public int Part1 { get; set; }

        /// <summary>
        /// 来源2
        /// </summary>
        public int Part2 { get; set; }

        /// <summary>
        /// 供应商来源
        /// </summary>
        public int SupplierSource { get; set; }

        /// <summary>
        /// 酒店id
        /// </summary>
        public string HotelId { get; set; }

        /// <summary>
        /// 酒店名称
        /// </summary>
        public string HotelName { get; set; }


        /// <summary>
        /// 供应商名称
        /// </summary>
        public string SupplierName { get; set; }
    }

    public class OrderStaticResponse : BaseResponse
    {
        /// <summary>
        /// 统计时间
        /// </summary>
        public string CreateTime { get; set; }

        /// <summary>
        /// 统计时间
        /// </summary>
        public string CheckOutDate { get; set; }

        /// <summary>
        /// 省份
        /// </summary>
        public string ProviceId { get; set; }

        /// <summary>
        /// 省份
        /// </summary>
        public string ProviceName { get; set; }

        /// <summary>
        /// 城市
        /// </summary>
        public string CityId { get; set; }

        /// <summary>
        /// 城市
        /// </summary>
        public string CityName { get; set; }

        /// <summary>
        /// 供应商名称
        /// </summary>
        public string SupplierName { get; set; }

        /// <summary>
        /// 酒店名称
        /// </summary>
        public string HotelName { get; set; }

        /// <summary>
        /// 创建订单
        /// </summary>
        public string TotalCreate { get; set; }

        /// <summary>
        /// 成功订单
        /// </summary>
        public string TotalSuccess { get; set; }

        /// <summary>
        /// 销售额
        /// </summary>
        public decimal TotalSell { get; set; }

        /// <summary>
        /// 结算额
        /// </summary>
        public decimal TotalContract { get; set; }

        /// <summary>
        /// 营收
        /// </summary>
        public decimal TotalRevenue { get; set; }
    }
}
