using IdentityServer.Core.Enums;

namespace IdentityServer.Core.DTOs;
public class RecentUserDto
{
    public string Id { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public UserStatus Status { get; set; }
}
