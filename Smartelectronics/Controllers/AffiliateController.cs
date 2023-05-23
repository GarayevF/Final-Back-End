using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Smartelectronics.Areas.Manage.ViewModels.ProductViewModels;
using Smartelectronics.DataAccessLayer;
using Smartelectronics.Models;

namespace Smartelectronics.Controllers
{
    public class AffiliateController : Controller
    {
        private readonly AppDbContext _context;

        public AffiliateController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {

            IEnumerable<Affiliate> affiliates = await _context.Affiliates.Where(b => b.IsDeleted == false)
                .ToListAsync();


            return View(affiliates);
        }

        public async Task<IActionResult> SendMessage(Message message)
        {
            if (!ModelState.IsValid)
            {
                return View(message);
            }

            if (message == null) return BadRequest();

            await _context.Messages.AddAsync(message);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
