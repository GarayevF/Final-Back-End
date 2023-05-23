using System.ComponentModel.DataAnnotations;

namespace Smartelectronics.Models
{
    public class ProductCategorySpecification : BaseEntity
    {
        public int? ProductId { get; set; }
        public Product? Product { get; set; }
        public int? CategorySpecificationId { get; set; }
        public CategorySpecification? CategorySpecification { get; set; }
        [StringLength(1000)]
        public string? Value { get; set; }
    }
}
