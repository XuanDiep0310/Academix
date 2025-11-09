using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Academix.Infrastructure;

namespace SetupAdmin
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            var connectionString = configuration.GetConnectionString("DbContext");


            var optionsBuilder = new DbContextOptionsBuilder<AcademixDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            using var context = new AcademixDbContext(optionsBuilder.Options);

            // Find admin user
            var adminUser = await context.Users
                .FirstOrDefaultAsync(u => u.Email == "admin@academix.com");

            if (adminUser == null)
            {
                Console.WriteLine("Admin user not found. Please run SQL scripts first.");
                return;
            }

            // Generate password hash using BCrypt
            var password = "Academix@K64";
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

            // Update user
            adminUser.PasswordHash = passwordHash;
            adminUser.UpdatedAt = DateTime.UtcNow;


            await context.SaveChangesAsync();

            Console.WriteLine($"Admin password set successfully!");
            Console.WriteLine($"Email: {adminUser.Email}");
            Console.WriteLine($"Password: {password}");
        }
    }
}
