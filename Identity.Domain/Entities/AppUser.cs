using Microsoft.AspNetCore.Identity;

namespace Identity.Domain.Entities;

// Add profile data for application users by adding properties to the AppUser class
public class AppUser : IdentityUser<Guid>
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public Uri? ImageUrl { get; set; }
    public bool IsActive { get; set; }
    public bool? IsOnline { get; set; }
    
    public string? RefreshToken { get; set; }
    
    public DateTime RefreshTokenExpiryTime { get; set; }
    
    public DateTime? LastLoginOn { get; set; }
    
    public DateTime CreatedOn { get; set; }
    
    public Guid? CreatedBy { get; set; }
    
    public Guid? LastModifiedBy { get; set; }

    public DateTime? LastModifiedOn { get; set; }
    
    public string? ObjectId { get; set; }
    

    // public List<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>()
    // public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = default!
    // public virtual ICollection<AccessToken> AccessTokens { get; set; } = default!
    // public virtual ICollection<ApplicationUserRole> UserRoles { get; set; } = default!

}
