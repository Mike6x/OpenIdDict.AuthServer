namespace Identity.Application.Users.Dtos;

public class RecentUserDto
{
    public string Id { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime CreatedOn { get; set; }
    public bool IsActive { get; set; } 
    public bool IsOnline { get; set; }
    public virtual DateTimeOffset? LockoutEnd { get; set; }

}
