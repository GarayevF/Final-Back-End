using System.ComponentModel.DataAnnotations.Schema;

namespace Smartelectronics.Models
{
    public class ProductLoanRange : BaseEntity
    {
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public int LoanRangeId { get; set; }
        public LoanRange LoanRange { get; set; }
        public double InterestForStandartUsers { get; set; }
        public double InterestForVipUsers { get; set; }
    }
}
