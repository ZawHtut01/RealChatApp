using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using RealChatApp.Data;
using RealChatApp.Models;
using Microsoft.EntityFrameworkCore;

namespace RealChatApp.Controllers
{
    //[Authorize]
    public class ChatController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ChatController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var currentUserId = HttpContext.Session.GetInt32("UserId");
            if (currentUserId == null) return RedirectToAction("Login", "Account");

            var users = _context.Users.Where(u => u.Id != currentUserId).ToList();
            ViewBag.CurrentUserId = currentUserId;
            return View(users);
        }

        [HttpGet]
        public IActionResult ChatWith(int receiverId)
        {
            var currentUserId = HttpContext.Session.GetInt32("UserId");
            if (currentUserId == null) return RedirectToAction("Login", "Account");

            var receiver = _context.Users.Find(receiverId);
            //var messages = _context.ChatMessages
            //    .Where(m => (m.SenderId == currentUserId && m.ReceiverId == receiverId) ||
            //                (m.SenderId == receiverId && m.ReceiverId == currentUserId))
            //    .OrderBy(m => m.Timestamp)
            //    .ToList();

            var messages = _context.ChatMessages
                            .Where(m => (m.SenderId == currentUserId && m.ReceiverId == receiverId) ||
                                        (m.SenderId == receiverId && m.ReceiverId == currentUserId))
                            .Include(m => m.Sender)    // <-- Include Sender navigation property
                            .OrderBy(m => m.Timestamp)
                            .ToList();

            ViewBag.Receiver = receiver;
            ViewBag.CurrentUserId = currentUserId;

            return View(messages);
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage([FromBody] SendMessageRequest request)
        {
            try
            {
                var senderId = HttpContext.Session.GetInt32("UserId");
                if (senderId == null) return Unauthorized();

                var chatMessage = new ChatMessage
                {
                    SenderId = senderId.Value,
                    ReceiverId = request.ReceiverId,
                    Message = request.Message,
                    Timestamp = DateTime.Now
                };

                _context.ChatMessages.Add(chatMessage);
                await _context.SaveChangesAsync();

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }


        //[HttpPost]
        //public async Task<IActionResult> SendMessage(int receiverId, string message)
        //{
        //    var senderId = HttpContext.Session.GetInt32("UserId");
        //    if (senderId == null) return Unauthorized();

        //    var chatMessage = new ChatMessage
        //    {
        //        SenderId = senderId.Value,
        //        ReceiverId = receiverId,
        //        Message = message,
        //        Timestamp = DateTime.Now
        //    };

        //    _context.ChatMessages.Add(chatMessage);
        //    await _context.SaveChangesAsync();

        //    return Ok();
        //}
    }

    public class SendMessageRequest
    {
        public int ReceiverId { get; set; }
        public string Message { get; set; }
    }


}