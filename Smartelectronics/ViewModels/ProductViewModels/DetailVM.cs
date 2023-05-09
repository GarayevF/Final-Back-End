using Smartelectronics.Models;

namespace Smartelectronics.ViewModels.ProductViewModels
{
    public class DetailVM
    {
        public Product Product { get; set; }
        public List<GroupedSpecificationsVM>? GroupedSpecificationsVMs { get; set; }
        public IEnumerable<ProductLoanRange>? LoanRanges { get; set; }
        public IEnumerable<ProductIFLoanRange>? IFLoanRanges { get; set; }
    }
}
