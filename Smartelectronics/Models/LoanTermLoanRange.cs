namespace Smartelectronics.Models
{
    public class LoanTermLoanRange : BaseEntity
    {
        public int? LoanTermId { get; set; }
        public LoanTerm LoanTerm { get; set; }
        public int LoanRangeId { get; set; }
        public LoanRange LoanRange { get; set; }
    }
}
