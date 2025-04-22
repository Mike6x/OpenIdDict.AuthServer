using Microsoft.AspNetCore.Identity;

namespace Identity.Domain.Models;

public class AppRole : IdentityRole
{
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    
    // public ICollection<AppPermission> Permissions { get; set; } = [];
    
    public AppRole(string name, string? description = null)
        : base(name)
    {
        Description = description;
        CreatedAt = DateTime.UtcNow;
        
        NormalizedName = name.ToUpperInvariant();
    }

}