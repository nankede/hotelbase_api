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
                if (!string.IsNullOrWhiteSpace(jsonvalue))
                {
                    JavaScriptSerializer js = new JavaScriptSerializer() { MaxJsonLength = Int32.MaxValue };
                    var item = js.Deserialize<OrderModel>(jsonvalue);
                    if (item != null)
                    {
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
                        var sign = AtourSignUtil.GetSignUtil(dic);
                        var orderrequest = new OrderRequest
                        {
                            appId = AtourAuth_APPID,
                            sign = sign,
                            hotelId = item.hotelId,
                            mebId = item.mebId,
                            roomTypeId = item.roomTypeId,
                            roomNum = item.roomNum,
                            roomRateList = JsonConvert.SerializeObject(item.roomRateList),
                            arrival = item.arrival,
                            assureTime = item.assureTime,
                            departure = item.departure,
                            mobile = item.mobile,
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
                        string parm = "";
                        PropertyInfo[] request = orderrequest.GetType().GetProperties();
                        for (int i = 0; i < request.Length; i++)
                        {
                            parm += request[i].Name + "=" + request[i].GetValue(orderrequest, null).ToString() + "&";

                        }
                        parm = parm.Substring(0, parm.Length - 1);
                        var orderresponse = ApiHelper.HttpPost(url, parm, "application/x-www-form-urlencoded");
                        if (!string.IsNullOrWhiteSpace(orderresponse))
                        {
                            var data = JsonConvert.DeserializeObject<JObject>(orderresponse);
                            if (data["msg"].ToString().ToLower() == "success")
                            {
                                var serialid = data["msg"]["atourOrderNo"].ToString();

                                result.Code = DataResultType.Sucess;
                                result.Data = Encrypt.DESEncrypt(serialid);
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
                                result.Message = data["msg"].ToString();
                            }
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