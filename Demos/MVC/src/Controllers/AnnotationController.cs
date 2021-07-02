using System.Web.Mvc;

namespace GroupDocs.Total.MVC.Controllers
{
    /// <summary>
    /// Signature Web page controller
    /// </summary>
    public class AnnotationController : Controller
    {
        // View Annotation
        public ActionResult Index()
        {
            return View();
        }
    }
}