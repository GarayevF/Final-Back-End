using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Smartelectronics.DataAccessLayer;
using Smartelectronics.Models;
using Smartelectronics.ViewModels;
using Smartelectronics.ViewModels.ProductViewModels;

namespace Smartelectronics.Controllers
{
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public ProductController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            int pageIndex = 1;

            IQueryable<Product> products = _context.Products.Where(c => c.IsDeleted == false)
                .Include(cb => cb.Category)
                .Include(cb => cb.Brand)
                .Include(p => p.ProductColors.Where(a => a.IsDeleted == false))
                 .ThenInclude(pa => pa.Color).Where(a => a.IsDeleted == false)
                .Include(p => p.ProductLoanRanges.Where(pl => pl.IsDeleted == false)).ThenInclude(plr => plr.LoanRange);

            IEnumerable<Category> categories = await _context.Categories.Where(c => c.IsDeleted == false && c.IsMain)
                .Include(c => c.Children.Where(ct => ct.IsDeleted == false && ct.IsMain == false))
                .ThenInclude(cb => cb.Products.Where(cb => cb.IsDeleted == false)).ToListAsync();

            IEnumerable<Brand> brands = await _context.Brands.Where(c => c.IsDeleted == false)
                .Include(cb => cb.Products.Where(cb => cb.IsDeleted == false)).ToListAsync();

            Product product = _context.Products.OrderBy(p => (p.DiscountedPrice > 0 ? p.DiscountedPrice : p.Price)).First();
            double minValue = (product.DiscountedPrice > 0 ? product.DiscountedPrice : product.Price);

            product = _context.Products.OrderByDescending(p => (p.DiscountedPrice > 0 ? p.DiscountedPrice : p.Price)).First();
            double maxValue = (product.DiscountedPrice > 0 ? product.DiscountedPrice : product.Price);

            ProductVM productVM = new ProductVM
            {
                Products = PageNatedList<Product>.Create(products, pageIndex, 12, 7),
                Categories = categories,
                Brands = brands,
                AllProducts = products/*.ToList()*/,
                SortSelect = 0,
                MinimumPrice = minValue,
                MaximumPrice = maxValue
            };

            return View(productVM);
        }

        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null) return BadRequest();

            Product productTemp = await _context.Products.FirstOrDefaultAsync(x => x.Id == id && x.IsDeleted == false);

            if (productTemp == null) return NotFound();

            Product product = await _context.Products
                .Include(cb => cb.Category).ThenInclude(ct => ct.Parent).Where(b => b.IsDeleted == false)
                .Include(cb => cb.Brand)
                .Include(p => p.ProductColors.Where(a => a.IsDeleted == false && a.ProductId == id))
                 .ThenInclude(pa => pa.Color).Where(a => a.IsDeleted == false)
                .Include(p => p.LoanTerms.Where(p => p.IsDeleted == false))
                .ThenInclude(lt => lt.LoanTermLoanRanges).ThenInclude(ltlr => ltlr.LoanRange)
                .Include(p => p.LoanTerms).ThenInclude(lt => lt.LoanCompany).Where(p => p.IsDeleted == false)
                .Include(p => p.ProductCategorySpecifications.Where(p => p.IsDeleted == false && p.ProductId == id))
                .ThenInclude(pcs => pcs.CategorySpecification).ThenInclude(cs => cs.Specification).ThenInclude(s => s.SpecificationGroup)
                .Include(p => p.ProductCategorySpecifications).ThenInclude(pcs => pcs.CategorySpecification)
                .ThenInclude(cs => cs.Category)
                .Include(p => p.ProductLoanRanges.Where(pl => pl.IsDeleted == false)).ThenInclude(plr => plr.LoanRange)
                .Include(p => p.ProductIFLoanRanges.Where(pl => pl.IsDeleted == false)).ThenInclude(plr => plr.LoanRange)
                .FirstOrDefaultAsync(p => p.IsDeleted == false && p.Id == id);

            if (product == null) return NotFound();

            List<ProductCategorySpecification> productCategorySpecifications = await _context.ProductCategorySpecifications
            .Include(pcs => pcs.CategorySpecification).ThenInclude(cs => cs.Specification).ThenInclude(s => s.SpecificationGroup)
            .Where(pcs => pcs.ProductId == id).ToListAsync();


            List<GroupedSpecificationsVM> groupedSpecifications = productCategorySpecifications
            .GroupBy(s => s.CategorySpecification?.Specification?.SpecificationGroup)
            .Select(g => new GroupedSpecificationsVM
            {
                GroupName = g.Key?.Name,
                GroupIcon = g.Key?.Icon,
                Specifications = g.Where(spec => spec != null)
                    .Select(spec => SpecificationViewModelMapper.Map(spec))
                    .ToList()
            })
            .ToList();

            DetailVM detailVM = new DetailVM
            {
                Product = product,
                GroupedSpecificationsVMs = groupedSpecifications,
                LoanRanges = product.ProductLoanRanges,
                IFLoanRanges = product.ProductIFLoanRanges
            };

            return View(detailVM);
        }

        //public async List<Specification> getSpecifications(int? id)
        //{
        //    if (id == null) return BadRequest();

        //    Category category = await _context.Categories.FirstOrDefaultAsync(c => c.IsDeleted == false && c.Id == id);

        //    List<Specification> specifications = category..Specifications.Where(s => s.IsDeleted == false && s.);
        //}

        
        public async Task<IActionResult> ChangeColorForSlider(int? id, int? colorId)
        {
            if (colorId == null) return BadRequest();

            IEnumerable<ProductColor> productColors = await _context.ProductColors.Where(pc => pc.IsDeleted == false && pc.ProductId == id && pc.ColorId == colorId).ToListAsync();

            return PartialView("_ProductImagesSlider", productColors);
        }

        public async Task<IActionResult> ChangeColor(int? productColorId)
        {
            if (productColorId == null) return BadRequest();

            ProductColor productColor = await _context.ProductColors.FirstOrDefaultAsync(pc => pc.IsDeleted == false && pc.Id == productColorId);

            if (productColor == null) return NotFound();

            return PartialView("_ProductImage", productColor);
        }

        public async Task<IActionResult> Filter(int? categoryId, int? brandId, int? colorId, double? min, double? max, int? sortby, int pageIndex = 1)
        {
            IEnumerable<Product> products = await _context.Products
        .Where(p => (!categoryId.HasValue || p.CategoryId == categoryId.Value) &&
                    (!brandId.HasValue || p.BrandId == brandId.Value) &&
                    (!colorId.HasValue || p.ProductColors.Any(c => c.ColorId == colorId.Value)) &&
                    (!min.HasValue || p.Price >= min.Value) &&
                    (!max.HasValue || p.Price <= max.Value))
        .Include(p => p.Category)
        .Include(p => p.Brand)
        .Include(p => p.ProductColors)
            .ThenInclude(pc => pc.Color)
        .ToListAsync();

            ProductVM productVM = new ProductVM
            {
                Products = PageNatedList<Product>.Create(products.AsQueryable(), pageIndex, 12, 7),
            };

            return PartialView("_ShopPaginationPartial", productVM);
        }
    }
}
