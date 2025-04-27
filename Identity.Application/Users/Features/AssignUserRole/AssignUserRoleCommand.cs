using Identity.Application.Users.Dtos;

namespace Identity.Application.Users.Features.AssignUserRole;

public class AssignUserRoleCommand
{
    public List<UserRoleDetail> UserRoles { get; set; } = [];
}
