using Smartelectronics.Models;

namespace Smartelectronics.ViewModels.AffiliateViewModels
{
	public class AffiliateVM
	{
		public IEnumerable<Affiliate>? Affiliates { get; set; }
		public Message? Message{ get; set; }
	}
}
