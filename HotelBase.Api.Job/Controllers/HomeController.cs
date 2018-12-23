using HotelBase.Api.Entity;
using HotelBase.Api.Entity.Models;
using HotelBase.Api.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Results;

namespace HotelBase.Api.Job.Controllers
{
    public class HomeController : BaseController
    {
        [HttpGet]
        public JsonResult<BasePageResponse<HotelSearchResponse>> Index()
        {
            var s = HotelBll.GetList(new HotelSearchRequest()
            {
                Name = "酒店"
            });

            return Json(s);
        }

        [HttpPost]
        public JsonResult<string> Post()
        {


            return Json("Post");
        }
    }
}
