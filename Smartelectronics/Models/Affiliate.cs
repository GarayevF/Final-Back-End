using System.ComponentModel.DataAnnotations;

namespace Smartelectronics.Models
{
    public class Affiliate : BaseEntity
    {
        [StringLength(100)]
        public string? Name { get; set; }
        [StringLength(10)]
        public string? Hotline { get; set; }
        [StringLength(10)]
        public string? Number { get; set; }
        [StringLength(20)]
        public string? Mail { get; set; }
        [StringLength(100)]
        public string? Address { get; set; }
    }
}
