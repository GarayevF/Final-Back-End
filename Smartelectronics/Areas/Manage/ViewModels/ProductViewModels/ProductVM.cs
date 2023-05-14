using Smartelectronics.Models;

namespace Smartelectronics.Areas.Manage.ViewModels.ProductViewModels
{
    public class ProductVM
    {
        public Product Product { get; set; }
        public List<SpecificationVM>? SpecificationVMs { get; set; }
        public int CategoryId { get; set; }
        public IEnumerable<int> ColorIds { get; set; }
        public IEnumerable<int> IFLoanRangeIds { get; set; }
        public IEnumerable<int> LoanRangeIds { get; set; }
        public IEnumerable<int>? LoanCompanyIds { get; set; }
        public List<ColorImageVM> ColorImageVMs { get; set; }
        public List<LoanTermVM>? LoanTermVMs { get; set; }
        public List<IFLoanVM>? IFLoanVMs { get; set; }
        public List<LoanVM>? LoanVMs { get; set; }
    }
}
