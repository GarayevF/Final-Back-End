using Microsoft.AspNetCore.Mvc;

namespace Smartelectronics.Areas.Manage.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
