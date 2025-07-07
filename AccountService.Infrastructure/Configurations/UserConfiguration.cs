using Application.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

// Configures User entity mapping for EF Core and Cosmos DB
public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToContainer("Users");
        builder.HasPartitionKey(u => u.Email); // Partition key // email or Id
        builder.HasKey(u => u.Id);
    }
}