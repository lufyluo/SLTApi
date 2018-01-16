using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace AppApi
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API 配置和服务
            // Web API 路由
            config.MapHttpAttributeRoutes();
            config.Routes.MapHttpRoute(
                name: "Api",
                routeTemplate: "api/{controller}/{action}/{Operation}",
                defaults: new { Operation = RouteParameter.Optional}
            );
            config.Routes.MapHttpRoute(
                name: "mail_Getlist",
                routeTemplate: "api/Mail/Getlist/{action}/{Operation}",
                defaults: new { controller="mail", Operation = RouteParameter.Optional }
            );
        }
    }
}
