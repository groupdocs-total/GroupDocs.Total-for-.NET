using System.Web.Mvc;

namespace GroupDocs.Total.MVC.Controllers
{
    /// <summary>
    /// Viewer Web page controller
    /// </summary>
    public class ConversionController : Controller
    {       
        public ActionResult Index()
        {
            return View("Index");
        }
    }
}