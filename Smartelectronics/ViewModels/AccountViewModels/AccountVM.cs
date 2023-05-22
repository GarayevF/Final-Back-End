using Smartelectronics.Enums;
using System.ComponentModel.DataAnnotations;

namespace Smartelectronics.ViewModels.AccountViewModels
{
    public class AccountVM
    {
        [StringLength(100)]
        public string? Name { get; set; }
        [StringLength(100)]
        public string? SurName { get; set; }
        public string? UserName { get; set; }
        [EmailAddress]
        public string? Email { get; set; }
        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }
        [StringLength(100)]
        public string? Number { get; set; }
        [DataType(DataType.Password)]
        public string? CurrentPassword { get; set; }
        [DataType(DataType.Password)]
        public string? Password { get; set; }
        [DataType(DataType.Password)]
        [Compare(nameof(Password))]
        public string? ConfirmPassword { get; set; }
    }
}
