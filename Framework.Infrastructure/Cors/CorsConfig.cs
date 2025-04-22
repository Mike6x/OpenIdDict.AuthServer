using Identity.Domain.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Framework.Infrastructure.Hosting;

public static class CorsConfig
{
    public static IHostApplicationBuilder ConfigureCorsPolicy(this IHostApplicationBuilder app)
    {
        var configuration = app.Configuration;
        var services = app.Services;

        var origins = configuration.GetSection("Cors:Origins").Get<string[]>() ?? Array.Empty<string>();

        var applications = configuration.GetSection("OpenIdDict:ApplicationConfigs").Get<IEnumerable<ApplicationConfig>>();
        if(applications?.Any() == true)
        {
            origins = origins.Concat(
                applications.SelectMany(
                    a => a.RedirectUri?.Select(r => GetBaseAddressFromRedirectUri(r)) ?? Enumerable.Empty<string>())
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

        return app;
    }

    public static IApplicationBuilder UseCorsPolicy(this WebApplication app)
    {
        app.UseCors();

        return app;
    }

    private static string GetBaseAddressFromRedirectUri(string redirectUri)
    {
        var uri = new Uri(redirectUri);
        return uri.GetLeftPart(UriPartial.Authority);
    }
}
