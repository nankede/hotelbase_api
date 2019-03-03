using HotelBase.Api.Entity;
using HotelBase.Api.Entity.CommonModel;
using HotelBase.Api.Entity.Models;
using HotelBase.Api.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Results;

namespace HotelBase.Api.Controllers
{
    public class HomeController : BaseController
    {
        //http://localhost:62523/api/home/index

        /// <summary>
        /// 测试Get
        /// </summary>
        /// <returns></returns>
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

        #region 亚朵资源

        /// <summary>
        /// 亚朵城市资源
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult<DataResult> CityInit()
        {
            var rtn = new DataResult();
            try
            {
                rtn = YaDuoApiService.GetCityList();
            }
            catch (Exception ex)
            {
                rtn.Message = ex.ToString();
            }
            return Json(rtn);
        }


        /// <summary>
        /// 亚朵城酒店列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult<DataResult> HotelListInit(int maxId, int top)
        {
            var rtn = new DataResult();
            try
            {
                rtn = YaDuoApiService.GetHotelList(maxId, top);
            }
            catch (Exception ex)
            {
                rtn.Message = ex.ToString();
            }
            return Json(rtn);
        }

        //

        /// <summary>
        /// 亚朵城酒店详情
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult<DataResult> GetHotelDetail(int maxId, int top)
        {
            var rtn = new DataResult();
            try
            {
                rtn = YaDuoApiService.GetHotelDetail(maxId, top);
            }
            catch (Exception ex)
            {
                rtn.Message = ex.ToString();
            }
            return Json(rtn);
        }
        #endregion
    }
}
