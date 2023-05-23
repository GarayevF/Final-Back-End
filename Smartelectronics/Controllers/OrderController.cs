using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using Smartelectronics.DataAccessLayer;
using Smartelectronics.Models;
using Smartelectronics.ViewModels;
using Smartelectronics.ViewModels.OrderViewModels;
using System.Security.Policy;
using System.Text;
using System.Xml.Linq;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace Smartelectronics.Controllers
{
    [Authorize(Roles = "Member")]
    public class OrderController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly AppDbContext _context;
        private readonly SmtpSetting _smtpSetting;

        public OrderController(UserManager<AppUser> userManager, AppDbContext context, IOptions<SmtpSetting> smtpSetting)
        {
            _userManager = userManager;
            _context = context;
            _smtpSetting = smtpSetting.Value;
        }

        [HttpGet]
        public async Task<IActionResult> Checkout()
        {
            AppUser appUser = await _userManager.Users
                .Include(u => u.Baskets.Where(b => b.IsDeleted == false)).ThenInclude(b => b.Product)
                .Include(u => u.Address)
                .FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);

            if (appUser.Baskets == null || appUser.Baskets.Count() <= 0)
            {
                return RedirectToAction("index", "home");
            }


			OrderVM orderVM = new OrderVM();

            

			if (appUser != null && appUser.Address != null)
			{
				orderVM.Order = new Order
				{
                    Name = appUser?.Name,
                    SurName = appUser?.SurName,
                    Email = appUser?.Email,
                    OrderAddress = appUser?.Address?.OrderAddress,
					City = appUser?.Address.City,
					AdditionalComment = appUser?.Address?.AdditionalComment,
					BirthDate = appUser?.BirthDate,
					IdSeria = appUser?.IdSeria,
					Patronymic = appUser?.Patronymic,
					Fin = appUser?.Fin,
					Number = appUser?.PhoneNumber,
					Gender = appUser?.Gender,
                    OrderMethod = "Nağd"
                };
            }

            if (appUser != null)
            {
                orderVM.Order = new Order
                {
                    Name = appUser.Name,
                    SurName = appUser.SurName,
                    Email = appUser.Email,
                    OrderMethod = "Nağd"
                };
                
            }

			orderVM.Baskets = appUser.Baskets;

			return View(orderVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(Order order)
        {
            AppUser appUser = await _userManager.Users
                .Include(u => u.Baskets.Where(b => b.IsDeleted == false)).ThenInclude(b => b.Product)
                .Include(u => u.Address)
                .Include(u => u.Orders)
                .FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);

            OrderVM orderVM = new OrderVM
            {
                Order = order,
                Baskets = appUser.Baskets
            };

            if (!ModelState.IsValid)
            {
                return View(orderVM);
            }

            order.CreatedAt = DateTime.UtcNow.AddHours(4);
            order.CreatedBy = $"{appUser.Name} {appUser.SurName}";
            order.No = appUser.Orders != null && appUser.Orders.Count > 0 ? appUser.Orders.Last().No + 1 : 1;

            appUser.Orders.Add(order);
            order.OrderItems = new List<OrderItem>();
            foreach (Basket basket in appUser.Baskets)
            {
                basket.IsDeleted = true;

                OrderItem orderItem = new OrderItem
                {
                    CreatedAt = DateTime.UtcNow.AddHours(4),
                    CreatedBy = $"{appUser.Name} {appUser.SurName}",
                    Count = basket.Count,
                    ProductId = basket.ProductId,
                    Price = basket.Product.DiscountedPrice > 0 ? basket.Product.DiscountedPrice : basket.Product.Price,
                };

                order.OrderItems.Add(orderItem);
            }

            HttpContext.Response.Cookies.Append("basket", "");

            await _context.SaveChangesAsync();

            StringBuilder sb = new StringBuilder();

            sb.Append("<div class=\"order-complete-message text-center\"><h1>Təşəkkürlər !</h1><p>Sifarişiniz qeydə alındı.Qısa müddət ərzində sizinlə əlaqə saxlanılacaq</p></div>");
            
            sb.Append($"<ul class=\"order-details-list\"><li>Sifariş nömrəsi: <strong>{order.No}</strong></li><li>Tarix: <strong>{DateTime.UtcNow.AddHours(4)}</strong></li>");
            sb.Append($"<li>Qiymət: <strong>{order.OrderItems.Sum(a => (a.Product.DiscountedPrice > 0 ? a.Product.DiscountedPrice : a.Product.Price) * a.Count)} AZN</strong></li>");
            sb.Append("</ul>");
            sb.Append("<h3 class=\"order-table-title\">Sifariş detalları</h3><div class=\"table-responsive\"><table style=\"width:100%\" class=\"table\"><thead><tr><th style=\"border: 1px solid #e5e5e5;padding: 8px;text-align: left;\">Məhsul</th><th>Qiymət</th style=\"border: 1px solid #e5e5e5;padding: 8px;text-align: left;\"></tr></thead><tbody>");



            foreach (Basket basket in appUser.Baskets)
            {
                sb.Append($"<tr><td style=\"border: 1px solid #e5e5e5;padding: 8px;text-align: left;\"><a asp-action=\"detail\" asp-controller=\"product\" asp-route-id=\"{basket.Product.Id}\">{basket.Product.Title}</a> <strong>× {basket.Count}</strong></td>");
                sb.Append($"<td style=\"border: 1px solid #e5e5e5;padding: 8px;text-align: left;\"><span>{basket.Product.Price} AZN</span></td></tr>");    
            }

            sb.Append($"</tbody><tfoot><tr><th style=\"border: 1px solid #e5e5e5;padding: 8px;text-align: left;\">Ümumi qiymət:</th><th style=\"border: 1px solid #e5e5e5;padding: 8px;text-align: left;\"><span>{order.OrderItems.Sum(a => (a.Product.DiscountedPrice > 0 ? a.Product.DiscountedPrice : a.Product.Price) * a.Count)} AZN</span></th></tr></tfoot></table></div>");

            string htmlTable = sb.ToString();

            MimeMessage message = new MimeMessage();
            message.From.Add(MailboxAddress.Parse(_smtpSetting.Email));
            message.To.Add(MailboxAddress.Parse(appUser.Email));
            message.Subject = "Sifariş detalları";
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

            TempData["Success"] = $"{order.No} Sifarisiniz ugurla gonderildi";

            return RedirectToAction("OrderCompleted", "order", new { id = order.Id });
        }

        public async Task<IActionResult> OrderCompleted(int? id)
        {
            Order order = await _context.Orders.Where(a => a.IsDeleted == false)
                .Include(o => o.OrderItems).ThenInclude(o => o.Product)
                .FirstOrDefaultAsync(o => o.IsDeleted == false && o.Id == id);



            return View(order);
        }

        [HttpGet]
        public async Task<IActionResult> CheckoutSingle(string? method, int? productId)
        {
            AppUser appUser = await _userManager.Users
                .Include(u => u.Baskets.Where(b => b.IsDeleted == false)).ThenInclude(b => b.Product)
                .Include(u => u.Address)
                .FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);

            if (productId == null) return BadRequest();

            Product product = await _context.Products.Where(p => p.IsDeleted == false)
                .FirstOrDefaultAsync(p => p.Id == productId);

            if (product == null) return NotFound();

            OrderVM orderVM = new OrderVM();

            if (appUser != null && appUser.Address != null)
            {
                orderVM.Order = new Order
                {
                    Name = appUser?.Name,
                    SurName = appUser?.SurName,
                    Email = appUser?.Email,
                    OrderAddress = appUser?.Address?.OrderAddress,
                    City = appUser?.Address.City,
                    AdditionalComment = appUser?.Address?.AdditionalComment,
                    BirthDate = appUser?.BirthDate,
                    IdSeria = appUser?.IdSeria,
                    Patronymic = appUser?.Patronymic,
                    Fin = appUser?.Fin,
                    Number = appUser?.PhoneNumber,
                    Gender = appUser?.Gender,
                    OrderMethod = Uri.UnescapeDataString(method)
                };
            }

            if (appUser != null)
            {
                orderVM.Order = new Order
                {
                    Name = appUser.Name,
                    SurName = appUser.SurName,
                    Email = appUser.Email,
                    OrderMethod = Uri.UnescapeDataString(method)
                };

            }

            orderVM.Product = product;

            return View(orderVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CheckoutSingle(Order order, int? productId)
        {
            if(productId == null) return BadRequest();

            Product product = await _context.Products.Where(p => p.IsDeleted == false)
                .FirstOrDefaultAsync(p => p.Id == productId);

            if (product == null) return NotFound();

            AppUser appUser = await _userManager.Users
                .Include(u => u.Baskets.Where(b => b.IsDeleted == false)).ThenInclude(b => b.Product)
                .Include(u => u.Address)
                .Include(u => u.Orders)
                .FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);

            OrderVM orderVM = new OrderVM
            {
                Order = order,
                Product = product
            };


            if (!ModelState.IsValid)
            {
                return View(orderVM);
            }

            order.CreatedAt = DateTime.UtcNow.AddHours(4);
            order.CreatedBy = $"{appUser.Name} {appUser.SurName}";
            order.No = appUser.Orders != null && appUser.Orders.Count > 0 ? appUser.Orders.Last().No + 1 : 1;

            appUser.Orders.Add(order);
            order.OrderItems = new List<OrderItem>();
            

                OrderItem orderItem = new OrderItem
                {
                    CreatedAt = DateTime.UtcNow.AddHours(4),
                    CreatedBy = $"{appUser.Name} {appUser.SurName}",
                    Count = 1,
                    ProductId = productId,
                    Price = product.DiscountedPrice > 0 ? product.DiscountedPrice : product.Price,
                };

                order.OrderItems.Add(orderItem);

            await _context.SaveChangesAsync();

            StringBuilder sb = new StringBuilder();

            sb.Append("<div class=\"order-complete-message text-center\"><h1>Təşəkkürlər !</h1><p>Sifarişiniz qeydə alındı.Qısa müddət ərzində sizinlə əlaqə saxlanılacaq</p></div>");

            sb.Append($"<ul class=\"order-details-list\"><li>Sifariş nömrəsi: <strong>{order.No}</strong></li><li>Tarix: <strong>{DateTime.UtcNow.AddHours(4)}</strong></li>");
            sb.Append($"<li>Qiymət: <strong>{order.OrderItems.Sum(a => (a.Product.DiscountedPrice > 0 ? a.Product.DiscountedPrice : a.Product.Price) * a.Count)} AZN</strong></li>");
            sb.Append("</ul>");
            sb.Append("<h3 class=\"order-table-title\">Sifariş detalları</h3><div class=\"table-responsive\"><table style=\"width:100%\" class=\"table\"><thead><tr><th style=\"border: 1px solid #e5e5e5;padding: 8px;text-align: left;\">Məhsul</th><th>Qiymət</th style=\"border: 1px solid #e5e5e5;padding: 8px;text-align: left;\"></tr></thead><tbody>");



                sb.Append($"<tr><td style=\"border: 1px solid #e5e5e5;padding: 8px;text-align: left;\"><a asp-action=\"detail\" asp-controller=\"product\" asp-route-id=\"{product.Id}\">{product.Title}</a> <strong>× {1}</strong></td>");
                sb.Append($"<td style=\"border: 1px solid #e5e5e5;padding: 8px;text-align: left;\"><span>{product.Price} AZN</span></td></tr>");
            

            sb.Append($"</tbody><tfoot><tr><th style=\"border: 1px solid #e5e5e5;padding: 8px;text-align: left;\">Ümumi qiymət:</th><th style=\"border: 1px solid #e5e5e5;padding: 8px;text-align: left;\"><span>{order.OrderItems.Sum(a => (a.Product.DiscountedPrice > 0 ? a.Product.DiscountedPrice : a.Product.Price) * a.Count)} AZN</span></th></tr></tfoot></table></div>");

            string htmlTable = sb.ToString();

            MimeMessage message = new MimeMessage();
            message.From.Add(MailboxAddress.Parse(_smtpSetting.Email));
            message.To.Add(MailboxAddress.Parse(appUser.Email));
            message.Subject = "Sifariş detalları";
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

            TempData["Success"] = $"{order.No} Sifarisiniz ugurla gonderildi";

            return RedirectToAction("OrderCompleted", "order", new { id = order.Id });
        }


        [HttpGet]
        public async Task<IActionResult> CheckoutCredit(string? method, double? price, int? productId)
        {
            AppUser appUser = await _userManager.Users
                .Include(u => u.Baskets.Where(b => b.IsDeleted == false)).ThenInclude(b => b.Product)
                .Include(u => u.Address)
                .FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);

            if (productId == null) return BadRequest();

            Product product = await _context.Products.Where(p => p.IsDeleted == false)
                .FirstOrDefaultAsync(p => p.Id == productId);

            if (price == null) return BadRequest();

            if (price != null)
                product.Price = (double)price;

            if (product == null) return NotFound();

            OrderVM orderVM = new OrderVM();

            if (appUser != null && appUser.Address != null)
            {
                orderVM.Order = new Order
                {
                    Name = appUser?.Name,
                    SurName = appUser?.SurName,
                    Email = appUser?.Email,
                    OrderAddress = appUser?.Address?.OrderAddress,
                    City = appUser?.Address.City,
                    AdditionalComment = appUser?.Address?.AdditionalComment,
                    BirthDate = appUser?.BirthDate,
                    IdSeria = appUser?.IdSeria,
                    Patronymic = appUser?.Patronymic,
                    Fin = appUser?.Fin,
                    Number = appUser?.PhoneNumber,
                    Gender = appUser?.Gender,
                    OrderMethod = Uri.UnescapeDataString(method)
                };
            }

            if (appUser != null)
            {
                orderVM.Order = new Order
                {
                    Name = appUser.Name,
                    SurName = appUser.SurName,
                    Email = appUser.Email,
                    OrderMethod = Uri.UnescapeDataString(method)
                };

            }

            orderVM.Product = product;

            return View(orderVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CheckoutCredit(Order order, double? price, int? productId)
        {
            if (productId == null) return BadRequest();

            Product product = await _context.Products.Where(p => p.IsDeleted == false)
                .FirstOrDefaultAsync(p => p.Id == productId);

            if (product == null) return NotFound();

            AppUser appUser = await _userManager.Users
                .Include(u => u.Baskets.Where(b => b.IsDeleted == false)).ThenInclude(b => b.Product)
                .Include(u => u.Address)
                .Include(u => u.Orders)
                .FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);

            OrderVM orderVM = new OrderVM
            {
                Order = order,
                Product = product
            };


            if (!ModelState.IsValid)
            {
                return View(orderVM);
            }

            order.CreatedAt = DateTime.UtcNow.AddHours(4);
            order.CreatedBy = $"{appUser.Name} {appUser.SurName}";
            order.No = appUser.Orders != null && appUser.Orders.Count > 0 ? appUser.Orders.Last().No + 1 : 1;

            appUser.Orders.Add(order);
            order.OrderItems = new List<OrderItem>();


            OrderItem orderItem = new OrderItem
            {
                CreatedAt = DateTime.UtcNow.AddHours(4),
                CreatedBy = $"{appUser.Name} {appUser.SurName}",
                Count = 1,
                ProductId = productId,
                Price = product.DiscountedPrice > 0 ? product.DiscountedPrice : product.Price,
            };

            order.OrderItems.Add(orderItem);

            await _context.SaveChangesAsync();

            StringBuilder sb = new StringBuilder();

            sb.Append("<div class=\"order-complete-message text-center\"><h1>Təşəkkürlər !</h1><p>Sifarişiniz qeydə alındı.Qısa müddət ərzində sizinlə əlaqə saxlanılacaq</p></div>");

            sb.Append($"<ul class=\"order-details-list\"><li>Sifariş nömrəsi: <strong>{order.No}</strong></li><li>Tarix: <strong>{DateTime.UtcNow.AddHours(4)}</strong></li>");
            sb.Append($"<li>Qiymət: <strong>{order.OrderItems.Sum(a => (a.Product.DiscountedPrice > 0 ? a.Product.DiscountedPrice : a.Product.Price) * a.Count)} AZN</strong></li>");
            sb.Append("</ul>");
            sb.Append("<h3 class=\"order-table-title\">Sifariş detalları</h3><div class=\"table-responsive\"><table style=\"width:100%\" class=\"table\"><thead><tr><th style=\"border: 1px solid #e5e5e5;padding: 8px;text-align: left;\">Məhsul</th><th>Qiymət</th style=\"border: 1px solid #e5e5e5;padding: 8px;text-align: left;\"></tr></thead><tbody>");



            sb.Append($"<tr><td style=\"border: 1px solid #e5e5e5;padding: 8px;text-align: left;\"><a asp-action=\"detail\" asp-controller=\"product\" asp-route-id=\"{product.Id}\">{product.Title}</a> <strong>× {1}</strong></td>");
            sb.Append($"<td style=\"border: 1px solid #e5e5e5;padding: 8px;text-align: left;\"><span>{product.Price} AZN</span></td></tr>");


            sb.Append($"</tbody><tfoot><tr><th style=\"border: 1px solid #e5e5e5;padding: 8px;text-align: left;\">Ümumi qiymət:</th><th style=\"border: 1px solid #e5e5e5;padding: 8px;text-align: left;\"><span>{order.OrderItems.Sum(a => (a.Product.DiscountedPrice > 0 ? a.Product.DiscountedPrice : a.Product.Price) * a.Count)} AZN</span></th></tr></tfoot></table></div>");

            string htmlTable = sb.ToString();

            MimeMessage message = new MimeMessage();
            message.From.Add(MailboxAddress.Parse(_smtpSetting.Email));
            message.To.Add(MailboxAddress.Parse(appUser.Email));
            message.Subject = "Sifariş detalları";
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

            TempData["Success"] = $"{order.No} Sifarisiniz ugurla gonderildi";

            return RedirectToAction("OrderCompleted", "order", new { id = order.Id });
        }


    }
}
