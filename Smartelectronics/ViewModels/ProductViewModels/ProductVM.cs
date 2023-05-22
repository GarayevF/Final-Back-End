using Smartelectronics.Models;

namespace Smartelectronics.ViewModels.ProductViewModels
{
    public class ProductVM
    {
        public PageNatedList<Product>? Products { get; set; }
        public IEnumerable<Category>? Categories { get; set; }
        public IEnumerable<Brand>? Brands { get; set; }
        public IEnumerable<Color>? Colors { get; set; }
        public int SortSelect { get; set; }
        public double? MinimumPrice { get; set; }
        public double? MaximumPrice { get; set; }
    }
}
