using Component.Access;
using HotelBase.Api.Entity.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBase.Api.DataAccess.Area
{
    public class H_DistributorAreaInfoAccess : BaseAccess<H_DistributorAreaInfoModel>
    {
        public H_DistributorAreaInfoAccess() : base(MysqlHelper.Db_HotelBase)
        {

        }


        /// <summary>
        /// 查询城市id
        /// </summary>
        /// <param name="cityid"></param>
        /// <returns></returns>
        public static int GetCiy(int cityid)
        {
            var query = new H_DistributorAreaInfoAccess().Query()
                .Where(x => x.AA_CityId == cityid).Distinct().FirstOrDefault();
            return query != null ? query.AA_CityId : 0;
        }

        /// <summary>
        /// 获取城市列表
        /// </summary>
        /// <returns></returns>
        public static List<H_DistributorAreaInfoModel> GetCiyList()
        {
            return new H_DistributorAreaInfoAccess().Query().Where(x => x.AA_Type == 1).Distinct().ToList();
        }
    }
}
