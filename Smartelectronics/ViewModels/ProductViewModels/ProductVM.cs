using Smartelectronics.Models;

namespace Smartelectronics.ViewModels.ProductViewModels
{
    public class ProductVM
    {
        public PageNatedList<Product>? Products { get; set; }
        public IEnumerable<Category>? Categories { get; set; }
        public IEnumerable<Product>? AllProducts { get; set; }
        public int SortSelect { get; set; }
        public double? MinimumPrice { get; set; }
        public double? MaximumPrice { get; set; }
    }
}
