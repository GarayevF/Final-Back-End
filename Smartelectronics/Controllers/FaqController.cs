using Microsoft.AspNetCore.Mvc;

namespace Smartelectronics.Controllers
{
    public class FaqController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
