using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Smartelectronics.Models
{
    public class LoanCompany : BaseEntity
    {
        [StringLength(50)]
        public string Name { get; set; }
        [StringLength(255)]
        public string? Logo { get; set; }

        [NotMapped]
        public IFormFile? LogoFile { get; set; }
        [StringLength(255)]
        public string? LabelImage { get; set; }

        [NotMapped]
        public IFormFile? LabelFile { get; set; }
    }
}
