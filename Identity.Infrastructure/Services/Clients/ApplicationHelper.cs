using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.Extensions.Options;
using OpenIddict.Abstractions;
using OpenIddict.EntityFrameworkCore.Models;

namespace Identity.Infrastructure.Services.Clients.Endpoints;

public static class ApplicationHelper
{
    /// <summary>
    /// Get all the available applications
    /// </summary>
    /// <returns></returns>
    public static IAsyncEnumerable<object> ListApplicationsAsync(IOpenIddictApplicationManager applicationManager)
    {
        Func<IQueryable<object>, IQueryable<OpenIddictEntityFrameworkCoreApplication>> query = (apps) =>
        {
            return apps.Where(app => true).Select(s => s as OpenIddictEntityFrameworkCoreApplication)!;
        }; 
        
        return applicationManager.ListAsync(query, CancellationToken.None);
    }
    
}


