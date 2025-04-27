using Framework.Core.Paging;

namespace Identity.Application.Clients.Features;

public class SearchApplicationsRequest : PaginationFilter
{
    public string? RoleFilter { get; set; }
}