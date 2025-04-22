using Framework.Core.Paging;
using Identity.Application.Clients.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Application.Clients;

public interface IApplicationService
{
    Task<ApplicationViewModel?> CreateAsync(ApplicationViewModel applicationDescriptor,
        CancellationToken cancellationToken);

    Task<ApplicationViewModel> GetAsync(string clientId, CancellationToken cancellationToken);
    
    Task<List<ApplicationViewModel>> GetAllAsync(CancellationToken cancellationToken);

    Task<PagedList<ApplicationViewModel>> SearchAsync(SearchApplicationsRequest request,
        CancellationToken cancellationToken);

    Task<IResult> DeleteAsync(string clientId, CancellationToken cancellationToken);

    Task<IResult> UpdateAsync(ApplicationViewModel applicationDescriptor, CancellationToken cancellationToken);

    Task<IResult> CallbackAsync(HttpContext httpContext, [FromServices] IHttpClientFactory httpClientFactory);
}