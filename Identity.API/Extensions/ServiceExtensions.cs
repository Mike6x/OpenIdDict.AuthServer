using Asp.Versioning.Conventions;
using Carter;
using Framework.Core.Identity.Users.Abstractions;
using Framework.Infrastructure.Auth.OpenIdDict;
using Framework.Infrastructure.Hosting;
using HealthChecks.UI.Client;
using Identity.Application.Authentication;
using Identity.Application.Authorization;
using Identity.Application.Clients;
using Identity.Application.Roles;
using Identity.Application.Scopes;
using Identity.Application.Users;
using Identity.Infrastructure.Data;
using Identity.Infrastructure.Extensions;
using Identity.Infrastructure.Services.Authentication;
using Identity.Infrastructure.Services.Authorization;
using Identity.Infrastructure.Services.Clients;
using Identity.Infrastructure.Services.Roles;
using Identity.Infrastructure.Services.Scopes;
using Identity.Infrastructure.Services.Users;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

namespace Identity.Api.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers().AddApplicationPart(typeof(Program).Assembly);
        services.AddRouting(options => options.LowercaseUrls = true);
        
        services.AddIdentityConfig(configuration);
        services.AddOpenIdDictConfig<IdentityContext>(configuration);
        
        services.AddHttpClient();
        
        services.RegisterInterface();
        
        services.AddCarter();
        
        return services;
    }

    private static IServiceCollection RegisterInterface(this IServiceCollection services)
    {
        services.AddScoped<ICurrentUser, CurrentUser>();
        services.AddScoped(sp => (ICurrentUserInitializer)sp.GetRequiredService<ICurrentUser>());
        services.AddTransient<IUserService, UserService>();
        services.AddTransient<IRoleService, RoleService>();
        
        services.AddTransient<IScopeService, ScopeService>();
        services.AddTransient<IApplicationService, ApplicationService>();
        services.AddTransient<IAuthorizationService, OpenIdDictService>();
        
        services.AddTransient<IAuthenticationService, AuthenticationService>();
        
       // services.AddTransient<IPermissionService, PermissionService>()
        // services.AddScoped<IEmailService, EmailService>()
        
        return services;
    }

    public static WebApplication UseApiServices(this WebApplication app)
    {
        app.MapControllers(); 
        app.UseRouting();
        
        app.UseUrlsFromConfig();
        app.UseReverseProxySupport();

        app.RegisterHealthCheckEndpoints();
        app.RegisterEndpoints();
        
        return app;
    }
    
    private static WebApplication RegisterHealthCheckEndpoints(this WebApplication app)
    {
        // Uncomment the following line to enable the Prometheus endpoint (requires the OpenTelemetry.Exporter.Prometheus.AspNetCore package)
        // app.MapPrometheusScrapingEndpoint()

        // Adding health checks endpoints to applications in non-development environments has security implications.
        // See https://aka.ms/dotnet/aspire/healthchecks for details before enabling these endpoints in non-development environments.
        if (app.Environment.IsDevelopment())
        {
            // All health checks must pass for app to be considered ready to accept traffic after starting
            app.UseHealthChecks("/api/v1/identity/health", new HealthCheckOptions
                {
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                });

            // Only health checks tagged with the "live" tag must pass for app to be considered alive
            app.MapHealthChecks("/api/v1/identity/alive", new HealthCheckOptions
            {
                Predicate = r => r.Tags.Contains("live")
            });
        }

        return app;
    }

    private static WebApplication RegisterEndpoints(this WebApplication app)
    {
       app.MapOpenIdDictEndpoints(); // OpenIdDict endpoint without version set
       
        var apiVersionSet = app.NewApiVersionSet()
            .HasApiVersion(1)
            .HasApiVersion(2)
            .ReportApiVersions()
            .Build();
   
        var versionGroup = app.MapGroup("api/v{version:apiVersion}").WithApiVersionSet(apiVersionSet);
       
        versionGroup.MapCarter();
        
        return app;
    }
}