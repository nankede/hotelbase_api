using HotelBase.Api.Common;
using HotelBase.Api.Common.SignMothed;
using HotelBase.Api.DataAccess.Resource;
using HotelBase.Api.DataAccess.System;
using HotelBase.Api.Entity.CommonModel;
using HotelBase.Api.Entity.CommonModel.Enum;
using HotelBase.Api.Entity.Response;
using HotelBase.Api.Entity.Tables;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HotelBase.Api.Service
{
    /// <summary>
    /// 亚朵接口
    /// </summary>
    public class YaDuoApiService
    {
        /// <summary>
        /// 获取酒店基础信息
        /// </summary>
        /// <returns></returns>
        public static DataResult GetHotelList(int maxId, int top)
        {
            var result = new DataResult();
            var url = AtourSignUtil.AtourAuth_URL + "hotel/getHotelList";


            var citylist = new Sys_AreaMatchAccess().Query().Where(x => x.OutType == 1 && x.HbId > maxId).OrderBy(x => x.OutCityId).ToList();
            if (citylist != null && citylist.Any())
            {
                result.Message = $"{maxId}未查询到数据";
                result.Data = $"{maxId + top}";
                return result;
            }
            var log = string.Empty;
            foreach (var item in citylist)
            {
                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add("appid", AtourSignUtil.AtourAuth_APPID);
                dic.Add("cityId", item.OutCityId.ToString());
                var sign = AtourSignUtil.GetSignUtil(dic);
                var hotellst = ApiHelper.HttpGet(url + "?appId=" + AtourSignUtil.AtourAuth_APPID + "&cityId=" + item.OutCityId.ToString() + "&sign=" + sign);
                if (!string.IsNullOrWhiteSpace(hotellst))
                {
                    var baseCity = new Sys_AreaInfoAccess2().Query().FirstOrDefault(x => x.id == item.HbId);
                    var data = hotellst.ToObject<AtourHotelResponse>();
                    data?.result?.ForEach(x =>
                    {
                        var m = new H_HotelInfoModel()
                        {
                            HIName = x.name ?? String.Empty,
                            HIAddName = "",
                            HIAddress = x.address ?? String.Empty,
                            HIAddTime = DateTime.Now,
                            HICheckIn = string.Empty,
                            HICheckOut = string.Empty,
                            HIChildRemark = string.Empty,
                            HICity = baseCity.name,
                            HICityId = baseCity.id,
                            HICounty = string.Empty,
                            HICountyId = 0,
                            HIFacilities = string.Empty,
                            HIHotelIntroduction = string.Empty,
                            HIIsValid = 1,
                            HILinkPhone = string.Empty,
                            HIPetRemark = string.Empty,
                            HIProvince = string.Empty,
                            HIProvinceId = baseCity.pid,
                            HIShoppingArea = string.Empty,
                            HIShoppingAreaId = 0,
                            HIUpdateName = string.Empty,
                            HIUpdateTime = DateTime.Now,
                        };
                        var id = new H_HotelInfoAccess().Add(m);
                        if (id > 0)
                        {
                            var picList = x.pictures?.Select(p => new H_HotelPictureModel
                            {
                                HPOutId = x.hotelId,
                                HPAddName = string.Empty,
                                HIId = id,
                                HPAddTime = DateTime.Now,
                                HPIsValid = 1,
                                HPType = 0,
                                HPUpdateName = string.Empty,
                                HPUpdateTime = DateTime.Now,
                                HPUrl = p,
                            })?.ToList();
                            if (picList != null && picList.Any())
                            {
                                new H_HotelPictureAccess().AddBatch(picList);
                            }
                        }
                    });
                }
                else
                {
                    log += $"[{item.OutCityId}]：无数据；";
                }
            }

            return result;
        }

        /// <summary>
        /// 获取城市列表
        /// </summary>
        /// <returns></returns>

        public static DataResult GetCityList()
        {
            var result = new DataResult();
            var url = AtourSignUtil. AtourAuth_URL + "city/getCityList";
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("appid", AtourSignUtil.AtourAuth_APPID);
            var sign = AtourSignUtil.GetSignUtil(dic);
            var citylist = ApiHelper.HttpGet(url + "?appId=" + AtourSignUtil.AtourAuth_APPID + "&sign=" + sign);
            if (string.IsNullOrWhiteSpace(citylist))
            {
                result = new DataResult { Code = DataResultType.Fail, Message = "系统异常" };
            }
            var data = citylist.ToObject<AtourCityResponse>();
            var modellist = data?.result?.Select(x =>
            new Sys_AreaMatchModel
            {
                OutProvId = 0,
                OutProvName = x.provinceName,
                OutCityId = x.cityId,
                OutCityName = x.cityName,
                OutType = 1
            })?.ToList();
            if (modellist != null && modellist.Any())
            {
                var issuccess = new Sys_AreaMatchAccess().AddBatch(modellist);
                result.Code = issuccess ? DataResultType.Sucess : DataResultType.Fail;
            }
            else
            {
                result = new DataResult { Code = DataResultType.Fail, Message = string.IsNullOrEmpty(data.msg) ? "系统异常" : data.msg };
            }
            return result;
        }

        public static DataResult GetHotelDetail(int maxId, int top)
        {
            var result = new DataResult();
            var url = AtourSignUtil.AtourAuth_URL + "hotel/getHotelList";


            var citylist = new Sys_AreaMatchAccess().Query().Where(x => x.OutType == 1 && x.HbId > maxId).OrderBy(x => x.OutCityId).ToList();
            if (citylist != null && citylist.Any())
            {
                result.Message = $"{maxId}未查询到数据";
                result.Data = $"{maxId + top}";
                return result;
            }
            var log = string.Empty;
            foreach (var item in citylist)
            {
                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add("appid", AtourSignUtil.AtourAuth_APPID);
                dic.Add("cityId", item.OutCityId.ToString());
                var sign = AtourSignUtil.GetSignUtil(dic);
                var hotellst = ApiHelper.HttpGet(url + "?appId=" + AtourSignUtil.AtourAuth_APPID + "&cityId=" + item.OutCityId.ToString() + "&sign=" + sign);
                if (!string.IsNullOrWhiteSpace(hotellst))
                {
                    var baseCity = new Sys_AreaInfoAccess2().Query().FirstOrDefault(x => x.id == item.HbId);
                    var data = hotellst.ToObject<AtourHotelResponse>();
                    data?.result?.ForEach(x =>
                    {
                        var m = new H_HotelInfoModel()
                        {
                            HIName = x.name ?? String.Empty,
                            HIAddName = "",
                            HIAddress = x.address ?? String.Empty,
                            HIAddTime = DateTime.Now,
                            HICheckIn = string.Empty,
                            HICheckOut = string.Empty,
                            HIChildRemark = string.Empty,
                            HICity = baseCity.name,
                            HICityId = baseCity.id,
                            HICounty = string.Empty,
                            HICountyId = 0,
                            HIFacilities = string.Empty,
                            HIHotelIntroduction = string.Empty,
                            HIIsValid = 1,
                            HILinkPhone = string.Empty,
                            HIPetRemark = string.Empty,
                            HIProvince = string.Empty,
                            HIProvinceId = baseCity.pid,
                            HIShoppingArea = string.Empty,
                            HIShoppingAreaId = 0,
                            HIUpdateName = string.Empty,
                            HIUpdateTime = DateTime.Now,
                        };
                        var id = new H_HotelInfoAccess().Add(m);
                        if (id > 0)
                        {
                            var picList = x.pictures?.Select(p => new H_HotelPictureModel
                            {
                                HPOutId = x.hotelId,
                                HPAddName = string.Empty,
                                HIId = id,
                                HPAddTime = DateTime.Now,
                                HPIsValid = 1,
                                HPType = 0,
                                HPUpdateName = string.Empty,
                                HPUpdateTime = DateTime.Now,
                                HPUrl = p,
                            })?.ToList();
                            if (picList != null && picList.Any())
                            {
                                new H_HotelPictureAccess().AddBatch(picList);
                            }
                        }
                    });
                }
                else
                {
                    log += $"[{item.OutCityId}]：无数据；";
                }
            }

            return result;
        }

    }
}
