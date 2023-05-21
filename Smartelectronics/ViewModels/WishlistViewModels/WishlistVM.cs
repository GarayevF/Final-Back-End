using Smartelectronics.Areas.Manage.ViewModels.ProductViewModels;
using Smartelectronics.Models;

namespace Smartelectronics.ViewModels.WishlistViewModels
{
    public class WishlistVM
    {
        public string? Title { get; set; }
        public string? BrandName { get; set; }
        public int Id { get; set; }
        public Product Product { get; set; }
        public IEnumerable<ProductColor> ProductColors { get; set; }
        public IEnumerable<LoanTerm>? LoanTerms { get; set; }
        public double Price { get; set; }
        public double DiscountedPrice { get; set; }
        public IEnumerable<ProductLoanRange>? ProductLoanRanges { get; set; }
    }
}
