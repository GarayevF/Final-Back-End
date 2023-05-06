using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Smartelectronics.DataAccessLayer;
using Smartelectronics.Interfaces;
using Smartelectronics.Models;
using Smartelectronics.ViewModels.BasketViewModels;
using Smartelectronics.ViewModels.CompareViewModels;
using Smartelectronics.ViewModels.WishlistViewModels;

namespace Smartelectronics.Services
{
    public class LayoutService : ILayoutService
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<AppUser> _userManager;

        public LayoutService(AppDbContext context, IHttpContextAccessor httpContextAccessor, UserManager<AppUser> userManager)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }

        public async Task<List<BasketVM>> GetBaskets()
        {
            AppUser appUser = null;
            List<Basket> baskets = null;
            if (_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated &&
                _httpContextAccessor.HttpContext.User.IsInRole("Member"))
            {
                appUser = await _userManager.Users
                    .Include(u => u.Baskets.Where(b => b.IsDeleted == false)).ThenInclude(b => b.Product)
                    .FirstOrDefaultAsync(u => u.UserName == _httpContextAccessor.HttpContext.User.Identity.Name);

                baskets = appUser.Baskets;
            }

            string cookie = _httpContextAccessor.HttpContext.Request.Cookies["basket"];

            if (!string.IsNullOrWhiteSpace(cookie))
            {
                List<BasketVM> basketVMs = null;
                if (baskets != null && baskets.Count() > 0)
                {
                    basketVMs = new List<BasketVM>();
                    foreach (Basket basket in baskets)
                    {
                        Product product = basket.Product;

                        if (product != null)
                        {
                            BasketVM basketVM = new BasketVM();

                            basketVM.Id = product.Id;
                            basketVM.Count = basket.Count;
                            basketVM.Title = product.Title;
                            basketVM.Price = product.DiscountedPrice > 0 ? product.DiscountedPrice : product.Price;
                            basketVM.Image = product.MainImage;

                            basketVMs.Add(basketVM);
                        }
                    }
                }
                else
                {
                    basketVMs = JsonConvert.DeserializeObject<List<BasketVM>>(cookie);
                    foreach (BasketVM basketVM1 in basketVMs)
                    {
                        Product product = await _context.Products.FirstOrDefaultAsync(p => p.Id == basketVM1.Id);

                        if (product != null)
                        {
                            basketVM1.Title = product.Title;
                            basketVM1.Price = product.DiscountedPrice > 0 ? product.DiscountedPrice : product.Price;
                            basketVM1.Image = product.MainImage;
                        }
                    }
                }

                return basketVMs;
            }

            return new List<BasketVM>();
        }

        public async Task<List<WishlistVM>> GetWishLists()
        {
            AppUser appUser = null;
            List<Wishlist> wishlists = null;
            if (_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated &&
                _httpContextAccessor.HttpContext.User.IsInRole("Member"))
            {
                appUser = await _userManager.Users
                    .Include(u => u.Wishlists.Where(b => b.IsDeleted == false)).ThenInclude(b => b.Product)
                    .FirstOrDefaultAsync(u => u.UserName == _httpContextAccessor.HttpContext.User.Identity.Name);

                wishlists = appUser.Wishlists;
            }

            string cookie = _httpContextAccessor.HttpContext.Request.Cookies["wishlist"];

            if (!string.IsNullOrWhiteSpace(cookie))
            {
                List<WishlistVM> wishlistVMs = null;
                if (wishlists != null && wishlists.Count() > 0)
                {
                    wishlistVMs = new List<WishlistVM>();
                    foreach (Wishlist wishlist in wishlists)
                    {
                        Product product = wishlist.Product;

                        if (product != null)
                        {
                            WishlistVM wishlistVM = new WishlistVM();

                            wishlistVM.Id = product.Id;
                            wishlistVM.Title = product.Title;
                            wishlistVM.Price = product.DiscountedPrice > 0 ? product.DiscountedPrice : product.Price;
                            wishlistVM.Image = product.MainImage;

                            wishlistVMs.Add(wishlistVM);
                        }
                    }
                }
                else
                {
                    wishlistVMs = JsonConvert.DeserializeObject<List<WishlistVM>>(cookie);
                    foreach (WishlistVM wishlistVM in wishlistVMs)
                    {
                        Product product = await _context.Products.FirstOrDefaultAsync(p => p.Id == wishlistVM.Id);

                        if (product != null)
                        {
                            wishlistVM.Title = product.Title;
                            wishlistVM.Price = product.DiscountedPrice > 0 ? product.DiscountedPrice : product.Price;
                            wishlistVM.Image = product.MainImage;
                        }
                    }
                }

                return wishlistVMs;
            }

            return new List<WishlistVM>();
        }

        public async Task<List<CompareVM>> GetCompares()
        {
            List<Compare> compares = null;

            string cookie = _httpContextAccessor.HttpContext.Request.Cookies["compare"];

            if (!string.IsNullOrWhiteSpace(cookie))
            {
                List<CompareVM> compareVMs = null;
                if (compares != null && compares.Count() > 0)
                {
                    compareVMs = new List<CompareVM>();
                    foreach (Compare compare in compares)
                    {
                        Product product = compare.Product;

                        if (product != null)
                        {
                            CompareVM compareVM = new CompareVM();

                            compareVM.Id = product.Id;
                            compareVM.Title = product.Title;
                            compareVM.Price = product.DiscountedPrice > 0 ? product.DiscountedPrice : product.Price;
                            compareVM.Image = product.MainImage;

                            compareVMs.Add(compareVM);
                        }
                    }
                }
                else
                {
                    compareVMs = JsonConvert.DeserializeObject<List<CompareVM>>(cookie);
                    foreach (CompareVM compareVM in compareVMs)
                    {
                        Product product = await _context.Products.FirstOrDefaultAsync(p => p.Id == compareVM.Id);

                        if (product != null)
                        {
                            compareVM.Title = product.Title;
                            compareVM.Price = product.DiscountedPrice > 0 ? product.DiscountedPrice : product.Price;
                            compareVM.Image = product.MainImage;
                        }
                    }
                }

                return compareVMs;
            }

            return new List<CompareVM>();
        }

        public async Task<IEnumerable<Category>> GetCategories()
        {
            return await _context.Categories
                .Include(c => c.Children).Where(c => c.IsDeleted == false)
                .Where(c => c.IsDeleted == false && c.IsMain).ToListAsync();
        }

        public async Task<IDictionary<string, string>> GetSettings()
        {
            IDictionary<string, string> settings = await _context.Settings.ToDictionaryAsync(s => s.Key, s => s.Value);

            return settings;
        }
    }
}
