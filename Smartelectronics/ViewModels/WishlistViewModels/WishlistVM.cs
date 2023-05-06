using Smartelectronics.Models;

namespace Smartelectronics.ViewModels.WishlistViewModels
{
    public class WishlistVM
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public Brand Brand { get; set; }
        public string Image { get; set; }
        public double Price { get; set; }
    }
}
