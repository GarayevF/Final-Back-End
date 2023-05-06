using System.ComponentModel.DataAnnotations;

namespace Smartelectronics.Models
{
    public class Slider : BaseEntity
    {
        [StringLength(255)]
        public string Link { get; set; }
        [StringLength(255)]
        public string Image { get; set; }
    }
}
