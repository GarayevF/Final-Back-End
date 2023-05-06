using System.ComponentModel.DataAnnotations;

namespace Smartelectronics.Models
{
    public class Color : BaseEntity
    {
        [StringLength(20)]
        public string Code { get; set; }
    }
}
