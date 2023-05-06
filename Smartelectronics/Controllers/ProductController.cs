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
                .Include(p => p.Category).ThenInclude(ct => ct.Parent).Where(b => b.IsDeleted == false)
                .Include(p => p.Brand)
                .Include(p => p.ProductColors.Where(a => a.IsDeleted == false && a.ProductId == id))
                 .ThenInclude(pa => pa.Color).Where(a => a.IsDeleted == false)
                .Include(p => p.LoanTerms.Where(p => p.IsDeleted == false))
                .ThenInclude(lt => lt.LoanTermLoanRanges).ThenInclude(ltlr => ltlr.LoanRange)
                .Include(p => p.LoanTerms).ThenInclude(lt => lt.LoanCompany).Where(p => p.IsDeleted == false)
                .Include(p => p.ProductCategorySpecifications.Where(p => p.IsDeleted == false && p.ProductId == id))
                .ThenInclude(pcs => pcs.CategorySpecification).Where(a => a.IsDeleted == false && a.CategoryId == productTemp.CategoryId)
                .FirstOrDefaultAsync(p => p.IsDeleted == false && p.Id == id);

            if (product == null) return NotFound();

            DetailVM detailVM = new DetailVM
            {
                Product = product,
            };

            return View(detailVM);
        }
    }
}
