using System.Web.Mvc;
using System.Web.Routing;

namespace site {
    public class RouteConfig {
        public static void RegisterRoutes(RouteCollection routes) {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute("Default", "{controller}/{action}/{nifOuNome}",
                new {controller = "Home", action = "Index", nifOuNome = UrlParameter.Optional}
                );
        }
    }
}