﻿using Smartelectronics.Enums;
using System.ComponentModel.DataAnnotations;

namespace Smartelectronics.Models
{
    public class Address : BaseEntity
    {
        public string? UserId { get; set; }
        public AppUser? User { get; set; }
        [StringLength(100)]
        public string? Name { get; set; }
        [StringLength(100)]
        public string? SurName { get; set; }
        [StringLength(100)]
        public string? Patronymic { get; set; }
        [StringLength(100)]
        public string? IdSeria { get; set; }
        public int? Fin { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        [StringLength(20)]
        public string? Number { get; set; }
        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }
        public GenderType Gender { get; set; }
        [StringLength(100)]
        public string? City { get; set; }
        [StringLength(200)]
        public string? OrderAddress { get; set; }
        [StringLength(500)]
        public string? AdditionalComment { get; set; }
    }
}
