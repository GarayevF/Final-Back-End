using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Smartelectronics.DataAccessLayer;
using Smartelectronics.Models;
using Smartelectronics.ViewModels.OrderViewModels;

namespace Smartelectronics.Controllers
{
    [Authorize(Roles = "Member")]
    public class OrderController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly AppDbContext _context;

        public OrderController(UserManager<AppUser> userManager, AppDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Checkout()
        {
            AppUser appUser = await _userManager.Users
                .Include(u => u.Baskets.Where(b => b.IsDeleted == false)).ThenInclude(b => b.Product)
                .Include(u => u.Address)
                .FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);

            if (appUser.Baskets == null || appUser.Baskets.Count() <= 0)
            {
                return RedirectToAction("index", "home");
            }


			OrderVM orderVM = new OrderVM();

			if (appUser != null && appUser.Address != null)
			{
				orderVM.Order = new Order
				{
					Name = appUser.Name,
					SurName = appUser.SurName,
					Email = appUser.Email,
					OrderAddress = appUser.Address.OrderAddress,
					City = appUser.Address.City,
					AdditionalComment = appUser.Address.AdditionalComment,
					BirthDate = appUser.Address.BirthDate,
					IdSeria = appUser.Address.IdSeria,
					Patronymic = appUser.Address.Patronymic,
					Fin = appUser.Address.Fin,
					Number = appUser.Address.Number,
					Gender = appUser.Address.Gender
				};
			}

			orderVM.Baskets = appUser.Baskets;

			return View(orderVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(Order order)
        {
            AppUser appUser = await _userManager.Users
                .Include(u => u.Baskets.Where(b => b.IsDeleted == false)).ThenInclude(b => b.Product)
                .Include(u => u.Address)
                .Include(u => u.Orders)
                .FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);

            OrderVM orderVM = new OrderVM
            {
                Order = order,
                Baskets = appUser.Baskets
            };

            if (!ModelState.IsValid)
            {
                return View(orderVM);
            }

            order.CreatedAt = DateTime.UtcNow.AddHours(4);
            order.CreatedBy = $"{appUser.Name} {appUser.SurName}";
            order.No = appUser.Orders != null && appUser.Orders.Count > 0 ? appUser.Orders.Last().No + 1 : 1;

            appUser.Orders.Add(order);
            order.OrderItems = new List<OrderItem>();
            foreach (Basket basket in appUser.Baskets)
            {
                basket.IsDeleted = true;

                OrderItem orderItem = new OrderItem
                {
                    CreatedAt = DateTime.UtcNow.AddHours(4),
                    CreatedBy = $"{appUser.Name} {appUser.SurName}",
                    Count = basket.Count,
                    ProductId = basket.ProductId,
                    Price = basket.Product.DiscountedPrice > 0 ? basket.Product.DiscountedPrice : basket.Product.Price,
                };

                order.OrderItems.Add(orderItem);
            }

            HttpContext.Response.Cookies.Append("basket", "");

            await _context.SaveChangesAsync();

            TempData["Success"] = $"{order.No} Sifarisiniz ugurla gonderildi";

            return RedirectToAction("OrderCompleted", "order", new { id = order.Id });
        }

        public async Task<IActionResult> OrderCompleted(int? id)
        {
            Order order = await _context.Orders.Where(a => a.IsDeleted == false)
                .Include(o => o.OrderItems).ThenInclude(o => o.Product)
                .FirstOrDefaultAsync(o => o.IsDeleted == false && o.Id == id);



            return View(order);
        }


    }
}
