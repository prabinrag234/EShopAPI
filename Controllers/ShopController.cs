using EShopAPI.Context;
using EShopAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EShopAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ShopController : ControllerBase
    {
        private readonly AppDbContext _db;

        public ShopController(AppDbContext db) => _db = db;

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll() =>
            Ok(await _db.Shops.Include(s => s.Products).ToListAsync());

        [HttpPost]
        [Authorize(Roles = "shop_owner")]
        public async Task<IActionResult> RegisterShop(Shop shop)
        {
            shop.OwnerId = User.FindFirst("sub")?.Value;
            _db.Shops.Add(shop);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAll), new { id = shop.Id }, shop);
        }
    }
}
