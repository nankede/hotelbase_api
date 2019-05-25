using HotelBase.Api.Entity.CommonModel;
using System;
using System.Configuration;
using System.Web.Script.Serialization;
using System.IO;
using System.Web;
using System.Web.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using HotelBase.Api.Entity.Request.Order;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using HotelBase.Api.Common.SignMothed;
using HotelBase.Api.Common;
using HotelBase.Api.Entity.Response.Order;
using HotelBase.Api.Entity.Tables;
using HotelBase.Api.Service;
using HotelBase.Api.Common.Extension;
using HotelBase.Api.Entity.CommonModel.Enum;
using System.Text;
using HotelBase.Api.DataAccess.Resource;
using HotelBase.Api.Entity.Models;

namespace HotelBase.Api.Controllers
{
    /// <summary>
    /// 亚朵订单
    /// </summary>
    public class OrderController : ApiController
    {
        private string AtourAuth_URL = ConfigurationManager.AppSettings["OpenApiTest"];
        private string AtourAuth_APPID = ConfigurationManager.AppSettings["appidtest"];
        private string AtourAuth_MebId = ConfigurationManager.AppSettings["mebidtest"];

        private static string HotelOrderUrl = $"http://{XiWanConst.XiWan_Url}/hotelapi/order/SubmitOrder.ashx";
        private static string HotelDPriceUrl = $"http://{XiWanConst.XiWan_Url}/hotelapi/hotel/GetHotel.ashx";


        /// <summary>
        /// 创建订单
        /// </summary>
        /// <returns></returns>
        public DataResult CreateAtourOrder([FromBody] string jsonvalue, string orderseridid = "")
        {
            var result = new DataResult();
            var createrequset = new CreateRequset();
            bool issned = false;
            try
            {
                if (string.IsNullOrWhiteSpace(jsonvalue) && string.IsNullOrWhiteSpace(orderseridid))
                {
                    return result;
                }
                else if (!string.IsNullOrWhiteSpace(jsonvalue))
                {
                    using (StreamReader sr = new StreamReader(HttpContext.Current.Request.GetBufferedInputStream()))
                    {
                        sr.ReadToEnd();
                    }

                    using (StreamReader sr = new StreamReader(HttpContext.Current.Request.InputStream))
                    {
                        jsonvalue = sr.ReadToEnd();
                        JavaScriptSerializer js = new JavaScriptSerializer() { MaxJsonLength = Int32.MaxValue };
                        createrequset = js.Deserialize<CreateRequset>(jsonvalue);
                        var item = createrequset.orderModel;
                        //需要推送的订单
                        if (createrequset.supplierSourceId == 1 || createrequset.supplierSourceId == 2)//1 亚朵 2喜玩
                        {
                            issned = true;
                        }
                        var hotelinfo = OrderBll.GetSupplierHotelList(item.roomTypeId);
                        if (hotelinfo != null)
                        {
                            #region 
                            var newmodel = new HO_HotelOrderModel
                            {
                                HOCustomerSerialId = ExtOrderNum.Gener("Z", 1),
                                HIId = hotelinfo.HotelId,
                                HName = hotelinfo.HotelName,
                                HRId = hotelinfo.HotelRoomId,
                                HRName = hotelinfo.HotelRoomName,
                                HRRId = hotelinfo.HotelRoomRuleId,
                                HRRName = hotelinfo.HotelRoomRuleName,
                                HOSupplierId = hotelinfo.HotelSupplierId,
                                HOSupperlierName = hotelinfo.HotelSupplierName,
                                HODistributorId = createrequset.distributorSourceId,
                                HODistributorName = createrequset.distributorSource,
                                HOSupplierSourceId = hotelinfo.HotelSupplierSourceId,
                                HOSupplierSourceName = hotelinfo.HotelSupplierSourceName,
                                HODistributorSerialId = item.thirdOrderNo,
                                HORoomCount = item.roomNum,
                                HONight = item.roomNum,
                                HOLinkerName = item.contactName,
                                HOCustomerName = item.guestName,
                                HOContractPrice = Convert.ToDecimal(item.basePrice),
                                HOSellPrice = Convert.ToDecimal(item.roomPrice),
                                HOCheckInDate = Convert.ToDateTime(item.arrival),
                                HOCheckOutDate = Convert.ToDateTime(item.departure),
                                HOLastCheckInTime = item.assureTime,
                                HOAddId = 0,
                                HOAddName = "system",
                                HOAddDepartId = 0,
                                HOAddDepartName = "system",
                                HOAddTime = DateTime.Now,
                                HORemark = item.remark,
                                HOUpdateId = 0,
                                HOUpdateName = "系统",
                                HOUpdateTime = DateTime.MinValue
                            };
                            //日志
                            var logmodel = new HO_HotelOrderLogModel
                            {
                                HOLOrderId = newmodel.HOCustomerSerialId,
                                HOLLogType = 1,//订单日志
                                HOLRemark = "创建订单：" + newmodel.HOCustomerSerialId,
                                HOLAddId = 0,
                                HOLAddName = "系统",
                                HOLAddDepartId = 0,
                                HOLAddDepartName = "系统",
                                HOLAddTime = DateTime.Now
                            };
                            OrderLogBll.AddOrderModel(logmodel);
                            var response = OrderBll.AddOrderModel(newmodel);
                            orderseridid = newmodel.HOCustomerSerialId;
                            #endregion
                        }
                    }
                }
                else if (!string.IsNullOrWhiteSpace(orderseridid))
                {
                    var order = OrderBll.GetModel(orderseridid);
                    if (order.Id > 0)
                    {
                        issned = true;
                        var search = new OrderPriceSearchRequest
                        {
                            HotelId = order.HIId,
                            RoomId = order.HRId,
                            RuleId = order.HRRId,
                            BDate = order.HOCheckInDate,
                            EDate = order.HOCheckOutDate
                        };
                        var roomRateList = new List<RateList>();
                        var pricelist = HotelPriceBll.GetOrderList(search);
                        if (pricelist != null && pricelist.Any())
                        {
                            foreach (var price in pricelist)
                            {
                                var ite = new RateList
                                {
                                    accDate = price.PriceDate,
                                    roomRate = price.ContractPrice
                                };
                                roomRateList.Add(ite);
                            }
                        }
                        createrequset.supplierSourceId = 2;
                        createrequset.orderModel = new OrderModel
                        {
                            hotelId = Convert.ToInt32(order.OutHotelId),
                            mebId = 0,
                            roomTypeId = Convert.ToInt32(order.OutRoomId),
                            roomNum = order.HORoomCount,
                            roomRateList = roomRateList,
                            arrival = order.HOCheckInDate.ToString("yyyy-MM-dd"),
                            assureTime = "",
                            departure = order.HOCheckOutDate.ToString("yyyy-MM-dd"),
                            mobile = order.HOLinkerMobile,
                            contactName = order.HOLinkerName,
                            guestName = order.HOCustomerName,
                            source = 0,
                            subSource = 0,
                            roomRateTypeId = 28,
                            thirdOrderNo = order.HODistributorSerialId,
                            basePrice = order.HOContractPrice.ToString(),
                            roomPrice = order.HOSellPrice.ToString()
                        };
                    }
                }
                if (issned)
                {
                    switch (createrequset.supplierSourceId)
                    {
                        case 1:
                            result = AtourOrder(createrequset, orderseridid);
                            break;
                        case 2:
                            result = XiWanOrder(createrequset, orderseridid);
                            break;
                    }
                    return result;
                }
            }
            catch (Exception ex)
            {

            }
            return result;
        }

        /// <summary>
        /// 亚朵订单
        /// </summary>
        /// <param name="createrequset"></param>
        /// <param name="orderseridid"></param>
        /// <returns></returns>
        public DataResult AtourOrder(CreateRequset createrequset,string orderseridid)
        {
            var result = new DataResult();
            var item = createrequset.orderModel;
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("appId", AtourAuth_APPID);
            Type entityType = item.GetType();
            PropertyInfo[] properties = entityType.GetProperties();
            for (int i = 0; i < properties.Length; i++)
            {
                if (!string.IsNullOrWhiteSpace(properties[i].GetValue(item, null).ToString()))
                {
                    if (properties[i].Name == "roomRateList")
                    {
                        dic.Add(properties[i].Name, JsonConvert.SerializeObject(properties[i].GetValue(item, null).ToString()));
                    }
                    else
                    {
                        dic.Add(properties[i].Name, properties[i].GetValue(item, null).ToString());
                    }
                }
            }
            dic.Remove("basePrice");
            dic.Remove("roomPrice");
            var sign = AtourSignUtil.GetSignUtil(dic);
            var orderrequest = new OrderRequest
            {
                appId = AtourAuth_APPID,
                sign = sign,
                hotelId = item.hotelId,
                mebId = Convert.ToInt32(AtourAuth_MebId),
                roomTypeId = item.roomTypeId,
                roomNum = item.roomNum,
                roomRateList = JsonConvert.SerializeObject(item.roomRateList),
                arrival = item.arrival,
                assureTime = item.arrival,
                departure = item.departure,
                mobile = "18962529773",
                contactName = item.contactName,
                guestName = item.guestName,
                source = 10,
                subSource = 85,
                roomRateTypeId = item.roomRateTypeId,
                thirdOrderNo = item.thirdOrderNo,
                couponsList = item.couponsList,
                remark = item.remark,
            };
            var url = AtourAuth_URL + "baoku/order/createOrder";
            StringBuilder parm = new StringBuilder();
            PropertyInfo[] request = orderrequest.GetType().GetProperties();
            int j = 0;
            foreach (var t in request)
            {
                if (j > 0) parm.Append("&");
                parm.AppendFormat("{0}={1}", t.Name, t.GetValue(orderrequest, null));
                j++;
            }
            var orderresponse = ApiHelper.HttpPost(url, parm.ToString(), "application/x-www-form-urlencoded");
            if (!string.IsNullOrWhiteSpace(orderresponse))
            {
                var data = JsonConvert.DeserializeObject<JObject>(orderresponse);
                if (data["msg"].ToString().ToLower() == "success")
                {
                    var serialid = data["msg"]["atourOrderNo"].ToString();

                    result.Code = DataResultType.Sucess;
                    result.Data = Encrypt.DESEncrypt(serialid);
                    OrderBll.UpdatesSupplier(orderseridid, serialid);
                }
                else
                {
                    result.Code = DataResultType.Fail;
                    result.Message = data["msg"].ToString();
                }
            }
            return result;
        }


        /// <summary>
        /// 喜玩订单
        /// </summary>
        /// <param name="createrequset"></param>
        /// <param name="orderseriald"></param>
        /// <returns></returns>
        public DataResult XiWanOrder(CreateRequset createrequset, string orderseriald)
        {
            var result = new DataResult();
            var item = createrequset.orderModel;
            var request = new XiWanOrderRequest();
            request.DistributeOrderNo = !string.IsNullOrWhiteSpace(item.thirdOrderNo) ? item.thirdOrderNo : orderseriald;
            request.HotelId = item.hotelId;
            request.RoomId = item.roomTypeId.ToString();
            request.RoomName = GetXiWanRoomName(item.roomTypeId.ToString());
            request.ProductSerial = 10;
            request.ComeDate = Convert.ToDateTime(item.arrival).Date;
            request.RoomNum = item.roomNum;
            request.LeaveDate = Convert.ToDateTime(item.departure).Date;
            request.LastArriveTime = "18:00";
            request.TotalPrice = Convert.ToDecimal(item.basePrice);
            request.ContactName = item.contactName;
            request.GuestNames = item.guestName.Split(',');
            request.NoteToHotel = !string.IsNullOrWhiteSpace(item.remark) ? item.remark : "无";
            var rtn = XiWanAPI.XiWanPost<XiWanOrderResponse, XiWanOrderRequest>(request, HotelOrderUrl);
            var order = rtn?.Result;
            if (!string.IsNullOrWhiteSpace(order.OrderNo))
            {
                result.Code = DataResultType.Sucess;
                result.Data = Encrypt.DESEncrypt(order.OrderNo);
                OrderBll.UpdatesSupplier(orderseriald, order.OrderNo);

            }
            else
            {
                result.Code = DataResultType.Fail;
                result.Message = rtn?.Msg;
            }
            return result;
        }


        public DataResult Test(int outid)
        {
            var result = new DataResult();
            var request = new XiWanPriceRequest
            {
                HotelId = outid,
                ComeDate = DateTime.Now.ToString("yyyy-MM-dd"),
                LeaveDate = DateTime.Now.AddDays(7).ToString("yyyy-MM-dd"),
            };
            var rtn = XiWanAPI.XiWanPost<XiWanPriceHotel, XiWanPriceRequest>(request, HotelDPriceUrl);
            var hotel = rtn?.Result;
            return result;
        }

        /// <summary>
        /// 获取名称
        /// </summary>
        /// <param name="roomid"></param>
        /// <returns></returns>
        public string GetRoomName(string roomid)
        {
            var roomDb = new H_HotelRoomRuleAccess();
            var oldRoom = roomDb.Query().Where(rd => rd.HRROutCode == roomid).FirstOrDefault();
            return oldRoom.HRRName;
        }

        /// <summary>
        /// 获取名称
        /// </summary>
        /// <param name="roomid"></param>
        /// <returns></returns>
        public string GetXiWanRoomName(string roomid)
        {
            var roomDb = new H_HotelRoomAccess();
            var oldRoom = roomDb.Query().Where(rd => rd.HROutId == Convert.ToInt32(roomid)).FirstOrDefault();
            return oldRoom.HRName;
        }

        /// <summary>
        /// 操作订单
        /// </summary>
        /// <remarks>
        /// des加密，key单独提供
        /// </remarks>
        /// <param name="orderid">加密过的orderid</param>
        /// <returns></returns>
        public DataResult OperatAtourOrder(string orderid)
        {
            var result = new DataResult();
            try
            {
                //解密
                var escorderid = Encrypt.DESDecrypt(orderid);
                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add("atourOrderNo", escorderid);
                dic.Add("appId", AtourAuth_APPID);
                var sign = AtourSignUtil.GetSignUtil(dic);
                var url = AtourAuth_URL + "baoku/order/cancelOrder";
                var orderrequest = new
                {
                    atourOrderNo = escorderid,
                    appId = AtourAuth_APPID,
                    sign = sign
                };
                var orderresponse = ApiHelper.HttpPost(url, JsonConvert.SerializeObject(orderrequest), "application/x-www-form-urlencoded");
                if (!string.IsNullOrWhiteSpace(orderresponse))
                {
                    var data = JsonConvert.DeserializeObject<JObject>(orderresponse);
                    if (data["msg"].ToString().ToLower() == "success")
                    {
                        var inresult = data["result"].ToString();

                        result.Code = DataResultType.Sucess;
                        result.Data = inresult;
                    }
                    else
                    {
                        result.Code = DataResultType.Fail;
                    }
                }

            }
            catch (Exception ex)
            {

            }
            return result;
        }
    }
}