using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Smartelectronics.Models
{
    public class Slider : BaseEntity
    {
        [StringLength(255)]
        public string? Link { get; set; }
        [StringLength(255)]
        public string? Image { get; set; }
        [NotMapped]
        public IFormFile? MainFile { get; set; }

    }
}
