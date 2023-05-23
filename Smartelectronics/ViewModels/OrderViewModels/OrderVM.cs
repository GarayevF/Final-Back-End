using Smartelectronics.Models;

namespace Smartelectronics.ViewModels.OrderViewModels
{
    public class OrderVM
    {
        public Order? Order { get; set; }
        public Product? Product { get; set; }
        public IEnumerable<Basket> Baskets { get; set; }
    }
}
