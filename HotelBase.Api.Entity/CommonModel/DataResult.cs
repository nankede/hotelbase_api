﻿using HotelBase.Api.Entity.CommonModel.Enum;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBase.Api.Entity.CommonModel
{
    public class DataResult
    {
        #region 属性

        /// <summary>
        ///     获取或设置 操作结果类型
        /// </summary>
        public DataResultType Code { get; set; }

        /// <summary>
        ///     获取或设置 操作返回信息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        ///     获取或设置 操作结果附加信息
        /// </summary>
        public object Data { get; set; }

        public Stopwatch Sw { get; set; } = Stopwatch.StartNew();
        #endregion
    }
}
