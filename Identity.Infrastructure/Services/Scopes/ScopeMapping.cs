using Identity.Application.Scopes;
using Newtonsoft.Json;
using OpenIddict.Abstractions;
using OpenIddict.EntityFrameworkCore.Models;

namespace Identity.Infrastructure.Services.Scopes;

public static class ScopeMapping
{
    public static ScopeViewModel ToDto(this OpenIddictEntityFrameworkCoreScope source)
    {
        return new ScopeViewModel
        {
            Id = source.Id ?? string.Empty,
            Name = source.Name ?? string.Empty,
            DisplayName = source.DisplayName ?? string.Empty,
            Description = source.Description ?? string.Empty,
            Resources = string.IsNullOrEmpty(source.Resources) 
                    ? []
                    : JsonConvert.DeserializeObject<IEnumerable<string>>(source.Resources).ToList(),

        };
    }

    public static OpenIddictScopeDescriptor ToModel(this ScopeViewModel source)
    {
        var destination = new OpenIddictScopeDescriptor
        {
            Name = source.Name,
            DisplayName = source.DisplayName,
            Description = source.Description,

        };

        if (source.Resources.Count > 0)
            foreach (var resource in source.Resources)
            {
                destination.Resources.Add(resource);
            }

        return destination;
    }
}