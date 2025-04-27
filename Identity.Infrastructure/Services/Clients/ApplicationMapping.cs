using System.Security.Cryptography;
using Identity.Application.Clients;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using OpenIddict.Abstractions;
using OpenIddict.EntityFrameworkCore.Models;

namespace Identity.Infrastructure.Services.Clients;


public static class ApplicationMapping
{
    public static ApplicationViewModel ToDto(this OpenIddictEntityFrameworkCoreApplication source)
    {
        return new ApplicationViewModel
        {
            Id = source.Id ?? string.Empty,
            ApplicationType = source.ApplicationType ?? string.Empty,
            ClientId = source.ClientId ?? string.Empty,
            ClientType = source.ClientType ?? string.Empty,
            ClientSecret = source.ClientSecret,
            JsonWebKeySet = source.JsonWebKeySet,
            ConsentType = source.ConsentType ?? string.Empty,
            DisplayName = source.DisplayName ?? string.Empty,
            
            Permissions = string.IsNullOrEmpty(source.Permissions)
                ? [] 
                : source.Permissions .Trim(']', '[').Split(',', StringSplitOptions.None).Select(p => p.Trim('\"')).ToList(),
            
            RedirectUris = string.IsNullOrEmpty(source.RedirectUris)
                ? []
                : source.RedirectUris
                    .Trim(']', '[')
                    .Split(',', StringSplitOptions.None)
                    .Select(u => new Uri(u.Trim('\"'), UriKind.RelativeOrAbsolute))
                    .ToList(),
            
            PostLogoutRedirectUris = string.IsNullOrEmpty(source.PostLogoutRedirectUris)
                ? []
                : source.PostLogoutRedirectUris
                    .Trim(']', '[')
                    .Split(',', StringSplitOptions.None)
                    .Select(u => new Uri(u.Trim('\"'), UriKind.RelativeOrAbsolute))
                    .ToList(),
            
            Requirements = string.IsNullOrEmpty(source.Requirements) 
                ? []
                : JsonConvert.DeserializeObject<IEnumerable<string>>(source.Requirements).ToList(),
            
             Settings = string.IsNullOrEmpty(source.Settings) 
                 ? new Dictionary<string, string>() 
                 : JsonConvert.DeserializeObject<Dictionary<string, string>>(source.Settings)
        };

    }
    
    public static OpenIddictApplicationDescriptor ToModel(this ApplicationViewModel source)
    {
        var destination = new OpenIddictApplicationDescriptor
        {
            ApplicationType = source.ApplicationType,
            ClientId = source.ClientId,
            ClientType = source.ClientType,
            ClientSecret = source.ClientSecret,
            JsonWebKeySet = JsonWebKeySetMapper(source),
            ConsentType = source.ConsentType,
            DisplayName = source.DisplayName,
            
        };
        
        if (source.Requirements.Count != 0)
            foreach (var requirement in source.Requirements)
            {
                destination.Requirements.Add(requirement);
            }
        if (source.Permissions.Count != 0)
            foreach (var permission in source.Permissions)
            {
                destination.Permissions.Add(permission);
            }
        if (source.RedirectUris.Count != 0)
            foreach (var uri in source.RedirectUris)
            {
                destination.RedirectUris.Add(uri);
            }

        if (source.PostLogoutRedirectUris.Count != 0)
            foreach (var uri in source.PostLogoutRedirectUris)
            {
                destination.PostLogoutRedirectUris.Add(uri);
            }
        
        if (source.Settings.Count != 0)
            foreach (var settingsDictionary in source.Settings)
            {
                destination.Settings.Add(settingsDictionary.Key, settingsDictionary.Value);
            }
        

        return destination;

    }

    private static ECDsaSecurityKey GetEcDsaSigningKey(ReadOnlySpan<char> key)
    {
        var algorithm = ECDsa.Create();
        algorithm.ImportFromPem(key);
        return new ECDsaSecurityKey(algorithm);
    }
    
    // Authorization changed the OpenIddictApplicationDescriptor.Type to be
    // OpenIddictApplicationDescriptor.ClientType and then marked the original
    // Type property as Obsolete with IsError = True. We cannot use AutoMapper's
    // ForMember(a => a.Type, opt => opt.Ignore()) method because it will not compile.
    // The workaround is to use the MemberList.Source option which allows AutoMapper
    // to ignore the Type property.
    private static readonly Func<ApplicationViewModel, JsonWebKeySet?> JsonWebKeySetMapper = (a) =>
    {
        if (!string.IsNullOrEmpty(a.JsonWebKeySet))
        {
            var jsonWebKey = JsonWebKeyConverter.ConvertFromECDsaSecurityKey(GetEcDsaSigningKey(a.JsonWebKeySet));
            return new JsonWebKeySet()
            {
                Keys = { jsonWebKey }
            };
        }
        return null;
    };
    
}


