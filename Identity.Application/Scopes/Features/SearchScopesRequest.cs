using Framework.Core.Paging;

namespace Identity.Application.Scopes.Features;

public class SearchScopesRequest : PaginationFilter
{
    public string ScopesFilter { get; set; } = string.Empty;
}