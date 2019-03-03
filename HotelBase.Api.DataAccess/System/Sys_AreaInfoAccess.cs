using Component.Access;
using HotelBase.Api.Entity.Models;
using HotelBase.Api.Entity.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBase.Api.DataAccess.System
{
    /// <summary>
    /// 国家地区相关
    /// </summary>
    public class Sys_AreaInfoAccess
    {
        private static List<Sys_AreaInfoModel> _AreaList = new List<Sys_AreaInfoModel>();
        /// <summary>
        /// 地区数据-中国
        /// </summary>
        public static List<Sys_AreaInfoModel> AreaList
        {
            get
            {
                if (_AreaList == null || _AreaList.Count == 0)
                {
                    var sql = "SELECT * FROM Sys_AreaInfo WHERE pid_path  LIKE '3106,%' ";
                    _AreaList = MysqlHelper.GetList<Sys_AreaInfoModel>(sql);

                }
                return _AreaList ?? new List<Sys_AreaInfoModel>();
            }
        }

        /// <summary>
        /// 查询列表
        /// </summary>
        /// <returns></returns>
        public static List<AreaInfoModel> GetAreaList(int pId)
        {
            var list = new List<AreaInfoModel>();
            var dataList = AreaList?.Where(x => x.pid == pId).ToList();
            dataList?.ForEach(x =>
            {
                list.Add(new AreaInfoModel
                {
                    Name = x.name,
                    PId = x.pid,
                    Type = x.type,
                    Id = x.id,
                });
            });
            return list;
        }


    }


    /// <summary>
    /// 国家地区相关
    /// </summary>
    public class Sys_AreaInfoAccess2 : BaseAccess<Sys_AreaInfoModel>
    {
        public Sys_AreaInfoAccess2() : base(MysqlHelper.Db_HotelBase)
        {
        }
    }

    /// <summary>
    /// 地区对应关系表
    /// </summary>
    public class Sys_AreaMatchAccess : BaseAccess<Sys_AreaMatchModel>
    {
        public Sys_AreaMatchAccess() : base(MysqlHelper.Db_HotelBase)
        {
        }
    }
}
