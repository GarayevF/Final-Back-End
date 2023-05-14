using Smartelectronics.Models;

namespace Smartelectronics.ViewModels.BasketViewModels
{
    public class BasketVM
    {
        public int Id { get; set; }
        public Brand Brand { get; set; }
        public Category Category { get; set; }
        public int Count { get; set; }
        public string Title { get; set; }
        public string Image { get; set; }
        public double Price { get; set; }
    }
}
