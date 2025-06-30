using Microsoft.Extensions.DependencyInjection;
using Infrastructure.Contexts;

namespace Infrastructure.Extensions;

public static class DbSetupExtensions
{
    public static async Task EnsureCosmosContainersCreatedAsync(this IServiceProvider services, string? environmentName)
    {
        if (environmentName != "Development" && environmentName != "Staging")
            return;

        try
        {
            using var scope = services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await db.Database.EnsureCreatedAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[EnsureCosmosContainersCreatedAsync] Exception: {ex}");
        }
    }
}
