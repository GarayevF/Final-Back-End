using System.ComponentModel.DataAnnotations;

namespace Smartelectronics.Models
{
    public class SpecificationGroup : BaseEntity
    {
        [StringLength(1000)]
        public string Icon { get; set; }
        [StringLength(100)]
        public string Name { get; set; }
    }
}
