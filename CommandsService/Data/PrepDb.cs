using CommandService.SyncDataServices.Grpc;
using CommandsService.Data;
using CommandService.Models;

namespace CommandService.Data
{
    public static class PrepDb
    {
        public static void PrepPopulation(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                var grpcClient = serviceScope.ServiceProvider.GetService<IPlatformDataClient>();
                if (grpcClient != null)
                {
                    var platforms = grpcClient.ReturnAllPlatforms();
                    var repo = serviceScope.ServiceProvider.GetService<ICommandRepo>();
                    if (repo != null)
                    {
                        SeedData(repo, platforms);
                    }
                    else
                    {
                        Console.WriteLine("Failed to retrieve ICommandRepo from service provider.");
                    }
                }
                else
                {
                    Console.WriteLine("Failed to retrieve IPlatformDataClient from service provider.");
                }
            }
        }
        private static void SeedData(ICommandRepo repo, IEnumerable<Platform> platforms)
        {
            Console.WriteLine("Seeding new platforms");
            foreach (var platform in platforms)
            {
                if (!repo.ExternalPlatformExists(platform.ExternalId))
                {
                    repo.CreatePlatform(platform);
                }
                repo.SaveChanges();
            }
        }
    }
}