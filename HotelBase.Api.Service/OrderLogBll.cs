using HotelBase.Api.DataAccess.Order;
using HotelBase.Api.Entity;
using HotelBase.Api.Entity.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBase.Api.Service
{
    public class OrderLogBll
    {
        /// <summary>
        /// 新增订单日志
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static bool AddOrderModel(HO_HotelOrderLogModel model)
        {
            var id = Ho_HotelOrderLogAccess.AddOrderLogModel(model);
            return id >= 1;
        }
    }
}
