using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace HotelBase.Api.Job
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
                routeTemplate: "job/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
