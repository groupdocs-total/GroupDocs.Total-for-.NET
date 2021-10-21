using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;

namespace GroupDocs.Total.MVC
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            Products.Common.Licensing.GroupDocsTotalLicenser.SetLicense();
            AreaRegistration.RegisterAllAreas();
            UnityConfig.RegisterComponents();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }
    }
}