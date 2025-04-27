// Summary:
//     Gets or sets the date and time, in UTC, when any user lockout ends.
//
// Remarks:
//     A value in the past means the user is not locked out.
namespace Identity.Application.Users.Dtos;

public class UserDetail
{
    public Guid Id { get; set; }
    
    public string? UserName { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    
    public string FullName => $"{FirstName} {LastName}".Trim();
    
    public Uri? ImageUrl { get; set; }

    public string? Email { get; set; }
    
    public bool EmailConfirmed { get; set; }

    public string? PhoneNumber { get; set; }

    public bool IsActive { get; set; } 
    public bool IsOnline { get; set; }

    public virtual DateTimeOffset? LockoutEnd { get; set; }
    public bool IsLocked => LockoutEnd != null && LockoutEnd > DateTime.UtcNow;
    
    public DateTime? LastLoginOn { get; set; }
    
    public Guid? CreatedBy { get; set; }

    public DateTime? CreatedOn { get; set; }

    public Guid? LastModifiedBy { get; set; }

    public DateTime? LastModifiedOn { get; set; }
}

    
