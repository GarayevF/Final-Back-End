using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Smartelectronics.DataAccessLayer;
using Smartelectronics.Models;

namespace Smartelectronics.Controllers
{
	public class CampaignController : Controller
	{
		private readonly AppDbContext _context;
		private readonly UserManager<AppUser> _userManager;

		public CampaignController(AppDbContext context, UserManager<AppUser> userManager)
		{
			_context = context;
			_userManager = userManager;
		}
		public async Task<IActionResult> Index()
		{

			IEnumerable<Campaign>? campaigns = await _context.Campaigns.Where(b => b.IsDeleted == false)
				.ToListAsync();


			return View(campaigns);
		}
	}
}
