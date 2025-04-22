using Carter;
using Identity.API.Endpoints.Users;

namespace Identity.Api.Endpoints;

public class IdentityEndpoints
{
    public class Endpoints : CarterModule
    {
        public Endpoints() : base("identity") { }

        public override void AddRoutes(IEndpointRouteBuilder app)
        {
            var userGroup = app.MapGroup("Users").WithTags("User APIs");
            userGroup.MapUserEndpoints();
 

        }
    }
}