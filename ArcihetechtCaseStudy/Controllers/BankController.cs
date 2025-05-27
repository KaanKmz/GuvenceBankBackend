using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ArcihetechtCaseStudy.Data;
using ArcihetechtCaseStudy.DTOS;

namespace ArchitechtCaseStudy.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BankController : ControllerBase
    {
        private readonly AuthDbContext _context;

        public BankController(AuthDbContext context)
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


       
        [HttpPost("topup")]
        public IActionResult TopUpBalance([FromBody] TopUpDto request)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
                return NotFound();

            user.Balance += request.Amount;
            _context.SaveChanges();

            return Ok(new
            {
                message = $"Balance topped up with {request.Amount}₺",
                newBalance = user.Balance
            });
        }
    }
}
