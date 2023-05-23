using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Smartelectronics.DataAccessLayer;
using Smartelectronics.Models;
using Smartelectronics.ViewModels;

namespace Smartelectronics.Areas.Manage.Controllers
{
    [Area("Manage")]
    [Authorize(Roles = "SuperAdmin, Admin")]
    public class MessageController : Controller
    {
        private readonly AppDbContext _context;

        public MessageController(AppDbContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "SuperAdmin, Admin")]
        public IActionResult Index(int pageIndex = 1)
        {
            IQueryable<Message> messages = _context.Messages
                .Where(o => o.IsDeleted == false);

            return View(PageNatedList<Message>.Create(messages, pageIndex, 5, 5));
        }

        [Authorize(Roles = "SuperAdmin, Admin")]
        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null) return BadRequest();

            Message message = await _context.Messages
                .FirstOrDefaultAsync(o => o.IsDeleted == false && o.Id == id);

            if (message == null) return NotFound();

            return View(message);
        }

        [Authorize(Roles = "SuperAdmin, Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return BadRequest();

            Message message = await _context.Messages
                .FirstOrDefaultAsync(o => o.IsDeleted == false && o.Id == id);

            if (message == null) return NotFound();

            message.IsDeleted = true;
            message.DeletedAt = DateTime.UtcNow.AddHours(4);
            message.DeletedBy = "System";

            return RedirectToAction(nameof(Index));
        }
    }
}
