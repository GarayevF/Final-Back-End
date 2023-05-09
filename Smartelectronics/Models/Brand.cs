using System.ComponentModel.DataAnnotations;

namespace Smartelectronics.Models
{
    public class Brand : BaseEntity
    {
        [StringLength(255)]
        public string Name { get; set; }
    }
}
