using System.ComponentModel.DataAnnotations.Schema;

namespace Smartelectronics.Models
{
    public class ProductIFLoanRange : BaseEntity
    {
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public int LoanRangeId { get; set; }
        public LoanRange LoanRange { get; set; }
        [Column(TypeName = "money")]
        public double? InitialPayment { get; set; }
        [Column(TypeName = "money")]
        public double? MonthlyPayment { get; set; }
        [Column(TypeName = "money")]
        public double? TotalPayment { get; set; }
    }
}
