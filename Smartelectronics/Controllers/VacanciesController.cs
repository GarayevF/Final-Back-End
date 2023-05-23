using Microsoft.AspNetCore.Mvc;

namespace Smartelectronics.Controllers
{
    public class VacanciesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
