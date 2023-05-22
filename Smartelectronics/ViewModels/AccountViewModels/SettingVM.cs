using Smartelectronics.Enums;
using System.ComponentModel.DataAnnotations;

namespace Smartelectronics.ViewModels.AccountViewModels
{
    public class SettingVM
    {
        [StringLength(100)]
        public string? Name { get; set; }
        [StringLength(100)]
        public string? SurName { get; set; }
        [StringLength(100)]
        public string? Patronymic { get; set; }
        [StringLength(100)]
        public string? IdSeria { get; set; }
        public int? Fin { get; set; }
        [StringLength(100)]
        public string? Number { get; set; }
        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }
        public GenderType Gender { get; set; }
    }
}
