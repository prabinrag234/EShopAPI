using EShopAPI.DBContext;
using Microsoft.AspNetCore.Mvc;

namespace EShopAPI.Controller
{
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TestController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("ping")]
        public async Task<IActionResult> PingDatabase()
        {
            try
            {
                var canConnect = await _context.Database.CanConnectAsync();
                return Ok(canConnect ? " Connected to PostgreSQL!" : " Failed to connect.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $" Error: {ex.Message}");
            }
        }
    }

}
