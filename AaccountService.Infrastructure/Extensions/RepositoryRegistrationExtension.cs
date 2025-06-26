using Application.Interfaces;
using Infrastructure.Contexts;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Extensions;

public static class RepositoryRegistrationExtension
{
    public static IServiceCollection AddRepositories(this IServiceCollection services, IConfiguration configuration)
    {
        // register the DbContext based on configuration
        var useCosmosDb = configuration.GetValue("UseCosmosDb", false);
        
        if (useCosmosDb)
        {
            // use Cosmos DB on Azure
            var cosmosConnectionString = configuration.GetConnectionString("CosmosDb")
                                         ?? throw new InvalidOperationException("Connection string 'CosmosDb' not found.");
            var databaseName = configuration["CosmosDB:Database"] 
                               ?? throw new InvalidOperationException("CosmosDB:Database not found in configuration.");
                
            services.AddDbContext<AppDbContext>(options =>
                options.UseCosmos(cosmosConnectionString, databaseName));
        }
        else
        {
            // use SQL Server locally
            var sqlConnectionString = configuration.GetConnectionString("SqlServer")
                                      ?? throw new InvalidOperationException("Connection string 'SqlServer' not found.");
                
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(sqlConnectionString));
        }
        
        // register repositories
        services.AddScoped<IAccountRepository, AccountRepository>();
        
        return services;
    }
}