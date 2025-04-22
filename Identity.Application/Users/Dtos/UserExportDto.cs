namespace Identity.Application.Users.Dtos;

public class UserExportDto
{
    public Guid Id { get; set; }

    public string? UserName { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? Email { get; set; }

    public bool IsActive { get; set; }

    public bool EmailConfirmed { get; set; }

    public string? PhoneNumber { get; set; }

    public Uri? ImageUrl { get; set; }
    
    public Guid CreatedBy { get; set; }
    public DateTimeOffset Created { get; set; }
    public Guid LastModifiedBy { get; set; }
    public DateTimeOffset? LastModified { get; set; }
}
