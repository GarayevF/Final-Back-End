using Smartelectronics.Models;
using Smartelectronics.ViewModels.AccountViewModels;

namespace Smartelectronics.ViewModels.HomeViewModels
{
    public class HomeVM
    {
        public IEnumerable<Slider> Sliders { get; set; }
        public IEnumerable<Category> Categories { get; set; }
        public IEnumerable<Product> NewArrival { get; set; }
        public IEnumerable<Product> MostViewed { get; set; }
        public IEnumerable<Product> Featured { get; set; }
        //public IEnumerable<Deal> Deals { get; set; }
        public IEnumerable<Product> DiscountedProducts { get; set; }
        public LoginVM LoginVM { get; set; }
    }
}
