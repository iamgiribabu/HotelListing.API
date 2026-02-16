using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelListing.API.Data.Configurations;

public class ApiKeyConfiguration : IEntityTypeConfiguration<ApiKey>
{
    public void Configure(EntityTypeBuilder<ApiKey> builder)
    {
        builder.HasIndex(a => a.Key).IsUnique();
        builder.HasData(
            new ApiKey
            {
                Id = 1,
                Key = "de9b1c8f-5a2e-4c3b-9f1a-2b3c4d5e6f7g",
                AppName = "MyApp",
                ExpiresAtUtc = new DateTimeOffset(2026, 02, 15, 7, 20, 34, TimeSpan.Zero),  // ✅ Fixed date
                CreatedAtUtc = new DateTimeOffset(2025, 02, 15, 7, 20, 34, TimeSpan.Zero), // ✅ Fixed date
            }
        );
    }
}
