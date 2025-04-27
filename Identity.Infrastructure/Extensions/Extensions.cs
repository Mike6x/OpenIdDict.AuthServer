using Framework.Infrastructure.Persistence;
using Identity.Infrastructure.Data;
using Identity.Infrastructure.Data.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quartz;

namespace Identity.Infrastructure.Extensions;


public static class Extensions
{
    public static IServiceCollection AddInfrastructureServices
        (this IServiceCollection services, IConfiguration configuration)
    {
        var dbContextAssembly = typeof(IdentityContext).Assembly;
        services.AddOpenIdDictDbConfig<IdentityContext>(configuration,dbContextAssembly);
        
        services.AddSingleton<IHostedService, QuartzHostedService>();
        services.AddHostedService<OpenIdDictWorker>();
        
        return services;
    }
    
}
