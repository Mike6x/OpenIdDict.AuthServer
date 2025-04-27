namespace Identity.Application.Roles.Features.CreateOrUpdateRole;

public class CreateOrUpdateRoleCommand
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}
