using Ardalis.Specification.EntityFrameworkCore;
using Framework.Core.Exceptions;
using Framework.Core.Paging;
using Framework.Core.Specifications;
using Identity.Application.Clients;
using Identity.Application.Clients.Features;
using Identity.Infrastructure.Services.CorsPolicy;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using OpenIddict.Abstractions;
using OpenIddict.EntityFrameworkCore.Models;

namespace Identity.Infrastructure.Services.Clients;

public class ApplicationService(
    IOpenIddictApplicationManager applicationManager,
    IOptions<CorsOptions> corsOptions) : IApplicationService
{
    
    public async Task<ApplicationViewModel?> CreateAsync(ApplicationViewModel applicationDescriptor, CancellationToken cancellationToken)
    {
        if (await applicationManager.FindByClientIdAsync(applicationDescriptor.ClientId, cancellationToken) is not null)
            throw new ConflictException($"Application: {applicationDescriptor.ClientId} have existed");
                
        var openIdApplicationDescriptor = applicationDescriptor.ToModel();
                
        if(applicationDescriptor.RedirectUris.Count != 0)
        {
            OriginHelper.AllowOriginsAsync(applicationDescriptor.RedirectUris, corsOptions);
        }

        var result = await applicationManager.CreateAsync(openIdApplicationDescriptor, cancellationToken)
            as OpenIddictEntityFrameworkCoreApplication;

        return result?.ToDto();

    }
    
    public async Task<ApplicationViewModel> GetAsync(string clientId, CancellationToken cancellationToken)
    {
        var existing = await applicationManager.FindByClientIdAsync(clientId, cancellationToken) 
                           as OpenIddictEntityFrameworkCoreApplication
                       ?? throw new NotFoundException($"Failed to find application: {clientId}");
                
        var applicationDescriptor = existing.ToDto();
        return applicationDescriptor;
    }
    
    public async Task<List<ApplicationViewModel>>GetAllAsync(CancellationToken cancellationToken)
    {
        List<ApplicationViewModel> applicationDescriptors = [];

        Func<IQueryable<object>, IQueryable<OpenIddictEntityFrameworkCoreApplication>> query;
                
        query = sources => sources.Where(app => true)
            .Select(s => s as OpenIddictEntityFrameworkCoreApplication)
            .OrderBy(s => s.ClientId);
                
        await foreach (var app in applicationManager.ListAsync(query, cancellationToken))
        {
            var applicationDescriptor = app.ToDto();
            applicationDescriptors.Add(applicationDescriptor);
        }
        
        return applicationDescriptors;
    }
    
    public async Task<PagedList<ApplicationViewModel>> SearchAsync ( SearchApplicationsRequest request, CancellationToken cancellationToken)
    {
        var spec = new EntitiesByPaginationFilterSpec<OpenIddictEntityFrameworkCoreApplication>(request);
                
        List<ApplicationViewModel> applicationDescriptors = [];

        Func<IQueryable<object>, IQueryable<OpenIddictEntityFrameworkCoreApplication>> query;
                
        query = apps => apps.Where(app => true)
            .Select(s => s as OpenIddictEntityFrameworkCoreApplication)
            .WithSpecification(spec)
            .OrderBy(s => s.ClientId);
                
        await foreach (var app in applicationManager.ListAsync(query, cancellationToken))
        {
            var applicationDescriptor = app.ToDto();
            //applicationDescriptor.ClientSecret = string.Empty
            applicationDescriptors.Add(applicationDescriptor);
        }

        var count = (int)await applicationManager.CountAsync(cancellationToken);
                
        return new PagedList<ApplicationViewModel>(applicationDescriptors, request.PageNumber, request.PageSize, count);

    }
    
    public async Task<IResult> DeleteAsync(string clientId, CancellationToken cancellationToken)
    {
        var existing = await applicationManager.FindByClientIdAsync(clientId, cancellationToken) 
                           as OpenIddictEntityFrameworkCoreApplication
                       ?? throw new NotFoundException($"Failed to find application: {clientId}");
                
        await applicationManager.DeleteAsync(existing, cancellationToken);
                
        var descriptor = existing.ToDto();
        if (descriptor.RedirectUris.Count != 0)
        {
            OriginHelper.RemoveOriginsAsync(descriptor.RedirectUris, corsOptions);
        }
                
        return Results.Ok();
    }
    
    public async Task<IResult> UpdateAsync (ApplicationViewModel applicationDescriptor, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(applicationDescriptor.ClientId)) return Results.BadRequest();
        
        var existing = await applicationManager.FindByIdAsync(applicationDescriptor.Id, cancellationToken) 
                       ?? throw new NotFoundException($"Failed to find application WITH ID: {applicationDescriptor.Id}");
        
        var descriptorFromExisting = new OpenIddictApplicationDescriptor();
        await applicationManager.PopulateAsync(descriptorFromExisting, existing, cancellationToken);
        
        if (applicationDescriptor.ClientId != descriptorFromExisting.ClientId 
            && await applicationManager.FindByClientIdAsync(applicationDescriptor.ClientId, cancellationToken) is not null)
            throw new ConflictException($"Application: {applicationDescriptor.ClientId} have existed");

        var openIdApplicationDescriptor = applicationDescriptor.ToModel();
        
        //No new secret to update. Populate existing on descriptor before updating
        if (applicationDescriptor.IsConfidentialClient)
        {                     
            if(string.IsNullOrEmpty(applicationDescriptor.ClientSecret))
            {
                openIdApplicationDescriptor.ClientSecret = descriptorFromExisting.ClientSecret;
            }
            if(string.IsNullOrEmpty(applicationDescriptor.JsonWebKeySet))
            {
                openIdApplicationDescriptor.JsonWebKeySet = descriptorFromExisting.JsonWebKeySet;
            }
        }  
        
        if(!openIdApplicationDescriptor.RedirectUris.SequenceEqual(descriptorFromExisting.RedirectUris))
        {
            OriginHelper.RemoveOriginsAsync(descriptorFromExisting.RedirectUris, corsOptions);
            OriginHelper.AllowOriginsAsync(applicationDescriptor.RedirectUris, corsOptions);
        }
        
        await applicationManager.UpdateAsync(existing, openIdApplicationDescriptor, cancellationToken);
        
        return Results.Ok();

    }
    
    //https://github.com/legimenes/articles-OpenIddictAuthorizationServer/blob/main/src/AuthorizationServer/Endpoints/ApplicationEndpoint.cs
    public async Task<IResult> CallbackAsync(HttpContext httpContext, [FromServices] IHttpClientFactory httpClientFactory)
    {
        IEnumerable<KeyValuePair<string, StringValues>> parameters = httpContext.Request.HasFormContentType ?
            httpContext.Request.Form : httpContext.Request.Query;

        Dictionary<string, string> formData = new()
        {
            { "grant_type", "authorization_code" },
            { "code_verifier", "AVA~cbYg_UDgPYrJNJX.kMotv0x.z8nY~C23XzWq4DxEUu0cw9rWk6SwlgHgihmBoPN4.WKV0H1ui6TTL3vCWC0jyv7fYlAef3Z-y-7rgC6~0m9bb06x8FEO24LJArH4" },
            { "client_id", "test_client" },
            { "client_secret", "test_secret" },
            { "redirect_uri", "https://localhost:4001/callback" }
        };
        var codeParameter = parameters.First(p => p.Key == "code");
        formData.Add(codeParameter.Key, codeParameter.Value);

        var httpClient = httpClientFactory.CreateClient("TokenApiClient");
        FormUrlEncodedContent content = new(formData);
        var response = await httpClient.PostAsync("connect/token", content);

        response.EnsureSuccessStatusCode();
        var responseContent = await response.Content.ReadAsStringAsync();
        dynamic? jsonObject = JsonConvert.DeserializeObject<dynamic>(responseContent);

        return Results.Json(jsonObject);
    }
}