using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace HRM
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("{*allaspx}", new { allaspx = @".*(CrystalImageHandler).*" });

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}/{cmp}",
                defaults: new { controller = "Profile", action = "Login", id = UrlParameter.Optional, cmp = UrlParameter.Optional }
            );
        }
    }
}
