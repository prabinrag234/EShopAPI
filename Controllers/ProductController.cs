using EShopAPI.Context;
using EShopAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EShopAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class ProductController : ControllerBase
    {
        private readonly AppDbContext _db;

        public ProductController(AppDbContext db) => _db = db;

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll() =>
            Ok(await _db.Products.Include(p => p.Shop).ToListAsync());

        [HttpPost]
        [Authorize(Roles = "shop_owner")]
        public async Task<IActionResult> Add(Product product)
        {
            var ownerId = User.FindFirst("sub")?.Value;
            var shop = await _db.Shops.FirstOrDefaultAsync(s => s.OwnerId == ownerId);

            if (shop == null) return BadRequest("Shop not found for this owner.");

            product.ShopId = shop.Id;
            _db.Products.Add(product);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAll), new { id = product.Id }, product);
        }
    }

}
