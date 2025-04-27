namespace Identity.Application.Roles.Features.UpdatePermissions;
public class UpdatePermissionsCommand
{
    public string RoleId { get; set; } = default!;
    public List<string> Permissions { get; set; } = default!;
}
