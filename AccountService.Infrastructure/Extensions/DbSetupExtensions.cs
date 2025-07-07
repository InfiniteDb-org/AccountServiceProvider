using Microsoft.Extensions.DependencyInjection;
using Infrastructure.Contexts;

namespace Infrastructure.Extensions;

// auto-creating Cosmos DB containers in dev/staging environments
public static class DbSetupExtensions
{
    public static async Task EnsureCosmosContainersCreatedAsync(this IServiceProvider services, string? environmentName)
    {
        // Skip in production for safety
        if (environmentName != "Development" && environmentName != "Staging")
            return;

        try
        {
            using var scope = services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await db.Database.EnsureCreatedAsync();
            // Ensures dev/staging always have required containers for smooth local/test runs
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[EnsureCosmosContainersCreatedAsync] Exception: {ex}");
        }
    }
}
