using BuildingBlocks.Auth.OpenIdDict;
using BuildingBlocks.Hosting;
using Identity.Api.Extensions;
using Identity.Application;
using Identity.Infrastructure.Extensions;
using Identity.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.

var dbContextAssembly = typeof(AppDbContext).Assembly;
builder
    .ConfigureOpenIdDictDbContext<AppDbContext>(dbContextAssembly);
builder
    .ConfigureOpenIdDict<AppDbContext>(dbContextAssembly);

builder
    .ConfigureIdentity()
    .ConfigureCors()
    .ConfigureReverseProxySupport();

builder.Services
    .AddIdentityApplicationServices(builder.Configuration)
    .AddIdentityApiServices(builder.Configuration);

builder.Services
    .AddHostedService<SeedClientsAndScopes>();
builder.Services
    .AddHostedService<SeedRolesAndUsers>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseIdentityApiServices();

app.Run();




// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
// builder.Services.AddOpenApi();
//
// var app = builder.Build();
//
// // Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
//     app.MapOpenApi();
// }
//
// app.UseHttpsRedirection();
//
//
//
// app.Run();