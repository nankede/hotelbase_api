using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBase.Api.Entity.Response.Order
{
    public class AtourCreateOrderResponse : BaseResponse
    {
        /// <summary>
        /// 订单号
        /// </summary>
        public string atourOrderNo { get; set; }
    }
}
