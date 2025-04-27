using Identity.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Data;

public class IdentityContext : 
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
    public IdentityContext(DbContextOptions options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }
    
}