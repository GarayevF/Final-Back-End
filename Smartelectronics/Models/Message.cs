using System.ComponentModel.DataAnnotations;

namespace Smartelectronics.Models
{
    public class Message : BaseEntity
    {
        [StringLength(100)]
        public string? Name { get; set; }
        [StringLength(100)]
        public string? Surname { get; set; }
        [StringLength(100)]
        [EmailAddress]
        public string? Mail { get; set; }
        [StringLength(100)]
        public string? Number { get; set; }
        [StringLength(100)]
        public string? YourMessage { get; set; }
    }
}
