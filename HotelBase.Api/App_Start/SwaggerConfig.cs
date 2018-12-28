using HotelBase.Api.App_Start;
using System.Web.Http;
using WebActivatorEx;
using HotelBase.Api;
using Swashbuckle.Application;
using System.Reflection;
using System.Linq;

[assembly: PreApplicationStartMethod(typeof(SwaggerConfig), "Register")]

namespace HotelBase.Api
{
    public class SwaggerConfig
    {
        public static void Register()
        {
            var thisAssembly = typeof(SwaggerConfig).Assembly;
            try
            {
                GlobalConfiguration.Configuration
                             .EnableSwagger(c =>
                             {

                                 c.SingleApiVersion("v2", "Offer.Contract.Web");

                                 c.IncludeXmlComments(GetXmlCommentsPath());
                                 c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
                                 c.CustomProvider((defaultProvider) => new SwaggerControllerDescProvider(defaultProvider, GetXmlCommentsPath()));

                             })
                             .EnableSwaggerUi(c =>
                             {
                                 c.InjectJavaScript(Assembly.GetExecutingAssembly(), "Offer.Contract.Web.Scripts.Swagger-Custom.js");
                             });

            }
            catch (System.Exception ex)
            {

            }

        }

        protected static string GetXmlCommentsPath()
        {
            var re = System.String.Format(@"{0}\bin\HotelBase.Api.XML", System.AppDomain.CurrentDomain.BaseDirectory);
            return re;
        }
    }
}
