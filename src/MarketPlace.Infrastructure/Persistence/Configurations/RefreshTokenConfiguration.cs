using MarketPlace.Infrastructure.Identity.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketPlace.Infrastructure.Persistence.Configurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.HasKey(rt => rt.Id);
        builder.Property(rt => rt.Token).IsRequired();
        builder.Property(rt => rt.Expires).IsRequired();
        builder.Property(rt => rt.Created).IsRequired();
        builder.Property(rt => rt.CreatedByIp).IsRequired();
        builder.Property(rt => rt.Revoked);
        builder.Property(rt => rt.RevokedByIp);

    }
    

  
}