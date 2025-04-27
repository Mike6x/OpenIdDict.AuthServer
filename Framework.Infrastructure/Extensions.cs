using System.Reflection;
using FluentValidation;
using Framework.Core;
using Framework.Core.Origin;
using Framework.Infrastructure.Behaviors;
using Framework.Infrastructure.Caching;
using Framework.Infrastructure.Cors;
using Framework.Infrastructure.DataIO;
using Framework.Infrastructure.Exceptions;
using Framework.Infrastructure.Jobs;
using Framework.Infrastructure.Logging.Serilog;
using Framework.Infrastructure.Mail;
using Framework.Infrastructure.OpenApi;
using Framework.Infrastructure.Storage.Files;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Framework.Infrastructure;

public static class Extensions
{
    public static IServiceCollection AddDefaultServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddExceptionHandler<CustomExceptionHandler>();
        services.AddProblemDetails();
        
        services.ConfigureJobs(configuration);
        services.ConfigureMailing();
        
        services.ConfigureCaching(configuration);
        services.ConfigureFileStorage();
        
        services.ConfigureDataImportExport();
        
        services.AddOptions<OriginOptions>().BindConfiguration(nameof(OriginOptions));
        var assemblies = new[]
        {
            typeof(AppCore).Assembly,
            typeof(AppInfrastructure).Assembly
        };

        // Register validators
         services.AddValidatorsFromAssemblies(assemblies);
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(assemblies);
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        });
        
        services.AddAntiforgery();
        services.AddCorsPolicy(configuration);
        services.AddOpenApi();
        services.AddSwaggerConfig();
        
        return services;
    }
    
    public static WebApplication UseDefaultServices(this WebApplication app)
    {
        // app.MapDefaultEndpoints()
        // app.UseRateLimit()
        // app.UseSecurityHeaders()
        // app.UseMultitenancy()
       
        
        app.UseExceptionHandler(options => { });
        
        app.UseJobDashboard(app.Configuration);
        
        app.UseStaticFiles();
        app.UseFileStorage();
        
        app.UseCorsPolicy();
        app.UseAntiforgery();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseSwagger();
        
        return app;
    }
    
    public static WebApplicationBuilder ConfigDefaultServices(this WebApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        
        // builder.AddServiceDefaults()
        builder.ConfigureSerilog();
        // builder.ConfigureDatabase()
        // builder.Services.ConfigureMultitenancy()
        // builder.Services.ConfigureIdentity()
        
        //builder.Services.AddCorsPolicy(builder.Configuration)

        // builder.Services.ConfigureJwtAuth()
        
        builder.Services.AddExceptionHandler<CustomExceptionHandler>();
        builder.Services.AddProblemDetails();
                                                  
        builder.Services.AddHealthChecks();
        
        builder.Services.ConfigureFileStorage();
        // builder.Services.AddSwaggerConfig()
        builder.Services.ConfigureJobs(builder.Configuration);
        builder.Services.ConfigureMailing();
        builder.Services.ConfigureCaching(builder.Configuration);

        
        builder.Services.ConfigureDataImportExport();
        builder.Services.AddOptions<OriginOptions>().BindConfiguration(nameof(OriginOptions));
        
        // Define module assemblies
        var assemblies = new Assembly[]
        {
            typeof(AppCore).Assembly,
            typeof(AppInfrastructure).Assembly
        };

        // Register validators
        builder.Services.AddValidatorsFromAssemblies(assemblies);

        // Register MediatR
        builder.Services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(assemblies);
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        });

        // builder.Services.ConfigureRateLimit(builder.Configuration)
        // builder.Services.ConfigureSecurityHeaders(builder.Configuration)
        
        return builder;
    }
    
}