using Component.Access.DapperExtensions.Lambda;
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
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HotelBase.Api.Service
{
    /// <summary>
    /// 喜玩接口
    /// </summary>
    public class XiWanApiService
    {
        private static string HotelListUrl = $"http://{XiWanConst.XiWan_Url}/hotelapi/hotel/GetHotelPage.ashx";
        private static string HotelDetailUrl = $"http://{XiWanConst.XiWan_Url}/hotelapi/hotel/GetHotelInfo.ashx";
        private static string HotelDPriceUrl = $"http://{XiWanConst.XiWan_Url}/hotelapi/hotel/GetHotel.ashx";
        //http://{地址}/hotelapi/hotel/ GetHotelInfo.ashx

        /// <summary>
        /// 获取酒店列表
        /// </summary>
        /// <returns></returns>
        public static DataResult GetHotelList(int maxId, int top)
        {
            var result = new DataResult()
            {
                Message = "1",
            };
            var sw = Stopwatch.StartNew();
            var totalCount = 0;
            var data = GetXiWanHotel(1);

            if (data != null)
            {
                totalCount = data.PageCount;

                //第一页
                if (data.HotelList != null && data.HotelList.Count > 0)
                {//存储酒店名称
                    HotelInsert(data.HotelList);
                }

                //后面几页
                for (var i = 2; i <= totalCount; i++)
                {
                    data = GetXiWanHotel(i);
                    if (data != null && data.HotelList != null && data.HotelList.Count > 0)
                    {//存储酒店名称
                        HotelInsert(data.HotelList);
                    }
                    result.Message += $"{i}:{sw.ElapsedMilliseconds.ToString()};";
                    Thread.Sleep(10);
                }
            }
            result.Message += $"||时间：" + sw.ElapsedMilliseconds.ToString();

            return result;
        }

        public static XiWanHotelList GetXiWanHotel(int pageIndex)
        {
            var request = new XiWanPageRequest { PageIndex = pageIndex, PageSize = 100 };
            var rtn = XiWanAPI.XiWanPost<XiWanHotelList, XiWanPageRequest>(request, HotelListUrl);
            return rtn?.Result;
        }

        /// <summary>
        /// 酒店新增
        /// </summary>
        private static void HotelInsert(List<XiWanHotelInfo> list)
        {
            var xwList = new List<XiWanHotelInfo>();
            list?.ForEach(x =>
            {
                xwList.Add(x);
                if (xwList.Count == 10)
                {
                    var wxIds = xwList.Select(a => a.HotelId).ToList();
                    var hDb = new H_HotelInfoAccess();
                    var dbList = hDb.Query().Where(h => h.HIOutId.In(wxIds) && h.HIOutType == 2)?.ToList()?.Select(h => h.HIOutId)?.ToList();
                    wxIds = wxIds.Where(a => !dbList.Contains(a))?.ToList();

                    var addList = new List<H_HotelInfoModel>();
                    wxIds?.ForEach(a =>
                    {
                        var hotel = xwList.FirstOrDefault(xx => xx.HotelId == a);
                        var model = new H_HotelInfoModel()
                        {
                            HIIsValid = 1,
                            HIOutId = hotel.HotelId,
                            HIOutType = 2,
                            HIName = hotel.HotelName,
                            HIAddName = "喜玩新增",
                        };
                        var id = (int)hDb.Add(model);
                        //增量同步
                        //OpenApi.SysInfo(id);
                        OpenApi.HotelOffline(id, 1);
                    });
                    //if (addList != null && addList.Count > 0)
                    //{
                    //    hDb.AddBatch(addList);
                    //}
                    xwList = new List<XiWanHotelInfo>();
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
            var hotelList = hDb.Query().Where(h => h.Id >= maxId && h.HIOutType == 2).Top(top).OrderBy(h => h.HIOutId)?.ToList();
            if (hotelList == null || hotelList.Count == 0)
            {
                result.Message = "无数据";

            }
            var provList = new Sys_AreaInfoAccess2().Query().Where(x => x.type == 2).ToList();
            var msgList = new List<string>();
            Parallel.ForEach(hotelList, new ParallelOptions() { MaxDegreeOfParallelism = 3 }, (x, loopstate) =>
            {
                var request = new XiWanHotelDetailRequest { HotelId = x.HIOutId };
                var rtn = XiWanAPI.XiWanPost<XiWanHotelDetail, XiWanHotelDetailRequest>(request, HotelDetailUrl);
                var hotel = rtn?.Result;
                if (hotel?.HotelId > 0)
                {
                    var city = AddCityCode(hotel.CityCode, hotel.CityName) ?? new Sys_AreaInfoModel();
                    var prov = new Sys_AreaInfoModel();
                    if (city.pid > 0)
                    {
                        prov = provList.FirstOrDefault(p => p.id == city.pid) ?? new Sys_AreaInfoModel();
                    }
                    hDb.Update().Set(h =>
                    h.HIGdLonLat == hotel.Position
                    && h.HIName == hotel.HotelName
                    && h.HIHotelIntroduction == hotel.Intro
                    && h.HIAddress == hotel.Address
                    && h.HILinkPhone == hotel.Tel
                    && h.HICityId == city.id
                    && h.HICity == (city.name ?? string.Empty)
                    && h.HIProvinceId == prov.id
                    && h.HIProvince == (prov.name ?? string.Empty)
                    && h.HIUpdateName == "喜玩详情接口更新"
                    && h.HIUpdateTime == DateTime.Now
                    ).Where(h => h.Id == x.Id).Execute();
                    //会员 暂时未开发
                }
                else
                {
                    msgList.Add(rtn?.Msg ?? "系统异常");
                }

                //房型等
                var d1 = Xw_HotelPrice(x.Id);
                msgList.Add($"||{x.Id}:{d1.Message}");
                Thread.Sleep(10);
            });
            result.Data = msgList;
            return result;
        }

        /// <summary>
        /// 城市
        /// </summary>
        /// <param name="code"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Sys_AreaInfoModel AddCityCode(string code, string name)
        {
            var city = new Sys_AreaInfoModel();
            var oldList = new Sys_AreaInfoAccess2().Query().Where(x => x.type == 3).ToList();
            var modellist = new List<Sys_AreaMatchModel>();
            var matchDb = new Sys_AreaMatchAccess();
            var cityMatch = matchDb.Query().Where(x => x.OutType == 2 && x.OutCityCode == code).FirstOrDefault();
            if (cityMatch == null || cityMatch.Id <= 0)
            {
                city = oldList.FirstOrDefault(x => x.name == name);
                cityMatch = new Sys_AreaMatchModel
                {
                    OutCityCode = code,
                    OutCityName = name,
                    OutType = 2,
                    HbId = city?.id ?? 0
                };
                matchDb.Add(cityMatch);
            }
            else
            {
                city = oldList.FirstOrDefault(x => x.id == cityMatch.HbId);
            }
            return city;
        }

        /// <summary>
        /// 喜玩价格
        /// </summary>
        /// <returns></returns>
        public static DataResult Xw_HotelPrice(long id = 0)
        {
            var rtn = new DataResult();

            var roomDb = new H_HotelRoomAccess();
            var hDb = new H_HotelInfoAccess();
            var query = hDb.Query().Where(h => h.HIOutType == 2);
            if (id > 0)
            {
                query.Where(x => x.Id == id);
            }
            var hotelList = query.ToList();
            hotelList.ForEach(x =>
            {
                var hotel = GetHotelPrice(x.HIOutId);
                if (hotel != null && hotel.HotelId > 0)
                {
                    var c = hotel?.Rooms.Count() ?? 0;
                    rtn.Message += $"{c}个；";
                    if (c == 0)
                    {//房型数量为0  酒店无效
                        hDb.Update().Where(h => h.Id == x.Id)
                        .Set(h => h.HIIsValid == 0 && h.HIUpdateName == "喜玩无房型更新" && h.HIUpdateTime == DateTime.Now)
                        .Execute();
                        OpenApi.HotelOffline(x.Id, 0);
                        rtn.Message += $"[无效]";
                    }
                    else
                    {
                        if (x.HIIsValid == 0)
                        {
                            hDb.Update().Where(h => h.Id == x.Id)
                           .Set(h => h.HIIsValid == 1 && h.HIUpdateName == "喜玩更新酒店有效" && h.HIUpdateTime == DateTime.Now)
                           .Execute();
                            OpenApi.HotelOffline(x.Id, 1);
                            rtn.Message += $"[有效]";
                        }
                        hotel?.Rooms.ForEach(r =>
                        {
                            var oldRoom = roomDb.Query().Where(rd => rd.HIId == x.Id && rd.HROutId == r.RoomTypeId.ToInt()).FirstOrDefault();
                            if (oldRoom == null || oldRoom.Id <= 0)
                            {
                                oldRoom = new H_HotelRoomModel
                                {
                                    HROutId = r.RoomTypeId.ToInt(),
                                    HIId = x.Id,
                                    Id = 0,
                                    HRAddName = "喜玩新增",
                                    HRAddTime = DateTime.Now,
                                    HRBedType = GetBedType(r.RoomName, r.BedType),//需要转化
                                    HRBedSize = GetBedSize(r.RoomName, r.BedType),
                                    HRFloor = string.Empty,
                                    HRIsValid = 1,
                                    HRName = r.RoomName ?? String.Empty,
                                    HROutType = 2,
                                    HRPersonCount = 0,
                                    HRRoomSIze = r.Description ?? string.Empty,
                                    HRUpdateName = string.Empty,
                                    HRUpdateTime = new DateTime(2000, 1, 1),
                                    HRWindowsType = 0//需要转化
                                };
                                oldRoom.Id = (int)roomDb.Add(oldRoom);

                            }
                            else
                            {//修改暂时不弄
                                if (oldRoom.HRBedType == 0)
                                {
                                    roomDb.Update().Set(rr => rr.HRBedType == GetBedType(r.RoomName, r.BedType))
                                    .Set(rr => rr.HRBedSize == GetBedSize(r.RoomName, r.BedType))
                                    .Where(rr => rr.Id == oldRoom.Id).Execute();
                                }
                            }

                            if (oldRoom.Id > 0)
                            {
                                var rrDb = new H_HotelRoomRuleAccess();
                                //价格策略
                                var oldRule = rrDb.Query().Where(rr => rr.HRId == oldRoom.Id && rr.HRROutCode == r.RoomId && rr.HRROutType == 2).FirstOrDefault();
                                if (oldRule == null || oldRule.Id <= 0)
                                {
                                    oldRule = new H_HotelRoomRuleModel
                                    {
                                        Id = 0,
                                        HRROutCode = r.RoomId,
                                        HRRXwProductSerial = r.ProductSerial,
                                        HIId = x.Id,
                                        HRId = oldRoom.Id,
                                        HRRAddName = "喜玩新增",
                                        HRRAddTime = DateTime.Now,
                                        HRRBreakfastRule = 0,
                                        HRRBreakfastRuleName = string.Empty,
                                        HRRCancelRule = 0,
                                        HRRCancelRuleName = string.Empty,
                                        HRRIsValid = 1,
                                        HRRName = r.RoomName ?? String.Empty,
                                        HRROutId = 0,
                                        HRROutType = 2,
                                        HRRSourceId = 10103,
                                        HRRSourceName = "代理",
                                        HRRSupplierId = 2,
                                        HRRSupplierName = "喜玩",
                                        HRRUpdateName = string.Empty,
                                        HRRUpdateTime = new DateTime(2000, 1, 1),
                                    };
                                    oldRule.Id = (int)rrDb.Add(oldRule);
                                }
                                else
                                {
                                    var sql = rrDb.Update().Where(rr => rr.Id == oldRule.Id);
                                    sql.Set(rr => rr.HRRXwProductSerial == r.ProductSerial
                                    && rr.HRRUpdateName == "喜玩更新" && rr.HRRIsValid == 1
                                    && rr.HRRUpdateTime == DateTime.Now
                                    ).Execute();
                                }
                                if (oldRule.Id > 0)
                                {
                                    var pDb = new H_HoteRulePriceAccess();
                                    r.Rates?.ForEach(p =>
                                    {
                                        var dateInit = ConvertHelper.ToInt32(p.Date.ToString("yyyyMMdd"), 0);
                                        var price = pDb.Query().Where(pr => pr.HRRId == oldRule.Id && pr.HRPDateInt == dateInit).FirstOrDefault();
                                        if (price == null || price.Id <= 0)
                                        {//新增价格和库存
                                            price = new H_HoteRulePriceModel
                                            {
                                                Id = 0,
                                                HIId = x.Id,
                                                HRId = oldRoom.Id,
                                                HRPAddName = "喜玩新增",
                                                HRPAddTime = DateTime.Now,
                                                HRPContractPrice = p.Price,
                                                HRPDate = p.Date,
                                                HRPCount = p.AvailableNum == 0 && p.Status ? 5 : p.AvailableNum,
                                                HRPDateInt = dateInit,
                                                HRPIsValid = 1,
                                                HRPRetainCount = 0,
                                                HRPSellPrice = p.Price,
                                                HRPStatus = 1,
                                                HRPUpdateName = string.Empty,
                                                HRPUpdateTime = new DateTime(2000, 1, 1),
                                                HRRId = oldRule.Id
                                            };
                                            price.Id = (int)pDb.Add(price);
                                        }
                                        else
                                        {
                                            var sql = pDb.Update().Where(pr => pr.Id == price.Id);
                                            sql.Set(pr => pr.HRPCount == (p.AvailableNum == 0 && p.Status ? 5 : p.AvailableNum));
                                            sql.Set(pr => pr.HRPContractPrice == p.Price
                                            && pr.HRPIsValid == (p.Status ? 1 : 0)
                                            && pr.HRPSellPrice == p.Price
                                            && pr.HRPUpdateName == "喜玩更新"
                                            && pr.HRPUpdateTime == DateTime.Now
                                            ).Execute();
                                        }
                                        OpenApi.AddRuleInfo(x.Id, oldRoom.Id, p.BreakfastNum, price.Id, p.Status ? 1 : 0);
                                        OpenApi.SysInfo(x.Id);
                                    });
                                }
                            }
                        });

                        var outRoomIds = hotel.Rooms.Select(r => r.RoomTypeId.ToInt()).ToList(); //接口返回的房型Id
                        var dbRooms = roomDb.Query().Where(rd => rd.HIId == x.Id).ToList();
                        var errDbRooms = dbRooms.Where(rd => !outRoomIds.Contains(rd.HROutId))?.ToList();
                        errDbRooms?.ForEach(err =>
                        {
                            //更新房型无效
                            roomDb.Update().Set(rr => rr.HRIsValid == 0)
                                   .Where(rr => rr.Id == err.Id).Execute();

                            var pDb = new H_HoteRulePriceAccess();
                            //查询价格Id
                            var errDbPrice = pDb.Query().Where(pr => pr.HRId == err.Id).ToList();
                            //更新库存为0
                            var sql = pDb.Update().Where(pr => pr.HRId == err.Id);
                            sql.Set(pr => pr.HRPCount == 0
                            && pr.HRPIsValid == 0
                            && pr.HRPUpdateName == "喜玩更新"
                            && pr.HRPUpdateTime == DateTime.Now
                            ).Execute();

                            //同步价格策略
                            errDbPrice.ForEach(pr =>
                            {
                                OpenApi.AddRuleInfo(x.Id, err.Id, 0, pr.Id, 0);
                            });
                        });

                        //同步房型
                        OpenApi.AddRoomInfo(x.Id);
                    }
                }
                else
                {//查询不到酒店信息无效
                    hDb.Update().Where(h => h.Id == x.Id)
                       .Set(h => h.HIIsValid == 0 && h.HIUpdateName == "喜玩酒店无信息更新" && h.HIUpdateTime == DateTime.Now)
                       .Execute();
                    OpenApi.HotelOffline(x.Id, 0);
                    rtn.Message += $"[无效]";
                }
            });
            return rtn;
        }

        /// <summary>
        /// 查询库存报价
        /// </summary>
        /// <param name="outId"></param>
        /// <param name="days"></param>
        /// <returns></returns>
        public static XiWanPriceHotel GetHotelPrice(int outId, int days = 7)
        {
            return GetHotelPrice(outId, DateTime.Now.ToString("yyyy-MM-dd"), DateTime.Now.AddDays(days).ToString("yyyy-MM-dd"));
        }

        /// <summary>
        /// 查询库存报价
        /// </summary>
        /// <param name="outId"></param>
        /// <param name="commeDate"></param>
        /// <param name="leaveDate"></param>
        /// <returns></returns>
        public static XiWanPriceHotel GetHotelPrice(int outId, string commeDate, string leaveDate)
        {
            var request = new XiWanPriceRequest
            {
                HotelId = outId,
                ComeDate = commeDate,
                LeaveDate = leaveDate,
            };
            var rtn = XiWanAPI.XiWanPost<XiWanPriceHotel, XiWanPriceRequest>(request, HotelDPriceUrl);
            var hotel = rtn?.Result;
            return hotel;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="remark"></param>
        /// <returns></returns>
        public static int GetBedType(string roomName, int bedType)
        {
            /// 大床 = 0, 双床 = 1, 大床或双床 = 2, 三床 = 3, 四床 = 4, 单人床 = 5,
            /// 上下铺 = 6, 通铺 = 7, 榻榻米 = 8, 水床 = 9, 圆床 = 10, 拼床 = 11, 子母床 = 12,
            /// 多床 = 13, 其他床型 = 999

            var type = 0;

            if (roomName.Contains("大床"))
            {
                type = 50101;
            }
            else if (roomName.Contains("双床") || roomName.Contains("标间"))
            {
                type = 50102;
            }
            else if (roomName.Contains("三人"))
            {
                type = 50103;
            }
            else
            {
                switch (bedType)
                {
                    case 0:
                        type = 50101;
                        break;
                    case 1:
                        type = 50102;
                        break;
                    case 2:
                        type = 50105;
                        break;
                    case 3:
                        type = 50103;
                        break;

                }
            }

            return type;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="remark"></param>
        /// <returns></returns>
        public static int GetBedSize(string roomName, int bedType)
        {
            /// 大床 = 0, 双床 = 1, 大床或双床 = 2, 三床 = 3, 四床 = 4, 单人床 = 5,
            /// 上下铺 = 6, 通铺 = 7, 榻榻米 = 8, 水床 = 9, 圆床 = 10, 拼床 = 11, 子母床 = 12,
            /// 多床 = 13, 其他床型 = 999

            var type = GetBedType(roomName, bedType);
            var size = 50203;//默认1.8
            switch (type)
            {
                case 50101:
                    size = 50203;//1.8
                    break;
                case 50102:
                    size = 50202;//1.5
                    break;
                case 50105:
                    size = 50203;
                    break;
                case 50103:
                    size = 50202;
                    break;
            }

            return size;
        }


        /// <summary>
        /// 清理价格
        /// </summary>
        public static string DeleteOldPrice(int days)
        {
            days = days >= 0 ? -3 : days;//不允许大于0
            var db = new H_HoteRulePriceAccess();
            var count = db.Delete().Where(x => x.HRPDate <= DateTime.Now.AddDays(days).Date).Top(10000).Execute();
            var msg = $"删除{days}天前的数据{count}条";
            LogHelper.Info(msg, "清理");
            return msg;
        }

    }

    public class XiWanHotelRequest
    {

    }

    /// <summary>
    /// 喜玩酒店
    /// </summary>
    public class XiWanHotelList : XiWanPageInfo
    {
        /// <summary>
        /// 酒店
        /// </summary>
        public List<XiWanHotelInfo> HotelList { get; set; }

    }

    /// <summary>
    /// 喜玩酒店
    /// </summary>
    public class XiWanHotelInfo
    {
        /// <summary>
        /// id
        /// </summary>
        public int HotelId { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string HotelName { get; set; }

    }

    /// <summary>
    /// 喜玩酒店详情
    /// </summary>
    public class XiWanHotelDetail
    {
        /// <summary> 酒店id </summary>
        public int HotelId { get; set; }
        /// <summary> 酒店名称 </summary>
        public string HotelName { get; set; }
        /// <summary> 酒店地址 </summary>
        public string Address { get; set; }
        /// <summary> 城市编号 </summary>
        public string CityCode { get; set; }
        /// <summary> 城市名称 </summary>
        public string CityName { get; set; }
        /// <summary> 酒店简介 </summary>
        public string Intro { get; set; }
        /// <summary> 酒店电话 </summary>
        public string Tel { get; set; }
        /// <summary> 酒店百度地图坐标 </summary>
        public string Position { get; set; }

        //        酒店id HotelId int 是       酒店id
        //酒店名称    HotelName string 是         酒店名称
        //酒店地址    Address
        //    string 是       酒店地址
        //城市编号    CityCode
        //    string 是       城市编号
        //城市名称    CityName string 是       城市名称
        //酒店简介    Intro string 是       酒店简介
        //酒店电话    Tel string 是       酒店电话
        //酒店坐标    Position string 否       酒店百度地图坐标


    }

    /// <summary>
    /// 价格查询请求
    /// </summary>
    public class XiWanPriceRequest
    {
        //酒店id HotelId int 是	54279	酒店id
        //入住日期    ComeDate DateTime    是	 2017-02-10	  日期格式2017-02-10
        //离店日期 LeaveDate   DateTime 是	2017-02-12	日期格式2017-02-12
        //房型id RoomId  string 否	 93258_937991	  查询具体房型
        //产品类别    ProductSerial string 否	11	当RoomId不为空是此项必填

        /// <summary>  酒店id </summary>
        public int HotelId { get; set; }
        /// <summary> 入住日期  </summary>
        public string ComeDate { get; set; }
        /// <summary> 离店日期  </summary>
        public string LeaveDate { get; set; }
        /// <summary>  房型id </summary>
        public string RoomId { get; set; }
        /// <summary>产品类别 当RoomId不为空是此项必填  </summary>
        public string ProductSerial { get; set; }

    }

    /// <summary>
    /// 价格查询响应
    /// </summary>
    public class XiWanPriceHotel
    {
        /// <summary>  酒店id </summary>
        public int HotelId { get; set; }
        /// <summary>  酒店名称 </summary>
        public string HotelName { get; set; }
        /// <summary>   </summary>
        public string Address { get; set; }

        /// <summary>   </summary>
        public string CityCode { get; set; }
        /// <summary>  酒店备注信息 </summary>
        public string CityName { get; set; }
        /// <summary>   </summary>
        public string Remark { get; set; }

        /// <summary>  房型列表 </summary>
        public List<XiWanPriceRoom> Rooms { get; set; }

    }

    public class XiWanPriceRoom
    {
        public string RoomId { get; set; }
        public string RoomTypeId { get; set; }

        public string RoomName { get; set; }
        /// <summary>
        /// 房型描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        ///  价格计划名称
        /// </summary>
        public string RatePlanName { get; set; }
        /// <summary>
        /// 查询日期内最少早餐，存在每天早餐不一样,-1为含早但份数不确定
        /// </summary>
        public int BreakfastNum { get; set; }
        /// <summary>
        /// 大床 = 0, 双床 = 1, 大床或双床 = 2, 三床 = 3, 四床 = 4, 单人床 = 5,
        /// 上下铺 = 6, 通铺 = 7, 榻榻米 = 8, 水床 = 9, 圆床 = 10, 拼床 = 11, 子母床 = 12,
        /// 多床 = 13, 其他床型 = 999
        /// </summary>
        public int BedType { get; set; }
        /// <summary>
        /// 网络 未知 = 0,无 = 1,免费 = 2,收费 = 3,     部分收费 = 4
        /// </summary>
        public int Net { get; set; }
        /// <summary>
        /// wifi 未知 = 0,有 = 1,无 = 2,
        /// </summary>
        public int Wifi { get; set; }
        /// <summary>
        /// 预定状态
        /// </summary>
        public bool Status { get; set; }
        /// <summary>
        /// 未知 = 0,有窗 = 1,无窗 = 2,部分有窗 = 3,内窗 = 4
        /// </summary>
        public int Window { get; set; }
        /// <summary>
        /// 现预付 0 预付 1 现付
        /// </summary>
        public int PaymentType { get; set; }
        /// <summary>
        /// 产品类别
        /// </summary>
        public int ProductSerial { get; set; }
        /// <summary>
        /// 适用客人类型  GuestType Enum    是 所有 = 0, 内宾 = 1, 外宾 = 2
        /// </summary>
        public int GuestType { get; set; }
        /// <summary>
        /// 提前预订天数
        /// </summary>
        public int PreBookDay { get; set; }
        /// <summary>
        /// 连住天数
        /// </summary>
        public int MinDays { get; set; }
        /// <summary>
        /// 每日报价信息
        /// </summary>
        public List<XiWanPriceResponse> Rates { get; set; }

        //Room内容
        //名称  编码 类型  必填 示例  描述
        //房型id    RoomId string 是       房型id
        //基础房型id  RoomTypeId string 是         基础房型id
        //房型名称    RoomName string 是       房型名称
        //房型描述    Description string 否       房型描述
        //价格计划名称  RatePlanName string 是       价格计划名称
        //早餐数 BreakfastNum int 是       查询日期内最少早餐，存在每天早餐不一样,-1为含早但份数不确定
        //床型  BedType Enum

        //    是 大床 = 0, 双床 = 1, 大床或双床 = 2, 三床 = 3, 四床 = 4, 单人床 = 5, 上下铺 = 6, 通铺 = 7, 榻榻米 = 8, 水床 = 9, 圆床 = 10, 拼床 = 11, 子母床 = 12, 多床 = 13, 其他床型 = 999
        //网络类型 Net Enum 是       未知 = 0,无 = 1,免费 = 2,收费 = 3,
        //        部分收费 = 4
        //Wifi信息 Wifi    Enum 是       未知 = 0,有 = 1,无 = 2,
        //Window信息 Window  Enum 是       未知 = 0,有窗 = 1,无窗 = 2,部分有窗 = 3,内窗 = 4
        //可订状态 Status  bool 是       可订状态，查询日期里全部可订
        //现预付 PaymentType Enum    是 PrePay = 0（暂只有预付）, SelfPay = 1
        //产品类别 ProductSerial   int 是       产品类别
        //适用客人类型  GuestType Enum    是 所有 = 0, 内宾 = 1, 外宾 = 2
        //提前预订天数 PreBookDay
        //int 是       价格计划的预订规则，提前预订天数
        //连住天数    MinDays int 是       价格计划的预订规则，连住天数
        //每日报价信息  Rates NightRate[] 是 每日报价信息



    }

    /// <summary>
    /// 价格
    /// </summary>
    public class XiWanPriceResponse
    {
        /// <summary>
        /// 日期
        /// </summary>
        public DateTime Date { get; set; }
        /// <summary>
        /// 协议价格
        /// </summary>
        public decimal Price { get; set; }
        /// <summary>
        /// 可订状态      True为可订
        /// </summary>
        public bool Status { get; set; }
        /// <summary>
        /// 参考库存数，暂无效，为0
        /// </summary>
        public int AvailableNum { get; set; }
        /// <summary>
        ///  当天的早餐份数，-1为含早但份数不确定，-99表示此日期的的早餐无效，并以Room的早餐数为准
        /// </summary>
        public int BreakfastNum { get; set; }

        //NightRate内容
        //名称  编码 类型  必填 示例  描述
        //日期  Date DateTime    是 格式yyyy-MM-dd
        //协议价格    Price decimal 是         协议价格
        //可订状态    Status bool 是       True为可订
        //参考库存数   AvailableNum int 是       参考库存数，暂无效，为0
        //早餐数 BreakfastNum int 是       当天的早餐份数，-1为含早但份数不确定，-99表示此日期的的早餐无效，并以Room的早餐数为准

    }


    public class XiWanOrderResponse
    {
        /// <summary>
        /// 订单号
        /// </summary>
        public string OrderNo { get; set; }
    }

    public class XiWanOrderQueryResponse
    {
        /// <summary>
        /// 状态
        /// </summary>
        public int Status { get; set; }
    }

    public class XiWanCancelOrderResponse
    {

    }
}




