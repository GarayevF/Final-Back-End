using Microsoft.AspNetCore.Mvc;

namespace Smartelectronics.Controllers
{
    public class OrderController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
