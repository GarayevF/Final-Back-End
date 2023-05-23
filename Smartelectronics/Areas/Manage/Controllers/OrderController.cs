using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Smartelectronics.DataAccessLayer;
using Smartelectronics.Models;
using Smartelectronics.ViewModels;

namespace Smartelectronics.Areas.Manage.Controllers
{
    [Area("Manage")]
    [Authorize(Roles = "SuperAdmin, Admin")]

    public class OrderController : Controller
    {
        private readonly AppDbContext _context;

        public OrderController(AppDbContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "SuperAdmin, Admin")]
        public IActionResult Index(int pageIndex = 1)
        {
            IQueryable<Order> orders = _context.Orders
                .Include(o => o.OrderItems)
                .Where(o => o.IsDeleted == false);

            return View(PageNatedList<Order>.Create(orders, pageIndex, 5, 5));
        }

        [Authorize(Roles = "SuperAdmin, Admin")]
        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null) return BadRequest();

            Order order = await _context.Orders
                .Include(o => o.OrderItems.Where(oi => oi.IsDeleted == false)).ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.IsDeleted == false && o.Id == id);

            if (order == null) return NotFound();

            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "SuperAdmin, Admin")]
        public async Task<IActionResult> ChangeOrder(int? id, Order order)
        {
            if (id == null) return BadRequest();

            if (id != order.Id) return BadRequest();

            Order dbOrder = await _context.Orders
                .Include(o => o.OrderItems.Where(oi => oi.IsDeleted == false)).ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == id && o.IsDeleted == false);

            if (dbOrder == null) return NotFound();

            if ((int)order.Status < 0 || (int)order.Status > 4)
            {
                ModelState.AddModelError("Status", "duzgun secim edin");
                return View("Detail", dbOrder);
            }

            dbOrder.Status = order.Status;
            dbOrder.Comment = order.Comment;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
