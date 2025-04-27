using Identity.Domain.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Framework.Infrastructure.Cors;

public static class CorsConfig
{
    public static IServiceCollection AddCorsPolicy(this IServiceCollection services, IConfiguration configuration)
    {
        // var configuration = app.Configuration
        // var services = app.Services

        var origins = configuration.GetSection("CorsOptions:Origins").Get<string[]>() ?? [];

        var applications = configuration.GetSection("OpenIdDict:ApplicationConfigs").Get<IEnumerable<ApplicationConfig>>();
        if(applications?.Any() == true)
        {
            origins = origins.Concat(
                applications.SelectMany(
                    a => a.RedirectUri?.Select(r => GetBaseAddressFromRedirectUri(r)) ?? [])
            ).ToArray();
        }

        services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy.WithOrigins(origins)
                    .AllowAnyHeader()
                    .AllowCredentials()
                    .AllowAnyMethod();
            });
        });

        return services;
    }
    
    private static string GetBaseAddressFromRedirectUri(string redirectUri)
    {
        var uri = new Uri(redirectUri);
        return uri.GetLeftPart(UriPartial.Authority);
    }
    
    private const string AllowAllOrigins = "AllowAll";
    public static IApplicationBuilder UseCorsPolicy(this WebApplication app)
    {
        app.UseCors(AllowAllOrigins);
    
        return app;
    }

}
