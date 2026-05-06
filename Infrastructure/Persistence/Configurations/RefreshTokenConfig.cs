using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class RefreshTokenConfig : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.TokenHash).IsRequired().HasMaxLength(200);
        b.HasIndex(x => x.TokenHash).IsUnique();
    }
}
