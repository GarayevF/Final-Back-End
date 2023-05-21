using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Smartelectronics.DataAccessLayer;
using Smartelectronics.Models;
using Smartelectronics.ViewModels.BasketViewModels;
using Smartelectronics.ViewModels.CompareViewModels;
using Smartelectronics.ViewModels.WishlistViewModels;

namespace Smartelectronics.Controllers
{
    public class CompareController : Controller
    {
        private readonly AppDbContext _context;

        public CompareController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            string cookie = HttpContext.Request.Cookies["compare"];
            List<CompareVM> compareVMs = null;

            if (!string.IsNullOrWhiteSpace(cookie))
            {
                compareVMs = JsonConvert.DeserializeObject<List<CompareVM>>(cookie);

                foreach (CompareVM compareVM in compareVMs)
                {
                    Product product = await _context.Products
                        .Include(p => p.Category)
                        .Include(p => p.ProductColors.Where(a => a.IsDeleted == false && a.ProductId == compareVM.Id))
                        .Include(p => p.LoanTerms).ThenInclude(lt => lt.LoanCompany).Where(p => p.IsDeleted == false)
                        .Include(p => p.ProductCategorySpecifications.Where(p => p.IsDeleted == false && p.ProductId == compareVM.Id))
                        .ThenInclude(pcs => pcs.CategorySpecification).ThenInclude(cs => cs.Specification).ThenInclude(s => s.SpecificationGroup)
                        .Include(p => p.ProductCategorySpecifications).ThenInclude(pcs => pcs.CategorySpecification)
                        .FirstOrDefaultAsync(p => p.IsDeleted == false && p.Id == compareVM.Id);

                    if (product != null)
                    {
                        compareVM.Title = product.Title;
                        compareVM.Price = product.Price;
                        compareVM.DiscountedPrice = product.DiscountedPrice;
                        compareVM.Image = product?.ProductColors?.FirstOrDefault()?.Image;
                        compareVM.ProductCategorySpecifications = product?.ProductCategorySpecifications;
                        compareVM.Category = product?.Category;
                        compareVM.LoanTerms = product?.LoanTerms;
                    }
                }
            }
            ViewBag.CategoryFilter = compareVMs?.FirstOrDefault()?.Category.Id;
            return View(compareVMs);
        }

        public async Task<IActionResult> AddCompare(int? id)
        {
            if (id == null) return BadRequest();

            if (!await _context.Products.AnyAsync(p => p.IsDeleted == false && p.Id == id)) return NotFound();

            string cookie = HttpContext.Request.Cookies["compare"];

            List<CompareVM> compareVMs = null;

            if (string.IsNullOrWhiteSpace(cookie))
            {
                compareVMs = new List<CompareVM>()
                {
                    new CompareVM
                    {
                        Id = (int)id,
                    }
                };
            }
            else
            {
                compareVMs = JsonConvert.DeserializeObject<List<CompareVM>>(cookie);

                if (!compareVMs.Exists(p => p.Id == id))
                {
                    compareVMs.Add(new CompareVM { Id = (int)id });
                }
                else
                {
                    compareVMs.Remove(compareVMs.FirstOrDefault(b => b.Id == id));
                }

            }

            cookie = JsonConvert.SerializeObject(compareVMs);
            HttpContext.Response.Cookies.Append("compare", cookie);

            foreach (CompareVM compareVM in compareVMs)
            {
                Product product = await _context.Products
                    .Include(cb => cb.Category)
                    .Include(p => p.ProductColors.Where(a => a.IsDeleted == false && a.ProductId == compareVM.Id))
                    .Include(p => p.LoanTerms).ThenInclude(lt => lt.LoanCompany).Where(p => p.IsDeleted == false)
                    .Include(p => p.ProductCategorySpecifications.Where(p => p.IsDeleted == false && p.ProductId == compareVM.Id))
                    .ThenInclude(pcs => pcs.CategorySpecification).ThenInclude(cs => cs.Specification).ThenInclude(s => s.SpecificationGroup)
                    .Include(p => p.ProductCategorySpecifications).ThenInclude(pcs => pcs.CategorySpecification)
                    .FirstOrDefaultAsync(p => p.IsDeleted == false && p.Id == compareVM.Id);

                if (product != null)
                {
                    compareVM.Title = product.Title;
                    compareVM.Price = product.Price;
                    compareVM.DiscountedPrice = product.DiscountedPrice;
                    compareVM.Image = product?.ProductColors?.FirstOrDefault()?.Image;
                    compareVM.ProductCategorySpecifications = product?.ProductCategorySpecifications;
                    compareVM.Category = product?.Category;
                    compareVM.LoanTerms = product?.LoanTerms;
                }
            }
            ViewBag.CategoryFilter = compareVMs?.FirstOrDefault()?.Category.Id;
            //succes toastr mesaji elave et
            return PartialView("_CompareMainPartial", compareVMs);
        }

        public async Task<IActionResult> GetCompareByCategory(int? id)
        {

            if (id == null) return BadRequest();

            Category category = await _context.Categories.FirstOrDefaultAsync(c => c.IsDeleted == false && c.Id == id);

            if (category == null) return NotFound();

            string cookie = HttpContext.Request.Cookies["compare"];

            List<CompareVM> compareVMs = null;

            if (!string.IsNullOrWhiteSpace(cookie))
            {
                compareVMs = JsonConvert.DeserializeObject<List<CompareVM>>(cookie);

                foreach (CompareVM compareVM in compareVMs)
                {
                    Product product = await _context.Products
                        .Include(p => p.Category)
                        .Include(p => p.ProductColors.Where(a => a.IsDeleted == false && a.ProductId == compareVM.Id))
                        .Include(p => p.LoanTerms).ThenInclude(lt => lt.LoanCompany).Where(p => p.IsDeleted == false)
                        .Include(p => p.ProductCategorySpecifications.Where(p => p.IsDeleted == false && p.ProductId == compareVM.Id))
                        .ThenInclude(pcs => pcs.CategorySpecification).ThenInclude(cs => cs.Specification).ThenInclude(s => s.SpecificationGroup)
                        .Include(p => p.ProductCategorySpecifications).ThenInclude(pcs => pcs.CategorySpecification)
                        .FirstOrDefaultAsync(p => p.IsDeleted == false && p.Id == compareVM.Id);

                    if (product != null)
                    {
                        compareVM.Title = product.Title;
                        compareVM.Price = product.Price;
                        compareVM.DiscountedPrice = product.DiscountedPrice;
                        compareVM.Image = product?.ProductColors?.FirstOrDefault()?.Image;
                        compareVM.ProductCategorySpecifications = product?.ProductCategorySpecifications;
                        compareVM.Category = product?.Category;
                        compareVM.LoanTerms = product?.LoanTerms;
                    }
                }
            }
            ViewBag.CategoryFilter = compareVMs?.FirstOrDefault(p => p.Category.Id == id)?.Category?.Id;

            return PartialView("_CompareMainPartial", compareVMs);
        }

        public int GetBasketCount()
        {
            string cookie = HttpContext.Request.Cookies["compare"];

            List<CompareVM> compareVMs = null;

            if (string.IsNullOrWhiteSpace(cookie))
            {
                return 0;
            }
            else
            {
                compareVMs = JsonConvert.DeserializeObject<List<CompareVM>>(cookie);

                if(compareVMs != null && compareVMs.Count() > 0)
                {
                    return compareVMs.Count();
                }

                return 0;

            }
        }
    }
}
