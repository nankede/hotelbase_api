using HotelBase.Api.Common;
using HotelBase.Api.Common.SignMothed;
using HotelBase.Api.DataAccess.Resource;
using HotelBase.Api.DataAccess.System;
using HotelBase.Api.Entity.CommonModel;
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
        /// 获取酒店列表
        /// </summary>
        /// <returns></returns>
        public static DataResult GetHotelList(int maxId, int top)
        {
            var result = new DataResult()
            {
                Data = $"{maxId + top}"
            };
            var url = AtourSignUtil.AtourAuth_URL + "baoku/hotel/getHotelList";

            var citylist = new Sys_AreaMatchAccess().Query().Where(x => x.OutType == 1 && x.HbId > maxId).OrderBy(x => x.OutCityId).ToList();
            if (citylist == null || !citylist.Any())
            {
                result.Message = $"{maxId}未查询到数据";
                result.Data = $"{maxId + top}";
                return result;
            }
            var log = string.Empty;
            foreach (var item in citylist)
            {
                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add("appId", AtourSignUtil.AtourAuth_APPID);
                dic.Add("cityId", item.OutCityId.ToString());
                var sign = AtourSignUtil.GetSignUtil(dic);
                url += "?appId=" + AtourSignUtil.AtourAuth_APPID + "&cityId=" + item.OutCityId.ToString() + "&sign=" + sign;
                var hotellst = ApiHelper.HttpGet(url);
                if (!string.IsNullOrWhiteSpace(hotellst))
                {
                    var baseCity = new Sys_AreaInfoAccess2().Query().FirstOrDefault(x => x.id == item.HbId);
                    var baseProv = new Sys_AreaInfoAccess2().Query().FirstOrDefault(x => x.id == baseCity.pid);
                    var data = hotellst.ToObject<AtourHotelResponse>();
                    data?.result?.ForEach(x =>
                    {
                        var hDb = new H_HotelInfoAccess();
                        var model = hDb.Query().Where(h => h.HIOutId == x.hotelId && h.HIOutType == 1).FirstOrDefault();
                        int id = model?.Id ?? 0;
                        if (model == null || id <= 0)
                        {
                            model = new H_HotelInfoModel()
                            {
                                HIOutId = x.hotelId,
                                HIOutType = 1,
                                HIName = x.name ?? String.Empty,
                                HIAddress = x.address ?? String.Empty,
                                HILinkPhone = x.tel ?? string.Empty,
                                HICity = baseCity.name,
                                HICityId = baseCity.id,
                                HIProvince = baseProv.name,
                                HIProvinceId = baseCity.pid,
                                HIAddName = "",
                                HIAddTime = DateTime.Now,
                                HICheckIn = string.Empty,
                                HICheckOut = string.Empty,
                                HIChildRemark = string.Empty,
                                HICounty = string.Empty,
                                HICountyId = 0,
                                HIFacilities = string.Empty,
                                HIHotelIntroduction = string.Empty,
                                HIIsValid = 1,
                                HIPetRemark = string.Empty,
                                HIShoppingArea = string.Empty,
                                HIShoppingAreaId = 0,
                                HIUpdateName = string.Empty,
                                HIUpdateTime = DateTime.Now,
                            };
                            id = (int)(hDb.Add(model));
                        }
                        else
                        {//更新
                            //hDb.Update().Where(h=>h.Id==id)
                            //.Set(h=>h.);
                        }
                        if (id > 0)
                        {
                            PidInit(x, id);
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
        /// 图片初始化，带验证图片是否存在
        /// </summary>
        /// <param name="x"></param>
        /// <param name="id"></param>
        private static void PidInit(HotelyList x, int id, int type = 20201)
        {
            var hpDb = new H_HotelPictureAccess();
            x.pictures?.ForEach(p =>
            {
                if (!string.IsNullOrEmpty(p))
                {
                    var pic = hpDb.Query().Where(hp => hp.HPUrl == p && hp.HIId == id).FirstOrDefault();
                    if (pic == null || pic.Id <= 0)
                    {
                        pic = new H_HotelPictureModel
                        {
                            HPOutId = x.hotelId,
                            HPAddName = string.Empty,
                            HIId = id,
                            HPAddTime = DateTime.Now,
                            HPIsValid = 1,
                            HPType = type,
                            HPUpdateName = string.Empty,
                            HPUpdateTime = DateTime.Now,
                            HPUrl = p,
                        };
                        hpDb.Add(pic);
                    }
                }
            });
        }

        /// <summary>
        /// 获取城市列表
        /// </summary>
        /// <returns></returns>

        public static DataResult GetCityList()
        {
            var result = new DataResult();
            var url = AtourSignUtil.AtourAuth_URL + "city/getCityList";
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("appId", AtourSignUtil.AtourAuth_APPID);
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

        /// <summary>
        /// 酒店详情
        /// </summary>
        /// <param name="maxId"></param>
        /// <param name="top"></param>
        /// <returns></returns>
        public static DataResult GetHotelDetail(int maxId, int top)
        {
            var result = new DataResult();
            var url = AtourSignUtil.AtourAuth_URL + "baoku/hotel/getHotel";

            var hDb = new H_HotelInfoAccess();
            var hotelList = hDb.Query().Where(h => h.HIOutId >= maxId && h.HIOutType == 1).Top(top).OrderBy(h => h.HIOutId)?.ToList();
            if (hotelList == null || hotelList.Count == 0)
            {

            }

            hotelList.ForEach(x =>
            {
                var dic = new Dictionary<string, string>();
                dic.Add("appId", AtourSignUtil.AtourAuth_APPID);
                dic.Add("hotelId", x.HIOutId.ToString());
                var sign = AtourSignUtil.GetSignUtil(dic);
                url += "?appId=" + AtourSignUtil.AtourAuth_APPID + "&hotelId=" + x.HIOutId + "&sign=" + sign;
                var rtn = ApiHelper.HttpGet(url)?.ToObject<AtourHotelDetailResponse>();
                var hotel = rtn?.result;
                if (hotel?.hotelId > 0)
                {
                    hDb.Update().Set(h =>
                    h.HIName == hotel.name
                    && h.HIUpdateName == "亚朵数据更新"
                    && h.HIUpdateTime == DateTime.Now
                    ).Where(h => h.Id == x.Id).Execute();

                    //更新图片
                    PidInit(hotel, x.Id);

                    //会员 暂时未开发
                }
                else
                {
                    result.Message = rtn?.msg ?? "系统异常";
                }
            });

            return result;
        }

        //房型 baoku/hotel/getRoomTypeList
        /// <summary>
        /// 酒店房型
        /// </summary>
        /// <param name="maxId"></param>
        /// <param name="top"></param>
        /// <returns></returns>
        public static DataResult GetRoomType(int id)
        {
            var result = new DataResult();
            var url = AtourSignUtil.AtourAuth_URL + "baoku/hotel/getRoomTypeList";

            var hDb = new H_HotelInfoAccess();
            var hotelList = hDb.Query().Where(h => h.HIOutId == id && h.HIOutType == 1).OrderBy(h => h.HIOutId)?.ToList();
            if (hotelList == null || hotelList.Count == 0)
            {

            }

            hotelList.ForEach(x =>
            {
                var dic = new Dictionary<string, string>();
                dic.Add("appId", AtourSignUtil.AtourAuth_APPID);
                dic.Add("hotelId", x.HIOutId.ToString());

                var sign = AtourSignUtil.GetSignUtil(dic);
                url += "?appId=" + AtourSignUtil.AtourAuth_APPID + "&hotelId=" + x.HIOutId + "&sign=" + sign;
                var rtn = ApiHelper.HttpGet(url)?.ToObject<YdRoomTypeResponse>();
                var list = rtn?.result;
                if (list?.Count > 0)
                {
                    var db = new H_HotelRoomAccess();
                    var room = db.Query().Where(r => r.HIId == x.Id).ToList();
                    if (room != null && room.Any())
                    {

                    }


                    //会员 暂时未开发
                }
                else
                {
                    result.Message = rtn?.msg ?? "系统异常";
                }
            });

            return result;
        }


        //房价 baoku/hotel/getRoomRateList

        //库存 baoku/hotel/getRoomInventoryList

    }

}
