using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Smartelectronics.DataAccessLayer;
using Smartelectronics.Helpers;
using Smartelectronics.Models;
using Smartelectronics.ViewModels;

namespace Smartelectronics.Areas.Manage.Controllers
{
    [Area("Manage")]
    [Authorize(Roles = "SuperAdmin, Admin")]
    public class AffiliateController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public AffiliateController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<IActionResult> Index(int pageIndex = 1)
        {

            IQueryable<Affiliate>? queries = _context.Affiliates
                .Where(p => p.IsDeleted == false);

            return View(PageNatedList<Affiliate>.Create(queries, pageIndex, 3, 5));
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Affiliate affiliate)
        {

            if (!ModelState.IsValid)
            {
                return View(affiliate);
            }

            affiliate.CreatedAt = DateTime.UtcNow.AddHours(4);
            affiliate.CreatedBy = "System";


            await _context.Affiliates.AddAsync(affiliate);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Update(int? id)
        {
            if (id == null) return BadRequest();

            Affiliate affiliate = await _context.Affiliates
                .FirstOrDefaultAsync(p => p.Id == id && p.IsDeleted == false);

            if (affiliate == null) return NotFound();


            return View(affiliate);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int? id, Affiliate affiliate)
        {

            if (!ModelState.IsValid)
            {
                return View(affiliate);
            }

            if (id == null) return BadRequest();

            if (id != affiliate.Id) return BadRequest();

            Affiliate dbaffiliate = await _context.Affiliates
                .FirstOrDefaultAsync(p => p.Id == id && p.IsDeleted == false);

            if (dbaffiliate == null) return NotFound();

            dbaffiliate.Mail = affiliate.Mail;
            dbaffiliate.Number = affiliate.Number;
            dbaffiliate.Address = affiliate.Address;
            dbaffiliate.Hotline = affiliate.Hotline;

            dbaffiliate.UpdatedBy = "System";
            dbaffiliate.UpdatedAt = DateTime.UtcNow.AddHours(4);

            await _context.SaveChangesAsync();


            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return BadRequest();

            Affiliate affiliate = await _context.Affiliates
                .FirstOrDefaultAsync(c => c.Id == id);

            if (affiliate == null) return NotFound();

            affiliate.IsDeleted = true;
            affiliate.DeletedBy = "System";
            affiliate.DeletedAt = DateTime.UtcNow.AddHours(4);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

    }
}
