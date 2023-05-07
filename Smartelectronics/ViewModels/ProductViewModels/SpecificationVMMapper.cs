using Smartelectronics.Models;

namespace Smartelectronics.ViewModels.ProductViewModels
{
    public class SpecificationViewModelMapper
    {
        public static SpecificationVM Map(ProductCategorySpecification spec)
        {
            return new SpecificationVM
            {
                Name = spec.CategorySpecification.Specification.Name,
                Value = spec.Value
            };
        }
    }
    
}
