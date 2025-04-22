using Microsoft.AspNetCore.Identity;
using Identity.Domain.Enums;

namespace Identity.Domain.Models;

// Add profile data for application users by adding properties to the AppUser class
public class AppUser : IdentityUser
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public Uri? ImageUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid? CreatedBy { get; set; }
    public DateTime? LastLoginDate { get; set; }
    public UserStatus Status { get; set; } = UserStatus.Active;
    public string? ObjectId { get; set; }
    
    // public List<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    // public string? RefreshToken { get; set; }
    // public DateTime RefreshTokenExpiryTime { get; set; }
   // public bool PasswordChangeRequired { get; set; } = false;
}
