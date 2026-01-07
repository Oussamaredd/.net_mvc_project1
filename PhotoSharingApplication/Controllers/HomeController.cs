using System.Web.Mvc;

namespace PhotoSharingApplication.Controllers
{
    [RoutePrefix("")]
    public class HomeController : Controller
    {
        // GET /
        [HttpGet]
        [Route("")]
        public ActionResult Index()
        {
            return RedirectToAction("Index", "Photo");
        }
    }
}
