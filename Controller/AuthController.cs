using EShopAPI.BaseLibrary;
using EShopAPI.DBContext;
using EShopAPI.Models;
using EShopAPI.Requiests;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace EShopAPI.Controller
{
    [ApiController]
    [Route("eshop/user")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context; 
        private readonly PasswordHasher _hasher;

        public AuthController(AppDbContext context, PasswordHasher hasher)
        {
            _context = context;
            _hasher = hasher;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequestDTO request)
        {
            if (request == null)
                return BadRequest("Invalid request");

            if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
                return BadRequest("Email and password are required");

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Username);

            if (user == null)
                return NotFound("User not found");

            if (string.IsNullOrEmpty(user.PasswordHash))
                return StatusCode(500, "Password hash missing");

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

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDTO dto)
        {
            if (dto == null)
                return BadRequest(new { errors = new[] { "Invalid request payload" } });

            var errors = new List<string>();

            // Basic field validation
            if (string.IsNullOrWhiteSpace(dto.Username))
                errors.Add("Username is required");

            if (string.IsNullOrWhiteSpace(dto.Email))
                errors.Add("Email is required");

            if (string.IsNullOrWhiteSpace(dto.Password))
                errors.Add("Password is required");

            if (string.IsNullOrWhiteSpace(dto.FullName))
                errors.Add("Full name is required");

            // Stop early if basic validation fails
            if (errors.Count > 0)
                return BadRequest(new { errors });

            // Database uniqueness checks
            if (await _context.Users.AnyAsync(u => u.Username == dto.Username))
                errors.Add("Username already exists");

            if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
                errors.Add("Email already registered");

            if (errors.Count > 0)
                return BadRequest(new { errors });

            // Create user
            var user = new Users
            {
                Username = dto.Username,
                Email = dto.Email,
                PasswordHash = _hasher.Hash(dto.Password),
                FullName = dto.FullName,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsActive = true
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Registration successful" });
        }


    }

}
