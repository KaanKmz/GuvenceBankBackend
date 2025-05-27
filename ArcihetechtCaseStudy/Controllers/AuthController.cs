using Microsoft.AspNetCore.Mvc;
using ArcihetechtCaseStudy.Data;
using ArcihetechtCaseStudy.Models;
using ArcihetechtCaseStudy.DTOS;
using ArcihetechtCaseStudy.Helpers;
using ArchitechtCaseStudy.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims; // Kimlik Doğrulaması yapmaya yarayan kütüphanemiz
using Microsoft.AspNetCore.Identity; // Kimlik Doğrulaması yapmaya yarayan kütüphanemiz

using NETCore.MailKit.Core;

// Bu controllerdaki amacımız , kullanıcıların kayıt olması, giriş yapması, profil bilgilerinin görüntülenmesi/güncellenmesi/silinmesi , şifre değişikliği , JWT Token Yönetimi

namespace ArcihetechtCaseStudy.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthDbContext _context;
        private readonly IPasswordHasher<User> _passwordHasher;

        private readonly JwtService _jwtService; 


        
        public AuthController(AuthDbContext context, JwtService jwtService, IPasswordHasher<User> passwordHasher)
        {
            _context = context;
            _jwtService = jwtService;
            _passwordHasher = passwordHasher;
        }

        [HttpPost("register")]
        public IActionResult Register(UserRegisterRequestDto request)
        {
            if (_context.Users.Any(u => u.Username == request.Username))
            {
                return BadRequest("Username already exists.");
            }

            var user = new User
            {
                Username = request.Username,
                Email = request.Email
            };
            user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);
            _context.Users.Add(user);
            _context.SaveChanges();

            return Ok(new { message = "User registered successfully." });
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] UserLoginRequestDto request)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == request.Email);

            if (user == null)
            {
                return Unauthorized(new { message = "Invalid credentials." });
            }

            
            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);

            if (result == PasswordVerificationResult.Failed)
            {
                return Unauthorized(new { message = "Invalid credentials." });
            }

            
            var token = _jwtService.GenerateToken(user);

            return Ok(new
            {
                message = "Login successful.",
                token
            });
        

    }
    [Authorize]
        [HttpGet("profile")]
        public IActionResult Profile()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var username = User.FindFirst(ClaimTypes.Name)?.Value;
            var email = User.FindFirst(ClaimTypes.Email)?.Value;

            return Ok(new
            {
                Id = userId,
                Username = username,
                Email = email
            });
        }

        [HttpPut("changepassword")]
        public IActionResult ChangePassword(ChangePasswordDto dto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);

            if (user == null)
                return NotFound();

            
            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.CurrentPassword);

            if (result == PasswordVerificationResult.Failed)
                return BadRequest("Current password is incorrect.");

            user.PasswordHash = _passwordHasher.HashPassword(user, dto.NewPassword);
            _context.SaveChanges();

            return Ok(new { message = "Password changed successfully." });
        }

        [HttpPut("profile")]
        public IActionResult UpdateProfile([FromBody] UpdateUserDto request)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null) return NotFound();

            user.Username = request.Username;
            user.Email = request.Email;

            if (!string.IsNullOrWhiteSpace(request.NewPassword))
            {
                user.PasswordHash = _passwordHasher.HashPassword(user, request.NewPassword);
            }

            _context.SaveChanges();

            return Ok(new { message = "Profile updated." });
        }

        [HttpDelete("profile")]
        public IActionResult DeleteProfile()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null) return NotFound();

            _context.Users.Remove(user);
            _context.SaveChanges();

            return Ok(new { message = "Your account has been deleted." });
        }

        


        [HttpGet("me")]
        public IActionResult GetMe()
        {
            var username = User.FindFirst(ClaimTypes.Name)?.Value;
            return Ok(new { Username = username });
        }

        [HttpPost("logout")]
        
        public IActionResult Logout()
        {
            return Ok(new { message = "User logged out." });
        }


    }
}
