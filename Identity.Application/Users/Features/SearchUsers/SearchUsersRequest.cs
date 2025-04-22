using FSH.Framework.Core.Paging;

namespace FSH.Framework.Core.Identity.Users.Features.ExportUsers;

public class SearchUsersRequest : PaginationFilter
{
    public bool? IsActive { get; set; }
}
