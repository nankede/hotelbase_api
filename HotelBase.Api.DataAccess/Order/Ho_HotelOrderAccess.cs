﻿using Component.Access;
using Dapper;
using HotelBase.Api.Entity;
using HotelBase.Api.Entity.Models;
using HotelBase.Api.Entity.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBase.Api.DataAccess.Order
{
    public class Ho_HotelOrderAccess : BaseAccess<HO_HotelOrderModel>
    {
        public Ho_HotelOrderAccess() : base(MysqlHelper.Db_HotelBase)
        {

        }
        //private static List<OrderSearchResponse> List = new List<OrderSearchResponse>();

        /// <summary>
        /// 订单列表
        /// </summary>
        public static BasePageResponse<OrderSearchResponse> GetOrderList(OrderSearchRequset request)
        {
            var response = new BasePageResponse<OrderSearchResponse>();
            StringBuilder sb = new StringBuilder();
            sb.Append(@"SELECT
                        Id,
                        HOCustomerSerialId,
                        HOSupplierSourceName,
                        HIId,
                        HName,
                        HOCheckInDate,
                        HOCheckOutDate,
                        HOLinkerName,
                        HOStatus,
                        HOSellPrice,
                        HOContractPrice
                    FROM
                        Ho_HotelOrder
                    WHERE
                        1 = 1");
            //订单号
            if (!string.IsNullOrWhiteSpace(request.HOCustomerSerialId))
            {
                sb.AppendFormat(" AND HOCustomerSerialId Like '%{0}%'", request.HOCustomerSerialId);
            }
            //人员归属查询
            if (!string.IsNullOrWhiteSpace(request.PeopleMobile) || !string.IsNullOrWhiteSpace(request.PeopleName))
            {
                if (request.CustomerType == 1)
                {
                    sb.AppendFormat(" AND HOCustomerName Like '%{0}%' AND HOCustomerMobile = '{1}'", request.PeopleName, request.PeopleMobile);
                }
                else
                {
                    sb.AppendFormat(" AND HOLinkerName Like '%{0}%' AND HOLinkerMobile = '{1}'", request.PeopleName, request.PeopleMobile);
                }
            }
            //时间
            if (!string.IsNullOrWhiteSpace(request.StartTime) || !string.IsNullOrWhiteSpace(request.EndTime))
            {
                if (request.TimeType == 1)
                {
                    sb.AppendFormat(" AND HOCheckInDate >= '{0}' AND HOCheckInDate<'{0}'", request.StartTime, Convert.ToDateTime(request.EndTime).AddDays(1).ToShortDateString());
                }
                else
                {
                    sb.AppendFormat(" AND HOAddTime >= '{0}' AND HOAddTime<'{0}'", request.StartTime, Convert.ToDateTime(request.EndTime).AddDays(1).ToShortDateString());
                }
            }
            //来源
            if (request.SourceId > 0)
            {
                sb.AppendFormat(" AND HOSupplierSourceId = {0}", request.SourceId);
            }
            //酒店名称
            if (!string.IsNullOrWhiteSpace(request.HotelName))
            {
                sb.AppendFormat(" AND HName Like '%{0}%'", request.HotelName);
            }
            //酒店Id
            if (!string.IsNullOrWhiteSpace(request.HIId))
            {
                sb.AppendFormat(" AND HIId = '{0}'", request.HIId);
            }

            //第三方流水
            if (!string.IsNullOrWhiteSpace(request.HOOutSerialId))
            {
                sb.AppendFormat(" AND HOOutSerialId = '{0}'", request.HOOutSerialId);
            }

            //订单状态
            if (request.HOStatus != null)
            {
                string state = "";
                foreach (var item in state)
                {
                    state += "'" + item + "',";
                }
                sb.AppendFormat(" AND HOStatus IN ({0})", state.Substring(0, state.Length - 1));
            }
            var list = MysqlHelper.GetList<OrderSearchResponse>(sb.ToString());
            var total = list?.Count ?? 0;
            if (total > 0)
            {
                response.IsSuccess = 1;
                response.Total = total;
                response.List = list.Skip((request.PageIndex - 1) * request.PageSize).Take(request.PageSize)?.ToList();
            }
            return response;
        }


        /// <summary>
        /// 订单详情
        /// </summary>
        /// <param name="orderid"></param>
        /// <returns></returns>
        public static HO_HotelOrderModel GetModel(int orderid)
        {
            if (orderid <= 0)
            {
                return null;
            }
            var para = new DynamicParameters();
            var sql = "SELECT * FROM ho_hotelorder  WHERE  id=@id  LIMIT 1;   ";
            para.Add("@id", orderid);
            var data = MysqlHelper.GetModel<HO_HotelOrderModel>(sql, para);
            return data;
        }

        /// <summary>
        /// 订单详情
        /// </summary>
        /// <param name="orderid"></param>
        /// <returns></returns>
        public static SeaOrdrModel GetSeaModel(string serialid)
        {
            if (string.IsNullOrWhiteSpace(serialid))
            {
                return null;
            }
            var para = new DynamicParameters();
            var sql = @"SELECT hl.*,hi.HIOutId AS OutHotelId,hr.HROutId AS OutRoomId,hrr.HRROutCode AS OutRoomCode,hrr.HRRXwProductSerial AS OutProductSerial FROM ho_hotelorder hl  
                        inner join h_hotelinfo hi on hl.HIId = hi.Id
                        inner join h_hotelroom hr on hr.Id = hl.HRId
                        inner join h_hotelroomrule hrr on hrr.Id=hl.HRRId
                        WHERE HOCustomerSerialId = @HOCustomerSerialId  LIMIT 1; ";
            para.Add("@HOCustomerSerialId", serialid);
            var data = MysqlHelper.GetModel<SeaOrdrModel>(sql, para);
            return data;
        }


        /// <summary>
        /// 订单详情--by供应商流水号
        /// </summary>
        /// <param name="orderid"></param>
        /// <returns></returns>
        public static SeaOrdrModel GetSeaModelBySupplier(string supplierserialid)
        {
            if (string.IsNullOrWhiteSpace(supplierserialid))
            {
                return null;
            }
            var para = new DynamicParameters();
            var sql = @"SELECT hl.*,hi.HIOutId AS OutHotelId,hr.HROutId AS OutRoomId,hrr.HRROutCode AS OutRoomCode,hrr.HRRXwProductSerial AS OutProductSerial FROM ho_hotelorder hl  
                        inner join h_hotelinfo hi on hl.HIId = hi.Id
                        inner join h_hotelroom hr on hr.Id = hl.HRId
                        inner join h_hotelroomrule hrr on hrr.Id=hl.HRRId
                        WHERE HOSupplierSerialId = @HOSupplierSerialId  LIMIT 1; ";
            para.Add("@HOSupplierSerialId", supplierserialid);
            var data = MysqlHelper.GetModel<SeaOrdrModel>(sql, para);
            return data;
        }


        /// <summary>
        /// 录单页面资源查询
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static BookSearchResponse GetHotelRuleList(int hotelid, int roomid)
        {
            var response = new BookSearchResponse();
            StringBuilder sbwhere = new StringBuilder();
            //酒店id
            sbwhere.AppendFormat(" AND  b.Id = {0}", hotelid);
            //房型id
            sbwhere.AppendFormat(" AND  r.Id = {0}", roomid);
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(@"SELECT
	                        b.Id AS HotelId,
	                        r.Id AS HotelRoomId,
	                        rr.Id AS HotelRoomRuleId,
	                        b.HIName AS HotelName,
	                        b.HIAddress AS HotelAddress,
	                        b.HILinkPhone AS HotelTel,
	                        r.HRName AS HotelRoomName,
	                        rr.HRRName AS HotelRoomRuleName,
	                        r.HRBedType AS HotelRoomBedType,
	                        rr.HRRBreakfastRuleName AS HotelRoomBreakfastRuleName,
	                        rr.HRRCancelRule AS HotelRoomCancelRule,
	                        rr.HRRCancelRuleName AS HotelRoomCancelRuleName,
	                        rr.HRRSourceId AS HotelSupplierSourceId,
	                        rr.HRRSourceName AS HotelSupplierSourceName,
	                        rr.HRRSupplierId AS HotelSupplierId,
	                        rr.HRRSupplierName AS HotelSupplierName
                        FROM
	                        h_hotelroomrule rr
                        INNER JOIN h_hotelroom r ON r.Id = rr.HRId
                        INNER JOIN h_hotelinfo b ON r.HIId = b.Id
                        WHERE
	                        1 = 1 {0}", sbwhere.ToString());

            var list = MysqlHelper.GetList<BookSearchResponse>(sb.ToString());
            return list.FirstOrDefault();
        }

        /// <summary>
        /// 获取供应商房型资源
        /// </summary>
        /// <param name="hotelid"></param>
        /// <param name="roomid"></param>
        public static BookSearchResponse GetSupplierHotelList(int roomid)
        {
            var response = new BookSearchResponse();
            StringBuilder sbwhere = new StringBuilder();
            //房型id
            sbwhere.AppendFormat(" AND  r.Id = {0}", roomid);
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(@"SELECT
	                        b.Id AS HotelId,
	                        r.Id AS HotelRoomId,
	                        rr.Id AS HotelRoomRuleId,
	                        b.HIName AS HotelName,
	                        b.HIAddress AS HotelAddress,
	                        b.HILinkPhone AS HotelTel,
	                        r.HRName AS HotelRoomName,
	                        rr.HRRName AS HotelRoomRuleName,
	                        r.HRBedType AS HotelRoomBedType,
	                        rr.HRRBreakfastRuleName AS HotelRoomBreakfastRuleName,
	                        rr.HRRCancelRule AS HotelRoomCancelRule,
	                        rr.HRRCancelRuleName AS HotelRoomCancelRuleName,
	                        rr.HRRSourceId AS HotelSupplierSourceId,
	                        rr.HRRSourceName AS HotelSupplierSourceName,
	                        rr.HRRSupplierId AS HotelSupplierId,
	                        rr.HRRSupplierName AS HotelSupplierName
                        FROM
	                        h_hotelroomrule rr
                        INNER JOIN h_hotelroom r ON r.Id = rr.HRId
                        INNER JOIN h_hotelinfo b ON r.HIId = b.Id
                        WHERE
	                        1 = 1 {0}", sbwhere.ToString());

            var list = MysqlHelper.GetList<BookSearchResponse>(sb.ToString());
            return list.FirstOrDefault();
        }


        /// <summary>
        /// 获取价格
        /// </summary>
        /// <param name="roomid"></param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static List<H_HoteRulePriceModel> GetHotelPriceList(int roomid, DateTime begin, DateTime end)
        {
            StringBuilder sbwhere = new StringBuilder();
            //房型id
            sbwhere.AppendFormat(" AND  rr.HRRId = {0}", roomid);
            sbwhere.AppendFormat(" AND  rr.HRPDate >= '{0}'", begin);
            sbwhere.AppendFormat(" AND  rr.HRPDate < '{0}'", end);
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(@"SELECT
	                        *
                        FROM
	                        h_hoteruleprice rr
                        WHERE
	                        1 = 1 {0}", sbwhere.ToString());

            var list = MysqlHelper.GetList<H_HoteRulePriceModel>(sb.ToString());
            return list;
        }

        /// <summary>
        /// 录单详情页酒店信息查询
        /// </summary>
        /// <param name="hid"></param>
        /// <param name="roomid"></param>
        /// <param name="ruleid"></param>
        /// <param name="supplierid"></param>
        /// <returns></returns>
        public static BookSearchResponse GetHotelRuleDetial(int hid, int roomid, int ruleid, int supplierid)
        {
            var response = new BookSearchResponse();
            StringBuilder sb = new StringBuilder();
            sb.Append(@"SELECT
	                            b.Id AS HotelId,
	                            r.Id AS HotelRoomId,
	                            rr.Id AS HotelRoomRuleId,
	                            b.HIName AS HotelName,
	                            b.HIAddress AS HotelAddress,
	                            b.HILinkPhone AS HotelTel,
	                            r.HRName AS HotelRoomName,
                                rr.HRRName AS HotelRoomRuleName,
	                            r.HRBedType AS HotelRoomBedType,
	                            rr.HRRBreakfastRuleName AS HotelRoomBreakfastRuleName,
	                            rr.HRRCancelRule AS HotelRoomCancelRule,
	                            rr.HRRCancelRuleName AS HotelRoomCancelRuleName,
	                            rr.HRRSourceId AS HotelSupplierSourceId,
	                            rr.HRRSourceName AS HotelSupplierSourceName,
	                            rr.HRRSupplierId AS HotelSupplierId,
	                            rr.HRRSupplierName AS HotelSupplierName,
	                            rp.HRPSellPrice AS HoteRoomRuleSellPrice,
	                            rp.HRPContractPrice AS HoteRoomRuleContractPrice,
	                            s.SSubWay AS HotelSupplierSubWay,
	                            s.SLinkMail AS HotelSupplierLinkMail
                            FROM
	                            h_hotelinfo b
                            INNER JOIN h_hotelroom r ON r.HIId = b.Id
                            INNER JOIN h_hotelroomrule rr ON r.Id = rr.HRId
                            INNER JOIN h_hoteruleprice rp ON rr.Id = rp.HRRId
                            INNER JOIN h_supplier s ON s.Id = rr.HRRSupplierId
                            WHERE
	                            b.HIIsValid = 1
                            AND r.HRIsValid = 1
                            AND rr.HRRIsValid = 1
                            AND rp.HRPIsValid = 1
                            AND s.SIsValid = 1");
            //酒店id
            if (hid > 0)
            {
                sb.AppendFormat(" AND  b.Id = {0}", hid);
            }
            //房间id
            if (roomid > 0)
            {
                sb.AppendFormat(" AND  r.Id = {0}", roomid);
            }
            //房型id
            if (ruleid > 0)
            {
                sb.AppendFormat(" AND  rr.Id = {0}", ruleid);
            }
            //供应商id
            if (ruleid > 0)
            {
                sb.AppendFormat(" AND  s.Id = {0}", supplierid);
            }
            var list = MysqlHelper.GetList<BookSearchResponse>(sb.ToString());
            if (list != null && list.Any())
            {
                response = list.FirstOrDefault();
            }
            return response;
        }

        /// <summary>
        /// 新增订单
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static int AddOrderModel(HO_HotelOrderModel model)
        {
            var sql = new StringBuilder();
            sql.Append(" INSERT INTO `ho_hotelorder` (`HOCustomerSerialId`, `HIId`, `HName`, `HRId`, `HRName`, `HRRId`, `HRRName`, `HOSupplierId`, `HOSupperlierName`,`HODistributorId`, `HODistributorName`, `HOSupplierSourceId`, `HOSupplierSourceName`, `HOOutSerialId`,`HODistributorSerialId`,`HOSupplierCorfirmSerialId`,`HONight`,`HOSupplierSerialId`, `HOStatus`, `HOPayStatus`, `HORoomCount`, `HOChild`, `HOAdult`, `HoPlat1`, `HoPlat2`, `HOContractPrice`, `HOSellPrice`, `HOCustomerName`, `HOCustomerMobile`, `HOLinkerName`, `HOLinkerMobile`, `HORemark`, `HOCheckInDate`, `HOCheckOutDate`, `HOLastCheckInTime`, `HOAddId`, `HOAddName`, `HOAddDepartId`, `HOAddDepartName`, `HOAddTime`, `HOUpdateId`, `HOUpdateName`, `HOUpdateTime`) VALUES ");
            sql.Append("( @HOCustomerSerialId, @HIId, @HName, @HRId, @HRName, @HRRId, @HRRName, @HOSupplierId, @HOSupperlierName,@HODistributorId, @HODistributorName, @HOSupplierSourceId, @HOSupplierSourceName, @HOOutSerialId, @HODistributorSerialId, @HOSupplierCorfirmSerialId, @HONight, @HOSupplierSerialId, @HOStatus, @HOPayStatus, @HORoomCount, @HOChild, @HOAdult, @HoPlat1, @HoPlat2, @HOContractPrice, @HOSellPrice, @HOCustomerName, @HOCustomerMobile, @HOLinkerName, @HOLinkerMobile, @HORemark, @HOCheckInDate, @HOCheckOutDate, @HOLastCheckInTime, @HOAddId, @HOAddName, @HOAddDepartId, @HOAddDepartName, @HOAddTime, @HOUpdateId, @HOUpdateName, @HOUpdateTime)");
            var para = new DynamicParameters();
            para.Add("@HOCustomerSerialId", model.HOCustomerSerialId);
            para.Add("@HIId", model.HIId);
            para.Add("@HName", model.HName);
            para.Add("@HRId", model.HRId);
            para.Add("@HRName", model.HRName);
            para.Add("@HRRId", model.HRRId);
            para.Add("@HRRName", model.HRRName);
            para.Add("@HOSupplierId", model.HOSupplierId);
            para.Add("@HOSupperlierName", model.HOSupperlierName ?? "");
            para.Add("@HODistributorId", model.HODistributorId);
            para.Add("@HODistributorName", model.HODistributorName ?? "");
            para.Add("@HOSupplierSourceId", model.HOSupplierSourceId);
            para.Add("@HOSupplierSourceName", model.HOSupplierSourceName ?? "");
            para.Add("@HOOutSerialId", model.HOOutSerialId ?? "");
            para.Add("@HODistributorSerialId", model.HODistributorSerialId ?? "");
            para.Add("@HOSupplierCorfirmSerialId", model.HOSupplierCorfirmSerialId ?? "");
            para.Add("@HONight", model.HONight);
            para.Add("@HOSupplierSerialId", model.HOSupplierSerialId ?? "");
            para.Add("@HOStatus", model.HOStatus);
            para.Add("@HOPayStatus", model.HOPayStatus);
            para.Add("@HORoomCount", model.HORoomCount);
            para.Add("@HOChild", model.HOChild);
            para.Add("@HOAdult", model.HOAdult);
            para.Add("@HoPlat1", model.HoPlat1);
            para.Add("@HoPlat2", model.HoPlat2);
            para.Add("@HOContractPrice", model.HOContractPrice);
            para.Add("@HOSellPrice", model.HOSellPrice);
            para.Add("@HOCustomerName", model.HOCustomerName ?? "");
            para.Add("@HOCustomerMobile", model.HOCustomerMobile ?? "");
            para.Add("@HOLinkerName", model.HOLinkerName ?? "");
            para.Add("@HOLinkerMobile", model.HOLinkerMobile ?? "");
            para.Add("@HORemark", model.HORemark ?? "");
            para.Add("@HOCheckInDate", model.HOCheckInDate);
            para.Add("@HOCheckOutDate", model.HOCheckOutDate);
            para.Add("@HOLastCheckInTime", model.HOLastCheckInTime ?? "");
            para.Add("@HOAddId", model.HOAddId);
            para.Add("@HOAddName", model.HOAddName ?? "");
            para.Add("@HOAddDepartId", model.HOAddDepartId);
            para.Add("@HOAddDepartName", model.HOAddDepartName ?? "");
            para.Add("@HOAddTime", model.HOAddTime);
            para.Add("@HOUpdateId", model.HOUpdateId);
            para.Add("@HOUpdateName", model.HOUpdateName ?? "");
            para.Add("@HOUpdateTime ", model.HOUpdateTime);
            var id = MysqlHelper.Insert(sql.ToString(), para);
            //_DicList = null;
            return id;
        }


        /// <summary>
        /// 更新订单
        /// </summary>
        /// <param name="orderid">订单id</param>
        /// <param name="type">修改类型  0：状态  1：第三方流水号  2：供应商流水号</param>
        /// <param name="serialid"></param>
        /// <returns></returns>

        public static int UpdateOrderSerialid(int orderid, int type, int state, string serialid)
        {
            if (orderid == 0) return 0;
            var sql = new StringBuilder();
            sql.Append(" UPDATE `ho_hotelorder` SET ");
            switch (type)
            {
                case 0:
                    if (state > 0)
                    {
                        sql.AppendFormat(" `HOStatus` = {0}  ", state);
                    }
                    break;
                case 1:
                    if (!string.IsNullOrWhiteSpace(serialid))
                    {
                        sql.AppendFormat(" `HOOutSerialId` = {0}  ", serialid);
                    }
                    break;
                case 2:
                    if (!string.IsNullOrWhiteSpace(serialid))
                    {
                        sql.AppendFormat(" `HOSupplierSerialId` = {0}  ", serialid);
                    }
                    break;
            }
            sql.Append(" WHERE  `Id` =@Id   Limit 1;  ");
            var c = MysqlHelper.Update(sql.ToString());
            return c;
        }


        /// <summary>
        /// 更新订单
        /// </summary>
        /// <param name="orderid">订单id</param>
        /// <param name="type">修改类型  0：状态  1：第三方流水号  2：供应商流水号</param>
        /// <param name="serialid"></param>
        /// <returns></returns>

        public static int UpdateOrderSerialid(string orderid, int state)
        {
            if (string.IsNullOrWhiteSpace(orderid)) return 0;
            var sql = new StringBuilder();
            sql.Append(" UPDATE `ho_hotelorder` SET ");
            if (state > 0)
            {
                sql.AppendFormat(" `HOStatus` = {0}  ", state);
            }
            sql.AppendFormat(" WHERE  `HOSupplierSerialId` = '{0}'  Limit 1;  ",orderid);
            var c = MysqlHelper.Update(sql.ToString());
            return c;
        }


        /// <summary>
        /// 更新供应商订单号和分销商订单号
        /// </summary>
        /// <param name="orderserialid"></param>
        /// <returns></returns>
        public static int UpdatesSupplierSerialid(string orderserialid, string supplierserialid, string disserialid)
        {
            if (string.IsNullOrWhiteSpace(orderserialid)) return 0;
            var sql = new StringBuilder();
            sql.AppendFormat(" UPDATE `ho_hotelorder` SET HOSupplierSerialId='{0}',HODistributorSerialId='{1}' WHERE HOCustomerSerialId='{2}'", supplierserialid, disserialid, orderserialid);
            var c = MysqlHelper.Update(sql.ToString());
            return c;
        }

        /// <summary>
        /// 更新供应商订单
        /// </summary>
        /// <param name="orderserialid"></param>
        /// <returns></returns>
        public static int UpdatesSupplier(string orderserialid, string supplierserialid, int status = 0)
        {
            if (string.IsNullOrWhiteSpace(orderserialid)) return 0;
            var sql = new StringBuilder();
            sql.AppendFormat(" UPDATE `ho_hotelorder` SET HOSupplierSerialId='{0}',HOStatus='{2}'  WHERE HOCustomerSerialId='{1}'", supplierserialid, orderserialid, status);
            var c = MysqlHelper.Update(sql.ToString());
            return c;
        }

        /// <summary>
        /// 更新状态--喜玩
        /// </summary>
        /// <param name="orderserialid"></param>
        /// <returns></returns>
        public static int UpdatesAutorStatus(string orderserialid, string status)
        {
            if (string.IsNullOrWhiteSpace(orderserialid)) return 0;
            var qulangstatus = 0;
            var sql = new StringBuilder();
            switch (status)
            {
                case "1":
                    qulangstatus = 0;
                    break;
                case "4":
                case "5":
                    qulangstatus = 1;
                    break;
                case "2":
                case "3":
                    qulangstatus = 3;
                    break;
            }
            sql.AppendFormat(" UPDATE `ho_hotelorder` SET HOStatus='{0}'  WHERE HOCustomerSerialId='{1}'", qulangstatus, orderserialid);
            var c = MysqlHelper.Update(sql.ToString());
            return c;
        }

        /// <summary>
        /// 更新状态--喜玩
        /// </summary>
        /// <param name="orderserialid"></param>
        /// <returns></returns>
        public static int UpdatesXiWanStatus(string orderserialid, int status)
        {
            if (string.IsNullOrWhiteSpace(orderserialid)) return 0;
            var qulangstatus = 0;
            var sql = new StringBuilder();
            switch (status)
            {
                case 1:
                case 6:
                    qulangstatus = 0;
                    break;
                case 3:
                case 4:
                    qulangstatus = 1;
                    break;
                case 2:
                    qulangstatus = 3;
                    break;
                case 5:
                    qulangstatus = 2;
                    break;
            }
            sql.AppendFormat(" UPDATE `ho_hotelorder` SET HOStatus='{0}'  WHERE HOCustomerSerialId='{1}'", qulangstatus, orderserialid);
            var c = MysqlHelper.Update(sql.ToString());
            return c;
        }

        /// <summary>
        /// 获取订单日志
        /// </summary>
        /// <param name="orderid"></param>
        /// <returns></returns>
        public static BasePageResponse<HO_HotelOrderLogModel> GetOrderLogList(OrderLogSearchRequset request)
        {
            var response = new BasePageResponse<HO_HotelOrderLogModel>();
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(@"SELECT * FROM ho_hotelorderlog  WHERE  HOLOrderId='{0}'  LIMIT 1;", request.CustomerSerialId);
            var list = MysqlHelper.GetList<HO_HotelOrderLogModel>(sb.ToString());
            var total = list?.Count ?? 0;
            if (total > 0)
            {
                response.IsSuccess = 1;
                response.Total = total;
                response.List = list.Skip((request.PageIndex - 1) * request.PageSize).Take(request.PageSize)?.ToList();
            }
            return response;
        }

        /// <summary>
        /// 统计订单
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static BasePageResponse<OrderStaticResponse> GetOrderStaticList(OrderStaticRequest request)
        {
            var response = new BasePageResponse<OrderStaticResponse>();
            StringBuilder sb = new StringBuilder();
            StringBuilder sbwhere = new StringBuilder();
            if (request.Type == 1 && request.TimeType != 0)
            {
                if (request.TimeType == 1)
                {
                    sbwhere.AppendFormat(" AND ho.HOAddTime>= '{0}'", request.StartTime);
                    sbwhere.AppendFormat(" AND ho.HOAddTime< '{0}'", request.EndTime);
                }
                else
                {
                    sbwhere.AppendFormat(" AND ho.HOCheckOutDate>= '{0}'", request.StartTime);
                    sbwhere.AppendFormat(" AND ho.HOCheckOutDate< '{0}'", request.EndTime);
                }
            }
            if (request.PrivoceId > 0)
            {
                sbwhere.AppendFormat(" AND hb.HIProvinceId= {0}", request.PrivoceId);
            }
            if (request.CityId > 0)
            {
                sbwhere.AppendFormat(" AND hb.HICityId= {0}", request.CityId);
            }
            if (request.Part1 > 0)
            {
                sbwhere.AppendFormat(" AND ho.HoPlat1= {0}", request.Part1);
            }
            if (request.Part2 > 0)
            {
                sbwhere.AppendFormat(" AND hb.HoPlat2= {0}", request.Part2);
            }
            if (!string.IsNullOrWhiteSpace(request.HotelName))
            {
                sbwhere.AppendFormat(" AND ho.HName IN ({0})", request.HotelName);
            }
            if (!string.IsNullOrWhiteSpace(request.HotelId))
            {
                sbwhere.AppendFormat(" AND ho.HId IN ({0})", request.HotelId);
            }
            if (!string.IsNullOrWhiteSpace(request.SupplierName))
            {
                sbwhere.AppendFormat(" AND ho.HOSupperlierName Like '%{0}%'", request.SupplierName);
            }
            if (request.SupplierSource > 0)
            {
                sbwhere.AppendFormat(" AND ho.HOSupplierSourceId ={0}", request.SupplierSource);
            }
            sb.AppendFormat(@"SELECT
	                                DATE_FORMAT(ho.HOAddTime, '%Y-%m-%d') AS CreateTime,
	                                DATE_FORMAT(
		                                ho.HOCheckOutDate,
		                                '%Y-%m-%d'
	                                ) AS CheckOutDate,
	                                hb.HIProvinceId AS ProvinceId,
	                                hb.HIProvince AS ProvinceName,
	                                hb.HICityId AS CityId,
	                                hb.HICity AS CityName,
	                                ho.HName AS HotelName,
	                                ho.HIId AS HotelId,
	                                ho.HOSupperlierName AS SupperlierName,
	                                ho.HOSupplierId AS SupperlierId,
	                                count(ho.Id) AS TotalCreate,
	                                count(
		                                CASE
		                                WHEN ho.HOStatus = 1 THEN
			                                1
		                                END
	                                ) AS TotalSuccess,
	                                sum(ho.HOSellPrice) AS TotalSell,
	                                sum(ho.HOContractPrice) AS TotalContract,
	                                sum(
		                                ho.HOSellPrice - ho.HOContractPrice
	                                ) AS TotalRevenue
                                FROM
	                                ho_hotelorder ho
                                INNER JOIN h_hotelinfo hb ON hb.Id = ho.HIId
                                WHERE
	                                1 = 1
                                    {0}
                                GROUP BY
	                                DATE_FORMAT(ho.HOAddTime, '%Y-%m-%d'),
	                                DATE_FORMAT(
		                                ho.HOCheckOutDate,
		                                '%Y-%m-%d'
	                                ),
	                                hb.HIProvinceId,
	                                hb.HIProvince,
	                                hb.HICityId,
	                                hb.HICity,
	                                ho.HName,
	                                ho.HIId,
	                                ho.HOSupperlierName,
	                                ho.HOSupplierId", sbwhere.ToString());
            var list = MysqlHelper.GetList<OrderStaticResponse>(sb.ToString());
            var total = list?.Count ?? 0;
            if (total > 0)
            {
                response.IsSuccess = 1;
                response.Total = total;
                response.List = list.Skip((request.PageIndex - 1) * request.PageSize).Take(request.PageSize)?.ToList();
            }
            return response;
        }
    }
}
