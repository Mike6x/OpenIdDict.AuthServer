using Framework.Core.Paging;
using Identity.Application.Scopes.Features;
using Microsoft.AspNetCore.Http;

namespace Identity.Application.Scopes;

public interface IScopeService
{
    Task<ScopeViewModel> CreateAsync(ScopeViewModel scopeDescriptor, CancellationToken cancellationToken);
    Task<ScopeViewModel> GetAsync(string scopeId, CancellationToken cancellationToken);
    Task<List<ScopeViewModel>> GetAllAsync(CancellationToken cancellationToken);
    Task<PagedList<ScopeViewModel>> SearchAsync(SearchScopesRequest request, CancellationToken cancellationToken);
    Task<IResult> DeleteAsync(string scopeId, CancellationToken cancellationToken);
    Task<IResult> UpdateAsync(ScopeViewModel descriptor, CancellationToken cancellationToken);

}