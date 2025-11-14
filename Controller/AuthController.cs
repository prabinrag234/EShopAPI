using EShopAPI.DBContext;
using EShopAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EShopAPI.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AuthController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.Email);
            if (user == null)
                return NotFound("User not found");

            var hasher = new PasswordHasher<Users>();
            var result = hasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);

            if (result == PasswordVerificationResult.Failed)
                return Unauthorized("Invalid credentials");

            return Ok(new
            {
                Message = "Login successful",
                User = new { user.Id, user.Username, user.Email, user.FullName }
            });
        }
    }

}
