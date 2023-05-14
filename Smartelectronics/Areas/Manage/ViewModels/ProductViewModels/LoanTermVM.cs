using Smartelectronics.Models;

namespace Smartelectronics.Areas.Manage.ViewModels.ProductViewModels
{
    public class LoanTermVM
    {
        public List<LoanRange> LoanRanges { get; set; }
        public IEnumerable<int> LoanRangeIds { get; set; }
        public int LoanCompanyId { get; set; }
        public string Title { get; set; }
    }
}
