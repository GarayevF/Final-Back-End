using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Smartelectronics.DataAccessLayer;
using Smartelectronics.Extensions;
using Smartelectronics.Helpers;
using Smartelectronics.Models;
using Smartelectronics.ViewModels;

namespace Smartelectronics.Areas.Manage.Controllers
{
    [Area("Manage")]
    [Authorize(Roles = "SuperAdmin, Admin")]
    public class SliderController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public SliderController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<IActionResult> Index(int pageIndex = 1)
        {
            IQueryable<Slider> query = _context.Sliders
                .Where(b => b.IsDeleted == false);

            return View(PageNatedList<Slider>.Create(query, pageIndex, 3, 3));
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Slider slider)
        {
            if (!ModelState.IsValid)
            {
                return View(slider);
            }

            if (slider.MainFile != null)
            {
                if (slider.MainFile.CheckFileContenttype("image/jpeg"))
                {
                    ModelState.AddModelError("MainFile", $"{slider.MainFile.FileName} adli fayl novu duzgun deyil");
                    return View(slider);
                }

                if (slider.MainFile.CheckFileLength(5000))
                {
                    ModelState.AddModelError("MainFile", $"{slider.MainFile.FileName} adli fayl hecmi coxdur");
                    return View(slider);
                }

                slider.Image = await slider.MainFile.CreateFileAsync(_env, "assets", "images", "sliders");
            }
            else
            {
                ModelState.AddModelError("MainFile", "Image mutleqdir");
                return View(slider);
            }

            slider.CreatedAt = DateTime.UtcNow.AddHours(4);
            slider.CreatedBy = "System";

            await _context.Sliders.AddAsync(slider);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));

        }

        


        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return BadRequest();

            Slider slider = await _context.Sliders
                .FirstOrDefaultAsync(b => b.Id == id && b.IsDeleted == false);

            if (slider == null) return NotFound();

            slider.IsDeleted = true;
            slider.DeletedBy = "system";
            slider.DeletedAt = DateTime.UtcNow.AddHours(4);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));

        }
    }
}
