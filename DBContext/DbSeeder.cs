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
                    Email = "admin@example.com",
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

            if (!context.Roles.Any())
            {
                context.Roles.AddRange(
                    new Role { Name = "Admin" },
                    new Role { Name = "Customer" }
                );
                context.SaveChanges();
                logger.LogInformation("Roles seeded.");
            }

            if (!context.Products.Any())
            {
                context.Products.AddRange(
                    new Product { Name = "Laptop", Price = 1200.00M, Description = "High-performance laptop" },
                    new Product { Name = "Mouse", Price = 25.50M, Description = "Wireless mouse" }
                );
                context.SaveChanges();
                logger.LogInformation("Products seeded.");
            }
        }
    }

}
