using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBase.Api.Entity.Response
{
    public class AtourResponse
    {

    }

    /// <summary>
    /// 城市列表
    /// </summary>
    public class AtourCityResponse: BaseResponse
    {
        public List<CityList> result { get; set; }
    }

    public class CityList
    {
        /// <summary>
        /// 城市名称
        /// </summary>
        public string cityName { get; set; }

        /// <summary>
        /// 城市Id
        /// </summary>
        public int cityId { get; set; }

        /// <summary>
        /// 省份名称
        /// </summary>
        public string provinceName { get; set; }
    }

    /// <summary>
    /// 酒店列表
    /// </summary>
    public class AtourHotelResponse : BaseResponse
    {
        public List<HotelyList> result { get; set; }
    }

    public class HotelyList
    {
        /// <summary>
        /// 酒店ID
        /// </summary>
        public int hotelId { get; set; }

        /// <summary>
        /// 酒店名称
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 酒店地址
        /// </summary>
        public string address { get; set; }

        /// <summary>
        /// 业务区域（例如：靠近3 4 9号线宜山路站）
        /// </summary>
        public string businessZone { get; set; }

        /// <summary>
        /// 经度（高德地图）
        /// </summary>
        public string longitude { get; set; }

        /// <summary>
        /// 纬度（高德地图）
        /// </summary>
        public string latitude { get; set; }

        /// <summary>
        /// 网站
        /// </summary>
        public string website { get; set; }

        /// <summary>
        /// 电话号码
        /// </summary>
        public string tel { get; set; }

        /// <summary>
        /// 备注（酒店基本介绍以及大体位置等信息）
        /// </summary>
        public string remark { get; set; }
        /// <summary>
        /// 城市ID
        /// </summary>
        public int cityId { get; set; }

        /// <summary>
        /// 城市名称
        /// </summary>
        public string cityName { get; set; }

        /// <summary>
        /// 创建时间字符串，格式: yyyy-MM-dd HH:mm:ss
        /// </summary>
        public string createTime { get; set; }

        /// <summary>
        /// 更新时间字符串，格式: yyyy-MM-dd HH:mm:ss
        /// </summary>
        public string updateTime { get; set; }

        /// <summary>
        /// 图片URL地址列表（没有头图区分）
        /// </summary>
        public List<string> pictures { get; set; }
    }
}
