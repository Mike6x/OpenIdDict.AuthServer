using System.Security.Claims;
using Identity.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Identity.Infrastructure.Data.Shared;

public class AppUserStore : UserStore<AppUser, AppRole, IdentityContext, Guid,
    IdentityUserClaim, IdentityUserRole<Guid>, IdentityUserLogin<Guid>,
    IdentityUserToken<Guid>, IdentityRoleClaim>
{
    public AppUserStore(IdentityContext context, IdentityErrorDescriber describer = null)
        : base(context, describer)
    {
    }

    public override Task<IList<Claim>> GetClaimsAsync(AppUser user, CancellationToken cancellationToken = default)
    {
        return base.GetClaimsAsync(user, cancellationToken);
    }

    protected override IdentityUserClaim CreateUserClaim(AppUser user, Claim claim)
    {
        var userClaim = base.CreateUserClaim(user, claim);
        userClaim.InitializeFromClaim(claim);
        return userClaim;
    }
}