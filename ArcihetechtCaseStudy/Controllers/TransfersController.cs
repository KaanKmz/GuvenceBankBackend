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

        [HttpPut("{id}")]
        public IActionResult UpdateTransfer(int id, [FromBody] UpdateTransferDto request)
        {
            var senderId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var transfer = _context.Transfers.FirstOrDefault(t => t.Id == id && t.SenderId == senderId);
            if (transfer == null)
                return NotFound("Transfer bulunamadı.");

            var sender = _context.Users.FirstOrDefault(u => u.Id == transfer.SenderId);
            var receiver = _context.Users.FirstOrDefault(u => u.Id == transfer.ReceiverId);
            if (sender == null || receiver == null)
                return NotFound("Gönderen veya alıcı bulunamadı.");

            if (request.Amount <= 0)
                return BadRequest("Geçersiz tutar.");

            decimal eskiTutar = transfer.Amount;
            decimal yeniTutar = request.Amount;

            // Bakiye kontrolü yaptık
            decimal senderAvailableBalance = sender.Balance + eskiTutar; 
            if (senderAvailableBalance < yeniTutar)
                return BadRequest("Yetersiz bakiye.");

            // Bakiyeleri güncelledik
            sender.Balance = senderAvailableBalance - yeniTutar;
            receiver.Balance = receiver.Balance - eskiTutar + yeniTutar;

            // Transfer bilgilerini güncelledik
            transfer.Amount = yeniTutar;
            

            _context.SaveChanges();

            return Ok(new { message = "Transfer başarıyla güncellendi." });
        }



        
        [HttpPost]
        public IActionResult MakeTransfer([FromBody] CreateTransferDto request)
        {
            var senderId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var sender = _context.Users.FirstOrDefault(u => u.Id == senderId);
            var receiver = _context.Users.FirstOrDefault(u => u.Username == request.ReceiverUsername);


            if (sender == null || receiver == null)
                return NotFound("Sender or receiver not found.");

            // Gönderen ve alan aynı kişi olamaz
            if (sender.Id == receiver.Id)
                return BadRequest("You cannot transfer money to yourself.");

            if (sender.Balance < request.Amount)
                return BadRequest("Insufficient balance.");

            // Transfer işlemi
            sender.Balance -= request.Amount;
            receiver.Balance += request.Amount;

            var transfer = new Transfer
            {
                SenderId = sender.Id,
                ReceiverId = receiver.Id,
                Amount = request.Amount,
                CreatedAt = DateTime.UtcNow
            };

            _context.Transfers.Add(transfer);
            _context.SaveChanges();

            return Ok(new { message = "Transfer başarıyla gerçekleştirildi" });
        }

        [HttpGet]
        public IActionResult GetMyTransfers()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var transfers = _context.Transfers
                .Where(t => t.SenderId == userId || t.ReceiverId == userId)
                .OrderByDescending(t => t.CreatedAt)
                .Select(t => new TransferDto
                {
                    Id = t.Id,
                    SenderUsername = _context.Users.FirstOrDefault(u => u.Id == t.SenderId)!.Username,
                    ReceiverUsername = _context.Users.FirstOrDefault(u => u.Id == t.ReceiverId)!.Username,
                    Amount = t.Amount,
                    CreatedAt = t.CreatedAt
                })
                .ToList();

            return Ok(transfers);
        }

    }
}
