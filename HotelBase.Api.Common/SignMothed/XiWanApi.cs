using Newtonsoft.Json;
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
    public static class XiWanConst
    {
        public static string XiWan_Url = "124.172.245.107";// 测试  124.172.245.107

        public static string XiWan_SecurityKey = "iaoPZolcKuhgJQYhdPglmbjYUahgsuNb";

        public static string XiWan_Account = "apitestqulang";
    }
    /// <summary>
    /// 喜玩接口
    /// </summary>
    public static class XiWanAPI
    {
        /// <summary>
        /// 喜玩请求
        /// </summary>
        /// <typeparam name="R"></typeparam>
        /// <param name="request"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public static XiWanResponse<T> XiWanPost<T, R>(R request, string url)
        {
            var allRequest = new XiWanRequest<R>
            {
                Body = request,
                RequestTime = DateTime.Now
            };
            var allUrl = $"{url}?Account={XiWanConst.XiWan_Account}&Time={allRequest.RequestTime}&Sign={allRequest.Sign}&Param={request.ToJson()}";
            var rtn = ApiHelper.HttpPost<XiWanResponse<T>>(allUrl, string.Empty);
            return rtn;
        }

    }

    /// <summary>
    /// 请求
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class XiWanRequest<T>
    {
        public string Account { get { return XiWanConst.XiWan_Account; } }
        /// <summary>
        /// 请求时间
        /// </summary>
        [JsonIgnore]
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
                return MD5Helper.GetMD5($"{XiWanConst.XiWan_Account}{XiWanConst.XiWan_SecurityKey}{RequestTime.ToString("yyyyMMddHHmmss")}");
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
        [JsonIgnore]
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
        public T Result { get; set; }
    }

    public class XiWanPageRequest
    {
        /// <summary> 每页大小 </summary>
        public int PageSize { get; set; }
        /// <summary> 页码 </summary>
        public int PageIndex { get; set; }
    }


    public class XiWanPageInfo
    {
        /// <summary> 每页大小 </summary>
        public int PageSize { get; set; }
        /// <summary> 页码 </summary>
        public int PageIndex { get; set; }
        /// <summary> 总页数 </summary>
        public int PageCount { get; set; }
    }

    public class XiWanHotelDetailRequest
    {
        public int HotelId { get; set; }

    }
}
