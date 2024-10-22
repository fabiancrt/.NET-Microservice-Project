using PlatformService.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using Microsoft.EntityFrameworkCore;
namespace PlatformService.Data
{
    public static class PrepDb
    {
        public static void PrepPopulation(IApplicationBuilder app, bool isProd)
        {
            using( var serviceScope = app.ApplicationServices.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<AppDbContext>();
                if (context != null)
                {
                    SeedData(context, isProd);
                }
                else
                {
                    System.Console.WriteLine("AppDbContext null.");
                }
            }
        }

        private static void SeedData(AppDbContext context, bool isProd)
        {

            if(isProd)
            {
                System.Console.WriteLine("Attempting to apply migrations...");
                try
                {
                    context.Database.Migrate();
                }
                catch (System.Exception ex)
                {
                    System.Console.WriteLine($"Could not run migrations: {ex.Message}");
                }
            }

            if (!context.Platforms.Any())
            {
                Console.WriteLine("Seeding Data...");
                context.Platforms.AddRange(
                    new Platform { Name = "DotNet", Publisher = "Microsoft", Cost = "Free" },
                    new Platform { Name = "SQL Server Express", Publisher = "Microsoft", Cost = "Free" },
                    new Platform { Name = "Kubernetes", Publisher = "Cloud Native Computing Foundation", Cost = "Free" }
                );
                context.SaveChanges();
            }
            else
            {
                Console.WriteLine("We already have data");
            }
        }
    }
}