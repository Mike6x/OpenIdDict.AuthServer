using Framework.Core.Paging;

namespace Identity.Application.Roles.Features.SearchRoles;

public class SearchRolesRequest : PaginationFilter
{
    public string? RoleFilter { get; set; }
}