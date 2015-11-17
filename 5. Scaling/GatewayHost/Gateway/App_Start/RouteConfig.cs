using System.Web.Http;

namespace Gateway.App_Start
{
    public static class RouteConfig
    {
        public static void RegisterRoutes(HttpRouteCollection routes)
        {
            routes.MapHttpRoute(
                name: "Leases",
                routeTemplate: "leases/{resource}/{leaseId}",
                defaults: new { controller = "Leases", resource = RouteParameter.Optional, leaseId = RouteParameter.Optional }
                );
        }
    }
}