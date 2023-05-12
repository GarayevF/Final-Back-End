using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Smartelectronics.DataAccessLayer;
using Smartelectronics.Models;
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
            return View();
        }

        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null) return BadRequest();

            Product productTemp = await _context.Products.FirstOrDefaultAsync(x => x.Id == id && x.IsDeleted == false);

            if (productTemp == null) return NotFound();

            Product product = await _context.Products
                .Include(p => p.CategoryBrand).ThenInclude(cb => cb.Category).ThenInclude(ct => ct.Parent).Where(b => b.IsDeleted == false)
                .Include(p => p.CategoryBrand).ThenInclude(cb => cb.Brand)
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
                    GroupName = g.Key.Name,
                    GroupIcon = g.Key.Icon,
                    Specifications = g.Select(spec => SpecificationViewModelMapper.Map(spec)).ToList()
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

        
    }
}
