using Identity.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Data;

public class AppDbContext : 
    IdentityDbContext<
        AppUser,
        AppRole,
        Guid, 
        IdentityUserClaim, 
        IdentityUserRole<Guid>, 
        IdentityUserLogin<Guid>, 
        IdentityRoleClaim, 
        IdentityUserToken<Guid>>
{
    public AppDbContext(DbContextOptions options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }
}