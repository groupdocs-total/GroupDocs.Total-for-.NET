using System.Web.Routing;
using Microsoft.AspNet.FriendlyUrls;

namespace GroupDocs.Total.WebForms
{
    public static class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            var settings = new FriendlyUrlSettings();
            settings.AutoRedirectMode = RedirectMode.Permanent;
            routes.EnableFriendlyUrls(settings);

            routes.MapPageRoute("Total", "", "~/Pages/Total.aspx");
            routes.MapPageRoute("Viewer", "viewer", "~/Pages/Viewer.aspx");
            routes.MapPageRoute("Annotation", "annotation", "~/Pages/Annotation.aspx");
            routes.MapPageRoute("Comparison", "comparison", "~/Pages/Comparison.aspx");
            routes.MapPageRoute("Conversion", "conversion", "~/Pages/Conversion.aspx");
            routes.MapPageRoute("Editor", "editor", "~/Pages/Editor.aspx");
            routes.MapPageRoute("Metadata", "metadata", "~/Pages/Metadata.aspx");
            routes.MapPageRoute("Search", "search", "~/Pages/Search.aspx");
            routes.MapPageRoute("Signature", "signature", "~/Pages/Signature.aspx");
        }
    }
}
