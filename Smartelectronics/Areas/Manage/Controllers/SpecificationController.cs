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
    public class SpecificationController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public SpecificationController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<IActionResult> Index(int pageIndex = 1)
        {
            IQueryable<Specification> query = _context.Specifications.Where(s => s.IsDeleted == false)
                .Include(s => s.SpecificationGroup)
                .OrderByDescending(c => c.Id);

            //ViewBag.TotalCount = (int)Math.Ceiling((decimal)categories.Count() / 3);
            //ViewBag.PageIndex = pageIndex;

            return View(PageNatedList<Specification>.Create(query, pageIndex, 3, 8));
        }


        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.SpecificationGroups = await _context.SpecificationGroups.Where(c => c.IsDeleted == false).ToListAsync();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Specification specification)
        {
            ViewBag.SpecificationGroups = await _context.SpecificationGroups.Where(c => c.IsDeleted == false).ToListAsync();

            if (!ModelState.IsValid) return View();

            if (await _context.Specifications.AnyAsync(c => c.IsDeleted == false && c.Name.ToLower() == specification.Name.Trim().ToLower()))
            {
                ModelState.AddModelError("Name", $"Bu adda {specification.Name} specification movcuddu");
                return View(specification);
            }

            specification.Name = specification.Name.Trim();
            specification.CreatedAt = DateTime.UtcNow.AddHours(4);
            specification.CreatedBy = "System";

            await _context.Specifications.AddAsync(specification);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Update(int? id)
        {

            if (id == null) return BadRequest();

            Specification specification = await _context.Specifications.FirstOrDefaultAsync(c => c.Id == id && c.IsDeleted == false);

            if (specification == null) return NotFound();

            ViewBag.SpecificationGroups = await _context.SpecificationGroups.Where(c => c.IsDeleted == false).ToListAsync();

            return View(specification);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int? id, Specification specification)
        {
            if (!ModelState.IsValid) return View(specification);

            if (id == null) return BadRequest();

            if (id != specification.Id) return BadRequest();

            Specification dbspecification = await _context.Specifications.FirstOrDefaultAsync(c => c.Id == id && c.IsDeleted == false);

            if (specification == null) return NotFound();

            if (await _context.Categories.AnyAsync(c => c.IsDeleted == false && c.Name.ToLower() == specification.Name.Trim().ToLower() && c.Id != specification.Id))
            {
                ModelState.AddModelError("Name", $"Bu adda {specification.Name} category movcuddur");
                return View(specification);
            }


            dbspecification.Name = specification.Name.Trim();
            dbspecification.UpdatedAt = DateTime.UtcNow.AddHours(4);
            dbspecification.UpdatedBy = "System";

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return BadRequest();

            Specification specification = await _context.Specifications
                .FirstOrDefaultAsync(c => c.IsDeleted == false && c.Id == id);

            if (specification == null) return NotFound();



            specification.IsDeleted = true;
            specification.DeletedBy = "System";
            specification.DeletedAt = DateTime.UtcNow.AddHours(4);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
