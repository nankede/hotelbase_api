using HotelBase.Api.Common;
using HotelBase.Api.Common.SignMothed;
using HotelBase.Api.Entity.CommonModel;
using HotelBase.Api.Entity.Response;
using HotelBase.Api.Entity.Tables;
using HotelBase.Api.Service;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace HotelBase.Api.Controllers
{
    /// <summary>
    /// 亚朵资源
    /// </summary>
    public class AtourController : ApiController
    {
        private string AtourAuth_URL = ConfigurationManager.AppSettings["OpenApiTest"];
        private string AtourAuth_APPID = ConfigurationManager.AppSettings["appidtest"];

        /// <summary>
        /// 获取城市列表
        /// </summary>
        /// <returns></returns>

        public DataResult GetCityList()
        {
            var result = new DataResult();
            var url = AtourAuth_URL + "city/getCityList";
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("appid", AtourAuth_APPID);
            var sign = AtourSignUtil.GetSignUtil(dic);
            var citylist = ApiHelper.HttpGet(url + "?appId=" + AtourAuth_APPID + "&sign=" + sign);
            if (!string.IsNullOrWhiteSpace(citylist))
            {
                var data = JsonConvert.DeserializeObject<AtourCityResponse>(citylist);
                if (data != null && data.msg == "success" && data.result.Any())
                {
                    var modellist = new List<H_DistributorAreaInfoModel>();
                    foreach (var item in data.result)
                    {
                        var cityid = AreaBll.IsInTable(item.cityId);
                        if (cityid == 0)
                        {
                            var model = new H_DistributorAreaInfoModel
                            {
                                AA_ProvinceId = 0,
                                AA_ProvinceName = item.provinceName,
                                AA_CityId = item.cityId,
                                AA_CityName = item.cityName,
                                AA_Type = 1
                            };
                            modellist.Add(model);
                        }
                    }
                    var issuccess = AreaBll.Insert(modellist);
                    result.Code = issuccess ? DataResultType.Sucess : DataResultType.Fail;
                }
            }
            return result;
        }

        /// <summary>
        /// 获取酒店基础信息
        /// </summary>
        /// <returns></returns>
        public DataResult GetHotelList()
        {
            var result = new DataResult();
            var url = AtourAuth_URL + "hotel/getHotelList";
            var citylist = AreaBll.GetCityList();
            if (citylist != null && citylist.Any())
            {
                foreach (var item in citylist)
                {
                    Dictionary<string, string> dic = new Dictionary<string, string>();
                    dic.Add("appid", AtourAuth_APPID);
                    dic.Add("cityId", item.AA_CityId.ToString());
                    var sign = AtourSignUtil.GetSignUtil(dic);
                    var hotellst = ApiHelper.HttpGet(url + "?appId=" + AtourAuth_APPID + "&cityId=" + item.AA_CityId.ToString() + "&sign=" + sign);
                    if (!string.IsNullOrWhiteSpace(hotellst))
                    {
                        var data = JsonConvert.DeserializeObject<AtourHotelResponse>(hotellst);
                        if (data != null && data.msg == "success" && data.result.Any())
                        {
                            foreach (var hotel in data.result)
                            {

                            }
                        }
                    }
                }
            }
            
            return result;
        }
    }
}