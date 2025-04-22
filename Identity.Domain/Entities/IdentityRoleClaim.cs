using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;

namespace Identity.Domain.Models;

/// <summary>
/// IdentityRoleClaim with <see cref="Guid"/> as the Identifier type
/// </summary>
public class IdentityRoleClaim : IdentityRoleClaim<Guid>
{
    /// <summary>
    /// Additional properties associated with this claim
    /// </summary>
    public virtual string Properties { get; set; }
    
    /// <summary>
    /// These from FSH
    /// </summary>
    public Guid? CreatedBy { get; init; }
    public DateTimeOffset CreatedOn { get; init; }

    public IdentityRoleClaim()
    {

    }

    /// <inheritdoc/>  
    public override Claim ToClaim()
    {
        var claim = base.ToClaim();
        foreach (var property in this.Properties?.Trim(']', '[').Split(',', StringSplitOptions.None).Select(r => r.Trim('\"')))
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