using Smartelectronics.Enums;
using System.ComponentModel.DataAnnotations;

namespace Smartelectronics.Models
{
    public class Address : BaseEntity
    {
        public string? UserId { get; set; }
        public AppUser? User { get; set; }
        [StringLength(100)]
        public string? City { get; set; }
        [StringLength(200)]
        public string? OrderAddress { get; set; }
        [StringLength(500)]
        public string? AdditionalComment { get; set; }
    }
}
