using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;

namespace Identity.Domain.Entities;

public class IdentityUserClaim : IdentityUserClaim<Guid>
{
    /// <summary>
    /// Additional properties associated with this claim
    /// </summary>
    public virtual string Properties { get; set; }


    public IdentityUserClaim()
    {

    }

    /// <inheritdoc/>  
    public override Claim ToClaim()
    {
        var claim = base.ToClaim();
        foreach (var property in this.Properties?.Trim(']', '[').Split(',', StringSplitOptions.None).Select(r => r.Trim('\"'))!)
        {
            var keyValue = property.Split(':');
            claim.Properties.Add(keyValue[0], keyValue[1]);
        }
        return claim;
    }

    /// <inheritdoc/>  
    public override void InitializeFromClaim(Claim claim)
    {
        base.InitializeFromClaim(claim);
        StringBuilder sb = new StringBuilder();
        foreach (var (key, value) in claim.Properties)
        {

            sb.Append("\"");
            sb.Append(key);
            sb.Append(":");
            sb.Append(value);
            sb.Append("\"");
            sb.Append(',');
        }
        this.Properties = $"[{sb.ToString().Trim(',')}]";
    }
}
