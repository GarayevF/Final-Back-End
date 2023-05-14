using Smartelectronics.Models;

namespace Smartelectronics.ViewModels.CompareViewModels
{
    public class CompareVM
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Image { get; set; }
        public double Price { get; set; }
        public double Star { get; set; }
        public Category? Category { get; set; }
        public Brand? Brand { get; set; }
        public IEnumerable<ProductCategorySpecification>? ProductCategorySpecifications { get; set; }
    }
}
