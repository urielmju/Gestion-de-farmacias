using System.Web.Mvc;
using PIV_PF_ProyectoFinal.Filters;

namespace PIV_PF_ProyectoFinal.Controllers
{
    [SessionAuthorize]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}
