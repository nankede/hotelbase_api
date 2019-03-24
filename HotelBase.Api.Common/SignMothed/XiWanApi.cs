using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBase.Api.Common.SignMothed
{
    /// <summary>
    /// 喜玩接口
    /// </summary>
    public static class XiWanApi
    {
        public static string XiWan_Url = "124.172.245.107";// 测试  124.172.245.107

        public static string XiWan_SecurityKey = "";

        public static string XiWan_Account = "";

    }

    /// <summary>
    /// 请求
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class XiWanRequest<T>
    {
        public string Account { get { return XiWanApi.XiWan_Account; } }
        /// <summary>
        /// 请求时间
        /// </summary>
        public DateTime RequestTime { get; set; } = DateTime.Now;
        /// <summary>
        /// 请求时间,yyyy-MM-dd HH:mm:ss
        /// </summary>
        public string Time { get { return RequestTime.ToString("yyyy-MM-dd HH:mm:ss"); } }

        /// <summary>
        /// 签名，格式:小写MD5(Acount+SecurityKey+Time.ToString(“yyyyMMddHHmmss”))
        /// </summary>
        public string Sign
        {
            get
            {
                return MD5Helper.GetMD5($"{XiWanApi.XiWan_Account}{XiWanApi.XiWan_SecurityKey}{RequestTime.ToString("yyyyMMddHHmmss")}");
            }
        }

        /// <summary>
        /// 实际参数得Json
        /// </summary>
        public string Para
        {
            get
            {
                return Body.ToJson();
            }
        }
        /// <summary>
        /// 请求参数
        /// </summary>
        public T Body { get; set; }
    }

    /// <summary>
    /// 响应
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class XiWanResponse<T>
    {

        public string Code { get; set; }
        public string Msg { get; set; }
        /// <summary>
        /// 响应
        /// </summary>
        public string Result { get; set; }

        /// <summary>
        /// 响应
        /// </summary>
        public T Body
        {
            get
            {
                if (Code == "0" && !string.IsNullOrEmpty(Result))
                {
                    return Result.ToObject<T>();
                }
                return default(T);
            }
        }
    }
}
