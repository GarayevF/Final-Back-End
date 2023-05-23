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

        public IActionResult Index(int pageIndex = 1)
        {
            IQueryable<Message> messages = _context.Messages
                .Where(o => o.IsDeleted == false);

            return View(PageNatedList<Message>.Create(messages, pageIndex, 5, 5));
        }

        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null) return BadRequest();

            Message message = await _context.Messages
                .FirstOrDefaultAsync(o => o.IsDeleted == false && o.Id == id);

            if (message == null) return NotFound();

            return View(message);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return BadRequest();

            Message message = await _context.Messages
                .FirstOrDefaultAsync(o => o.IsDeleted == false && o.Id == id);

            if (message == null) return NotFound();

            message.IsDeleted = true;
            message.DeletedAt = DateTime.UtcNow.AddHours(4);
            message.DeletedBy = "System";

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
