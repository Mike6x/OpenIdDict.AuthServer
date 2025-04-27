using Framework.Core.Paging;

namespace Identity.Application.Users.Features.SearchUsers;

public class SearchUsersRequest : PaginationFilter
{
    public bool? IsActive { get; set; }
}
