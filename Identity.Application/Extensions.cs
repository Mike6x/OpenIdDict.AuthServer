using System.Reflection;
using BuildingBlocks.Behaviors;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.FeatureManagement;

namespace Identity.Application;

public static class Extensions
{
    public static IServiceCollection AddIdentityApplicationServices
        (this IServiceCollection services, IConfiguration configuration)
    {
        
        services.AddFeatureManagement();
        // services.AddMessageBroker(configuration, Assembly.GetExecutingAssembly());

        return services;
    }
}