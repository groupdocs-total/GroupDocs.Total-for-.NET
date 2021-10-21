using System.Web.Mvc;

namespace GroupDocs.Total.MVC.Controllers
{
    /// <summary>
    /// Parser Web page controller.
    /// </summary>
    public class ParserController : Controller
    {
        public ActionResult Index()
        {
            return View("Index");
        }
    }
}