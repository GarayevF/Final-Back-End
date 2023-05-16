using Smartelectronics.Models;

namespace Smartelectronics.ViewModels.ProductViewModels
{
    public class DetailVM
    {
        public Product Product { get; set; }
        public List<GroupedSpecificationsVM>? GroupedSpecificationsVMs { get; set; }
        public List<ProductLoanRange>? LoanRanges { get; set; }
        public List<ProductIFLoanRange>? IFLoanRanges { get; set; }
    }
}
