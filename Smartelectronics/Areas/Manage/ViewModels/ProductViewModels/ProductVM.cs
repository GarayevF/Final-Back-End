using Smartelectronics.Models;

namespace Smartelectronics.Areas.Manage.ViewModels.ProductViewModels
{
    public class ProductVM
    {
        public Product Product { get; set; }
        public List<SpecificationVM> SpecificationVMs { get; set; }
        public int CategoryId { get; set; }
        public IEnumerable<int> ColorIds { get; set; }
        public List<ColorImageVM> ColorImageVMs { get; set; }
    }
}
