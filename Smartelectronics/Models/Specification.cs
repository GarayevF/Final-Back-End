using System.ComponentModel.DataAnnotations;

namespace Smartelectronics.Models
{
    public class Specification : BaseEntity
    {
        [StringLength(100)]
        public string Name { get; set; }
    }
}
