using Asp.Versioning.Conventions;
using BuildingBlocks.Auth.OpenIdDict;
using BuildingBlocks.Behaviors;
using BuildingBlocks.Exceptions.Handler;
using BuildingBlocks.Hosting;
using BuildingBlocks.OpenApi;
using Carter;
using FluentValidation;
using HealthChecks.UI.Client;
using Identity.Application.Users.Abstractions;
using Identity.Infrastructure.Services.Users;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Quartz;

namespace Identity.Api.Extensions;

public static class Extensions
{
    // private const string AllowAllOrigins = "AllowAll";
    public static IServiceCollection AddIdentityApiServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IHostedService, QuartzHostedService>();
        
        services.AddTransient<IUserService, UserService>();
        
        var applicationAssembly = typeof(Program).Assembly;
        services.AddValidatorsFromAssembly(applicationAssembly);

        services.AddMediatR(config =>
        {
            // config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            config.RegisterServicesFromAssembly(applicationAssembly);
            config.AddOpenBehavior(typeof(ValidationBehavior<,>));
            config.AddOpenBehavior(typeof(LoggingBehavior<,>));
        });
        
        // services.AddCors(options =>
        // {
        //     options.AddPolicy(name: AllowAllOrigins,builder => 
        //             builder
        //                 .AllowAnyOrigin()
        //                 .AllowAnyMethod()
        //                 .AllowAnyHeader());
        // });
        //
        // Net 8 Identity template
        // services.AddControllers();
        services.AddControllers().AddApplicationPart(typeof(Program).Assembly);
        
        services.AddRouting(options => options.LowercaseUrls = true);
        
        services.AddCarter();
        
        services.AddExceptionHandler<CustomExceptionHandler>();
        
        services.AddHealthChecks()
            .AddNpgSql(configuration.GetConnectionString("DefaultConnection")!);
        
        services.AddAntiforgery();

        services.ConfigureOpenApi();
    
        return services;
    }

    public static WebApplication UseIdentityApiServices(this WebApplication app)
    {
        // register api versions
        var apiVersionSet = app.NewApiVersionSet()
            .HasApiVersion(1)
            .HasApiVersion(2)
            .ReportApiVersions()
            .Build();
        // map versioned endpoint
        var versionGroup = app
            .MapGroup("api/v{version:apiVersion}")
            .WithApiVersionSet(apiVersionSet);
        // use carter
        versionGroup.MapCarter();

        app.UseExceptionHandler(options => { });
        // IdentityPlus
        // if (!app.Environment.IsDevelopment())  
        // {
        //     app.UseExceptionHandler("/Error");
        //     app.UseHsts();
        // }

        
        app.UseHealthChecks("/api/v1/identity/health",
            new HealthCheckOptions
            {
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });
        
        app.UseStaticFiles(); // IdentityPlus
        app.UseRouting(); // IdentityPlus
        app.UseUrlsFromConfig();
        app.UseReverseProxySupport();
        
        app.UseOpenIdDict(); // or CoreConfig
        app.UseAntiforgery();
        
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
        
        app.UseOpenApi(); //Preserve Order - swaggerUI-UseOpenApi, always last
        
        return app;
    }
    
}