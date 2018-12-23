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
        public JsonResult<bool> Index()
        {


            return Json(true);
        }

        [HttpPost]
        public JsonResult<string> Post()
        {


            return Json("Post");
        }
    }
}
