using System;
using System.Web;
using System.Web.Routing;
using System.Web.Http;
using GroupDocs.Total.WebForms.Products.Common.Licensing;

namespace GroupDocs.Total.WebForms
{
    public class Global : HttpApplication
    {
        void Application_Start(object sender, EventArgs e)
        {
            GroupDocsTotalLicenser.SetLicense();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }
    }
}