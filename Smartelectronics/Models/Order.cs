using MailKit.Search;
using Smartelectronics.Enums;
using System.ComponentModel.DataAnnotations;

namespace Smartelectronics.Models
{
    public class Order : BaseEntity
    {
        public string? UserId { get; set; }
        public AppUser? User { get; set; }
        public int? No { get; set; }
        [StringLength(500)]
        public string? Comment { get; set; }
        public OrderType Status { get; set; }
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
        [EmailAddress]
        public string? Email { get; set; }
        [StringLength(20)]
        public string? Number { get; set; }
        [DataType(DataType.Date)]
        public DateTime? BirthDate { get; set; }
        public GenderType? Gender { get; set; }
        [StringLength(100)]
        public string? City { get; set; }
        [StringLength(200)]
        public string? OrderAddress { get; set; }
        [StringLength(500)]
        public string? AdditionalComment { get; set; }
        public List<OrderItem>? OrderItems { get; set; }
        [StringLength(100)]
        public string? OrderMethod { get; set; }
    }
}
