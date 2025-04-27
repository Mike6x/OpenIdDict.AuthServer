using Identity.Application.Claims;

namespace Identity.Application.Roles.Features.CreateOrUpdateRole;

public class UpdateRoleCommand
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    
    public string? Description { get; set; }
    
    public List<ClaimViewModel> Claims { get; set; } = [];
}