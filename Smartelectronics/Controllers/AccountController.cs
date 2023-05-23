using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MimeKit;
using Newtonsoft.Json;
using Smartelectronics.DataAccessLayer;
using Smartelectronics.Models;
using Smartelectronics.ViewModels;
using Smartelectronics.ViewModels.AccountViewModels;
using Smartelectronics.ViewModels.BasketViewModels;
using System.Data;
using MailKit.Net.Smtp;
using Smartelectronics.ViewModels.WishlistViewModels;
using Microsoft.AspNetCore.Authorization;
using System.Text;

namespace Smartelectronics.Controllers
{
    public class AccountController : Controller
    {
		private readonly UserManager<AppUser> _userManager;
		private readonly SignInManager<AppUser> _signInManager;
        private readonly AppDbContext _context;
        private readonly IConfiguration _con;
        private readonly SmtpSetting _smtpSetting;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager,
            AppDbContext context, IConfiguration con, IOptions<SmtpSetting> smtpSetting)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _con = con;
            _smtpSetting = smtpSetting.Value;
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            if (!ModelState.IsValid)
            {
                return View(registerVM);
            }


            AppUser appUser = new AppUser
            {
                UserName = registerVM.UserName,
                Email = registerVM.Email,
                Name = registerVM.Name,
                SurName = registerVM.SurName,
                IsActive = true,
                
            };

            

            IdentityResult identityResult = await _userManager.CreateAsync(appUser, registerVM.Password);

            if (!identityResult.Succeeded)
            {
                foreach (IdentityError identityError in identityResult.Errors)
                {
                    ModelState.AddModelError("", identityError.Description);
                }
                return View(registerVM);
            }

            await _userManager.AddToRoleAsync(appUser, "Member");

            string token = await _userManager.GenerateEmailConfirmationTokenAsync(appUser);

            string url = Url.Action("EmailConfirm", "Account", new { id = appUser.Id, token = token }, HttpContext.Request.Scheme, HttpContext.Request.Host.ToString());

            //string templatePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Templates", "_EmailConfirm.cshtml");
            //string templateContent = await System.IO.File.ReadAllTextAsync(templatePath);
            //templateContent = templateContent.Replace("{{email}}", appUser.Email);
            //templateContent = templateContent.Replace("{{url}}", url);

            //MimeMessage mimeMessage = new MimeMessage();
            //mimeMessage.From.Add(MailboxAddress.Parse(_smtpSetting.Email));
            //mimeMessage.To.Add(MailboxAddress.Parse(appUser.Email));
            //mimeMessage.Subject = "Email Confirmation";
            //mimeMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            //{
            //    Text = templateContent
            //};

            //using (SmtpClient smtpClient = new SmtpClient())
            //{
            //    await smtpClient.ConnectAsync(_smtpSetting.Host, _smtpSetting.Port, MailKit.Security.SecureSocketOptions.StartTls);
            //    await smtpClient.AuthenticateAsync(_smtpSetting.Email, _smtpSetting.Password);
            //    await smtpClient.SendAsync(mimeMessage);
            //    await smtpClient.DisconnectAsync(true);
            //    smtpClient.Dispose();
            //}


            StringBuilder sb = new StringBuilder();

            sb.Append(@$"<div class=""app font-sans min-w-screen min-h-screen bg-grey-lighter py-8 px-4"">
                <div class=""mail__wrapper max-w-md mx-auto"">
                    <div class=""mail__content bg-white p-8 shadow-md"">
                        <div class=""content__header text-center tracking-wide border-b"">
                            <div class=""text-red text-sm font-bold"">SmartElectronics.az</div>
                            <h1 class=""text-3xl h-48 flex items-center justify-center"">SmartElectronics E-mail Təsdiqləmə</h1>
                        </div>
                        <div class=""content__body py-8 border-b"">
                            <p>Zəhmət olmazsa aşağıdakı linkdən emailinizi təsdiqləyin.</p>
                            <a href=""{url}"" class=""text-white text-sm tracking-wide bg-red rounded w-full my-8 p-4"">EMAİLİ TƏSDİQLƏ</a>
                        </div>
                        <div class=""content__footer mt-8 text-center text-grey-darker"">
                            <h3 class=""text-base sm:text-lg mb-4"">Thanks for using The App!</h3>
                            <p>www.smartelectronics.az</p>
                        </div>
                    </div>
                </div>
            </div>");


            string htmlTable = sb.ToString();

            MimeMessage message = new MimeMessage();
            message.From.Add(MailboxAddress.Parse(_smtpSetting.Email));
            message.To.Add(MailboxAddress.Parse(appUser.Email));
            message.Subject = "Hesab təsdiqləmə";
            var bodyBuilder = new BodyBuilder();

            bodyBuilder.HtmlBody = htmlTable;

            var body = new TextPart("html")
            {
                Text = bodyBuilder.HtmlBody
            };

            message.Body = body;

            using (SmtpClient smtpClient = new SmtpClient())
            {
                await smtpClient.ConnectAsync(_smtpSetting.Host, _smtpSetting.Port, MailKit.Security.SecureSocketOptions.StartTls);
                await smtpClient.AuthenticateAsync(_smtpSetting.Email, _smtpSetting.Password);
                await smtpClient.SendAsync(message);
                await smtpClient.DisconnectAsync(true);
                smtpClient.Dispose();
            }

            TempData["Success"] = "Tesdiq maili gonderildi. emailinizi tesdiqleyin";

            return RedirectToAction("Login");
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM loginVM)
        {
            if (!ModelState.IsValid)
            {
                return View(loginVM);
            }

            AppUser appUser = await _userManager.Users
                .Include(u => u.Baskets.Where(b => b.IsDeleted == false))
                .Include(u => u.Wishlists.Where(b => b.IsDeleted == false))
                .FirstOrDefaultAsync(u => u.NormalizedEmail == loginVM.Email.Trim().ToUpperInvariant());

            if (appUser == null)
            {
                ModelState.AddModelError("", "Email or Password Is Incorrect");
                return View(loginVM);
            }

            Microsoft.AspNetCore.Identity.SignInResult signInResult = null;

            if (appUser.EmailConfirmed)
            {
                signInResult = await _signInManager
                .PasswordSignInAsync(appUser, loginVM.Password, true, true);
            }
            else
            {
                ModelState.AddModelError("", "emailinizi tesdiqleyin");
                return View();
            }

            if (appUser.LockoutEnd > DateTime.UtcNow)
            {
                ModelState.AddModelError("", "Hesabiniz bloklanib");
                return View(loginVM);
            }

            if (!signInResult.Succeeded)
            {
                ModelState.AddModelError("", "Email or Password Is Incorrect");
                return View(loginVM);
            }

            string cookie = HttpContext.Request.Cookies["basket"];

            if (appUser.Baskets != null && appUser.Baskets.Count() > 0)
            {
                List<BasketVM> basketVMs = new List<BasketVM>();

                foreach (Basket basket in appUser.Baskets)
                {
                    BasketVM basketVM = new BasketVM
                    {
                        Id = (int)basket.ProductId,
                        Count = basket.Count
                    };

                    basketVMs.Add(basketVM);
                }

                cookie = JsonConvert.SerializeObject(basketVMs);

                HttpContext.Response.Cookies.Append("basket", cookie);
            }
            else
            {
                if(cookie == null || cookie == "")
                HttpContext.Response.Cookies.Append("basket", "");
            }

            cookie = HttpContext.Request.Cookies["wishlist"];

            if (appUser.Wishlists != null && appUser.Wishlists.Count() > 0)
            {
                List<WishlistVM> wishlistVMs = new List<WishlistVM>();

                foreach (Wishlist wishlist in appUser.Wishlists)
                {
                    WishlistVM wishlistVM = new WishlistVM
                    {
                        Id = (int)wishlist.ProductId,
                    };

                    wishlistVMs.Add(wishlistVM);
                }

                cookie = JsonConvert.SerializeObject(wishlistVMs);

                HttpContext.Response.Cookies.Append("wishlist", cookie);
            }
            else
            {
                if (cookie == null || cookie == "")
                    HttpContext.Response.Cookies.Append("wishlist", "");
            }


            return RedirectToAction("index", "home");
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> Profile()
        {
            AppUser appUser = await _userManager.Users
                .Include(u => u.Address)
                .Include(u => u.Orders.Where(o => o.IsDeleted == false)).ThenInclude(o => o.OrderItems.Where(oi => oi.IsDeleted == false)).ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);

            ProfileVM profileVM = new ProfileVM
            {
                Orders = appUser.Orders,
                AccountVM = new AccountVM
                {
                    Name = appUser.Name,
                    SurName = appUser.SurName,
                    Email = appUser.Email,
                    BirthDate = appUser?.BirthDate,
                    Number = appUser?.PhoneNumber,
                },
                SettingVM = new SettingVM
                {
                    Name = appUser.Name,
                    SurName = appUser.SurName,
                    BirthDate = appUser?.BirthDate,
                    Fin = appUser?.Fin,
                    Gender = appUser?.Gender,
                    IdSeria = appUser?.IdSeria,
                    Number = appUser?.PhoneNumber,
                    Patronymic = appUser?.Patronymic
                }
            };

            return View(profileVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> AddAddress(Address address)
        {
            AppUser appUser = await _userManager.Users
                .FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);

            TempData["Tab"] = "Address";

            ProfileVM profileVM = new ProfileVM
            {
                Address = address
            };

            if (!ModelState.IsValid)
            {
                TempData["ModelError"] = "Error";
                return View("Profile", profileVM);
            }

            address.CreatedAt = DateTime.UtcNow.AddHours(4);
            address.CreatedBy = $"{appUser.Name} {appUser.SurName}";

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Profile));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> UpdateProfile(AccountVM accountVM)
        {
            TempData["Tab"] = "Setting";

            AppUser appUser = await _userManager.Users
                .Include(u => u.Orders.Where(o => o.IsDeleted == false)).ThenInclude(o => o.OrderItems.Where(oi => oi.IsDeleted == false)).ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);

            ProfileVM profileVM = new ProfileVM
            {
                Address = new Address(),
                Orders = appUser.Orders,
                AccountVM = accountVM
            };

            if (!ModelState.IsValid)
            {
                return View("Profile", profileVM);
            }

            if (accountVM.Name != null)
            {
                appUser.Name = accountVM.Name;
            }

            if (accountVM.SurName != null)
            {
                appUser.SurName = accountVM.SurName;
            }

            if (accountVM.Email != null && appUser.NormalizedEmail != accountVM.Email.Trim().ToUpperInvariant())
            {
                if (await _userManager.Users.AnyAsync(u => u.NormalizedEmail == accountVM.Email.Trim().ToUpperInvariant() && u.Id != appUser.Id))
                {
                    ModelState.AddModelError("Email", $"Email {accountVM.Email} already exists.");
                    return View("Profile", profileVM);
                }
                else
                {
                    appUser.Email = accountVM.Email.Trim();
                }
            }

            appUser.Email = accountVM.Email;

            if (accountVM.BirthDate != null)
            {
                appUser.BirthDate = accountVM.BirthDate;
            }

            if (accountVM.Number != null)
            {
                appUser.PhoneNumber = accountVM.Number;
            }

            IdentityResult identityResult = await _userManager.UpdateAsync(appUser);

            if (!identityResult.Succeeded)
            {
                foreach (IdentityError identityError in identityResult.Errors)
                {
                    ModelState.AddModelError("", identityError.Description);
                }
                return View("Profile", profileVM);
            }

            if (!string.IsNullOrWhiteSpace(accountVM.CurrentPassword))
            {
                if (await _userManager.CheckPasswordAsync(appUser, accountVM.CurrentPassword))
                {
                    string token = await _userManager.GeneratePasswordResetTokenAsync(appUser);

                    identityResult = await _userManager.ResetPasswordAsync(appUser, token, accountVM.Password);

                    if (!identityResult.Succeeded)
                    {
                        foreach (IdentityError identityError in identityResult.Errors)
                        {
                            ModelState.AddModelError("", identityError.Description);
                        }

                        return View("Profile", profileVM);
                    }
                }
                else
                {
                    ModelState.AddModelError("CurrentPassword", "CurrentPassword yanlisdir");
                    return View("Profile", profileVM);
                }
            }


            await _signInManager.SignInAsync(appUser, true);

            TempData["Success"] = "Hesabiniz ugurla yenilendi";

            return RedirectToAction(nameof(Profile));
        }
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> UpdateSetting(SettingVM settingVM)
        {
            TempData["Tab"] = "Account";

            AppUser appUser = await _userManager.Users
                .Include(u => u.Orders.Where(o => o.IsDeleted == false)).ThenInclude(o => o.OrderItems.Where(oi => oi.IsDeleted == false)).ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);

            ProfileVM profileVM = new ProfileVM
            {
                Orders = appUser.Orders,
                SettingVM = settingVM
            };

            if (!ModelState.IsValid)
            {
                return View("Profile", settingVM);
            }


            if (settingVM.Name != null)
            {
                appUser.Name = settingVM.Name;
            }

            if (settingVM.SurName != null)
            {
                appUser.SurName = settingVM.SurName;
            }

            if (settingVM.Patronymic != null)
            {
                appUser.Patronymic = settingVM.Patronymic;
            }

            if (settingVM.IdSeria != null)
            {
                appUser.IdSeria = settingVM.IdSeria;
            }

            if (settingVM.Gender != null)
            {
                appUser.Gender = settingVM.Gender;
            }

            if (settingVM.Fin != null)
            {
                appUser.Fin = settingVM.Fin;
            }

            if (settingVM.BirthDate != null)
            {
                appUser.BirthDate = settingVM.BirthDate;
            }

            if (settingVM.Number != null)
            {
                appUser.PhoneNumber = settingVM.Number;
            }

            IdentityResult identityResult = await _userManager.UpdateAsync(appUser);

            if (!identityResult.Succeeded)
            {
                foreach (IdentityError identityError in identityResult.Errors)
                {
                    ModelState.AddModelError("", identityError.Description);
                }
                return View("Profile", profileVM);
            }


            await _signInManager.SignInAsync(appUser, true);

            TempData["Success"] = "Hesabiniz ugurla yenilendi";

            return RedirectToAction(nameof(Profile));
        }

        [HttpGet]
        public async Task<IActionResult> EmailConfirm(string? id, string? token)
        {
            if (string.IsNullOrWhiteSpace(id)) return BadRequest();
            if (string.IsNullOrWhiteSpace(token)) return BadRequest();

            AppUser appUser = await _userManager.FindByIdAsync(id);

            if (appUser == null) return NotFound();

            IdentityResult identityResult = await _userManager.ConfirmEmailAsync(appUser, token);

            if (!identityResult.Succeeded) return BadRequest();

            TempData["Success"] = $"{appUser.Email} tesdiqlendi";

            return RedirectToAction(nameof(Login));
        }

        
    }
}
