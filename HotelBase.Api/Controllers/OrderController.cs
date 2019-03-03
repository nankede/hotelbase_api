using HotelBase.Api.Entity.CommonModel;
using System;
using System.Configuration;
using System.Web.Script.Serialization;
using System.IO;
using System.Web;
using System.Web.Http;
using Newtonsoft.Json;
using HotelBase.Api.Entity.Request.Order;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using HotelBase.Api.Common.SignMothed;
using HotelBase.Api.Common;
using HotelBase.Api.Entity.Response.Order;
using Newtonsoft.Json.Linq;
using HotelBase.Api.Entity.Tables;
using HotelBase.Api.Service;
using HotelBase.Api.Common.Extension;
using HotelBase.Api.Entity.CommonModel.Enum;

namespace HotelBase.Api.Controllers
{
    /// <summary>
    /// 亚朵订单
    /// </summary>
    public class OrderController : ApiController
    {
        private string AtourAuth_URL = ConfigurationManager.AppSettings["OpenApiTest"];
        private string AtourAuth_APPID = ConfigurationManager.AppSettings["appidtest"];


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
                if (string.IsNullOrWhiteSpace(jsonvalue))
                {
                    JavaScriptSerializer js = new JavaScriptSerializer() { MaxJsonLength = Int32.MaxValue };
                    var item = js.Deserialize<AtourCreateOrderRequest>(jsonvalue);
                    if (item != null)
                    {
                        Dictionary<string, string> dic = new Dictionary<string, string>();
                        dic.Add("appId", AtourAuth_APPID);
                        //foreach (var item in entity)
                        //{
                        Type entityType = item.order.GetType();
                        PropertyInfo[] properties = entityType.GetProperties();
                        for (int i = 0; i < properties.Length; i++)
                        {
                            if (!string.IsNullOrWhiteSpace(properties[i].GetValue(item.order, null).ToString()))
                            {
                                dic.Add(properties[i].Name, properties[i].GetValue(item.order, null).ToString());
                            }
                        }
                        var sign = AtourSignUtil.GetSignUtil(dic);
                        var orderrequest = new OrderRequest
                        {
                            appId = AtourAuth_APPID,
                            sign = sign,
                            hotelId = item.order.hotelId,
                            mebId = item.order.hotelId,
                            roomTypeId = item.order.hotelId,
                            roomNum = item.order.hotelId,
                            roomRateList = item.order.roomRateList,
                            arrival = item.order.arrival,
                            assureTime = item.order.assureTime,
                            departure = item.order.departure,
                            mobile = item.order.mobile,
                            contactName = item.order.contactName,
                            guestName = item.order.guestName,
                            source = 10,
                            subSource = 85,
                            roomRateTypeId = item.order.roomRateTypeId,
                            thirdOrderNo = item.order.thirdOrderNo,
                            couponsList = item.order.couponsList,
                            remark = item.order.remark,
                        };
                        var url = AtourAuth_URL + "baoku/order/createOrder";
                        var orderresponse = ApiHelper.HttpPost(url, JsonConvert.SerializeObject(orderrequest), "application/x-www-form-urlencoded");
                        if (!string.IsNullOrWhiteSpace(orderresponse))
                        {
                            var data = JsonConvert.DeserializeObject<JObject>(orderresponse);
                            if (data["msg"].ToString().ToLower() == "success")
                            {
                                var serialid = data["msg"]["atourOrderNo"].ToString();

                                result.Code = DataResultType.Sucess;
                                result.Data = new { Result = serialid };
                                #region 操作订单
                                //var logmodel = new HO_HotelOrderLogModel();
                                ////日志
                                //logmodel.HOLAddId = 0;
                                //logmodel.HOLAddName = "系统";
                                //logmodel.HOLLogType = 1;
                                //logmodel.HOLAddDepartId = 0;
                                //logmodel.HOLOrderId = item.qlorderserialid;
                                ////手动录单更新订单
                                //if (!string.IsNullOrWhiteSpace(item.qlorderserialid))
                                //{

                                //    logmodel.HOLRemark = "更新供应商订单号：" + serialid + ";分销商订单号：" + item.disserialid;
                                //    OrderBll.UpdatesSupplierSerialid(item.qlorderserialid, serialid, item.disserialid);
                                //}
                                ////新增订单
                                //else
                                //{
                                //    var newmodel = new HO_HotelOrderModel
                                //    {
                                //        HOCustomerSerialId = ExtOrderNum.Gener("Z", 1),
                                //        HIId = orderrequest.hotelId,
                                //        HName = hotelmodel.HotelName,
                                //        HRId = hotelmodel.HotelRoomId,
                                //        HRName = hotelmodel.HotelRoomName,
                                //        HRRId = hotelmodel.HotelRoomRuleId,
                                //        HRRName = hotelmodel.HotelRoomRuleName,
                                //        HOSupplierId = hotelmodel.HotelSupplierId,
                                //        HOSupperlierName = hotelmodel.HotelSupplierName,
                                //        HODistributorId = ordermodel.HODistributorId,
                                //        HODistributorName = ordermodel.HODistributorName ?? string.Empty,
                                //        HOSupplierSourceId = hotelmodel.HotelSupplierSourceId,
                                //        HOSupplierSourceName = hotelmodel.HotelSupplierSourceName,
                                //        HODistributorSerialId = ordermodel.HODistributorSerialId ?? string.Empty,
                                //        HOOutSerialId = ordermodel.HOOutSerialId ?? string.Empty,
                                //        HOSupplierSerialId = ordermodel.HOSupplierSerialId ?? string.Empty,
                                //        HOSupplierCorfirmSerialId = ordermodel.HOSupplierCorfirmSerialId ?? string.Empty,
                                //        HOStatus = 0,
                                //        HOPayStatus = 0,
                                //        HORoomCount = ordermodel.HORoomCount,
                                //        HONight = ordermodel.HONight,
                                //        HOChild = ordermodel.HOChild,
                                //        HOAdult = ordermodel.HOAdult,
                                //        HoPlat1 = ordermodel.HoPlat1,
                                //        HoPlat2 = ordermodel.HoPlat2,
                                //        HOContractPrice = ordermodel.HOContractPrice,
                                //        HOSellPrice = ordermodel.HOSellPrice,
                                //        HOCustomerName = ordermodel.HOCustomerName,
                                //        HOCustomerMobile = ordermodel.HOCustomerMobile,
                                //        HOLinkerName = ordermodel.HOLinkerName,
                                //        HOLinkerMobile = ordermodel.HOLinkerMobile,
                                //        HOCheckInDate = ordermodel.HOCheckInDate,
                                //        HOCheckOutDate = ordermodel.HOCheckOutDate,
                                //        HOLastCheckInTime = ordermodel.HOLastCheckInTime,
                                //        HOAddId = CurrtUser.Id,
                                //        HOAddName = CurrtUser.Name,
                                //        HOAddDepartId = CurrtUser.DepartId,
                                //        HOAddDepartName = CurrtUser.DepartName,
                                //        HOAddTime = DateTime.Now,
                                //        HORemark = ordermodel.HORemark,
                                //        HOUpdateId = ordermodel.HOUpdateId,
                                //        HOUpdateName = ordermodel.HOUpdateName,
                                //        HOUpdateTime = DateTime.MinValue
                                //    };

                                //    logmodel.HOLRemark = "创建订单：" + newmodel.HOCustomerSerialId;
                                //}
                                #endregion
                            }
                            else
                            {
                                result.Code = DataResultType.Fail;
                            }
                        }
                        //}
                        return result;
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