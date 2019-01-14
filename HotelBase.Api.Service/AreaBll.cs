using HotelBase.Api.DataAccess.Area;
using HotelBase.Api.Entity.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBase.Api.Service
{
    public static class AreaBll
    {
        /// <summary>
        /// 批量新增城市详情
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static bool Insert(List<H_DistributorAreaInfoModel> model)
        {
            return new H_DistributorAreaInfoAccess().AddBatch(model);
            
        }

        /// <summary>
        /// 查询城市
        /// </summary>
        /// <param name="cityId"></param>
        /// <returns></returns>
        public static int IsInTable(int cityId)
        {
            return H_DistributorAreaInfoAccess.GetCiy(cityId);
        }

        /// <summary>
        /// 获取城市列表
        /// </summary>
        /// <returns></returns>
        public static List<H_DistributorAreaInfoModel> GetCityList()
        {
            return H_DistributorAreaInfoAccess.GetCiyList();
        }
    }
}
