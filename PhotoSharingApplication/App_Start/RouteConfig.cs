using System.Web.Mvc;
using System.Web.Routing;

namespace PhotoSharingApplication
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapMvcAttributeRoutes();

            routes.MapRoute(
                name: "LegacyPhoto",
                url: "Photo/{action}/{id}",
                defaults: new { controller = "Photo", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Photo", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
