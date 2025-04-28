using System.Reflection;
using Framework.Core.Exceptions;
using Framework.Core.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Framework.Infrastructure.Persistence;

public static class DatabaseConfig
{
    public static IServiceCollection AddOpenIdDictDbConfig<T>(
        this IServiceCollection services, 
        IConfiguration configuration,
        Assembly dbContextAssembly, 
        string connectionName = "DefaultConnection") where T : DbContext
    {
        var dbOptions = configuration.GetSection(nameof(DatabaseOptions)).Get<DatabaseOptions>() ??
                        throw new InvalidOperationException("database options cannot be null");
        
        if(string.IsNullOrEmpty(dbOptions.ConnectionString))
            throw new InvalidOperationException($"Connection string {connectionName} not found.");

        switch (dbOptions.Provider.ToUpperInvariant())
        {
            case DbProviders.PostgreSql:
                
                services.AddDbContext<T>(options =>
                {
                    options.UseNpgsql(dbOptions?.ConnectionString, m => m.MigrationsAssembly(dbContextAssembly.FullName));
                    options.ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning));
                    options.UseOpenIddict();
                });
                
                services.AddHealthChecks()
                    .AddCheck("self", () => HealthCheckResult.Healthy(), ["live"])
                    .AddNpgSql(dbOptions.ConnectionString);
                break;

            case DbProviders.Mssql:
                services.AddDbContext<T>(options =>
                {
                    options.UseSqlServer(dbOptions?.ConnectionString, m => m.MigrationsAssembly(dbContextAssembly.FullName));
                    options.UseOpenIddict();
                });
                
                services.AddHealthChecks()
                    .AddCheck("self", () => HealthCheckResult.Healthy(), ["live"])
                    .AddSqlServer(dbOptions.ConnectionString);
                break;

            default:
                throw new GeneralException($"Identity storage provider {dbOptions.Provider} is not supported");
        }
        
        // services.AddDatabaseDeveloperPageExceptionFilter()
        
        return services;
    }
    
    public static IServiceCollection AddDatabaseConfig<T>(
        this IServiceCollection services, 
        IConfiguration configuration,
        Assembly dbContextAssembly, 
        string connectionName = "DefaultConnection") where T : DbContext
    {
        var dbOptions = configuration.GetSection(nameof(DatabaseOptions)).Get<DatabaseOptions>() ??
                        throw new InvalidOperationException("database options cannot be null");
        if(string.IsNullOrEmpty(dbOptions?.ConnectionString))
            throw new InvalidOperationException($"Connection string {connectionName} not found.");
        
        switch (dbOptions.Provider.ToUpperInvariant())
        {
            case DbProviders.PostgreSql:
                
                services.AddDbContext<T>(options =>
                {
                    options.UseNpgsql(dbOptions?.ConnectionString, m => m.MigrationsAssembly(dbContextAssembly.FullName));
                    options.ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning));
                });
                
                services.AddHealthChecks()
                    .AddCheck("self", () => HealthCheckResult.Healthy(), ["live"])
                    .AddNpgSql(dbOptions.ConnectionString);
                break;

            case DbProviders.Mssql:
                services.AddDbContext<T>(options =>
                {
                    options.UseSqlServer(dbOptions?.ConnectionString, m => m.MigrationsAssembly(dbContextAssembly.FullName));
                });
                
                services.AddHealthChecks()
                    .AddCheck("self", () => HealthCheckResult.Healthy(), ["live"])
                    .AddSqlServer(dbOptions.ConnectionString);
                break;

            default:
                throw new GeneralException($"Identity storage provider {dbOptions.Provider} is not supported");
        }
        
        return services;
    }

}