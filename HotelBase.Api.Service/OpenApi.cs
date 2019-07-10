using HotelBase.Api.Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBase.Api.Service
{
    /// <summary>
    /// 内部API数据同步
    /// </summary>
    public static class OpenApi
    {

        /// <summary>
        /// 增量同步房型
        /// </summary>
        /// <param name="hotelId"></param>
        /// <returns></returns>
        public static string AddRoomInfo(int hotelId)
        {
            var url = $"http://openapi.lyqllx.com/HotelData/AddNewInfo?hotelId={hotelId}";
            return OpenApiGet(url);
        }
        /// <summary>
        /// 增量同步酒店
        /// </summary>
        /// <param name="hotelId"></param>
        /// <returns></returns>
        public static string AddHotelInfo(int hotelId)
        {
            var url = $"http://openapi.lyqllx.com/HotelData/SysInfo?hotelId={hotelId}";

            return OpenApiGet(url);
        }


        /// <summary>
        /// 增量同步价格策略
        /// </summary>
        /// <param name="hotelId"></param>
        /// <param name="roomId"></param>
        /// <param name="bfRule"></param>
        /// <param name="ruleId"></param>
        /// <param name="status">0 下架 1 上线</param>
        /// <returns></returns>
        public static string AddRuleInfo(int hotelId, int roomId, int bfRule, int ruleId, int status)
        {
            var url = $"http://openapi.lyqllx.com/HotelData/Offline?hotelId={hotelId}&roomId={roomId}&breakfastRule={bfRule}&roomRuleId={ruleId}&status={status}";

            return OpenApiGet(url);
        }

        public static string OpenApiGet(string url)
        {
            var isTest = ConfigurationManager.AppSettings["IsTest"];
            if (isTest != "0")
            {//正式环境才调用
                return ApiHelper.HttpGet(url);
            }
            return string.Empty;
        }

    }
}
