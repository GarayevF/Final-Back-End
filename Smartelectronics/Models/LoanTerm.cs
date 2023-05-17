using System.ComponentModel.DataAnnotations.Schema;

namespace Smartelectronics.Models
{
    public class LoanTerm : BaseEntity
    {
        public List<LoanTermLoanRange> LoanTermLoanRanges { get; set; }
        [NotMapped]
        public IEnumerable<int> LoanRangeIds { get; set; }
        public int LoanCompanyId { get; set; }
        public LoanCompany LoanCompany { get; set; }
        public int? ProductId { get; set; }
        public Product Product { get; set; }
        public string? Title { get; set; }
    }
}
