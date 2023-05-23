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
    public class SettingController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        public SettingController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Index(int pageIndex = 1)
        {
            IQueryable<Setting> query = _context.Settings
                .OrderByDescending(c => c.Id);

            return View(PageNatedList<Setting>.Create(query, pageIndex, 3, 8));
        }

        [HttpGet]
        [Authorize(Roles = "SuperAdmin, Admin")]
        public async Task<IActionResult> Create()
        {

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "SuperAdmin, Admin")]
        public async Task<IActionResult> Create(Setting setting)
        {
            if (!ModelState.IsValid) return View();

            if (await _context.Settings.AnyAsync(c => c.Key.ToLower() == setting.Key.Trim().ToLower()))
            {
                ModelState.AddModelError("Name", $"Bu {setting.Key} key movcuddur");
                return View(setting);
            }



            setting.Key = setting.Key.Trim();
            setting.Value = setting.Value.Trim();

            await _context.Settings.AddAsync(setting);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Authorize(Roles = "SuperAdmin, Admin")]
        public async Task<IActionResult> Update(int? id)
        {

            if (id == null) return BadRequest();

            Setting setting = await _context.Settings.FirstOrDefaultAsync(c => c.Id == id);

            if (setting == null) return NotFound();

            return View(setting);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "SuperAdmin, Admin")]
        public async Task<IActionResult> Update(int? id, Setting setting)
        {
            if (!ModelState.IsValid) return View(setting);

            if (id == null) return BadRequest();

            if (id != setting.Id) return BadRequest();

            Setting dbSetting = await _context.Settings.FirstOrDefaultAsync(c => c.Id == id);

            if (dbSetting == null) return NotFound();

            if (await _context.Settings.AnyAsync(c => c.Key.ToLower() == setting.Key.Trim().ToLower() && c.Id != setting.Id))
            {
                ModelState.AddModelError("Name", $"Bu adda {setting.Key} key movcuddur");
                return View(setting);
            }


            dbSetting.Key = setting.Key.Trim();
            dbSetting.Value = setting.Value.Trim();

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Authorize(Roles = "SuperAdmin, Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return BadRequest();

            Setting setting = await _context.Settings
                .FirstOrDefaultAsync(c => c.Id == id);

            if (setting == null) return NotFound();

            _context.Settings.Remove(setting);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
