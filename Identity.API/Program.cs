using Framework.Infrastructure;
using Framework.Infrastructure.Auth.OpenIdDict;
using Framework.Infrastructure.Cors;
using Framework.Infrastructure.Hosting;
using Framework.Infrastructure.Logging.Serilog;
using Framework.Infrastructure.Persistence;
using Identity.Api.Extensions;
using Identity.Application;
using Identity.Infrastructure.Data;
using Identity.Infrastructure.Data.Worker;
using Identity.Infrastructure.Extensions;
using Serilog;

StaticLogger.EnsureInitialized();
Log.Information("server booting up..");
try
{
    var builder = WebApplication.CreateBuilder(args);
 
    builder.Services
        .AddApplicationServices(builder.Configuration)
        .AddInfrastructureServices(builder.Configuration)
        .AddApiServices(builder.Configuration);
    
    builder.Services
        .AddDefaultServices(builder.Configuration);

    builder.ConfigureReverseProxySupport();
    builder.ConfigureSerilog();

    var app = builder.Build();

    app.UseDefaultServices();
    app.UseApiServices();

    await app.RunAsync();
    
}
catch (Exception ex) when (!ex.GetType().Name.Equals("HostAbortedException", StringComparison.Ordinal))
{
    StaticLogger.EnsureInitialized();
#pragma warning disable S6667
    Log.Fatal(ex.Message, "unhandled exception");
#pragma warning restore S6667
}
finally
{
    StaticLogger.EnsureInitialized();
    Log.Information("server shutting down..");
    await Log.CloseAndFlushAsync();
}



