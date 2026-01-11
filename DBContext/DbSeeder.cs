using EShopAPI.Models;
using Microsoft.AspNetCore.Identity;
using System.Data;

namespace EShopAPI.DBContext
{
    public static class DbSeeder
    {
        public static void Seed(AppDbContext context, ILogger logger)
        {
            logger.LogInformation("Starting seeding...");

            if (!context.Users.Any())
            {
                var hasher = new PasswordHasher<Users>();
                var admin = new Users
                {
                    Username = "admin",
                    Email = "admin@admin.com",
                    FullName = "Admin User",
                    PasswordHash = hasher.HashPassword(null, "admin123"),
                    PhoneNumber = "1234567890",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                context.Users.Add(admin);
                context.SaveChanges();
                logger.LogInformation("Admin user seeded.");
                logger.LogInformation($"Seeding admin: {admin.Username}, {admin.Email}");
            }
            else
            {
                logger.LogInformation("Admin user already exists.");
            }
        }
    }

}
