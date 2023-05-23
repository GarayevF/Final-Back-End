using Microsoft.AspNetCore.Identity;
using Smartelectronics.Enums;
using System.ComponentModel.DataAnnotations;

namespace Smartelectronics.Models
{
    public class AppUser : IdentityUser
    {
        [StringLength(100)]
        public string? Name { get; set; }
        [StringLength(100)]
        public string? SurName { get; set; }
        [StringLength(100)]
        public string? Patronymic { get; set; }
        [StringLength(10)]
        public string? IdSeria { get; set; }
        [StringLength(10)]
        public string? Fin { get; set; }
        [DataType(DataType.Date)]
        public DateTime? BirthDate { get; set; }
        public GenderType? Gender { get; set; }
        public bool IsActive { get; set; }
        public Address? Address { get; set; }
        public List<Basket>? Baskets { get; set; }
        public List<Wishlist>? Wishlists { get; set; }
        public List<Order>? Orders { get; set; }
    }
}
