using Smartelectronics.Models;

namespace Smartelectronics.ViewModels.ProductViewModels
{
    public class GroupedSpecificationsVM
    {
        public string? GroupName { get; set; }
        public string? GroupIcon { get; set; }
        public List<SpecificationVM>? Specifications { get; set; }
    }
}
