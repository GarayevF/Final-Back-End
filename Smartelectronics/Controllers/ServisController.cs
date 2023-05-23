using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Smartelectronics.DataAccessLayer;
using Smartelectronics.Models;
using Smartelectronics.ViewModels.AffiliateViewModels;

namespace Smartelectronics.Controllers
{
    public class ServisController : Controller
    {
		private readonly AppDbContext _context;
		private readonly UserManager<AppUser> _userManager;

		public ServisController(AppDbContext context, UserManager<AppUser> userManager)
		{
			_context = context;
			_userManager = userManager;
		}
		public async Task<IActionResult> Index()
		{
			AppUser appUser = await _userManager.Users
				.Include(u => u.Address)
				.FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);


			Message message = new Message
			{
				Name = appUser.Name,
				Surname = appUser.SurName,
				Mail = appUser.Email,
				Number = appUser.PhoneNumber
			};


			return View(message);
		}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(Message message)
        {
            if (!ModelState.IsValid)
            {
                return View(message);
            }


            if (message == null) return BadRequest();

            if (message.Name == null)
            {
                ModelState.AddModelError("Name", $"Ad mutleqdir");
                return View(message);
            }

            if (message.Surname == null)
            {
                ModelState.AddModelError("Surname", $"Soyad mutleqdir");
                return View(message);
            }

            if (message.Mail == null)
            {
                ModelState.AddModelError("Mail", $"Mail mutleqdir");
                return View(message);
            }

            if (message.Number == null)
            {
                ModelState.AddModelError("Number", $"Nömrə mutleqdir");
                return View(message);
            }

            await _context.Messages.AddAsync(message);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }
    }
}
