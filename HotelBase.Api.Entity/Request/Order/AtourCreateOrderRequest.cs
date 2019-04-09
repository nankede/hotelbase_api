using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBase.Api.Entity.Request.Order
{
    public class AtourCreateOrderRequest
    {
        #region 暂定
        ///// <summary>
        ///// 我方订单流水号
        ///// </summary>
        //public string qlorderserialid { get; set; }

        ///// <summary>
        ///// 分销商订单号
        ///// </summary>
        //public string disserialid { get; set; }

        ///// <summary>
        ///// 酒店名称
        ///// </summary>
        //public string hotelname { get; set; }

        ///// <summary>
        ///// 酒店房型id
        ///// </summary>
        //public string hotelroomid { get; set; }

        ///// <summary>
        ///// 酒店房型名称
        ///// </summary>
        //public string hotelroomname { get; set; }

        ///// <summary>
        ///// 酒店政策id
        ///// </summary>
        //public string hotelroomruleid { get; set; }

        ///// <summary>
        ///// 酒店政策名称
        ///// </summary>
        //public string hotelroomrulename { get; set; }

        ///// <summary>
        ///// 供应商id
        ///// </summary>
        //public string supplierid { get; set; }

        ///// <summary>
        ///// 供应商名称
        ///// </summary>
        //public string suppliername { get; set; }

        ///// <summary>
        ///// 供应商来源id
        ///// </summary>
        //public string suppliersourceid { get; set; }

        ///// <summary>
        ///// 供应商来源名称
        ///// </summary>
        //public string suppliersourcename { get; set; }

        ///// <summary>
        ///// 分销商id
        ///// </summary>
        //public string distributorId { get; set; }

        ///// <summary>
        ///// 分销商名称
        ///// </summary>
        //public string distributorname { get; set; }

        #endregion

        /// <summary>
        /// 供应商订单请求
        /// </summary>
        public OrderModel order { get; set; }
    }

    /// <summary>
    /// 亚朵订单列表
    /// </summary>
    public class OrderModel
    {
        /// <summary>
        /// 酒店id
        /// </summary>
        public int hotelId { get; set; }

        /// <summary>
        /// 会员id
        /// </summary>
        public int mebId { get; set; }

        /// <summary>
        /// 酒店房型ID 
        /// </summary>
        public int roomTypeId { get; set; }

        /// <summary>
        /// 房间数
        /// </summary>
        public int roomNum { get; set; }

        /// <summary>
        /// 入住期间每日的房价列表
        /// </summary>
        public List<RateList> roomRateList { get; set; }

        /// <summary>
        /// 入住日期 格式 yyyy-MM-dd 
        /// </summary>
        public string arrival { get; set; }

        /// <summary>
        /// /入住日期 格式 yyyy-MM-dd 
        /// </summary>
        public string assureTime { get; set; }

        /// <summary>
        /// 入住日期 格式 yyyy-MM-dd 
        /// </summary>
        public string departure { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        public string mobile { get; set; }

        /// <summary>
        /// 联系人
        /// </summary>
        public string contactName { get; set; }

        /// <summary>
        /// 入住人 多个入住人用,号隔开， 如：张三,李四,王五
        /// </summary>
        public string guestName { get; set; }

        /// <summary>
        /// 一级来源，由亚朵提供
        /// </summary>
        public int source { get; set; }

        /// <summary>
        /// 二级来源，由亚朵提供
        /// </summary>
        public int subSource { get; set; }

        /// <summary>
        /// 旧版本价格类型： 28-预付 29-现付 18-协议价格；HRS渠道为新版本房价代码ID，不固定，和房价接口吐出字段数据保持一致即可；
        /// </summary>
        public int roomRateTypeId { get; set; }

        /// <summary>
        /// 第三方订单号
        /// </summary>
        public string thirdOrderNo { get; set; }


        /// <summary>
        /// 使用的券列表,json字符串格式,请参照备注中的couponsList格式
        /// </summary>
        public string couponsList { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string remark { get; set; }
    }

    public class OrderRequest
    {
        /// <summary>
        /// appId
        /// </summary>
        public string appId { get; set; }

        /// <summary>
        /// 参数签名
        /// </summary>
        public string sign { get; set; }

        /// <summary>
        /// 酒店id
        /// </summary>
        public int hotelId { get; set; }

        /// <summary>
        /// 会员id
        /// </summary>
        public int mebId { get; set; }

        /// <summary>
        /// 酒店房型ID 
        /// </summary>
        public int roomTypeId { get; set; }

        /// <summary>
        /// 房间数
        /// </summary>
        public int roomNum { get; set; }

        /// <summary>
        /// 入住期间每日的房价列表
        /// </summary>
        public string roomRateList { get; set; }

        /// <summary>
        /// 入住日期 格式 yyyy-MM-dd 
        /// </summary>
        public string arrival { get; set; }

        /// <summary>
        /// /入住日期 格式 yyyy-MM-dd 
        /// </summary>
        public string assureTime { get; set; }

        /// <summary>
        /// 入住日期 格式 yyyy-MM-dd 
        /// </summary>
        public string departure { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        public string mobile { get; set; }

        /// <summary>
        /// 联系人
        /// </summary>
        public string contactName { get; set; }

        /// <summary>
        /// 入住人 多个入住人用,号隔开， 如：张三,李四,王五
        /// </summary>
        public string guestName { get; set; }

        /// <summary>
        /// 一级来源，由亚朵提供
        /// </summary>
        public int source { get; set; }

        /// <summary>
        /// 二级来源，由亚朵提供
        /// </summary>
        public int subSource { get; set; }

        /// <summary>
        /// 旧版本价格类型： 28-预付 29-现付 18-协议价格；HRS渠道为新版本房价代码ID，不固定，和房价接口吐出字段数据保持一致即可；
        /// </summary>
        public int roomRateTypeId { get; set; }

        /// <summary>
        /// 第三方订单号
        /// </summary>
        public string thirdOrderNo { get; set; }


        /// <summary>
        /// 使用的券列表,json字符串格式,请参照备注中的couponsList格式
        /// </summary>
        public string couponsList { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string remark { get; set; }
    }


    public class RateList
    {
        public string accDate { get; set; }

        public decimal roomRate { get; set; }
    }

    public class CoopList
    {

    }
}
