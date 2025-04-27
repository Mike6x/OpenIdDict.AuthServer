namespace Identity.Application.Users.Dtos;

public class UserDto
{
    public Guid? Id { get; set; }
    
    public string? UserName { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    
    public string FullName => $"{FirstName} {LastName}".Trim();
    
    public DateTime CreatedOn { get; set; }
    
    public DateTime? LastLoginOn { get; set; }
    
    public string? Email { get; set; }
    public bool EmailConfirmed { get; set; }
    public string? PhoneNumber { get; set; }
    public bool PhoneNumberConfirmed { get; set; }

    public bool IsActive { get; set; }
}
