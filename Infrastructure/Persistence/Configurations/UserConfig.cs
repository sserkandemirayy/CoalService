using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class UserConfig : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.Email).IsRequired().HasMaxLength(256);
        b.HasIndex(x => x.Email).IsUnique();
        b.Property(x => x.PasswordHash).IsRequired();

        b.HasMany(x => x.UserRoles).WithOne(ur => ur.User).HasForeignKey(ur => ur.UserId);
        b.HasMany(x => x.RefreshTokens).WithOne(rt => rt.User).HasForeignKey(rt => rt.UserId);
    }
}
