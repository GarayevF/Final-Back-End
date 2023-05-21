using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Smartelectronics.DataAccessLayer;
using Smartelectronics.ViewModels.HomeViewModels;

namespace Smartelectronics.Controllers
{
    public class HomeController : Controller
    {

        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            ViewBag.LoanRanges = await _context.LoanRanges.Where(c => c.IsDeleted == false).ToListAsync();

            HomeVM homeVM = new HomeVM
            {
                Sliders = await _context.Sliders.Where(s => s.IsDeleted == false).ToListAsync(),
                Categories = await _context.Categories.Where(c => c.IsDeleted == false && c.IsMain)
                .Include(c => c.Children.Where(ct => ct.IsDeleted == false && ct.IsMain == false))
                .ThenInclude(c => c.Products.Where(ctb => ctb.IsDeleted == false)).ThenInclude(e => e.Brand).ToListAsync(),
                NewArrival = await _context.Products.Where(c => c.IsDeleted == false && c.IsNewArrival)
                .Include(p=>p.ProductColors).ThenInclude(pc => pc.Color).Take(8)
                .Include(p => p.ProductLoanRanges.Where(pl => pl.IsDeleted == false)).ThenInclude(plr => plr.LoanRange).ToListAsync(),
                MostViewed = await _context.Products.Where(c => c.IsDeleted == false && c.IsMostViewed)
                .Include(p => p.ProductColors).ThenInclude(pc => pc.Color).Take(8)
                .Include(p => p.ProductLoanRanges.Where(pl => pl.IsDeleted == false)).ThenInclude(plr => plr.LoanRange).ToListAsync(),
                DiscountedProducts = await _context.Products.Where(c => c.IsDeleted == false && c.DiscountedPrice > 0)
                .Include(p => p.ProductColors).ThenInclude(pc => pc.Color).Take(8)
                .Include(p => p.ProductLoanRanges.Where(pl => pl.IsDeleted == false)).ThenInclude(plr => plr.LoanRange).ToListAsync(),
            };

            return View(homeVM);
        }
    }
}
