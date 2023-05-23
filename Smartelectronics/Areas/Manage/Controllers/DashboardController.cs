using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Smartelectronics.DataAccessLayer;
using Smartelectronics.Models;

namespace Smartelectronics.Areas.Manage.Controllers
{
    [Area("Manage")]
    [Authorize(Roles = "SuperAdmin, Admin")]
    public class DashboardController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        public DashboardController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            IEnumerable<Order> orders = await _context.Orders.Where(o => o.IsDeleted == false)
                .Include(o => o.OrderItems.Where(oi => oi.IsDeleted == false))
                .OrderByDescending(a => a.Id)
                .Take(5).ToListAsync();

            IEnumerable<Message> messages = await _context.Messages.Where(o => o.IsDeleted == false)
                .OrderByDescending(a => a.Id)
                .Take(5).ToListAsync();

            ViewBag.Orders = orders;
            ViewBag.Messages = messages;
            return View();
        }
    }
}
