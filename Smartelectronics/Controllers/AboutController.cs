using Microsoft.AspNetCore.Mvc;

namespace Smartelectronics.Controllers
{
    public class AboutController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
