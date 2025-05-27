using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using ArcihetechtCaseStudy.Data;
using ArcihetechtCaseStudy.DTOS;
using ArcihetechtCaseStudy.Models;

namespace ArchitechtCaseStudy.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TransactionsController : ControllerBase
    {
        private readonly AuthDbContext _context;

        public TransactionsController(AuthDbContext context)
        {
            _context = context;
        }

        
        [HttpPost]
        public IActionResult CreateTransaction([FromBody] CreateTransactionDto request)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null) return NotFound();

            if (user.Balance < request.Amount)
                return BadRequest("Insufficient balance.");

            var transaction = new Transaction
            {
                UserId = userId,
                Amount = request.Amount,
                Description = request.Description,
                Category = request.Category,
                CreatedAt = DateTime.UtcNow
            };

            user.Balance -= request.Amount;

            _context.Transactions.Add(transaction);
            _context.SaveChanges();

            return Ok(new { message = "Transaction created successfully." });

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

       
        [HttpGet]
        public IActionResult GetMyTransactions()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var transactions = _context.Transactions
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.CreatedAt)
                .ToList();

            return Ok(transactions);
        }

        
        [HttpPut("{id}")]
        public IActionResult UpdateTransaction(int id, [FromBody] UpdateTransactionDto request)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var transaction = _context.Transactions.FirstOrDefault(t => t.Id == id && t.UserId == userId);
            if (transaction == null)
                return NotFound();

            transaction.Amount = request.Amount;
            transaction.Description = request.Description;
            transaction.Category = request.Category;

            _context.SaveChanges();

            return Ok(new { message = "Transaction updated." });
        }

    
        [HttpDelete("{id}")]
        public IActionResult DeleteTransaction(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var transaction = _context.Transactions.FirstOrDefault(t => t.Id == id && t.UserId == userId);
            if (transaction == null)
                return NotFound();

            _context.Transactions.Remove(transaction);
            _context.SaveChanges();

            return Ok(new { message = "Transaction deleted." });
        }

        [HttpGet("category/{category}")]
        public IActionResult GetByCategory(string category)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var transactions = _context.Transactions
                .Where(t => t.UserId == userId && t.Category == category)
                .OrderByDescending(t => t.CreatedAt)
                .ToList();

            return Ok(transactions);
        }

     




    }
}
