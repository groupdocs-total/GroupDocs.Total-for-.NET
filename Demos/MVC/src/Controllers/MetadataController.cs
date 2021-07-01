using System.Web.Mvc;

namespace GroupDocs.Total.MVC.Controllers
{
    /// <summary>
    /// Metadata Web page controller
    /// </summary>
    public class MetadataController : Controller
    {       
        public ActionResult Index()
        {
            return View("Index");
        }
    }
}