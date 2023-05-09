namespace Smartelectronics.Models
{
    public class CategoryBrand : BaseEntity
    {
        public int? CategoryId { get; set; }
        public Category? Category { get; set; }
        public int? BrandId { get; set; }
        public Brand? Brand { get; set; }
        public IEnumerable<Product>? Products { get; set; }
    }
}
