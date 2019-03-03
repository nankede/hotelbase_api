using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace HotelBase.Api
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API 配置和服务

            // Web API 路由
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}",
                 defaults: new { controller = "Home", action = "Index", id = RouteParameter.Optional }
            );
        }
    }
}
