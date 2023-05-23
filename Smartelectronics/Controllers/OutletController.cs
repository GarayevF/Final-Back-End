using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Smartelectronics.DataAccessLayer;
using Smartelectronics.Models;
using Smartelectronics.ViewModels.HomeViewModels;

namespace Smartelectronics.Controllers
{
    public class OutletController : Controller
    {
        private readonly AppDbContext _context;

        public OutletController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {

            IEnumerable<Product> products = await _context.Products.Where(p => p.IsDeleted == false)
                .Include(cb => cb.Category)
                .Include(cb => cb.Brand)
                .Include(p => p.LoanTerms.Where(p => p.IsDeleted == false))
                .ThenInclude(lt => lt.LoanTermLoanRanges).ThenInclude(ltlr => ltlr.LoanRange)
                .Include(p => p.LoanTerms).ThenInclude(lt => lt.LoanCompany).Where(p => p.IsDeleted == false)
                .Include(p => p.ProductColors.Where(a => a.IsDeleted == false))
                .ThenInclude(pa => pa.Color).Where(a => a.IsDeleted == false)
                .Include(p => p.ProductLoanRanges.Where(pl => pl.IsDeleted == false)).ThenInclude(plr => plr.LoanRange)
                .ToListAsync();

            return View(products);
        }
    }
}