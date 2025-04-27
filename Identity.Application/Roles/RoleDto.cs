using Identity.Application.Claims;
using Identity.Application.Users.Dtos;

namespace Identity.Application.Roles;
/// <summary>
/// UserRoleViewModel
/// </summary>
public class RoleDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public List<string> Permissions { get; set; } = [];
    
    public List<ClaimViewModel> Claims { get; set; } = [];
    
    public RoleDto()
    {

    }
    public RoleDto(string roleName)
    {
        Id = Guid.NewGuid();
        Name = roleName;
    }
    public RoleDto(Guid roleId, string roleName) : this(roleName)
    {
        Id = roleId;         
    }
}
