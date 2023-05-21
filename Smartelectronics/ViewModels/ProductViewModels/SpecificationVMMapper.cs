using Smartelectronics.Models;

namespace Smartelectronics.ViewModels.ProductViewModels
{
    public class SpecificationViewModelMapper
    {
        public static SpecificationVM Map(ProductCategorySpecification spec)
        {
            if (spec != null && spec.CategorySpecification != null && spec.CategorySpecification.Specification != null)
            {
                return new SpecificationVM
                {
                    Name = spec.CategorySpecification.Specification.Name,
                    Value = spec.Value
                };
            }

            return null; // Ya da varsayılan bir değer döndürebilirsiniz
        }
    }
    
}
