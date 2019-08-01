using Component.Access;
using Component.Access.DapperExtensions.Lambda;
using Dapper;
using HotelBase.Api.Entity;
using HotelBase.Api.Entity.Models;
using HotelBase.Api.Entity.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBase.Api.DataAccess.Resource
{
    /// <summary>
    /// 酒店查询
    /// </summary>
    public class H_ResourceLogAccess : BaseAccess<H_ResourceLogModel>
    {
        public H_ResourceLogAccess() : base(MysqlHelper.Db_HotelBase)
        {
        }

        public void AddLog(int Id, string log, ResourceLogType type)
        {
            Task.Factory.StartNew(() =>
            {
                var model = new H_ResourceLogModel
                {
                    Id = 0,
                    RLAddDepartId = 0,
                    RLAddDepartName = "",
                    RLAddId = 0,
                    RLAddName = "",
                    RLAddTime = DateTime.Now,
                    RLLogType = type.GetHashCode(),
                    RLOutId = Id,
                    RLRemark = log
                };
                this.Add(model);
            });
        }
    }

    public enum ResourceLogType
    {
        HotelAdd = 101,
        HotelDelete = 102,
        HotelUpdate = 103,
        RoomAdd = 201,
        RoomDelete = 202,
        RoomUpdate = 203,
        RuleAdd = 301,
        RuleDelete = 302,
        RuleUpdate = 303,
        PriceAdd = 401,
        PriceDelete = 402,
        PriceUpdate = 403,
    }
}
