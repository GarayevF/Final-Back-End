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
    public class CampaignController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public CampaignController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> Index(int pageIndex = 1)
        {

            IQueryable<Campaign>? queries = _context.Campaigns
                .Where(p => p.IsDeleted == false);

            return View(PageNatedList<Campaign>.Create(queries, pageIndex, 3, 5));
        }

        [HttpGet]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> Create()
        {

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> Create(Campaign campaign)
        {

            if (!ModelState.IsValid)
            {
                return View(campaign);
            }

            if (campaign.File != null)
            {
                if (campaign.File.CheckFileContenttype("image/jpeg"))
                {
                    ModelState.AddModelError("MainFile", $"{campaign.File.FileName} adli fayl novu duzgun deyil");
                    return View(campaign);
                }

                if (campaign.File.CheckFileLength(300))
                {
                    ModelState.AddModelError("MainFile", $"{campaign.File.FileName} adli fayl hecmi coxdur");
                    return View(campaign);
                }

                campaign.Image = await campaign.File.CreateFileAsync(_env, "assets", "images", "campaigns");
            }
            else
            {
                ModelState.AddModelError("File", "File mutleqdir");
                return View(campaign);
            }

            campaign.CreatedAt = DateTime.UtcNow.AddHours(4);
            campaign.CreatedBy = "System";


            await _context.Campaigns.AddAsync(campaign);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> Update(int? id)
        {
            if (id == null) return BadRequest();

            Campaign campaign = await _context.Campaigns
                .FirstOrDefaultAsync(p => p.Id == id && p.IsDeleted == false);

            if (campaign == null) return NotFound();


            return View(campaign);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> Update(int? id, Campaign campaign)
        {

            if (!ModelState.IsValid)
            {
                return View(campaign);
            }

            if (id == null) return BadRequest();

            if (id != campaign.Id) return BadRequest();

            Campaign dbcampaign = await _context.Campaigns
                .FirstOrDefaultAsync(p => p.Id == id && p.IsDeleted == false);

            if (dbcampaign == null) return NotFound();

            if (campaign.File != null)
            {
                if (campaign.File.CheckFileContenttype("image/jpeg"))
                {
                    ModelState.AddModelError("File", $"{campaign.File.FileName} adli fayl novu duzgun deyil");
                    return View(campaign);
                }

                if (campaign.File.CheckFileLength(300))
                {
                    ModelState.AddModelError("File", $"{campaign.File.FileName} adli fayl hecmi coxdur");
                    return View(campaign);
                }

                FileHelper.DeleteFile(dbcampaign.Image, _env, "assets", "images", "campaigns");

                dbcampaign.Image = await campaign.File.CreateFileAsync(_env, "assets", "images", "campaigns");
            }

            dbcampaign.Title = campaign.Title;
            dbcampaign.Desc = campaign.Desc;

            dbcampaign.UpdatedBy = "System";
            dbcampaign.UpdatedAt = DateTime.UtcNow.AddHours(4);

            await _context.SaveChangesAsync();


            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return BadRequest();

            Campaign campaign = await _context.Campaigns
                .FirstOrDefaultAsync(c => c.Id == id);

            if (campaign == null) return NotFound();

            campaign.IsDeleted = true;
            campaign.DeletedBy = "System";
            campaign.DeletedAt = DateTime.UtcNow.AddHours(4);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

    }
}
