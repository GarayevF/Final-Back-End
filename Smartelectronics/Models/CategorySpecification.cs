namespace Smartelectronics.Models
{
    public class CategorySpecification : BaseEntity
    {
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public int SpecificationId { get; set; }
        public Specification Specification { get; set; }
    }
}
