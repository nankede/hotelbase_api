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
        /// 获取酒店列表
        /// </summary>
        /// <returns></returns>
        public static DataResult GetHotelList(int maxId, int top)
        {
            var result = new DataResult()
            {
                Data = $"{maxId + top};"
            };

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
                result.Data += $"[{item.OutCityId}]：";
                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add("appId", AtourSignUtil.AtourAuth_APPID);
                dic.Add("cityId", item.OutCityId.ToString());
                var sign = AtourSignUtil.GetSignUtil(dic);
                var url = AtourSignUtil.AtourAuth_URL + "baoku/hotel/getHotelList";
                url += "?appId=" + AtourSignUtil.AtourAuth_APPID + "&cityId=" + item.OutCityId.ToString() + "&sign=" + sign;
                var hotellst = ApiHelper.HttpGet(url);
                if (!string.IsNullOrWhiteSpace(hotellst))
                {
                    var baseCity = new Sys_AreaInfoAccess2().Query().FirstOrDefault(x => x.id == item.HbId);
                    var baseProv = new Sys_AreaInfoAccess2().Query().FirstOrDefault(x => x.id == baseCity.pid);
                    var data = hotellst.ToObject<AtourHotelResponse>();
                    result.Data += $"{baseCity.name}酒店数量：{data?.result?.Count};";
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
                                 HIGdLonLat = $"{x.longitude},{x.latitude}"
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
                    result.Data += $"无数据；";
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
        /// 酒店详情
        /// </summary>
        /// <param name="maxId"></param>
        /// <param name="top"></param>
        /// <returns></returns>
        public static DataResult GetHotelDetail(int maxId, int top)
        {
            var result = new DataResult();

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
                var url = AtourSignUtil.AtourAuth_URL + "baoku/hotel/getHotel";
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
        public static DataResult GetRoomType(int id, int top)
        {
            var result = new DataResult();
            result.Data = string.Empty;
            var hDb = new H_HotelInfoAccess();
            var hotelList = hDb.Query().Where(h => h.HIOutId >= id && h.HIOutType == 1).Top(top).OrderBy(h => h.HIOutId)?.ToList();
            if (hotelList == null || hotelList.Count == 0)
            {
                result.Message = "未查询到酒店";
                return result;
            }
            var indexCount = 0;
            hotelList.ForEach(x =>
            {
                var dic = new Dictionary<string, string>();
                dic.Add("appId", AtourSignUtil.AtourAuth_APPID);
                dic.Add("hotelId", x.HIOutId.ToString());

                var sign = AtourSignUtil.GetSignUtil(dic);
                var url = AtourSignUtil.AtourAuth_URL + "baoku/hotel/getRoomTypeList";
                url += "?appId=" + AtourSignUtil.AtourAuth_APPID + "&hotelId=" + x.HIOutId + "&sign=" + sign;
                var rtn = ApiHelper.HttpGet(url)?.ToObject<YdRoomTypeResponse>();
                var list = rtn?.result;
                if (list?.Count > 0)
                {
                    var listId = list.Select(l => l.roomTypeId).ToList();
                    var db = new H_HotelRoomAccess();
                    var room = db.Query().Where(r => r.HIId == x.Id).ToList();
                    var updateList = room?.Where(r => listId.Contains(r.HROutId) && r.HROutType == 1)?.ToList();
                    result.Data += $"[{indexCount++}]{x.HIName}新增房型{list.Count()}个；";
                    list.ForEach(l =>
                    {
                        var baseRoom = room?.Where(r => r.HROutId == l.roomTypeId && r.HROutType == 1)?.FirstOrDefault();
                        if (baseRoom == null || baseRoom.Id <= 0)
                        {
                            db.Add(new H_HotelRoomModel
                            {
                                Id = 0,
                                HIId = x.Id,
                                HROutType = 1,
                                HROutId = l.roomTypeId,
                                HRName = l.roomTypeName ?? string.Empty,
                                HRRoomSIze = string.Empty,
                                HRAddName = "亚朵新增",
                                HRAddTime = DateTime.Now,
                                HRBedSize = GetBedSize(l.bedRemark),
                                HRBedType = 0,
                                HRFloor = string.Empty,
                                HRIsValid = 1,
                                HRPersonCount = 0,
                                HRUpdateName = string.Empty,
                                HRUpdateTime = DateTime.Now,
                                HRWindowsType = 0
                            });
                        }
                        //是否要修改
                    });
                }
                else
                {
                    result.Message = rtn?.msg ?? "系统异常";
                }
                //查询最近三天的价格和库存
                var rrRtn = GetRoomRate(x.HIOutId, DateTime.Now, 3);
                result.Data += rrRtn.Data?.ToString() ?? string.Empty;
            });

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="remark"></param>
        /// <returns></returns>
        public static int GetBedSize(string remark)
        {
            if (remark == "大床")
            {
                return 50101;
            }
            if (remark == "双床")
            {
                return 50102;
            }

            return 0;
        }



        //房价 baoku/hotel/getRoomRateList
        /// <summary>
        /// 酒店价格
        /// </summary>
        /// <param name="maxId"></param>
        /// <param name="top"></param>
        /// <returns></returns>
        public static DataResult GetRoomRate(int id, DateTime start, int top)
        {
            var result = new DataResult();
            var url = AtourSignUtil.AtourAuth_URL + "baoku/hotel/getRoomRateList";

            var hDb = new H_HotelInfoAccess();
            var hotelList = hDb.Query().Where(h => h.HIOutId == id && h.HIOutType == 1).OrderBy(h => h.HIOutId)?.ToList();
            if (hotelList == null || hotelList.Count == 0)
            {
                result.Message = "未查询到酒店";
                return result;
            }


            //hotelId 是店ID
            //roomTypeId  是0房间类型ID（取接口4中的对应字段即可）
            //roomRateTypeId 否0旧版本价格类型ID(18; 协议价格 28:"【中介预付】ota-npp；29: "【中介现付】ota - npc"，41,:"【中介预付】卖价"等，可兼容)
            //mebId 否12345会员ID；HRS渠道可以不传（不传则输出该渠道下所有房价数据），其它渠道必传；
            //start 是"2018-08-10"开始日期，格式： yyyy - MM - dd
            //end 是"2018-08-11"结束日期，格式：yyyy - MM - dd
            hotelList.ForEach(x =>
            {
                SetRoomRate(x, start, top);
            });

            return result;
        }

        /// <summary>
        /// 单酒店处理房型价格
        /// </summary>
        /// <param name="hotel"></param>
        private static DataResult SetRoomRate(H_HotelInfoModel hotel, DateTime start, int top)
        {
            var result = new DataResult();
            result.Data = string.Empty;
            start = start.Year <= 2000 ? DateTime.Now : start;
            var roomdb = new H_HotelRoomAccess();
            var roomType = roomdb.Query().Where(x => x.HIId == hotel.Id && x.HROutType == 1).ToList();
            roomType?.ForEach(x =>
            {
                var priceRtn = GetYdPrice(hotel.HIOutId, start, top, x);//价格
                var storeRtn = GetYdStore(hotel.HIOutId, start, top, x);//库存
                if (priceRtn != null && priceRtn.result != null)
                {
                    var rrDb = new H_HotelRoomRuleAccess();
                    var pDb = new H_HoteRulePriceAccess();
                    priceRtn.result.ForEach(p =>
                    {
                        var oldRule = rrDb.Query().Where(rr => rr.HRROutId == p.roomTypeId && rr.HRROutType == 1
                        && rr.HRRName == p.roomRateTypeName
                        ).FirstOrDefault();
                        var newStore = storeRtn?.result?.FirstOrDefault(ns => ns.roomTypeId == p.roomTypeId && ns.accDate == p.accDate);

                        if (oldRule == null)
                        {
                            oldRule = new H_HotelRoomRuleModel
                            {
                                HRRName = p.roomRateTypeName ?? string.Empty,
                                HRROutId = p.roomTypeId,
                                HRRIsValid = 1,
                                HRROutType = 1,
                                HIId = hotel.Id,
                                Id = 0,
                                HRId = x.Id,
                                HRRAddName = "亚朵新增",
                                HRRAddTime = DateTime.Now,
                                HRRBreakfastRule = 0,
                                HRRBreakfastRuleName = string.Empty,
                                HRRCancelRule = 0,
                                HRRCancelRuleName = string.Empty,
                                HRRSourceId = 10104,
                                HRRSourceName = "集团直连",
                                HRRSupplierId = 1,
                                HRRSupplierName = "亚朵集团",
                                HRRUpdateName = string.Empty,
                                HRRUpdateTime = DateTime.Now
                            };
                            oldRule.Id = (int)rrDb.Add(oldRule);
                        }
                        else
                        {
                            rrDb.Update().Set(rr => rr.HRRSupplierName == "亚朵集团" && rr.HRRSupplierId == 4).Execute();
                        }
                        if (oldRule != null && oldRule.Id > 0)
                        {
                            var date = DateTime.MinValue;
                            DateTime.TryParse(p.accDate, out date);
                            var dateInit = ConvertHelper.ToInt32(date.ToString("yyyyMMdd"), 0);
                            var price = pDb.Query().Where(pr => pr.HRRId == oldRule.Id && pr.HRPDateInt == dateInit).FirstOrDefault();
                            if (price == null || price.Id <= 0)
                            {//新增价格和库存
                                price = new H_HoteRulePriceModel
                                {
                                    Id = 0,
                                    HIId = hotel.Id,
                                    HRId = x.Id,
                                    HRPAddName = "亚朵新增",
                                    HRPAddTime = DateTime.Now,
                                    HRPContractPrice = p.roomRate,
                                    HRPDate = date,
                                    HRPCount = newStore?.inventoryNum ?? 0,
                                    HRPDateInt = dateInit,
                                    HRPIsValid = 1,
                                    HRPRetainCount = 0,
                                    HRPSellPrice = p.roomRate,
                                    HRPStatus = 1,
                                    HRPUpdateName = string.Empty,
                                    HRPUpdateTime = DateTime.Now,
                                    HRRId = oldRule.Id
                                };
                                price.Id = (int)pDb.Add(price);
                            }
                            else
                            {
                                var sql = pDb.Update().Where(pr => pr.Id == price.Id);
                                if (newStore != null)
                                {
                                    sql.Set(pr => pr.HRPCount == newStore.inventoryNum);
                                }
                                sql.Set(pr => pr.HRPContractPrice == p.roomRate
                                && pr.HRPSellPrice == p.roomRate
                                && pr.HRPUpdateName == "亚朵更新"
                                && pr.HRPUpdateTime == DateTime.Now
                                ).Execute();
                            }
                        }
                    });
                }
            });
            return result;
        }

        /// <summary>
        /// 获取亚朵价格
        /// </summary>
        /// <param name="hotelId"></param>
        /// <param name="start"></param>
        /// <param name="top"></param>
        /// <param name="room"></param>
        /// <returns></returns>
        private static YdRoomRateResponse GetYdPrice(int hotelId, DateTime start, int top, H_HotelRoomModel room)
        {
            top = top < 7 ? 7 : top;
            var dic = new Dictionary<string, string>
            {
                { "appId", AtourSignUtil.AtourAuth_APPID },
                { "hotelId",hotelId.ToString() },
                { "roomTypeId", room.HROutId.ToString() },
                { "mebId", AtourSignUtil.AtourAuth_MebId },

                { "start", start.ToString("yyyy-MM-dd") },
                { "end", start.AddDays(top).ToString("yyyy-MM-dd") }
            };
            //hotelId 是店ID
            //roomTypeId  是0房间类型ID（取接口4中的对应字段即可）
            //roomRateTypeId 否0旧版本价格类型ID(18; 协议价格 28:"【中介预付】ota-npp；29: "【中介现付】ota - npc"，41,:"【中介预付】卖价"等，可兼容)
            //mebId 否12345会员ID；HRS渠道可以不传（不传则输出该渠道下所有房价数据），其它渠道必传；
            //start 是"2018-08-10"开始日期，格式： yyyy - MM - dd
            //end 是"2018-08-11"结束日期，格式：yyyy - MM - dd

            var url = AtourSignUtil.AtourAuth_URL + "baoku/hotel/getRoomRateList";
            url += GetUrlPara(dic);
            return ApiHelper.HttpGet(url)?.ToObject<YdRoomRateResponse>();
        }

        /// <summary>
        /// 获取亚朵库存
        /// </summary>
        /// <param name="hotelId"></param>
        /// <param name="start"></param>
        /// <param name="top"></param>
        /// <param name="room"></param>
        /// <returns></returns>
        private static YdRoomStoreResponse GetYdStore(int hotelId, DateTime start, int top, H_HotelRoomModel room)
        {
            var dic = new Dictionary<string, string>
            {
                { "appId", AtourSignUtil.AtourAuth_APPID },
                { "hotelId",hotelId.ToString() },
                { "roomTypeId", room.HROutId.ToString() },
                { "start", start.ToString("yyyy-MM-dd") },
                { "end", start.AddDays(top).ToString("yyyy-MM-dd") }
            };
            //hotelId 是店ID
            //roomTypeId  是0房间类型ID（取接口4中的对应字段即可）
            //roomRateTypeId 否0旧版本价格类型ID(18; 协议价格 28:"【中介预付】ota-npp；29: "【中介现付】ota - npc"，41,:"【中介预付】卖价"等，可兼容)
            //mebId 否12345会员ID；HRS渠道可以不传（不传则输出该渠道下所有房价数据），其它渠道必传；
            //start 是"2018-08-10"开始日期，格式： yyyy - MM - dd
            //end 是"2018-08-11"结束日期，格式：yyyy - MM - dd

            var url = AtourSignUtil.AtourAuth_URL + "baoku/hotel/getRoomInventoryList";
            url += GetUrlPara(dic);
            return ApiHelper.HttpGet(url)?.ToObject<YdRoomStoreResponse>();
        }

        /// <summary>
        /// 获取Url参数
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        private static string GetUrlPara(Dictionary<string, string> dic)
        {
            var rtn = string.Empty;
            var sign = AtourSignUtil.GetSignUtil(dic);
            dic.Add("sign", sign);
            foreach (var d in dic)
            {
                if (!string.IsNullOrEmpty(d.Value))
                {
                    rtn += $"&{d.Key}={d.Value}";
                }
            }
            rtn = $"?{rtn.TrimStart('&')}";
            return rtn;
        }

        //库存 baoku/hotel/getRoomInventoryList、

        #region 城市匹配


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

            var oldList = new Sys_AreaInfoAccess2().Query().Where(x => x.type == 3).ToList();
            var modellist = new List<Sys_AreaMatchModel>();
            var data = citylist.ToObject<AtourCityResponse>();
            data?.result?.ForEach(n =>
            {
                var old = oldList.FirstOrDefault(x => x.name == n.cityName.Replace("市", ""));
                var model = new Sys_AreaMatchModel
                {
                    OutProvId = 0,
                    OutProvName = n.provinceName,
                    OutCityId = n.cityId,
                    OutCityName = n.cityName,
                    HbId = old?.id ?? 0,
                    OutType = 1
                };
                var db = new Sys_AreaMatchAccess();
                var m = db.Query().Where(x => x.OutType == 1 && x.OutCityId == n.cityId).FirstOrDefault();
                if (m == null || m.Id <= 0)
                {
                    db.Add(model);
                    result.Data += $"{old?.id}:{n.cityName}；";
                }
                else
                {

                }
            });

            return result;
        }

        #endregion
    }

}




