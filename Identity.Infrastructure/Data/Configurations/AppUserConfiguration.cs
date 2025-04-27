using Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Infrastructure.Data.Configurations;

internal class AppUserConfiguration : IEntityTypeConfiguration<AppUser>
{
    public void Configure(EntityTypeBuilder<AppUser> builder)
    {
        builder.ToTable("Users", IdentityConstants.SchemaName);
        
        builder.Property(e => e.FirstName).HasMaxLength(63);
        builder.Property(e => e.LastName).HasMaxLength(63);
        
        builder.Property(e => e.ObjectId).HasMaxLength(256);
    }
}
