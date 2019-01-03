using HotelBase.Api.Entity.CommonModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace HotelBase.Api.Controllers
{
    public class AtourController : ApiController
    {
        private string AtourAuth_URL = ConfigurationManager.AppSettings["OpenApiTest"];
        private string AtourAuth_APPID = ConfigurationManager.AppSettings["appid"];

        /// <summary>
        /// 获取城市列表
        /// </summary>
        /// <returns></returns>

        public DataResult GetCityList()
        {
            var result = new DataResult();

            return result;
        }
    }
}