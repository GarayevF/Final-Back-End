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
    public class SpecificationGroupController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public SpecificationGroupController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<IActionResult> Index(int pageIndex = 1)
        {
            IQueryable<SpecificationGroup> query = _context.SpecificationGroups.Where(s => s.IsDeleted == false)
                .OrderByDescending(c => c.Id);

            //ViewBag.TotalCount = (int)Math.Ceiling((decimal)categories.Count() / 3);
            //ViewBag.PageIndex = pageIndex;

            return View(PageNatedList<SpecificationGroup>.Create(query, pageIndex, 3, 8));
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null) return BadRequest();

            SpecificationGroup specificationGroup = await _context.SpecificationGroups
                .FirstOrDefaultAsync(c => c.IsDeleted == false && c.Id == id);

            if (specificationGroup == null) return NotFound();

            return View(specificationGroup);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SpecificationGroup specificationGroup)
        {

            if (!ModelState.IsValid) return View();

            if (await _context.SpecificationGroups.AnyAsync(c => c.IsDeleted == false && c.Name.ToLower() == specificationGroup.Name.Trim().ToLower()))
            {
                ModelState.AddModelError("Name", $"Bu adda {specificationGroup.Name} category movcuddu");
                return View(specificationGroup);
            }


            specificationGroup.Name = specificationGroup.Name.Trim();
            specificationGroup.CreatedAt = DateTime.UtcNow.AddHours(4);
            specificationGroup.CreatedBy = "System";

            await _context.SpecificationGroups.AddAsync(specificationGroup);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Update(int? id)
        {

            if (id == null) return BadRequest();

            SpecificationGroup specificationGroup = await _context.SpecificationGroups.FirstOrDefaultAsync(c => c.Id == id && c.IsDeleted == false);

            if (specificationGroup == null) return NotFound();

            return View(specificationGroup);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int? id, SpecificationGroup specificationGroup)
        {
            if (!ModelState.IsValid) return View(specificationGroup);

            if (id == null) return BadRequest();

            if (id != specificationGroup.Id) return BadRequest();

            SpecificationGroup dbSpecificationGroup = await _context.SpecificationGroups.FirstOrDefaultAsync(c => c.Id == id && c.IsDeleted == false);

            if (specificationGroup == null) return NotFound();

            if (await _context.Categories.AnyAsync(c => c.IsDeleted == false && c.Name.ToLower() == specificationGroup.Name.Trim().ToLower() && c.Id != specificationGroup.Id))
            {
                ModelState.AddModelError("Name", $"Bu adda {specificationGroup.Name} category movcuddur");
                return View(specificationGroup);
            }

            dbSpecificationGroup.Icon = specificationGroup.Icon;
            dbSpecificationGroup.Name = specificationGroup.Name.Trim();
            dbSpecificationGroup.UpdatedAt = DateTime.UtcNow.AddHours(4);
            dbSpecificationGroup.UpdatedBy = "System";

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return BadRequest();

            SpecificationGroup specificationGroup = await _context.SpecificationGroups
                .FirstOrDefaultAsync(b => b.Id == id && b.IsDeleted == false);

            if (specificationGroup == null) return NotFound();

            specificationGroup.IsDeleted = true;
            specificationGroup.DeletedBy = "system";
            specificationGroup.DeletedAt = DateTime.UtcNow.AddHours(4);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        
    }
}
