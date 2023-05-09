using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Smartelectronics.Models
{
    public class Product : BaseEntity
    {
        [StringLength(255)]
        public string Title { get; set; }
        [Column(TypeName = "money")]
        public double Price { get; set; }
        [Column(TypeName = "money")]
        public double DiscountedPrice { get; set; }
        public int Count { get; set; }

        [StringLength(4)]
        public string? Seria { get; set; }
        public int? Code { get; set; }
        public bool IsNewArrival { get; set; }
        public bool IsMostViewed { get; set; }
        [StringLength(255)]
        public string? MainImage { get; set; }
        public List<LoanTerm>? LoanTerms { get; set; }
        public List<ProductIFLoanRange>? ProductIFLoanRanges { get; set; }
        public List<ProductLoanRange>? ProductLoanRanges { get; set; }
        public int? CategoryBrandId { get; set; }
        public CategoryBrand? CategoryBrand { get; set; }
        public List<ProductImage>? ProductImages { get; set; }
        public IEnumerable<Basket>? Baskets { get; set; }
        public IEnumerable<OrderItem>? OrderItems { get; set; }
        [NotMapped]
        public IFormFile? MainFile { get; set; }
        [NotMapped]
        public IEnumerable<IFormFile>? Files { get; set; }
        public List<ProductColor>? ProductColors { get; set; }
        [NotMapped]
        public IEnumerable<int>? ColorIds { get; set; }
        public List<ProductCategorySpecification>? ProductCategorySpecifications { get; set; }
        [NotMapped]
        public IEnumerable<int>? CategorySpecificationIds { get; set; }
        [NotMapped]
        public IEnumerable<int>? IFLoanRangeIds { get; set; }
        [NotMapped]
        public IEnumerable<int>? LoanRangeIds { get; set; }

    }
}
