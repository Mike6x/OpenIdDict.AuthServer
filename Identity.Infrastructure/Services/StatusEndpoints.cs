using Identity.App.Data;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Identity.App.EndPoints;

public static class StatusEndpoints
{
    public static IEndpointRouteBuilder MapStatusEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app
            .MapGroup("status")
            .WithTags("Status")
            .WithName("Status");

        group.MapGet("", StatusHandler)
            .WithName("Status")
            .AllowAnonymous();

        return app;
    }

    private static async Task<Ok<StatusDto>> StatusHandler(HttpContext httpContext, ApplicationDbContext dbContext)
    {
        var status = await dbContext.Database.CanConnectAsync();


        return TypedResults.Ok(new StatusDto
        {
            Api = "Ok",
            Db = status ? "Ok" : "Error",
            TimeStamp = DateTime.UtcNow
        });
    }

    public class StatusDto
    {
        public string Api { get; set; }
        public string Db { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}
