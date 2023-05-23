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
    public class LoanCompanyController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public LoanCompanyController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<IActionResult> Index(int pageIndex = 1)
        {
            IQueryable<LoanCompany> query = _context.LoanCompanies
                .Where(b => b.IsDeleted == false)
                .OrderByDescending(c => c.Id); 

            return View(PageNatedList<LoanCompany>.Create(query, pageIndex, 3, 3));
        }

        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null) return BadRequest();

            Brand brand = await _context.Brands.Include(b => b.Products.Where(p => p.IsDeleted == false))
                .FirstOrDefaultAsync(b => b.Id == id && b.IsDeleted == false);

            if (brand == null) return NotFound();

            return View(brand);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(LoanCompany loanCompany)
        {
            if (!ModelState.IsValid)
            {
                return View(loanCompany);
            }

            if (await _context.LoanCompanies.AnyAsync(b => b.IsDeleted == false && b.Name.ToLower() == loanCompany.Name.Trim().ToLower()))
            {
                ModelState.AddModelError("Name", $"Bu adda {loanCompany.Name} movcuddur");
            }

            if (loanCompany.LogoFile != null)
            {
                if (loanCompany.LogoFile.CheckFileContenttype("image/jpeg"))
                {
                    ModelState.AddModelError("LogoFile", $"{loanCompany.LogoFile.FileName} adli fayl novu duzgun deyil");
                    return View(loanCompany);
                }

                if (loanCompany.LogoFile.CheckFileLength(5000))
                {
                    ModelState.AddModelError("LogoFile", $"{loanCompany.LogoFile.FileName} adli fayl hecmi coxdur");
                    return View(loanCompany);
                }

                loanCompany.Logo = await loanCompany.LogoFile.CreateFileAsync(_env, "assets", "images", "loancompanies");
            }
            else
            {
                ModelState.AddModelError("LogoFile", "Logo mutleqdir");
                return View(loanCompany);
            }

            if (loanCompany.LabelFile != null)
            {
                if (loanCompany.LabelFile.CheckFileContenttype("image/jpeg"))
                {
                    ModelState.AddModelError("LabelFile", $"{loanCompany.LabelFile.FileName} adli fayl novu duzgun deyil");
                    return View(loanCompany);
                }

                if (loanCompany.LabelFile.CheckFileLength(5000))
                {
                    ModelState.AddModelError("LabelFile", $"{loanCompany.LabelFile.FileName} adli fayl hecmi coxdur");
                    return View(loanCompany);
                }

                loanCompany.LabelImage = await loanCompany.LabelFile.CreateFileAsync(_env, "assets", "images", "loancompanies");
            }
            else
            {
                ModelState.AddModelError("LabelFile", "Label şəkli mutleqdir");
                return View(loanCompany);
            }

            loanCompany.Name = loanCompany.Name.Trim();
            loanCompany.CreatedAt = DateTime.UtcNow.AddHours(4);
            loanCompany.CreatedBy = "System";

            await _context.LoanCompanies.AddAsync(loanCompany);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));

        }

        [HttpGet]
        public async Task<IActionResult> Update(int? id)
        {
            if (id == null) return BadRequest();

            LoanCompany loanCompany = await _context.LoanCompanies.FirstOrDefaultAsync(b => b.Id == id && b.IsDeleted == false);

            if (loanCompany == null) return NotFound();

            return View(loanCompany);

        }

        [HttpPost]
        public async Task<IActionResult> Update(int? id, LoanCompany loanCompany)
        {
            if (!ModelState.IsValid)
            {
                return View(loanCompany);
            }

            if (id == null) return BadRequest();

            if (id != loanCompany.Id) return BadRequest();

            LoanCompany dbloanCompany = await _context.LoanCompanies
                .FirstOrDefaultAsync(p => p.IsDeleted == false && p.Id == id);

            if (dbloanCompany == null) return NotFound();

            if (await _context.LoanCompanies.AnyAsync(b => b.IsDeleted == false && b.Name.ToLower() == loanCompany.Name.Trim().ToLower() && b.Id != loanCompany.Id))
            {
                ModelState.AddModelError("Name", $"Bu adda {loanCompany.Name} movcuddur");
            }

            if (loanCompany.LogoFile != null)
            {
                if (loanCompany.LogoFile.CheckFileContenttype("image/jpeg"))
                {
                    ModelState.AddModelError("LogoFile", $"{loanCompany.LogoFile.FileName} adli fayl novu duzgun deyil");
                    return View(loanCompany);
                }

                if (loanCompany.LogoFile.CheckFileLength(5000))
                {
                    ModelState.AddModelError("LogoFile", $"{loanCompany.LogoFile.FileName} adli fayl hecmi coxdur");
                    return View(loanCompany);
                }

                FileHelper.DeleteFile(dbloanCompany.Logo, _env, "assets", "images", "loancompanies");
                dbloanCompany.Logo = await loanCompany.LogoFile.CreateFileAsync(_env, "assets", "images", "loancompanies");
            }

            if (loanCompany.LabelFile != null)
            {
                if (loanCompany.LabelFile.CheckFileContenttype("image/jpeg"))
                {
                    ModelState.AddModelError("LabelFile", $"{loanCompany.LabelFile.FileName} adli fayl novu duzgun deyil");
                    return View(loanCompany);
                }

                if (loanCompany.LabelFile.CheckFileLength(5000))
                {
                    ModelState.AddModelError("LabelFile", $"{loanCompany.LabelFile.FileName} adli fayl hecmi coxdur");
                    return View(loanCompany);
                }

                FileHelper.DeleteFile(dbloanCompany.LabelImage, _env, "assets", "images", "loancompanies");
                dbloanCompany.LabelImage = await loanCompany.LabelFile.CreateFileAsync(_env, "assets", "images", "loancompanies");
            }

            

            

            dbloanCompany.Name = loanCompany.Name.Trim();
            dbloanCompany.UpdatedAt = DateTime.UtcNow.AddHours(4);
            loanCompany.UpdatedBy = "System";

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));

        }



        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return BadRequest();

            LoanCompany loanCompany = await _context.LoanCompanies
                .FirstOrDefaultAsync(b => b.Id == id && b.IsDeleted == false);

            if (loanCompany == null) return NotFound();

            loanCompany.IsDeleted = true;
            loanCompany.DeletedBy = "system";
            loanCompany.DeletedAt = DateTime.UtcNow.AddHours(4);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));

        }
    }
}
