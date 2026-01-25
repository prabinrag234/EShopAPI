using EShopAPI.DBContext;
using EShopAPI.Helpers;
using EShopAPI.Models;
using EShopAPI.Requiests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EShopAPI.Controller
{
    /// <summary>
    /// Handles authentication, session, and token-related operations.
    /// </summary>
    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly AppDbContext _context;

        public AuthController(IConfiguration config, AppDbContext context)
        {
            _config = config;
            _context = context;
        }

        /// <summary>
        /// Authenticates a user with email and password, returns access + refresh tokens and user info.
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null)
                return Unauthorized(new { message = "Invalid email or password" });

            var hashed = PasswordHasher.Hash(request.Password);
            if (hashed != user.PasswordHash)
                return Unauthorized(new { message = "Invalid email or password" });

            var jwtSection = _config.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSection["Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim("email", user.Email),
                new Claim("role", "User")
            };

            var token = new JwtSecurityToken(
                issuer: jwtSection["Issuer"],
                audience: jwtSection["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            var refreshToken = new RefreshToken
            {
                UserId = user.Id,
                Token = TokenGenerator.GenerateRefreshToken(),
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(30)
            };

            _context.RefreshTokens.Add(refreshToken);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                accessToken = tokenString,
                refreshToken = refreshToken.Token,
                user = new
                {
                    user.Id,
                    user.Email,
                    user.Username
                }
            });
        }

        /// <summary>
        /// Registers a new user with email, username, and password.
        /// </summary>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == request.Email);

            if (existingUser != null)
                return BadRequest(new { message = "Email already registered" });

            var user = new User
            {
                Email = request.Email,
                Username = request.Username,
                PasswordHash = PasswordHasher.Hash(request.Password),
                Provider = "local"
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "User registered successfully" });
        }

        /// <summary>
        /// Refreshes an access token using a valid refresh token and rotates the refresh token.
        /// </summary>
        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken([FromBody] string refreshToken)
        {
            var token = await _context.RefreshTokens
                .FirstOrDefaultAsync(t => t.Token == refreshToken);

            if (token == null || !token.IsActive)
                return Unauthorized(new { message = "Invalid refresh token" });

            var user = await _context.Users.FindAsync(token.UserId);
            if (user == null)
                return Unauthorized();

            var jwtSection = _config.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSection["Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim("email", user.Email),
                new Claim("role", "User")
            };

            var newAccessToken = new JwtSecurityToken(
                issuer: jwtSection["Issuer"],
                audience: jwtSection["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );

            var accessTokenString = new JwtSecurityTokenHandler().WriteToken(newAccessToken);

            var newRefreshToken = TokenGenerator.GenerateRefreshToken();
            token.Token = newRefreshToken;
            token.ExpiresAt = DateTime.UtcNow.AddDays(30);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                accessToken = accessTokenString,
                refreshToken = newRefreshToken,
                user = new
                {
                    user.Id,
                    user.Email,
                    user.Username
                }
            });
        }

        /// <summary>
        /// Logs out from the current session by revoking the given refresh token.
        /// </summary>
        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] string refreshToken)
        {
            var token = await _context.RefreshTokens
                .FirstOrDefaultAsync(t => t.Token == refreshToken);

            if (token == null)
                return Ok(new { message = "Already logged out" });

            token.RevokedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Logged out successfully" });
        }

        /// <summary>
        /// Logs out from all active sessions for a given user.
        /// </summary>
        [HttpPost("logout-all")]
        public async Task<IActionResult> LogoutAll([FromBody] Guid userId)
        {
            var tokens = await _context.RefreshTokens
                .Where(t => t.UserId == userId && t.RevokedAt == null)
                .ToListAsync();

            foreach (var t in tokens)
                t.RevokedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Logged out from all devices" });
        }

        /// <summary>
        /// Returns all sessions (refresh tokens) for a given user, including status.
        /// </summary>
        [HttpGet("sessions/{userId}")]
        public async Task<IActionResult> GetSessions(Guid userId)
        {
            var sessions = await _context.RefreshTokens
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.CreatedAt)
                .Select(t => new
                {
                    t.Id,
                    t.Token,
                    t.CreatedAt,
                    t.ExpiresAt,
                    t.RevokedAt,
                    IsActive = t.IsActive
                })
                .ToListAsync();

            return Ok(sessions);
        }
    }
}
