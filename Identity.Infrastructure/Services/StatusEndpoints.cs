using Identity.Infrastructure.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Identity.Infrastructure.Services;

public static class GetStatusEndpoints
{
    public static IEndpointRouteBuilder MapStatusEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGetStatusEndpoint();
        
        return app;
    }
    
    private static RouteHandlerBuilder MapGetStatusEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapGet("/status", StatusHandler )
            .WithName(nameof(GetStatusEndpoints))
            .WithSummary("Get status of application ")
            // .RequirePermission("Permissions.Handlers.View")
            .WithDescription("Return status of Application");
    }

    private static async Task<StatusDto> StatusHandler(IdentityContext dbContext)
    {
        var status = await dbContext.Database.CanConnectAsync();

        return new StatusDto
        {
            Api = "Ok",
            Db = status ? "Ok" : "Error",
            TimeStamp = DateTime.UtcNow
        };
    }
    private sealed class StatusDto
    {
        public string Api { get; set; }  = string.Empty;
        public string Db { get; set; } = string.Empty;
        public DateTime TimeStamp { get; set; }
    }
}
