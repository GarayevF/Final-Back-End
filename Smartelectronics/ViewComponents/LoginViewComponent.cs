using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp;
using Smartelectronics.ViewModels.AccountViewModels;

namespace Smartelectronics.ViewComponents
{
    public class LoginViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var loginVM = new LoginVM(); // LoginVM'in istenen özellikleri ayarlayın
            return View("Default", loginVM);
        }

        public async Task<IViewComponentResult> Login(LoginVM loginVM)
        {
            return View();
        }
    }
}
