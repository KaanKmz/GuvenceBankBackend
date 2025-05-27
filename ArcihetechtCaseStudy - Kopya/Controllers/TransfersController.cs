using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ArcihetechtCaseStudy.Data;
using ArcihetechtCaseStudy.DTOS;
using ArcihetechtCaseStudy.Models;

namespace ArchitechtCaseStudy.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TransfersController : ControllerBase
    {
        private readonly AuthDbContext _context;

        public TransfersController(AuthDbContext context)
        {
            _context = context;
        }

        [HttpGet("balance")]
        public IActionResult GetBalance()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
                return NotFound();

            return Ok(new { balance = user.Balance });
        }



        // ✅ POST: api/transfers
        [HttpPost]
        public IActionResult MakeTransfer([FromBody] CreateTransferDto request)
        {
            var senderId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            // Gönderen ve alan aynı kişi olamaz
            if (senderId == request.ReceiverId)
                return BadRequest("You cannot transfer money to yourself.");

            var sender = _context.Users.FirstOrDefault(u => u.Id == senderId);
            var receiver = _context.Users.FirstOrDefault(u => u.Id == request.ReceiverId);

            if (sender == null || receiver == null)
                return NotFound("Sender or receiver not found.");

            if (sender.Balance < request.Amount)
                return BadRequest("Insufficient balance.");

            // Transfer işlemi
            sender.Balance -= request.Amount;
            receiver.Balance += request.Amount;

            var transfer = new Transfer
            {
                SenderId = senderId,
                ReceiverId = request.ReceiverId,
                Amount = request.Amount
            };

            _context.Transfers.Add(transfer);
            _context.SaveChanges();

            return Ok(new { message = "Transfer successful." });
        }

        [HttpGet]
        public IActionResult GetMyTransfers()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var myTransfers = _context.Transfers
                .Where(t => t.SenderId == userId || t.ReceiverId == userId)
                .OrderByDescending(t => t.CreatedAt)
                .ToList();

            return Ok(myTransfers);
        }

    }
}
