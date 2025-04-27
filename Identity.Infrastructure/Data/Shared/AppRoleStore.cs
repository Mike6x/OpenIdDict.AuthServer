using System.Security.Claims;
using Identity.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Data.Shared;

public class AppRoleStore: RoleStore<AppRole, IdentityContext, Guid, IdentityUserRole<Guid>, IdentityRoleClaim>
{
    private DbSet<IdentityRoleClaim> RoleClaims { get { return Context.Set<IdentityRoleClaim>(); } }

    public AppRoleStore(IdentityContext context, IdentityErrorDescriber? describer = null)
        : base(context, describer)
    {
    }

    public async override Task<IList<Claim>> GetClaimsAsync(AppRole role, CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        if (role == null)
        {
            throw new ArgumentNullException(nameof(role));
        }

        return await RoleClaims.Where(rc => rc.RoleId.Equals(role.Id)).Select(c => c.ToClaim()).ToListAsync(cancellationToken);
    }

    protected override IdentityRoleClaim CreateRoleClaim(AppRole role, Claim claim)
    {
        var roleClaim = base.CreateRoleClaim(role, claim);
        roleClaim.InitializeFromClaim(claim);
        return roleClaim;
    }
}
