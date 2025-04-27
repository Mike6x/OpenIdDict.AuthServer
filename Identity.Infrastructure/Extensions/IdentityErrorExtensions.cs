using Microsoft.AspNetCore.Identity;

namespace Identity.Infrastructure.Extensions
{
    public static class IdentityErrorExtensions
    {
        public static IEnumerable<string> GetErrors(this IdentityResult identityResult)
        {
            return identityResult.Errors.Select(s => $"{s.Code}:{s.Description}").Distinct();
        }
    }
}
