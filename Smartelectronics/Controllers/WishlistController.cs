using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Smartelectronics.DataAccessLayer;
using Smartelectronics.Models;
using Smartelectronics.ViewModels.WishlistViewModels;

namespace Smartelectronics.Controllers
{
    public class WishlistController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public WishlistController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            string cookie = HttpContext.Request.Cookies["wishlist"];
            List<WishlistVM> wishlistVMs = null;

            if (!string.IsNullOrWhiteSpace(cookie))
            {
                wishlistVMs = JsonConvert.DeserializeObject<List<WishlistVM>>(cookie);

                foreach (WishlistVM wishlistVM in wishlistVMs)
                {
                    Product product = await _context.Products.FirstOrDefaultAsync(p => p.IsDeleted == false && p.Id == wishlistVM.Id);

                    if (product != null)
                    {
                        wishlistVM.Title = product.Title;
                        wishlistVM.Price = product.DiscountedPrice > 0 ? product.DiscountedPrice : product.Price;
                        wishlistVM.Image = product.ProductColors.FirstOrDefault().Image;
                    }
                }
            }

            return View(wishlistVMs);
        }

        public async Task<IActionResult> DeleteWishlist(int? id)
        {
            if (id == null) return BadRequest();

            if (!await _context.Products.AnyAsync(p => p.IsDeleted == false && p.Id == id)) return NotFound();

            string cookie = HttpContext.Request.Cookies["wishlist"];

            List<WishlistVM> wishlistVMs = null;

            if (!string.IsNullOrWhiteSpace(cookie))
            {
                wishlistVMs = JsonConvert.DeserializeObject<List<WishlistVM>>(cookie);

                if (wishlistVMs.Exists(p => p.Id == id))
                {
                    wishlistVMs.Remove(wishlistVMs.FirstOrDefault(b => b.Id == id));

                }
                else
                {
                    return BadRequest();
                }
            }

            foreach (WishlistVM wishlistVM in wishlistVMs)
            {
                Product product = await _context.Products.FirstOrDefaultAsync(p => p.IsDeleted == false && p.Id == wishlistVM.Id);

                if (product != null)
                {
                    wishlistVM.Title = product.Title;
                    wishlistVM.Price = product.DiscountedPrice > 0 ? product.DiscountedPrice : product.Price;
                    wishlistVM.Image = product.MainImage;
                }
            }

            cookie = JsonConvert.SerializeObject(wishlistVMs);
            HttpContext.Response.Cookies.Append("wishlist", cookie);

            return PartialView("_WishlistMainPartial", wishlistVMs);
        }

        public async Task<IActionResult> AddWishlist(int? id)
        {
            if (id == null) return BadRequest();

            if (!await _context.Products.AnyAsync(p => p.IsDeleted == false && p.Id == id)) return NotFound();


            string cookie = HttpContext.Request.Cookies["wishlist"];

            List<WishlistVM> wishlistVMs = null;

            if (string.IsNullOrWhiteSpace(cookie))
            {
                wishlistVMs = new List<WishlistVM>()
                {
                    new WishlistVM
                    {
                        Id = (int)id,
                    }
                };
            }
            else
            {
                wishlistVMs = JsonConvert.DeserializeObject<List<WishlistVM>>(cookie);

                if (!wishlistVMs.Exists(p => p.Id == id))
                {
                    wishlistVMs.Add(new WishlistVM { Id = (int)id });
                }

            }

            if (User.Identity.IsAuthenticated && User.IsInRole("Member"))
            {
                AppUser appUser = await _userManager.Users
                    .Include(u => u.Wishlists.Where(b => b.IsDeleted == false))
                    .FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);

                if (appUser.Wishlists != null && appUser.Wishlists.Count() > 0)
                {
                    if (!appUser.Wishlists.Any(b => b.ProductId == id))
                    {
                        Wishlist wishlist = new Wishlist
                        {
                            ProductId = id,
                        };

                        appUser.Wishlists.Add(wishlist);
                    }
                }
                else
                {
                    Wishlist wishlist = new Wishlist
                    {
                        ProductId = id,
                    };

                    appUser.Wishlists.Add(wishlist);
                }

                await _context.SaveChangesAsync();
            }

            cookie = JsonConvert.SerializeObject(wishlistVMs);
            HttpContext.Response.Cookies.Append("wishlist", cookie);

            //foreach (WishlistVM wishlistVM in wishlistVMs)
            //{
            //    Product product = await _context.Products.FirstOrDefaultAsync(p => p.IsDeleted == false && p.Id == wishlistVM.Id);

            //    if (product != null)
            //    {
            //        wishlistVM.Title = product.Title;
            //        wishlistVM.Price = product.DiscountedPrice > 0 ? product.DiscountedPrice : product.Price;
            //        wishlistVM.Image = product.MainImage;
            //        wishlistVM.ExTax = product.ExTax;
            //    }
            //}

            return Ok();
        }
    }
}
