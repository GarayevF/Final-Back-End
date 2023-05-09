using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Smartelectronics.Models
{
    public class Category : BaseEntity
    {
        [StringLength(255)]
        public string Name { get; set; }
        [StringLength(1000)]
        public string? Icon { get; set; }
        public bool IsMain { get; set; }
        public int? ParentId { get; set; }
        public Category? Parent { get; set; }
        public IEnumerable<Category>? Children { get; set; }
        //public IEnumerable<Product>? Products { get; set; }
        public List<CategoryBrand>? CategoryBrands { get; set; }
        [NotMapped]
        public IEnumerable<int>? BrandIds { get; set; }
    }
}
