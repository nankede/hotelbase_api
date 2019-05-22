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


        /// <summary>
        /// 创建订单
        /// </summary>
        /// <returns></returns>
        public DataResult CreateAtourOrder([FromBody] string jsonvalue)
        {
            var result = new DataResult();
            try
            {
                using (StreamReader sr = new StreamReader(HttpContext.Current.Request.GetBufferedInputStream()))
                {
                    sr.ReadToEnd();
                }

                using (StreamReader sr = new StreamReader(HttpContext.Current.Request.InputStream))
                {
                    jsonvalue = sr.ReadToEnd();
                }
                if (!string.IsNullOrWhiteSpace(jsonvalue))
                {
                    bool issned = false;
                    JavaScriptSerializer js = new JavaScriptSerializer() { MaxJsonLength = Int32.MaxValue };
                    var createrequset = js.Deserialize<CreateRequset>(jsonvalue);
                    if (createrequset != null && createrequset.orderModel != null)
                    {
                        var item = createrequset.orderModel;



                        #region 创建订单
                        var hotelinfo = OrderBll.GetHotelRuleList(item.hotelId, item.roomTypeId);
                        if (hotelinfo != null)
                        {
                            var newmodel = new HO_HotelOrderModel
                            {
                                HOCustomerSerialId = ExtOrderNum.Gener("Z", 1),
                                HIId = item.hotelId,
                                HName = hotelinfo.HotelName,
                                HRId = hotelinfo.HotelRoomId,
                                HRName = hotelinfo.HotelRoomName,
                                HRRId = hotelinfo.HotelRoomRuleId,
                                HRRName = hotelinfo.HotelRoomRuleName,
                                HOSupplierId = hotelinfo.HotelSupplierId,
                                HOSupperlierName = hotelinfo.HotelSupplierName,
                                HODistributorId = createrequset.distributorSourceId,
                                HODistributorName = createrequset.distributorSource,
                                HOSupplierSourceId = createrequset.supplierSourceId,
                                HOSupplierSourceName = createrequset.supplierSource,
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
                                HOAddName = "系统-喜玩",
                                HOAddDepartId = 0,
                                HOAddDepartName = "系统-喜玩",
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
                            result.Code = DataResultType.Sucess;
                            result.Data = newmodel.HOCustomerSerialId;
                        }
                        #endregion








                        //需要推送的订单
                        if (createrequset.supplierSourceId == 1 || createrequset.supplierSourceId == 2)//1 亚朵 2喜玩
                        {
                            issned = true;
                        }
                        if (issned)
                        {
                            switch (createrequset.supplierSourceId)
                            {
                                case 1:
                                    result = AtourOrder(createrequset);
                                    break;
                                case 2:
                                    result = XiWanOrder(createrequset);
                                    break;
                            }
                        }
                        else
                        {
                            #region 创建订单
                            var hotelinfo = OrderBll.GetHotelRuleList(item.hotelId, item.roomTypeId);
                            if (hotelinfo != null)
                            {
                                var newmodel = new HO_HotelOrderModel
                                {
                                    HOCustomerSerialId = ExtOrderNum.Gener("Z", 1),
                                    HIId = item.hotelId,
                                    HName = hotelinfo.HotelName,
                                    HRId = hotelinfo.HotelRoomId,
                                    HRName = hotelinfo.HotelRoomName,
                                    HRRId = hotelinfo.HotelRoomRuleId,
                                    HRRName = hotelinfo.HotelRoomRuleName,
                                    HOSupplierId = hotelinfo.HotelSupplierId,
                                    HOSupperlierName = hotelinfo.HotelSupplierName,
                                    HODistributorId = createrequset.distributorSourceId,
                                    HODistributorName = createrequset.distributorSource,
                                    HOSupplierSourceId = createrequset.supplierSourceId,
                                    HOSupplierSourceName = createrequset.supplierSource,
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
                                    HOAddName = "系统",
                                    HOAddDepartId = 0,
                                    HOAddDepartName = "系统",
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
                                result.Code = DataResultType.Sucess;
                                result.Data = newmodel.HOCustomerSerialId;
                            }
                            #endregion
                        }

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
        /// <returns></returns>
        public DataResult AtourOrder(CreateRequset createrequset)
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
                    #region 创建订单
                    var hotelinfo = OrderBll.GetHotelRuleList(item.hotelId, item.roomTypeId);
                    if (hotelinfo != null)
                    {
                        var newmodel = new HO_HotelOrderModel
                        {
                            HOCustomerSerialId = ExtOrderNum.Gener("Z", 1),
                            HIId = item.hotelId,
                            HName = hotelinfo.HotelName,
                            HRId = hotelinfo.HotelRoomId,
                            HRName = hotelinfo.HotelRoomName,
                            HRRId = hotelinfo.HotelRoomRuleId,
                            HRRName = hotelinfo.HotelRoomRuleName,
                            HOSupplierId = hotelinfo.HotelSupplierId,
                            HOSupperlierName = hotelinfo.HotelSupplierName,
                            HODistributorId = createrequset.distributorSourceId,
                            HODistributorName = createrequset.distributorSource,
                            HOSupplierSourceId = createrequset.supplierSourceId,
                            HOSupplierSourceName = createrequset.supplierSource,
                            HODistributorSerialId = item.thirdOrderNo,
                            HOOutSerialId = serialid,
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
                            HOAddName = "系统",
                            HOAddDepartId = 0,
                            HOAddDepartName = "系统",
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
                        result.Code = DataResultType.Sucess;
                        result.Data = newmodel.HOCustomerSerialId;
                    }
                    #endregion
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
        /// <returns></returns>
        public DataResult XiWanOrder(CreateRequset createrequset)
        {
            var result = new DataResult();
            var item = createrequset.orderModel;
            var request = new XiWanOrderRequest
            {
                DistributeOrderNo = item.thirdOrderNo,
                HotelId = item.hotelId,
                RoomId = item.roomTypeId.ToString(),
                RoomName = GetRoomName(item.roomTypeId.ToString()),
                ProductSerial = 11,
                ComeDate = Convert.ToDateTime(item.arrival),
                LeaveDate = Convert.ToDateTime(item.departure),
                LastArriveTime = "18:00",
                TotalPrice = Convert.ToDecimal(item.basePrice),
                ContactName = item.contactName,
                GuestNames = item.guestName.Split(','),
                NoteToHotel = item.remark
            };
            var rtn = XiWanAPI.XiWanPost<XiWanOrderResponse, XiWanOrderRequest>(request, HotelOrderUrl);
            var order = rtn?.Result;
            if (!string.IsNullOrWhiteSpace(order.OrderNo))
            {
                result.Code = DataResultType.Sucess;
                result.Data = Encrypt.DESEncrypt(order.OrderNo);
                #region 创建订单
                var hotelinfo = OrderBll.GetHotelRuleList(item.hotelId, item.roomTypeId);
                if (hotelinfo != null)
                {
                    var newmodel = new HO_HotelOrderModel
                    {
                        HOCustomerSerialId = ExtOrderNum.Gener("Z", 1),
                        HIId = item.hotelId,
                        HName = hotelinfo.HotelName,
                        HRId = hotelinfo.HotelRoomId,
                        HRName = hotelinfo.HotelRoomName,
                        HRRId = hotelinfo.HotelRoomRuleId,
                        HRRName = hotelinfo.HotelRoomRuleName,
                        HOSupplierId = hotelinfo.HotelSupplierId,
                        HOSupperlierName = hotelinfo.HotelSupplierName,
                        HODistributorId = createrequset.distributorSourceId,
                        HODistributorName = createrequset.distributorSource,
                        HOSupplierSourceId = createrequset.supplierSourceId,
                        HOSupplierSourceName = createrequset.supplierSource,
                        HODistributorSerialId = item.thirdOrderNo,
                        HOOutSerialId = order.OrderNo,
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
                        HOAddName = "系统-喜玩",
                        HOAddDepartId = 0,
                        HOAddDepartName = "系统-喜玩",
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
                    result.Code = DataResultType.Sucess;
                    result.Data = newmodel.HOCustomerSerialId;
                }
                #endregion
            }
            else
            {
                result.Code = DataResultType.Fail;
                result.Message = rtn?.Msg;
            }
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