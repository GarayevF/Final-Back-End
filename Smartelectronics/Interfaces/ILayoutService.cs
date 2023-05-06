using Smartelectronics.Models;
using Smartelectronics.ViewModels.BasketViewModels;
using Smartelectronics.ViewModels.CompareViewModels;
using Smartelectronics.ViewModels.WishlistViewModels;

namespace Smartelectronics.Interfaces
{
    public interface ILayoutService
    {
        Task<IDictionary<string, string>> GetSettings();
        Task<IEnumerable<Category>> GetCategories();
        Task<List<BasketVM>> GetBaskets();
        Task<List<WishlistVM>> GetWishLists();
        Task<List<CompareVM>> GetCompares();
    }
}
