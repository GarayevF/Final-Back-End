using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Smartelectronics.Models
{
	public class Campaign : BaseEntity
	{
		[StringLength(255)]
		public string? Title { get; set; }
		public string? Desc { get; set; }
        [DataType(DataType.Date)]
        public DateTime? EndDate { get; set; }
        public string? Image { get; set; }
		[NotMapped]
		public IFormFile? File { get; set; }
	}
}
