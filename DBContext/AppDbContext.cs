using EShopAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace EShopAPI.DBContext
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<Users> Users { get; set; }
    }
}
