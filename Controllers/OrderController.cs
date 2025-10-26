using EShopAPI.Context;
using EShopAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EShopAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly AppDbContext _db;

        public OrderController(AppDbContext db) => _db = db;

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetMyOrders()
        {
            var customerId = User.FindFirst("sub")?.Value;
            var orders = await _db.Orders
                .Where(o => o.CustomerId == customerId)
                .Include(o => o.Items)
                .ThenInclude(i => i.Product)
                .ToListAsync();

            return Ok(orders);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> PlaceOrder(Order order)
        {
            order.CustomerId = User.FindFirst("sub")?.Value;
            order.OrderDate = DateTime.UtcNow;
            order.TotalAmount = order.Items.Sum(i => i.Quantity * i.UnitPrice);

            _db.Orders.Add(order);
            await _db.SaveChangesAsync();

            return Ok(order);
        }
    }
}
