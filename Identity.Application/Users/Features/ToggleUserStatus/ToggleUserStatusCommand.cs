namespace Identity.Application.Users.Features.ToggleUserStatus;
public class ToggleUserStatusCommand
{
    public bool IsActive { get; set; }
    public string? UserId { get; set; }
}
